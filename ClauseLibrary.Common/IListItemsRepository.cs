// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// A repository definition.
    /// </summary>
    public interface IListItemsRepository<T>
    {
        /// <summary>
        /// Get all ListItems from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="query">The query.</param>
        IEnumerable<T> GetAll(string webUrl, string accessToken, string query);

        /// <summary>
        /// Get a single ListItem from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="id">Unique Id of ListItem to retrieve</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>List Item with specified Id</returns>
        T Get(string webUrl, int id, string accessToken);

        /// <summary>
        /// Gets the specified web URL.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expand">The expand.</param>
        T Get(string webUrl, int id, string accessToken, string expand);

        /// <summary>
        /// Create a new ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to create</param>
        /// <returns></returns>
        T Create(string webUrl, string accessToken, T item);

        /// <summary>
        /// Update an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to update</param>
        string Update(string webUrl, string accessToken, T item);

        /// <summary>
        /// Delete an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="id">Unique Id of ListItem to delete</param>
        /// <returns></returns>
        string Delete(string webUrl, string accessToken, int id);

        /// <summary>
        /// Get count of ListItems in SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>Count of items in SharePoint List</returns>
        int Count(string webUrl, string accessToken);
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