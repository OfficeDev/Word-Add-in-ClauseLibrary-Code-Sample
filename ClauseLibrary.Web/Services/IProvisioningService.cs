// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// A contract for provisioning SharePoint content.
    /// </summary>
    public interface IProvisioningService
    {
        /// <summary>
        /// Creates the clause library web.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="clauseWebTitle">The clause web title.</param>
        /// <param name="accessToken">The access token.</param>
        Common.Models.Web CreateClauseLibraryWeb(string hostWebUrl, string clauseWebTitle, string accessToken);
        
        /// <summary>
        /// Requireses the update.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        bool RequiresUpdate(string hostWebUrl, string accessToken);

        /// <summary>
        /// Upgrades the specified host web URL.
        /// </summary>
        /// <param name="hostWebUrl">The host web URL.</param>
        /// <param name="accessToken">The access token.</param>
        bool Upgrade(string hostWebUrl, string accessToken);
        
        /// <summary>
        /// Gets the sites where user has permissions.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        List<Site> GetSitesWhereUserHasPermissions(string tenantUrl, string accessToken,
            SPBasePermissions effectiveBasePermissions);

        /// <summary>
        /// Gets the web where user has permissions.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        Common.Models.Web GetWebWhereUserHasPermissions(string tenantUrl, string accessToken,
            SPBasePermissions effectiveBasePermissions);
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