// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using ClauseLibrary.Common;
using Microsoft.SharePoint.Client;
using Site = ClauseLibrary.Common.Site;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// Creates the requisite lists required for clauses to be stored in SharePoint.
    /// </summary>
    public class ProvisioningService : IProvisioningService
    {
        private const string WEB_VERSION_PROPERTY_KEY = "_ClauseLibraryVersion";
        private static readonly Version CurrentAppVersion = new Version(1, 0, 0, 3);
        private readonly ISharePointService _sharePointService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisioningService"/> class.
        /// </summary>
        public ProvisioningService(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }

        /// <summary>
        /// Creates the clause library web.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="clauseWebTitle">The clause web title.</param>
        /// <param name="accessToken">The access token.</param>
        public Common.Models.Web CreateClauseLibraryWeb(string hostWebUrl, string clauseWebTitle, string accessToken)
        {
            if (!Exists(hostWebUrl, accessToken))
            {
                CreateClauseList(hostWebUrl, accessToken);
                CreateGroupsList(hostWebUrl, accessToken);
                CreateFavouritesList(hostWebUrl, accessToken);
                CreateTagsList(hostWebUrl, accessToken);
                CreateExternalLinksList(hostWebUrl, accessToken);
            }
            else
            {
                bool requiresUpdate = RequiresUpdate(hostWebUrl, accessToken);
                if (requiresUpdate)
                {
                    var wasUpgraded = Upgrade(hostWebUrl, accessToken);
                    if (!wasUpgraded)
                    {
                        throw new Exception("Unable to perform an upgrade to the existing Clause Library.");
                    }
                }
            }

            return _sharePointService.GetWeb(hostWebUrl, accessToken, SPBasePermissions.FullMask);
        }

        private bool Exists(string hostWebUrl, string accessToken)
        {
            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(hostWebUrl, accessToken))
                {
                    var clauseList = clientContext.Web.Lists.GetByTitle("Clauses");
                    var groupsList = clientContext.Web.Lists.GetByTitle("Groups");
                    clientContext.Load(clauseList.RootFolder.Properties);
                    clientContext.Load(groupsList.RootFolder.Properties);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the hostweb to see if an update is required, e.g. to add additional fields
        /// </summary>
        /// <param name="hostWebUrl"></param>
        /// <param name="accessToken"></param>
        public bool RequiresUpdate(string hostWebUrl, string accessToken)
        {
            if (!Exists(hostWebUrl, accessToken))
            {
                return false;
            }

            // cannot use WebProperties as it requires full control
            var clientContext = TokenHelper.GetClientContextWithAccessToken(hostWebUrl, accessToken);
            var list = clientContext.Web.Lists.GetByTitle("Clauses");
            clientContext.Load(list.RootFolder.Properties);
            clientContext.ExecuteQuery();
            try
            {
                var property = list.RootFolder.Properties[WEB_VERSION_PROPERTY_KEY];

                return property == null || new Version(property.ToString()).CompareTo(CurrentAppVersion) != 0;
            }
            catch (PropertyOrFieldNotInitializedException)
            {
                return true;
            }
        }

        /// <summary>
        /// Upgrades the clauses list at the specified host web URL.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        public bool Upgrade(string hostWebUrl, string accessToken)
        {
            var clientContext = TokenHelper.GetClientContextWithAccessToken(hostWebUrl, accessToken);
            var list = clientContext.Web.Lists.GetByTitle("Clauses");
            clientContext.Load(list.RootFolder.Properties);
            clientContext.ExecuteQuery();
            object property;
            try
            {
                property = list.RootFolder.Properties[WEB_VERSION_PROPERTY_KEY];
            }
            catch (PropertyOrFieldNotInitializedException)
            {
                property = null;
            }
            var currentWebVersion = new Version(property?.ToString() ?? "0.0.0.1");

            if (currentWebVersion.CompareTo(CurrentAppVersion) < 0)
            {
                if (currentWebVersion.CompareTo(new Version(1, 0, 0, 999)) < 0)
                {
                    // upgrade to version 1.0.0.x. Add UsageGuidelines
                    _sharePointService.CreateListField(
                        hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken,
                        ProvisioningJson.Fields.UsageGuidelines
                        );
                }
            }
            list.RootFolder.Properties[WEB_VERSION_PROPERTY_KEY] = CurrentAppVersion;
            list.RootFolder.Update();
            clientContext.ExecuteQuery();
            return true;
        }

        /// <summary>
        /// Creates the clause list.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        private void CreateClauseList(string hostWebUrl, string accessToken)
        {
            _sharePointService.CreateList(
                hostWebUrl, accessToken, ProvisioningJson.Lists.Clauses
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.Tags
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.Text
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.GroupId
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.Designees
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.Owner
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.IsLocked
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.CLAUSES_LIST_NAME, accessToken, ProvisioningJson.Fields.UsageGuidelines
                );

            // set version to current version
            var clientContext = TokenHelper.GetClientContextWithAccessToken(hostWebUrl, accessToken);
            var list = clientContext.Web.Lists.GetByTitle("Clauses");
            clientContext.Load(list.RootFolder.Properties);
            clientContext.ExecuteQuery();

            list.RootFolder.Properties[WEB_VERSION_PROPERTY_KEY] = CurrentAppVersion;
            list.RootFolder.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Creates the groups list.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        private void CreateGroupsList(string hostWebUrl, string accessToken)
        {
            _sharePointService.CreateList(hostWebUrl, accessToken, ProvisioningJson.Lists.Groups);
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.GROUPS_LIST_NAME, accessToken, ProvisioningJson.Fields.ParentId
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.GROUPS_LIST_NAME, accessToken, ProvisioningJson.Fields.Designees
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.GROUPS_LIST_NAME, accessToken, ProvisioningJson.Fields.Owner
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.GROUPS_LIST_NAME, accessToken, ProvisioningJson.Fields.IsLocked
                );
        }

        /// <summary>
        /// Creates the favourites list.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        private void CreateFavouritesList(string hostWebUrl, string accessToken)
        {
            _sharePointService.CreateList(hostWebUrl, accessToken, ProvisioningJson.Lists.Favourites);
            _sharePointService.CreateListField(hostWebUrl, SettingsHelper.FAVOURITES_LIST_NAME, accessToken,
                ProvisioningJson.Fields.ClauseId);
            _sharePointService.CreateListField(hostWebUrl, SettingsHelper.FAVOURITES_LIST_NAME, accessToken,
                ProvisioningJson.Fields.GroupId);
        }

        /// <summary>
        /// Creates the tags list.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        private void CreateTagsList(string hostWebUrl, string accessToken)
        {
            _sharePointService.CreateList(hostWebUrl, accessToken, ProvisioningJson.Lists.Tags);
        }

        /// <summary>
        /// Creates the external links list.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        private void CreateExternalLinksList(string hostWebUrl, string accessToken)
        {
            _sharePointService.CreateList(hostWebUrl, accessToken, ProvisioningJson.Lists.ExternalLinks);
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.EXTERNAL_LINKS_LIST_NAME, accessToken, ProvisioningJson.Fields.Text
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.EXTERNAL_LINKS_LIST_NAME, accessToken, ProvisioningJson.Fields.Url
                );
            _sharePointService.CreateListField(
                hostWebUrl, SettingsHelper.EXTERNAL_LINKS_LIST_NAME, accessToken, ProvisioningJson.Fields.ClauseId
                );
        }

        /// <summary>
        /// Gets the sites where user has permissions.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        public List<Site> GetSitesWhereUserHasPermissions(string tenantUrl, string accessToken,
                    SPBasePermissions effectiveBasePermissions)
        {
            return _sharePointService.GetSites(tenantUrl, accessToken, effectiveBasePermissions);
        }

        /// <summary>
        /// Gets the web where user has permissions.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        public Common.Models.Web GetWebWhereUserHasPermissions(string tenantUrl, string accessToken,
                    SPBasePermissions effectiveBasePermissions)
        {
            return _sharePointService.GetWeb(tenantUrl, accessToken, effectiveBasePermissions);
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