// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;
using ClauseLibrary.Web.Models;
using ClauseLibrary.Web.Models.DataModel;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// A generic service to handle list item requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListItemRequestService<T> : IListItemRequestService<T> where T : ListItem, new()
    {
        private IListItemsRepository<T> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupService"/> class.
        /// </summary>
        public ListItemRequestService(IListItemsRepository<T> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates items in the given item request model
        /// </summary>
        public abstract T Create(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail, bool isUserAdmin);

        /// <summary>
        /// Updates items in the given item request model
        /// </summary>
        public abstract T Update(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail, bool isUserAdmin);

        /// <summary>
        /// Deletes the item with the given id
        /// </summary>
        public abstract T Delete(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail, bool isUserAdmin);

        /// <summary>
        /// Retrieves all items from SharePoint
        /// </summary>
        public abstract IEnumerable<T> GetAll(string webUrl, string query, string accessToken, string currentUserEmail, bool isUserAdmin);

        /// <summary>
        /// Sets the group ownership.
        /// </summary>
        protected Group SetGroupOwnership(Group group, Group parentGroup, string currentUserEmail, bool isUserAdmin)
        {
            group.IsOwner = IsOwnerOrDesignee(group, currentUserEmail);
            group.UserCanModify = UserCanModify(currentUserEmail, group.Owner, group.DesigneesList, isUserAdmin,
                group.IsLocked);
            return group;
        }

        /// <summary>
        /// Sets the clause ownership.
        /// </summary>
        protected Clause SetClauseOwnership(Clause clause, Group parentGroup, string currentUserEmail, bool isUserAdmin)
        {
            clause.IsOwner = IsOwnerOrDesignee(clause, currentUserEmail);
            clause.UserCanModify = UserCanModify(currentUserEmail, clause.Owner, clause.DesigneesList, isUserAdmin,
                clause.IsLocked);
            return clause;
        }

        /// <summary>
        /// Determines if a user the can modify a clause.
        /// </summary>
        public bool UserCanModify(string userEmail, SharePointUser owner, List<SharePointUser> designees,
            bool isUserAdmin, bool isLocked)
        {
            return isUserAdmin || (!isLocked && (IsUserOwner(owner, userEmail) ||
                                                 (designees != null && designees.Any() &&
                                                  designees.Select(d => d.EMail == userEmail).Any())));
        }

        private bool IsUserOwner(SharePointUser owner, string userEmail)
        {
            return userEmail == owner.EMail;
        }

        /// <summary>
        /// Determines whether the specified email address is either the owner or the designee.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="currentUserEmail">The current user email.</param>
        /// <returns></returns>
        protected bool IsOwnerOrDesignee(Group group, string currentUserEmail)
        {
            var currentUserMatches = group.Owner.EMail == currentUserEmail;
            if (group.DesigneesList != null && group.DesigneesList.Any())
                return currentUserMatches || group.DesigneesList.Select(user => user.EMail == currentUserEmail).Any();

            return currentUserMatches;
        }

        /// <summary>
        /// Determines whether the specified email address is either the owner or the designee.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="currentUserEmail">The current user email.</param>
        /// <returns></returns>
        protected bool IsOwnerOrDesignee(Clause clause, string currentUserEmail)
        {
            var currentUserMatches = clause.Owner.EMail == currentUserEmail;
            if (clause.DesigneesList != null && clause.DesigneesList.Any())
                return currentUserMatches || clause.DesigneesList.Select(user => user.EMail == currentUserEmail).Any();

            return currentUserMatches;
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