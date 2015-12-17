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
    /// A controller for loading and saving clauses.
    /// </summary>
    [ExceptionHandling]
    public class ClausesController : ListItemsController<Clause>
    {
        private readonly IListItemRequestService<Clause> _clauseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClausesController"/> class.
        /// </summary>
        /// <param name="clauseService">The clause service.</param>
        /// <param name="clauseRepository">The clause repository.</param>
        public ClausesController(ClauseService clauseService,
            IListItemsRepository<Clause> clauseRepository)
            : base(clauseRepository)
        {
            _clauseService = clauseService;
        }

        /// <summary>
        /// Gets the clauses at the root.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Clause> GetRoot(string webUrl, string accessToken = "")
        {
            return GetByGroupId(webUrl);
        }

        /// <summary>
        /// Gets the clauses by the specified group identifier.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Clause> GetByGroupId(string webUrl, int groupId = 0, string accessToken = "")
        {
            return GetAllClauses(webUrl, "&$filter=(GroupId eq " + groupId + ")");
        }

        /// <summary>
        /// Gets all clauses.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="query">The query.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpGet]
        public IEnumerable<Clause> GetAllClauses(string webUrl, string query = "", string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();

            // retrieve all clauses
            return _clauseService.GetAll(webUrl, accessToken, query, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Creates the specified clause.
        /// </summary>
        [HttpPost]
        public Clause Create(string webUrl, [FromBody] ItemRequestModel itemRequestModel, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();
            return _clauseService.Create(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Updates the specified clause.
        /// </summary>
        [HttpPut]
        public Clause Update(string webUrl, [FromBody] ItemRequestModel itemRequestModel, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();
            return _clauseService.Update(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Deletes the clause.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="itemRequestModel">The item request model.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpPost]
        public Clause Delete(string webUrl, [FromBody] ItemRequestModel itemRequestModel,
            string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var currentUserEmail = GetCurrentUserEmail();
            var isUserAdmin = IsUserAdmin();

            return _clauseService.Delete(itemRequestModel, webUrl, accessToken, currentUserEmail, isUserAdmin);
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