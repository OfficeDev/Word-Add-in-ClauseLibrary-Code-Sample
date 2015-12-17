// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using System.Web.Http;
using ClauseLibrary.Common;
using ClauseLibrary.Web.Models;
using ClauseLibrary.Web.Models.DataModel;
using ClauseLibrary.Web.Services;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// Provides API access to groups.
    /// </summary>
    [ExceptionHandling]
    public class GroupsController : ListItemsController<Group>
    {
        private readonly IListItemRequestService<Group> _groupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsController"/> class.
        /// </summary>
        /// <param name="groupService">The group service.</param>
        /// <param name="groupRepository">The group repository.</param>
        public GroupsController(GroupService groupService, IListItemsRepository<Group> groupRepository) : base(groupRepository)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Gets the groups at the root.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpGet]
        public IEnumerable<Group> GetRoot(string webUrl, string accessToken = "")
        {
            return GetByParentId(webUrl);
        }

        /// <summary>
        /// Gets the groups by the specified parent identifier.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpGet]
        public IEnumerable<Group> GetByParentId(string webUrl, int parentId = 0, string accessToken = "")
        {
            return GetAllGroups(webUrl, "&$filter=(ParentId eq " + parentId + ")");
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="query">The query.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpGet]
        public IEnumerable<Group> GetAllGroups(string webUrl, string query = "", string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();
            return _groupService.GetAll(webUrl, accessToken, query, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Creates the item passed in the item request model
        /// </summary>
        [HttpPost]
        public Group Create(string webUrl, [FromBody] ItemRequestModel itemRequestModel, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();

            return _groupService.Create(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Updates the item passed in the item request model
        /// </summary>
        [HttpPut]
        public Group Update(string webUrl, [FromBody] ItemRequestModel itemRequestModel, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();

            return _groupService.Update(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Deletes the item passed in the item request model
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="itemRequestModel"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [HttpPost]
        public Group Delete(string webUrl, [FromBody] ItemRequestModel itemRequestModel, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();

            return _groupService.Delete(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
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