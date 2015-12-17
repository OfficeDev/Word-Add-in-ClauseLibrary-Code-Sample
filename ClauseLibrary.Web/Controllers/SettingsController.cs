// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ClauseLibrary.Common;
using ClauseLibrary.Web.Models;
using ClauseLibrary.Web.Models.Database.LoginSettings;
using ClauseLibrary.Web.Models.Database.Services;
using ClauseLibrary.Web.Models.DataModel;
using ClauseLibrary.Web.Services;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// Provides functionality to control settings used within the application.
    /// </summary>
    public class SettingsController : ApiControllerBase
    {
        private readonly IProvisioningService _provisioningService;
        private readonly ILoginSettingsService _loginSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController" /> class.
        /// </summary>
        /// <param name="provisioningService">The provisioning service.</param>
        /// <param name="loginSettingsService">The login settings service.</param>
        public SettingsController(IProvisioningService provisioningService, ILoginSettingsService loginSettingsService)
        {
            _provisioningService = provisioningService;
            _loginSettingsService = loginSettingsService;
        }

        /// <summary>
        /// Gets the subset of the existing libraries for a tenant which the user has access to.
        /// </summary>
        /// <param name="tenantWebUrl">URL of tenant</param>
        /// <param name="userId">string guid of the logged in user. Can be accessed on SharePointAccessInfo</param>
        /// <returns>Enumerable of LibraryModel</returns>
        [System.Web.Http.HttpGet]
        public IEnumerable<LibraryModel> GetAllExistingLibraries(string tenantWebUrl, string userId)
        {
            //get list of libraries from the database; permission checking will be done when
            //the user attempts to connect, not here.
            var libraries = new List<Library>();
            var user = _loginSettingsService.GetUserById(new Guid(userId));
            if (user != null)
            {
                libraries = user.Tenant.Libraries;
            }

            return libraries.Select(library => new LibraryModel(library));
        }

        /// <summary>
        /// Gets a list of locations where the user can create a library (has site create site permissions)
        /// </summary>
        /// <param name="tenantWebUrl">URL of tenant</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>Enumerable of WebModel</returns>
        [System.Web.Http.HttpGet]
        public List<Site> GetPotentialLibraryLocations(string tenantWebUrl, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);

            var sites = _provisioningService.GetSitesWhereUserHasPermissions(tenantWebUrl, accessToken,
                SPBasePermissions.FullMask);
            
            return sites;
        }

        /// <summary>
        /// Checks the version of the SharePoint site, and return false if an upgrade is required
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public bool SpUpdateRequired(string webUrl, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var requiresUpdate = _provisioningService.RequiresUpdate(webUrl, accessToken);

            return requiresUpdate;
        }

        /// <summary>
        /// Upgrades the specified web URL.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        [System.Web.Http.HttpGet]
        public bool Upgrade(string webUrl, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            return _provisioningService.Upgrade(webUrl, accessToken);
        }

        /// <summary>
        /// Creates a library (sharepoint site) at specified location.
        /// </summary>
        /// <param name="webUrl">URL where to create the subsite</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="newLibrary">Library to be created</param>
        /// <returns>http Response message</returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Create(string webUrl, Library newLibrary, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            
            try
            {
                Common.Models.Web libraryWebModel = _provisioningService.CreateClauseLibraryWeb(webUrl, newLibrary.Name, accessToken);

                Library library = new Library
                {
                    LibraryId = new Guid(libraryWebModel.Id),
                    TenantId = newLibrary.TenantId,
                    Name = newLibrary.Name,
                    Description = newLibrary.Description,
                    HostWebUrl = webUrl
                };

                //Create Library in Database
                _loginSettingsService.Add(library);
                _loginSettingsService.Save();

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(library.LibraryId.ToString()) };
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = e.Message};
            }
        }

        /// <summary>
        /// Connects to existing library (sharepoint site)
        /// </summary>
        /// <param name="tenantWebUrl">webUrl of tenant</param>
        /// <param name="library">The library.</param>
        /// <param name="userId">string guid of the logged in user. Can be accessed on SharePointAccessInfo</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>
        /// http Response message
        /// </returns>
        [System.Web.Http.HttpPut]
        public LibraryConnectionResult Connect(string tenantWebUrl, Library library, string userId, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var userEmail = GetCurrentUserEmail();

            //check with provisioning service that they are authoriesed to connect
            var libraryId = library.LibraryId;

            var libraryIdWithReadPermission = _provisioningService.GetWebWhereUserHasPermissions(
                library.HostWebUrl, accessToken, SPBasePermissions.ViewListItems);

            if (libraryIdWithReadPermission == null || libraryIdWithReadPermission.Id == null || new Guid(libraryIdWithReadPermission.Id) != libraryId)
            {
                return null;
            }

            // update db connected User if they have connection
            var user = _loginSettingsService.GetUserById(new Guid(userId));
            if (user != null)
            {
                user.DefaultLibraryId = library.LibraryId;
                _loginSettingsService.Save();

                var accessInfo = new SharePointAccessInfo(library.HostWebUrl)
                {
                    AccessToken = accessToken,
                    UserEmail = userEmail
                };
                accessInfo.Update();

                var libraryConnectionResult = new LibraryConnectionResult
                {
                    Library = library,
                    AccessInfo = accessInfo
                };

                return libraryConnectionResult;
            }
            return null;
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