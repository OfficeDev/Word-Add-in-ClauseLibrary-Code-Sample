// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// ApiController to expose ListItem CRUD operations
    /// </summary>
    /// <typeparam name="T">Type of ListItem to manage</typeparam>
    [ExceptionHandling]
    public abstract class ListItemsController<T> : ApiControllerBase where T : ListItem, new()
    {
        /// <summary>
        /// Repository to manage CRUD operations
        /// </summary>
        protected readonly IListItemsRepository<T> Repository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="repository">Repository to manage CRUD operations</param>
        protected ListItemsController(IListItemsRepository<T> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Get count of ListItems in SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>Count of List Items</returns>
        public virtual int GetCount(string webUrl, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            return Repository.Count(webUrl, accessToken);
        }

        /// <summary>
        /// Get all ListItems from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="query">OData Query Parameters</param>
        /// <returns>All List Items</returns>
        public virtual IEnumerable<T> GetAll(string webUrl, string query = "", string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var results = Repository.GetAll(webUrl, accessToken, query).Select(t =>
            {
                t.ToClient = true;
                return t;
            }).ToList();
            return results;
        }

        /// <summary>
        /// Get a single ListItem from SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="id">Unique Id of ListItem to retrieve</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns>List Item with specified Id</returns>
        public virtual T Get(string webUrl, int id, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var t = Repository.Get(webUrl, id, accessToken);
            t.ToClient = true;
            return t;
        }

        /// <summary>
        /// Create a new ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to create</param>
        /// <returns>Newly Created ListItem</returns>
        [HttpPost]
        public virtual T Post(string webUrl, [FromBody] T item, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            item.ToClient = false;
            return Repository.Create(webUrl, accessToken, item);
        }

        /// <summary>
        /// Update an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="isLocked"></param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <param name="item">ListItem to update</param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [HttpPut]
        public virtual string Put(string webUrl, [FromBody] T item, string userEmail, bool isLocked = false,
            string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isAdmin = IsUserAdmin();
            if (currentUserEmail != userEmail)
                throw new Exception("You do not have permission to update this item.");

            if (!isAdmin && isLocked)
                throw new Exception("You cannot update a locked item.");

            item.ToClient = false;
            return Repository.Update(webUrl, accessToken, item);
        }

        /// <summary>
        /// Delete an existing ListItem in the SharePoint List
        /// </summary>
        /// <param name="webUrl">URL of SharePoint Web which contains the list</param>
        /// <param name="id"></param>
        /// <param name="isLocked"></param>
        /// <param name="accessToken">OAuth Access Token to be used as authentication with SharePoint REST API</param>
        /// <returns></returns>
        [HttpDelete]
        public virtual string Delete(string webUrl, int id, bool isLocked = false, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);

            return Repository.Delete(webUrl, accessToken, id);
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