// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using ClauseLibrary.Common.Models;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// A service to interact with SharePoint.
    /// </summary>
    public interface ISharePointService
    {
        #region Webs

        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        List<Site> GetSites(string tenantUrl, string accessToken, SPBasePermissions effectiveBasePermissions);

        /// <summary>
        /// Gets the web.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        Web GetWeb(string webUrl, string accessToken, SPBasePermissions effectiveBasePermissions);

        #endregion

        #region List Methods

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="listJson">The list json.</param>
        string CreateList(string webUrl, string accessToken, string listJson);

        /// <summary>
        /// Creates the list field.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listTitle">The list title.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="fieldJson">The field json.</param>
        string CreateListField(string webUrl, string listTitle, string accessToken, string fieldJson);

        #endregion

        #region ListItem

        /// <summary>
        /// Gets the list items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="query">The query.</param>
        IEnumerable<T> GetListItems<T>(string webUrl, string listName, string accessToken, string query = "")
            where T : ListItem, new();

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        T GetListItem<T>(string webUrl, string listName, int id, string accessToken) where T : ListItem, new();

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expand">The expand.</param>
        T GetListItem<T>(string webUrl, string listName, int id, string accessToken, string expand)
            where T : ListItem, new();

        /// <summary>
        /// Posts the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="item">The item.</param>
        T PostListItem<T>(string webUrl, string listName, string accessToken, T item) where T : ListItem, new();
        
        /// <summary>
        /// Merges the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="item">The item.</param>
        string MergeListItem<T>(string webUrl, string listName, string accessToken, T item) where T : ListItem;
        
        /// <summary>
        /// Deletes the list item.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="id">The identifier.</param>
        string DeleteListItem(string webUrl, string listName, string accessToken, int id);
        
        /// <summary>
        /// Gets the list item count.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        int GetListItemCount(string webUrl, string listName, string accessToken);

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        SharePointUser GetCurrentUser(string webUrl, string accessToken);

        #endregion
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