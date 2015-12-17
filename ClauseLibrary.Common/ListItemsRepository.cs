// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using ClauseLibrary.Common.Services;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Generic Repository to perform CRUD operations for ListItems on SharePoint
    /// </summary>
    /// <typeparam name="T">Must Inherit <see cref="ListItem"/></typeparam>
    public abstract class ListItemsRepository<T> : IListItemsRepository<T> where T : ListItem, new()
    {
        private readonly ISharePointService _service;
        private readonly string _listName;
        private readonly ValidationService _validator = new ValidationService();
        private readonly ExceptionService _exceptionService = new ExceptionService();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listName">Name of SharePoint List containg ListItems to perform CRUD operations on</param>
        /// <param name="service">ISharePointService to use</param>
        protected ListItemsRepository(string listName, ISharePointService service)
        {
            _service = service;
            _listName = listName;
        }

        /// <summary>
        /// Get all ListItems from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="query"></param>
        /// <returns>
        /// All List Items
        /// </returns>
        public IEnumerable<T> GetAll(string webUrl, string accessToken, string query)
        {
            if (!_validator.IsSharePointUrlValid(webUrl) || !_validator.IsAccessTokenValid(accessToken))
                throw _exceptionService.InvalidUrlOrToken();

            return _service.GetListItems<T>(webUrl, _listName, accessToken, query);
        }

        /// <summary>
        /// Get a single ListItem from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="id">Unique Id of ListItem to retrieve</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>List Item with specified Id</returns>
        public T Get(string webUrl, int id, string accessToken)
        {
            return Get(webUrl, id, accessToken, "");
        }

        /// <summary>
        /// Get a single ListItem from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="id">Unique Id of ListItem to retrieve</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="expand"></param>
        /// <returns>
        /// List Item with specified Id
        /// </returns>
        public T Get(string webUrl, int id, string accessToken, string expand)
        {
            return string.IsNullOrWhiteSpace(expand)
                ? _service.GetListItem<T>(webUrl, _listName, id, accessToken)
                : _service.GetListItem<T>(webUrl, _listName, id, accessToken, expand);
        }

        /// <summary>
        /// Create a new ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to create</param>
        /// <returns></returns>
        public T Create(string webUrl, string accessToken, T item)
        {
            return _service.PostListItem(webUrl, _listName, accessToken, item);
        }

        /// <summary>
        /// Update an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to update</param>
        /// <returns></returns>
        public string Update(string webUrl, string accessToken, T item)
        {
            return _service.MergeListItem(webUrl, _listName, accessToken, item);
        }

        /// <summary>
        /// Delete an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="id">Unique Id of ListItem to delete</param>
        /// <returns></returns>
        public string Delete(string webUrl, string accessToken, int id)
        {
            return _service.DeleteListItem(webUrl, _listName, accessToken, id);
        }

        /// <summary>
        /// Get count of ListItems in SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>Count of items in SharePoint List</returns>
        public int Count(string webUrl, string accessToken)
        {
            return _service.GetListItemCount(webUrl, _listName, accessToken);
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