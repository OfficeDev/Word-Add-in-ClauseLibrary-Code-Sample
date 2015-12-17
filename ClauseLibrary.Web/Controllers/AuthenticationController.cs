// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Services;
using ClauseLibrary.Web.Hubs;
using ClauseLibrary.Web.Models.Database.LoginSettings;
using ClauseLibrary.Web.Models.Database.Services;
using ClauseLibrary.Web.Services;
using ClauseLibrary.Web.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using FormCollection = System.Web.Mvc.FormCollection;
using User = ClauseLibrary.Web.Models.Database.LoginSettings.User;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// Provides authentication to the application.
    /// </summary>
    [HandleError]
    public class AuthenticationController : Controller
    {
        private readonly ILoginSettingsService _loginSettingsService;
        private readonly LoggingService _loggingService;
        private readonly IRefreshTokenManager _refreshTokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        public AuthenticationController(ILoginSettingsService loginSettingsService, LoggingService loggingService, IRefreshTokenManager refreshTokenManager)
        {
            _loginSettingsService = loginSettingsService;
            _loggingService = loggingService;
            _refreshTokenManager = refreshTokenManager;
        }

        /// <summary>
        /// Signs in the user.
        /// </summary>
        public ActionResult SignIn(string parentId = "")
        {
            if (Request == null || Request.Url == null)
                throw new NullReferenceException("The request object is null during sign in.");

            string stateMarker = Guid.NewGuid().ToString();

            string authorizationRequest = String.Format(
                "{0}/oauth2/authorize?response_type=code&client_id={1}&resource={2}&redirect_uri={3}&state={4}",
                SettingsHelper.AUTHORITY + "Common",
                Uri.EscapeDataString(SettingsHelper.ClientId),
                Uri.EscapeDataString(SettingsHelper.GRAPH_RESOURCE_ID),
                Uri.EscapeDataString(Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ProcessCode", "Authentication")),
                Uri.EscapeDataString(stateMarker)
                );

            Session["parentid"] = parentId;

            return new RedirectResult(authorizationRequest);
        }

        /// <summary>
        /// Removes the login credentials of the current user.
        /// </summary>
        public ActionResult SignOut(string signOutId)
        {
            Session["signOutId"] = signOutId;

            if (Request == null || Request.Url == null)
                throw new Exception("The request object is null during sign out.");

            string callbackUrl = Url.Action(
                "SignOutSuccess", "Authentication", null, Request.Url.Scheme
                ) + "/" + signOutId;

            string signoutRequest = String.Format(
                "{0}/oauth2/logout?post_logout_redirect_uri={1}",
                SettingsHelper.AUTHORITY + "Common",
                callbackUrl
                );

            // Empty the Session AccessInfo
            Session.Remove("AccessInfo");

            return new RedirectResult(signoutRequest);
        }

        /// <summary>
        /// The landing page after a signout.
        /// </summary>
        public ActionResult SignOutSuccess(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                parentId = (string)Session["signOutId"];
            }
            
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AuthenticationHub>();
            hubContext.Clients.Client(parentId).onSignOutSuccess(parentId);
            return View();
        }

        /// <summary>
        /// Makes a request to retrieve a new access token.
        /// </summary>
        /// <returns>
        /// A new access token
        /// </returns>
        [HttpPost]
        public string RefreshAccessToken(FormCollection form)
        {
            string spWebUrl = form["spWebUrl"];
            string tenantId = form["tenantId"];

            string refreshTokenContent = _refreshTokenManager.RetrieveToken(HttpContext.Request);

            var credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientKey);
            var authContext = new AuthenticationContext(SettingsHelper.AUTHORITY + tenantId);

            try
            {
                AuthenticationResult result = authContext.AcquireTokenByRefreshToken(refreshTokenContent, credential, spWebUrl);

                if (result == null)
                    throw new Exception("Error acquiring SharePoint AccessToken.");

                return result.AccessToken;
            }
            catch (AdalServiceException exception)
            {
                throw new Exception("SharePoint RefreshToken is invalid.", exception);
            }
        }

        /// <summary>
        /// Processes the authentication code response.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="state">The state.</param>
        public async Task<ActionResult> ProcessCode(
            string code, string error, string errorDescription, string resource, string state)
        {
            if (Request.Url == null) return RedirectToAction("Index", "App");

            // Set up the credential and auth context used to make token requests
            var parentId = (string) Session["parentId"];
            var credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientKey);
            var authContext = new AuthenticationContext(SettingsHelper.AUTHORITY + "Common");

            // Retrieve all pertinent access info for app usage
            try
            {
                var result = authContext.AcquireTokenByAuthorizationCode(
                    code,
                    new Uri(Request.Url.GetLeftPart(UriPartial.Path)),
                    credential
                    );

                var accessInfo = await GetSharePointAccessInfo(authContext, result);

                // store the access info in the session for retrieval
                Session["AccessInfo"] = accessInfo;

                return ContactParentPageToNavigateAway(parentId, accessInfo);
            }
            catch (Exception exception)
            {
                var errorMessage = "SharePoint authorization failed. Please contact your admin.";
                _loggingService.LogException(exception);
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AuthenticationHub>();
                hubContext.Clients.Client(parentId).onLoginError(parentId, errorMessage);

                return View(new ProcessCodeViewModel {ParentId = parentId});
            }
        }

        /// <summary>
        /// Contacts the parent page to navigate away.
        /// </summary>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="accessInfo">The access information.</param>
        private ActionResult ContactParentPageToNavigateAway(string parentId, SharePointAccessInfo accessInfo)
        {
            var hasSent = false;
            var retryCount = 0;
            const int maximumRetryCount = 3;
            while (!hasSent && retryCount < maximumRetryCount)
            {
                try
                {
                    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AuthenticationHub>();
                    hubContext.Clients.Client(parentId)
                        .onLoginSuccess(parentId, JsonConvert.SerializeObject(accessInfo));
                    hasSent = true;
                }
                catch (Exception exception)
                {
                    Console.Write(exception);
                    retryCount++;
                    if (retryCount < maximumRetryCount)
                    {
                        // unfortunately the parent page will require a refresh.
                    }
                }
            }

            return View(new ProcessCodeViewModel {ParentId = parentId});
        }

        /// <summary>
        /// Gets the share point access information.
        /// </summary>
        /// <param name="authContext">The authentication context.</param>
        /// <param name="result">The result.</param>
        public async Task<SharePointAccessInfo> GetSharePointAccessInfo(AuthenticationContext authContext,
            AuthenticationResult result)
        {
            SharePointAccessInfo accessInfo;
            try
            {
                // retrieve sharepoint tenant url for the current user
                accessInfo = await AcquireSharePointAuthentication(authContext, result);
                _refreshTokenManager.StoreAccessToken(accessInfo, HttpContext);
            }
            catch (AdalSilentTokenAcquisitionException exception)
            {
                Exception thrownException = new Exception("An error occurred while trying to obtain SharePoint access information.\n" + exception.Message);
                _loggingService.LogException(thrownException);
                throw thrownException;
            }

            try
            {
                // retrieve user-specific data from db
                var user = EnsureLoginSettings(accessInfo);

                // get library the current user is connected to
                var defaultLibrary = user.DefaultLibrary;

                // Update access info for the with connectedLibrary
                if (defaultLibrary == null) return accessInfo;

                accessInfo.DefaultLibrary = defaultLibrary;
                accessInfo.HostWebUrl = defaultLibrary.HostWebUrl;

                // Double check whether the user is an admin now that we have their default library selected
                // from the database
                if (!string.IsNullOrEmpty(accessInfo.AccessToken) && !string.IsNullOrEmpty(accessInfo.UserEmail))
                {
//                    accessInfo.IsAdmin = CheckUserSiteAdmin(defaultLibrary.HostWebUrl, accessInfo.AccessToken,
//                        accessInfo.UserEmail);
                    accessInfo.Update();
                }

                return accessInfo;
            }
            catch (Exception exception)
            {
                Exception thrownException = new Exception("An exception was thrown while trying to obtain SharePoint access information.\n" + exception.Message);
                _loggingService.LogException(thrownException);
                throw thrownException;
            }
        }


        private async Task<SharePointAccessInfo> AcquireSharePointAuthentication(
            AuthenticationContext authContext, AuthenticationResult authResult)
        {
            var authToken =
                await GetAuthorisationTokenAsync(authContext, SettingsHelper.DISCOVERY_SVC_RESOURCE_ID, authResult);
            string webUrl;
            try
            {
                var discoveryClient = new DiscoveryClient(SettingsHelper.DiscoveryServiceEndpointUri, () => (authToken.AccessToken));

                var capability = await discoveryClient.DiscoverCapabilityAsync("RootSite");

                webUrl = capability.ServiceResourceId ?? String.Empty;
            }
            catch (Exception ex)
            {
                _loggingService.LogException(ex);
                throw;
            }

            if (String.IsNullOrEmpty(webUrl)) return null;

            var sharePointAuthResult = await GetAuthorisationTokenAsync(authContext, webUrl, authResult);
            var accessInfo = new SharePointAccessInfo(webUrl, sharePointAuthResult);

            accessInfo.Update();

            return accessInfo;
        }

        /// <summary>
        /// Allows checking for whether a user is a site collection admin
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="userEmail"></param>
        /// <returns>Returns whether the given user is a site collection admin at the given sharepoint url</returns>
        [HttpPost]
        public bool CheckUserSiteAdmin(string webUrl, string accessToken, string userEmail)
        {
            return GetUserSiteAdmin(webUrl, accessToken, userEmail);
        }

        /// <summary>
        /// Gets the user site admin.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="userEmail">The user email.</param>
        public static bool GetUserSiteAdmin(string webUrl, string accessToken, string userEmail)
        {
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
            {
                var user = ctx.Web.EnsureUser(userEmail);
                ctx.Load(user);
                ctx.ExecuteQuery();

                return IsUserSiteAdmin(ctx, user.Id);
            }
        }

        private static bool IsUserSiteAdmin(ClientContext ctx, int userId)
        {
            var userInfoList = ctx.Site.RootWeb.SiteUserInfoList;
            var item = userInfoList.GetItemById(userId);
            ctx.Load(item);
            ctx.ExecuteQuery();
            return (bool) item["IsSiteAdmin"];
        }

        private static async Task<AuthenticationResult> GetAuthorisationTokenAsync(
            AuthenticationContext context, string resourceId, AuthenticationResult authResult)
        {
            var user = new UserIdentifier(authResult.UserInfo.UniqueId, UserIdentifierType.UniqueId);
            var credential = ApplicationClientCredentials;
            AuthenticationResult result = await context.AcquireTokenSilentAsync(resourceId, credential, user);
            return result;
        }

        private static ClientCredential ApplicationClientCredentials
        {
            get
            {
                var credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientKey);
                return credential;
            }
        }

        private User EnsureLoginSettings(SharePointAccessInfo accessInfo)
        {
            //Persist user and Tenant data if it doesn't already exist and update refresh token
            var user = new User();
            using (var loginService = _loginSettingsService)
            {
                var tenantGuid = new Guid(accessInfo.TenantId);
                var tenant = loginService.GetTenantById(tenantGuid);

                if (tenant == null)
                {
                    tenant = new Tenant
                    {
                        TenantId = tenantGuid
                    };

                    loginService.Add(tenant);
                }

                var userGuid = new Guid(accessInfo.UserId);
                user = loginService.GetUserById(userGuid);

                if (user == null)
                {
                    user = new User
                    {
                        UserId = new Guid(accessInfo.UserId),
                        TenantId = tenant.TenantId,
                        RefreshToken = accessInfo.RefreshToken
                    };

                    loginService.Add(user);
                }
                else
                {
                    user.RefreshToken = accessInfo.RefreshToken;
                }

                loginService.Save();
            }
            return user;
        }
    }
}

#region License 
// ClauseLibrary, https://github.com/OfficeDev/clauselibrary 
//   
// Copyright 2015(c) Microsoft Corporation 
//   
// All rights reserved. 
//   
// MIT License: 
//   
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
// following conditions: 
//   
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software. 
//   
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT 
// SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
// USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion