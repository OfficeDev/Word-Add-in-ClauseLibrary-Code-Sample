// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using ClauseLibrary.Common.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// A container of basic access information for SharePoint services.
    /// </summary>
    public class SharePointAccessInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointAccessInfo"/> class.
        /// </summary>
        public SharePointAccessInfo(string webUrl)
        {
            SPTenantUrl = webUrl;
            HostWebUrl = webUrl;
            User = new SharePointUser();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointAccessInfo"/> class.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="authenticationResult">The authentication result.</param>
        public SharePointAccessInfo(string webUrl, AuthenticationResult authenticationResult) : this(webUrl)
        {
            // Dont expose the refresh token on the client side!
            AccessToken = authenticationResult.AccessToken;
            ExpiresOn = authenticationResult.ExpiresOn;
            TenantId = authenticationResult.TenantId;
            UserId = authenticationResult.UserInfo.UniqueId;
            RefreshToken = authenticationResult.RefreshToken;
            UserEmail = authenticationResult.UserInfo.DisplayableId;
            User = new SharePointUser();
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date time of the access token
        /// </summary>
        public DateTimeOffset ExpiresOn { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the sp tenant web URL.
        /// </summary>
        public string SPTenantUrl { get; set; }

        /// <summary>
        /// Gets or sets the sp web URL(url of the connected library).
        /// </summary>
        public string HostWebUrl { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the currently-logged in user's email
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the full sharepoint user object
        /// </summary>
        public SharePointUser User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is admin.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the default library.
        /// </summary>
        public dynamic DefaultLibrary { get; set; }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(HostWebUrl, AccessToken))
            {
                var spWeb = ctx.Web;
                var user = ctx.Web.EnsureUser(UserEmail);
                ctx.Load(spWeb, w => w.EffectiveBasePermissions);
                ctx.Load(user);
                ctx.ExecuteQuery();

                User.Id = user.Id;
                User.EMail = user.Email;
                User.Title = user.Title;

                // Check whether the user is a site collection admin
                var userInfoList = ctx.Site.RootWeb.SiteUserInfoList;
                var item = userInfoList.GetItemById(User.Id);
                ctx.Load(item);
                ctx.ExecuteQuery();
                IsAdmin = (bool) item["IsSiteAdmin"];
            }
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