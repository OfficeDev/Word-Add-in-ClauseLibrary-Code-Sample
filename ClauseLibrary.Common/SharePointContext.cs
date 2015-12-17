// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SharePoint.Client;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Encapsulates all the information from SharePoint.
    /// </summary>
    public abstract class SharePointContext
    {
        /// <summary>
        /// The sp host URL key
        /// </summary>
        public const string SPHostUrlKey = "SPHostUrl";
        
        /// <summary>
        /// The sp application web URL key
        /// </summary>
        public const string SPAppWebUrlKey = "SPAppWebUrl";
        
        /// <summary>
        /// The sp language key
        /// </summary>
        public const string SPLanguageKey = "SPLanguage";
        
        /// <summary>
        /// The sp client tag key
        /// </summary>
        public const string SPClientTagKey = "SPClientTag";
        
        /// <summary>
        /// The sp product number key
        /// </summary>
        public const string SPProductNumberKey = "SPProductNumber";

        /// <summary>
        /// The access token lifetime tolerance
        /// </summary>
        protected static readonly TimeSpan AccessTokenLifetimeTolerance = TimeSpan.FromMinutes(5.0);

        private readonly Uri _spHostUrl;
        private readonly Uri _spAppWebUrl;
        private readonly string _spLanguage;
        private readonly string _spClientTag;
        private readonly string _spProductNumber;

        // <AccessTokenString, UtcExpiresOn>
        /// <summary>
        /// The user access token for sp host
        /// </summary>
        protected Tuple<string, DateTime> userAccessTokenForSPHost;
        /// <summary>
        /// The user access token for sp application web
        /// </summary>
        protected Tuple<string, DateTime> userAccessTokenForSPAppWeb;
        /// <summary>
        /// The application only access token for sp host
        /// </summary>
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPHost;
        /// <summary>
        /// The application only access token for sp application web
        /// </summary>
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPAppWeb;

        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.</returns>
        public static Uri GetSPHostUrl(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            string spHostUrlString = TokenHelper.EnsureTrailingSlash(httpRequest.QueryString[SPHostUrlKey]);
            Uri spHostUrl;
            if (Uri.TryCreate(spHostUrlString, UriKind.Absolute, out spHostUrl) &&
                (spHostUrl.Scheme == Uri.UriSchemeHttp || spHostUrl.Scheme == Uri.UriSchemeHttps))
            {
                return spHostUrl;
            }

            return null;
        }

        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.</returns>
        public static Uri GetSPHostUrl(HttpRequest httpRequest)
        {
            return GetSPHostUrl(new HttpRequestWrapper(httpRequest));
        }

        /// <summary>
        /// The SharePoint host url.
        /// </summary>
        public Uri SPHostUrl
        {
            get { return _spHostUrl; }
        }

        /// <summary>
        /// The SharePoint app web url.
        /// </summary>
        public Uri SPAppWebUrl
        {
            get { return _spAppWebUrl; }
        }

        /// <summary>
        /// The SharePoint language.
        /// </summary>
        public string SPLanguage
        {
            get { return _spLanguage; }
        }

        /// <summary>
        /// The SharePoint client tag.
        /// </summary>
        public string SPClientTag
        {
            get { return _spClientTag; }
        }

        /// <summary>
        /// The SharePoint product number.
        /// </summary>
        public string SPProductNumber
        {
            get { return _spProductNumber; }
        }

        /// <summary>
        /// The user access token for the SharePoint host.
        /// </summary>
        public abstract string UserAccessTokenForSPHost { get; }

        /// <summary>
        /// The user access token for the SharePoint app web.
        /// </summary>
        public abstract string UserAccessTokenForSPAppWeb { get; }

        /// <summary>
        /// The app only access token for the SharePoint host.
        /// </summary>
        public abstract string AppOnlyAccessTokenForSPHost { get; }

        /// <summary>
        /// The app only access token for the SharePoint app web.
        /// </summary>
        public abstract string AppOnlyAccessTokenForSPAppWeb { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        protected SharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag,
            string spProductNumber)
        {
            if (spHostUrl == null)
            {
                throw new ArgumentNullException("spHostUrl");
            }

            if (string.IsNullOrEmpty(spLanguage))
            {
                throw new ArgumentNullException("spLanguage");
            }

            if (string.IsNullOrEmpty(spClientTag))
            {
                throw new ArgumentNullException("spClientTag");
            }

            if (string.IsNullOrEmpty(spProductNumber))
            {
                throw new ArgumentNullException("spProductNumber");
            }

            this._spHostUrl = spHostUrl;
            this._spAppWebUrl = spAppWebUrl;
            this._spLanguage = spLanguage;
            this._spClientTag = spClientTag;
            this._spProductNumber = spProductNumber;
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateUserClientContextForSPHost()
        {
            return CreateClientContext(SPHostUrl, UserAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateUserClientContextForSPAppWeb()
        {
            return CreateClientContext(SPAppWebUrl, UserAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Creates app only ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateAppOnlyClientContextForSPHost()
        {
            return CreateClientContext(SPHostUrl, AppOnlyAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates an app only ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateAppOnlyClientContextForSPAppWeb()
        {
            return CreateClientContext(SPAppWebUrl, AppOnlyAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Gets the database connection string from SharePoint for autohosted app.
        /// </summary>
        /// <returns>The database connection string. Returns <c>null</c> if the app is not autohosted or there is no database.</returns>
        public string GetDatabaseConnectionString()
        {
            string dbConnectionString = null;

            using (ClientContext clientContext = CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var result = AppInstance.RetrieveAppDatabaseConnectionString(clientContext);

                    clientContext.ExecuteQuery();

                    dbConnectionString = result.Value;
                }
            }

            if (dbConnectionString == null)
            {
                const string LocalDBInstanceForDebuggingKey = "LocalDBInstanceForDebugging";

                var dbConnectionStringSettings =
                    WebConfigurationManager.ConnectionStrings[LocalDBInstanceForDebuggingKey];

                dbConnectionString = dbConnectionStringSettings != null
                    ? dbConnectionStringSettings.ConnectionString
                    : null;
            }

            return dbConnectionString;
        }

        /// <summary>
        /// Determines if the specified access token is valid.
        /// It considers an access token as not valid if it is null, or it has expired.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <returns>True if the access token is valid.</returns>
        protected static bool IsAccessTokenValid(Tuple<string, DateTime> accessToken)
        {
            return accessToken != null &&
                   !string.IsNullOrEmpty(accessToken.Item1) &&
                   accessToken.Item2 > DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a ClientContext with the specified SharePoint site url and the access token.
        /// </summary>
        /// <param name="spSiteUrl">The site url.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>A ClientContext instance.</returns>
        private static ClientContext CreateClientContext(Uri spSiteUrl, string accessToken)
        {
            if (spSiteUrl != null && !string.IsNullOrEmpty(accessToken))
            {
                return TokenHelper.GetClientContextWithAccessToken(spSiteUrl.AbsoluteUri, accessToken);
            }

            return null;
        }
    }

    /// <summary>
    /// Redirection status.
    /// </summary>
    public enum RedirectionStatus
    {
        /// <summary>
        /// The ok
        /// </summary>
        Ok,
        
        /// <summary>
        /// The should redirect
        /// </summary>
        ShouldRedirect,
        
        /// <summary>
        /// The can not redirect
        /// </summary>
        CanNotRedirect
    }

    /// <summary>
    /// Provides SharePointContext instances.
    /// </summary>
    public abstract class SharePointContextProvider
    {
        private static SharePointContextProvider current;

        /// <summary>
        /// The current SharePointContextProvider instance.
        /// </summary>
        public static SharePointContextProvider Current
        {
            get { return current; }
        }

        /// <summary>
        /// Initializes the default SharePointContextProvider instance.
        /// </summary>
        static SharePointContextProvider()
        {
            if (!TokenHelper.IsHighTrustApp())
            {
                current = new SharePointAcsContextProvider();
            }
            else
            {
                current = new SharePointHighTrustContextProvider();
            }
        }

        /// <summary>
        /// Registers the specified SharePointContextProvider instance as current.
        /// It should be called by Application_Start() in Global.asax.
        /// </summary>
        /// <param name="provider">The SharePointContextProvider to be set as current.</param>
        public static void Register(SharePointContextProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            current = provider;
        }

        /// <summary>
        /// Checks if it is necessary to redirect to SharePoint for user to authenticate.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="redirectUrl">The redirect url to SharePoint if the status is ShouldRedirect. <c>Null</c> if the status is Ok or CanNotRedirect.</param>
        /// <returns>Redirection status.</returns>
        public static RedirectionStatus CheckRedirectionStatus(HttpContextBase httpContext, out Uri redirectUrl)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            redirectUrl = null;

            if (Current.GetSharePointContext(httpContext) != null)
            {
                return RedirectionStatus.Ok;
            }

            const string SPHasRedirectedToSharePointKey = "SPHasRedirectedToSharePoint";

            if (!string.IsNullOrEmpty(httpContext.Request.QueryString[SPHasRedirectedToSharePointKey]))
            {
                return RedirectionStatus.CanNotRedirect;
            }

            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);

            if (spHostUrl == null)
            {
                return RedirectionStatus.CanNotRedirect;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(httpContext.Request.HttpMethod, "POST"))
            {
                return RedirectionStatus.CanNotRedirect;
            }

            Uri requestUrl = httpContext.Request.Url;

            var queryNameValueCollection = HttpUtility.ParseQueryString(requestUrl.Query);

            // Removes the values that are included in {StandardTokens}, as {StandardTokens} will be inserted at the beginning of the query string.
            queryNameValueCollection.Remove(SharePointContext.SPHostUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPAppWebUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPLanguageKey);
            queryNameValueCollection.Remove(SharePointContext.SPClientTagKey);
            queryNameValueCollection.Remove(SharePointContext.SPProductNumberKey);

            // Adds SPHasRedirectedToSharePoint=1.
            queryNameValueCollection.Add(SPHasRedirectedToSharePointKey, "1");

            UriBuilder returnUrlBuilder = new UriBuilder(requestUrl) {Query = queryNameValueCollection.ToString()};

            // Inserts StandardTokens.
            const string StandardTokens = "{StandardTokens}";
            string returnUrlString = returnUrlBuilder.Uri.AbsoluteUri;
            returnUrlString = returnUrlString.Insert(returnUrlString.IndexOf("?") + 1, StandardTokens + "&");

            // Constructs redirect url.
            string redirectUrlString = TokenHelper.GetAppContextTokenRequestUrl(spHostUrl.AbsoluteUri,
                Uri.EscapeDataString(returnUrlString));

            redirectUrl = new Uri(redirectUrlString, UriKind.Absolute);

            return RedirectionStatus.ShouldRedirect;
        }

        /// <summary>
        /// Checks if it is necessary to redirect to SharePoint for user to authenticate.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="redirectUrl">The redirect url to SharePoint if the status is ShouldRedirect. <c>Null</c> if the status is Ok or CanNotRedirect.</param>
        /// <returns>Redirection status.</returns>
        public static RedirectionStatus CheckRedirectionStatus(HttpContext httpContext, out Uri redirectUrl)
        {
            return CheckRedirectionStatus(new HttpContextWrapper(httpContext), out redirectUrl);
        }

        /// <summary>
        /// Creates a SharePointContext instance with the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if errors occur.</returns>
        public SharePointContext CreateSharePointContext(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            // SPHostUrl
            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpRequest);
            if (spHostUrl == null)
            {
                return null;
            }

            // SPAppWebUrl
            string spAppWebUrlString =
                TokenHelper.EnsureTrailingSlash(httpRequest.QueryString[SharePointContext.SPAppWebUrlKey]);
            Uri spAppWebUrl;
            if (!Uri.TryCreate(spAppWebUrlString, UriKind.Absolute, out spAppWebUrl) ||
                !(spAppWebUrl.Scheme == Uri.UriSchemeHttp || spAppWebUrl.Scheme == Uri.UriSchemeHttps))
            {
                spAppWebUrl = null;
            }

            // SPLanguage
            string spLanguage = httpRequest.QueryString[SharePointContext.SPLanguageKey];
            if (string.IsNullOrEmpty(spLanguage))
            {
                return null;
            }

            // SPClientTag
            string spClientTag = httpRequest.QueryString[SharePointContext.SPClientTagKey];
            if (string.IsNullOrEmpty(spClientTag))
            {
                return null;
            }

            // SPProductNumber
            string spProductNumber = httpRequest.QueryString[SharePointContext.SPProductNumberKey];
            if (string.IsNullOrEmpty(spProductNumber))
            {
                return null;
            }

            return CreateSharePointContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, httpRequest);
        }

        /// <summary>
        /// Creates a SharePointContext instance with the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if errors occur.</returns>
        public SharePointContext CreateSharePointContext(HttpRequest httpRequest)
        {
            return CreateSharePointContext(new HttpRequestWrapper(httpRequest));
        }

        /// <summary>
        /// Gets a SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if not found and a new instance can't be created.</returns>
        public SharePointContext GetSharePointContext(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
            if (spHostUrl == null)
            {
                return null;
            }

            SharePointContext spContext = LoadSharePointContext(httpContext);

            if (spContext == null || !ValidateSharePointContext(spContext, httpContext))
            {
                spContext = CreateSharePointContext(httpContext.Request);

                if (spContext != null)
                {
                    SaveSharePointContext(spContext, httpContext);
                }
            }

            return spContext;
        }

        /// <summary>
        /// Gets a SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if not found and a new instance can't be created.</returns>
        public SharePointContext GetSharePointContext(HttpContext httpContext)
        {
            return GetSharePointContext(new HttpContextWrapper(httpContext));
        }

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if errors occur.</returns>
        protected abstract SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage,
            string spClientTag, string spProductNumber, HttpRequestBase httpRequest);

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>True if the given SharePointContext can be used with the specified HTTP context.</returns>
        protected abstract bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext);

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if not found.</returns>
        protected abstract SharePointContext LoadSharePointContext(HttpContextBase httpContext);

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected abstract void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext);
    }

    #region ACS

    /// <summary>
    /// Encapsulates all the information from SharePoint in ACS mode.
    /// </summary>
    public class SharePointAcsContext : SharePointContext
    {
        private readonly string contextToken;
        private readonly SharePointContextToken contextTokenObj;

        /// <summary>
        /// The context token.
        /// </summary>
        public string ContextToken
        {
            get { return contextTokenObj.ValidTo > DateTime.UtcNow ? contextToken : null; }
        }

        /// <summary>
        /// The context token's "CacheKey" claim.
        /// </summary>
        public string CacheKey
        {
            get { return contextTokenObj.ValidTo > DateTime.UtcNow ? contextTokenObj.CacheKey : null; }
        }

        /// <summary>
        /// The context token's "refreshtoken" claim.
        /// </summary>
        public string RefreshToken
        {
            get { return contextTokenObj.ValidTo > DateTime.UtcNow ? contextTokenObj.RefreshToken : null; }
        }

        /// <summary>
        /// The user access token for the SharePoint host.
        /// </summary>
        public override string UserAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref userAccessTokenForSPHost,
                    () => TokenHelper.GetAccessToken(contextTokenObj, SPHostUrl.Authority));
            }
        }

        /// <summary>
        /// The user access token for the SharePoint app web.
        /// </summary>
        public override string UserAccessTokenForSPAppWeb
        {
            get
            {
                if (SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref userAccessTokenForSPAppWeb,
                    () => TokenHelper.GetAccessToken(contextTokenObj, SPAppWebUrl.Authority));
            }
        }

        /// <summary>
        /// The app only access token for the SharePoint host.
        /// </summary>
        public override string AppOnlyAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref appOnlyAccessTokenForSPHost,
                    () =>
                        TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, SPHostUrl.Authority,
                            TokenHelper.GetRealmFromTargetUrl(SPHostUrl)));
            }
        }

        /// <summary>
        /// The app only access token for the SharePoint app web.
        /// </summary>
        public override string AppOnlyAccessTokenForSPAppWeb
        {
            get
            {
                if (SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref appOnlyAccessTokenForSPAppWeb,
                    () =>
                        TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, SPAppWebUrl.Authority,
                            TokenHelper.GetRealmFromTargetUrl(SPAppWebUrl)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointAcsContext"/> class.
        /// </summary>
        /// <param name="spHostUrl">The sp host URL.</param>
        /// <param name="spAppWebUrl">The sp application web URL.</param>
        /// <param name="spLanguage">The sp language.</param>
        /// <param name="spClientTag">The sp client tag.</param>
        /// <param name="spProductNumber">The sp product number.</param>
        /// <param name="contextToken">The context token.</param>
        /// <param name="contextTokenObj">The context token object.</param>
        public SharePointAcsContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag,
            string spProductNumber, string contextToken, SharePointContextToken contextTokenObj)
            : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
        {
            if (string.IsNullOrEmpty(contextToken))
            {
                throw new ArgumentNullException("contextToken");
            }

            if (contextTokenObj == null)
            {
                throw new ArgumentNullException("contextTokenObj");
            }

            this.contextToken = contextToken;
            this.contextTokenObj = contextTokenObj;
        }

        /// <summary>
        /// Ensures the access token is valid and returns it.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        /// <returns>The access token string.</returns>
        private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken,
            Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

            return IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
        }

        /// <summary>
        /// Renews the access token if it is not valid.
        /// </summary>
        /// <param name="accessToken">The access token to renew.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken,
            Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            if (IsAccessTokenValid(accessToken))
            {
                return;
            }

            try
            {
                OAuth2AccessTokenResponse oAuth2AccessTokenResponse = tokenRenewalHandler();

                DateTime expiresOn = oAuth2AccessTokenResponse.ExpiresOn;

                if ((expiresOn - oAuth2AccessTokenResponse.NotBefore) > AccessTokenLifetimeTolerance)
                {
                    // Make the access token get renewed a bit earlier than the time when it expires
                    // so that the calls to SharePoint with it will have enough time to complete successfully.
                    expiresOn -= AccessTokenLifetimeTolerance;
                }

                accessToken = Tuple.Create(oAuth2AccessTokenResponse.AccessToken, expiresOn);
            }
            catch (WebException)
            {
            }
        }
    }

    /// <summary>
    /// Default provider for SharePointAcsContext.
    /// </summary>
    public class SharePointAcsContextProvider : SharePointContextProvider
    {
        private const string SPContextKey = "SPContext";
        private const string SPCacheKeyKey = "SPCacheKey";

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage,
            string spClientTag, string spProductNumber, HttpRequestBase httpRequest)
        {
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(httpRequest);
            if (string.IsNullOrEmpty(contextTokenString))
            {
                return null;
            }

            SharePointContextToken contextToken;
            try
            {
                contextToken = TokenHelper.ReadAndValidateContextToken(contextTokenString, httpRequest.Url.Authority);
            }
            catch (WebException)
            {
                return null;
            }
            catch (AudienceUriValidationFailedException)
            {
                return null;
            }

            return new SharePointAcsContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber,
                contextTokenString, contextToken);
        }

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// True if the given SharePointContext can be used with the specified HTTP context.
        /// </returns>
        protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

            if (spAcsContext != null)
            {
                Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
                string contextToken = TokenHelper.GetContextTokenFromRequest(httpContext.Request);
                HttpCookie spCacheKeyCookie = httpContext.Request.Cookies[SPCacheKeyKey];
                string spCacheKey = spCacheKeyCookie != null ? spCacheKeyCookie.Value : null;

                return spHostUrl == spAcsContext.SPHostUrl &&
                       !string.IsNullOrEmpty(spAcsContext.CacheKey) &&
                       spCacheKey == spAcsContext.CacheKey &&
                       !string.IsNullOrEmpty(spAcsContext.ContextToken) &&
                       (string.IsNullOrEmpty(contextToken) || contextToken == spAcsContext.ContextToken);
            }

            return false;
        }

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found.
        /// </returns>
        protected override SharePointContext LoadSharePointContext(HttpContextBase httpContext)
        {
            return httpContext.Session[SPContextKey] as SharePointAcsContext;
        }

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected override void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

            if (spAcsContext != null)
            {
                HttpCookie spCacheKeyCookie = new HttpCookie(SPCacheKeyKey)
                {
                    Value = spAcsContext.CacheKey,
                    Secure = true,
                    HttpOnly = true
                };

                httpContext.Response.AppendCookie(spCacheKeyCookie);
            }

            httpContext.Session[SPContextKey] = spAcsContext;
        }
    }

    #endregion ACS

    #region HighTrust

    /// <summary>
    /// Encapsulates all the information from SharePoint in HighTrust mode.
    /// </summary>
    public class SharePointHighTrustContext : SharePointContext
    {
        private readonly WindowsIdentity logonUserIdentity;

        /// <summary>
        /// The Windows identity for the current user.
        /// </summary>
        public WindowsIdentity LogonUserIdentity
        {
            get { return logonUserIdentity; }
        }

        /// <summary>
        /// The user access token for the SharePoint host.
        /// </summary>
        public override string UserAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref userAccessTokenForSPHost,
                    () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(SPHostUrl, LogonUserIdentity));
            }
        }

        /// <summary>
        /// The user access token for the SharePoint app web.
        /// </summary>
        public override string UserAccessTokenForSPAppWeb
        {
            get
            {
                if (SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref userAccessTokenForSPAppWeb,
                    () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(SPAppWebUrl, LogonUserIdentity));
            }
        }

        /// <summary>
        /// The app only access token for the SharePoint host.
        /// </summary>
        public override string AppOnlyAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref appOnlyAccessTokenForSPHost,
                    () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(SPHostUrl, null));
            }
        }

        /// <summary>
        /// The app only access token for the SharePoint app web.
        /// </summary>
        public override string AppOnlyAccessTokenForSPAppWeb
        {
            get
            {
                if (SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref appOnlyAccessTokenForSPAppWeb,
                    () => TokenHelper.GetS2SAccessTokenWithWindowsIdentity(SPAppWebUrl, null));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointHighTrustContext"/> class.
        /// </summary>
        /// <param name="spHostUrl">The sp host URL.</param>
        /// <param name="spAppWebUrl">The sp application web URL.</param>
        /// <param name="spLanguage">The sp language.</param>
        /// <param name="spClientTag">The sp client tag.</param>
        /// <param name="spProductNumber">The sp product number.</param>
        /// <param name="logonUserIdentity">The logon user identity.</param>
        /// <exception cref="System.ArgumentNullException">logonUserIdentity</exception>
        public SharePointHighTrustContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag,
            string spProductNumber, WindowsIdentity logonUserIdentity)
            : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
        {
            if (logonUserIdentity == null)
            {
                throw new ArgumentNullException("logonUserIdentity");
            }

            this.logonUserIdentity = logonUserIdentity;
        }

        /// <summary>
        /// Ensures the access token is valid and returns it.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        /// <returns>The access token string.</returns>
        private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken,
            Func<string> tokenRenewalHandler)
        {
            RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

            return IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
        }

        /// <summary>
        /// Renews the access token if it is not valid.
        /// </summary>
        /// <param name="accessToken">The access token to renew.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken,
            Func<string> tokenRenewalHandler)
        {
            if (IsAccessTokenValid(accessToken))
            {
                return;
            }

            DateTime expiresOn = DateTime.UtcNow.Add(TokenHelper.HighTrustAccessTokenLifetime);

            if (TokenHelper.HighTrustAccessTokenLifetime > AccessTokenLifetimeTolerance)
            {
                // Make the access token get renewed a bit earlier than the time when it expires
                // so that the calls to SharePoint with it will have enough time to complete successfully.
                expiresOn -= AccessTokenLifetimeTolerance;
            }

            accessToken = Tuple.Create(tokenRenewalHandler(), expiresOn);
        }
    }

    /// <summary>
    /// Default provider for SharePointHighTrustContext.
    /// </summary>
    public class SharePointHighTrustContextProvider : SharePointContextProvider
    {
        private const string SPContextKey = "SPContext";

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if errors occur.
        /// </returns>
        protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage,
            string spClientTag, string spProductNumber, HttpRequestBase httpRequest)
        {
            WindowsIdentity logonUserIdentity = httpRequest.LogonUserIdentity;
            if (logonUserIdentity == null || !logonUserIdentity.IsAuthenticated || logonUserIdentity.IsGuest ||
                logonUserIdentity.User == null)
            {
                return null;
            }

            return new SharePointHighTrustContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber,
                logonUserIdentity);
        }

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// True if the given SharePointContext can be used with the specified HTTP context.
        /// </returns>
        protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            SharePointHighTrustContext spHighTrustContext = spContext as SharePointHighTrustContext;

            if (spHighTrustContext != null)
            {
                Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
                WindowsIdentity logonUserIdentity = httpContext.Request.LogonUserIdentity;

                return spHostUrl == spHighTrustContext.SPHostUrl &&
                       logonUserIdentity != null &&
                       logonUserIdentity.IsAuthenticated &&
                       !logonUserIdentity.IsGuest &&
                       logonUserIdentity.User == spHighTrustContext.LogonUserIdentity.User;
            }

            return false;
        }

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        /// The SharePointContext instance. Returns <c>null</c> if not found.
        /// </returns>
        protected override SharePointContext LoadSharePointContext(HttpContextBase httpContext)
        {
            return httpContext.Session[SPContextKey] as SharePointHighTrustContext;
        }

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected override void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            httpContext.Session[SPContextKey] = spContext as SharePointHighTrustContext;
        }
    }

    #endregion HighTrust
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