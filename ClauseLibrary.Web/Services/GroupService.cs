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
    /// Manages CRUD operations for groups
    /// </summary>
    public class GroupService : ListItemRequestService<Group>
    {
        private readonly IListItemsRepository<Group> _groupRepository;
        private readonly IListItemsRepository<Clause> _clauseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupService"/> class.
        /// </summary>
        public GroupService(
            IListItemsRepository<Group> groupRepository,
            IListItemsRepository<Clause> clauseRepository
            ) : base(groupRepository)
        {
            _groupRepository = groupRepository;
            _clauseRepository = clauseRepository;
        }

        private IEnumerable<Group> BuildGroups(IEnumerable<Group> groups, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // retrieve groups
            var allGroups = _groupRepository.GetAll(webUrl, accessToken, "").ToList();
            return groups.Select(group =>
            {
                var parentGroup = allGroups.SingleOrDefault(g => g.Id == group.ParentId);
                if (parentGroup != null)
                {
                    group.IsLocked = parentGroup.IsLocked;
                    group.ParentGroupTitle = parentGroup.Title;
                }
                else
                {
                    group.ParentGroupTitle = "Ungrouped";
                }
                group.ToClient = true;
                group.DesigneesList = group.Designees != null ? group.Designees.results : new List<SharePointUser>();
                return SetGroupOwnership(group, parentGroup, currentUserEmail, isUserAdmin);
            });
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="query">The query.</param>
        /// <param name="currentUserEmail">The current user email.</param>
        /// <param name="isUserAdmin">if set to <c>true</c> the user is an admin.</param>
        public override IEnumerable<Group> GetAll(string webUrl, string accessToken, string query,
            string currentUserEmail, bool isUserAdmin)
        {
            var groups = _groupRepository.GetAll(webUrl, accessToken, query).Select(t =>
            {
                t.ToClient = true;
                return t;
            }).ToList();
            return BuildGroups(groups, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="itemRequestModel">The item request model containing a valid group to create.</param>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin">Boolean flag for whether user is an admin</param>
        /// <returns></returns>
        public override Group Create(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // User cannot create a group in a group they do not own
            //if(groupRequest.Parent != null && !IsOwnerOrDesignee(groupRequest.Parent))
            //    throw new Exception("You cannot create a group in a group you do not own.");

            // Non-admin cannot create a clause in a locked group
            if (itemRequestModel.Parent != null && itemRequestModel.Parent.IsLocked && !isUserAdmin)
                throw new Exception("You cannot modify a locked group.");

            try
            {
                var createdGroup = _groupRepository.Create(webUrl, accessToken, itemRequestModel.Group);

                // need to explicitly retrieve the new group in order to get its full set of properties
                var retrievedNewGroup = _groupRepository.Get(webUrl, createdGroup.Id, accessToken,
                    SpApiConstants.Lists.GROUP_EXPAND);
                return
                    BuildGroups(new List<Group> {retrievedNewGroup}, webUrl, accessToken, currentUserEmail, isUserAdmin)
                        .First();
            }
            catch (Exception)
            {
                throw new Exception("Failed to create group");
            }
        }

        /// <summary>
        /// Updates the group.
        /// </summary>
        /// <param name="itemRequestModel">The item request model containing a valid group to update.</param>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        public override Group Update(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // User cannot edit a group they do not own
            if (!IsOwnerOrDesignee(itemRequestModel.Group, currentUserEmail) && !isUserAdmin)
                throw new Exception("You do not have permission to edit this group.");

            // User cannot add a group to a group they do not own
            //if(groupRequest.Parent != null && !IsOwnerOrDesignee(groupRequest.Parent))
            //    throw new Exception("You do not have permission to modify the selected group.");

            // User cannot edit a locked group
            if (itemRequestModel.Group.IsLocked && !isUserAdmin)
                throw new Exception("You cannot modify a locked group.");

            try
            {
                _groupRepository.Update(webUrl, accessToken, itemRequestModel.Group);
                // need to explicitly retrieve the new group in order to get its full set of properties
                var retrievedUpdatedGroup = _groupRepository.Get(webUrl, itemRequestModel.Group.Id, accessToken,
                    SpApiConstants.Lists.GROUP_EXPAND);
                return
                    BuildGroups(new List<Group> {retrievedUpdatedGroup}, webUrl, accessToken, currentUserEmail,
                        isUserAdmin).First();
            }
            catch (Exception)
            {
                throw new Exception("Failed to update group");
            }
        }

        /// <summary>
        /// Deletes the group.
        /// </summary>
        /// <param name="itemRequestModel"></param>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        public override Group Delete(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            var group = itemRequestModel.Group;
            // User cannot delete a group they do not own
            if (!IsOwnerOrDesignee(group, currentUserEmail) && !isUserAdmin)
                throw new Exception("You do not have permission to delete this group.");

            // Non-admin cannot delete a locked group
            if (group.IsLocked && !isUserAdmin)
                throw new Exception("You cannot delete a locked item.");

            try
            {
                ProcessChildGroups(group, webUrl, accessToken);

                var deletedGroup = _groupRepository.Get(webUrl, group.Id, accessToken, SpApiConstants.Lists.GROUP_EXPAND);
                _groupRepository.Delete(webUrl, accessToken, group.Id);
                return deletedGroup;
            }
            catch (Exception)
            {
                throw new Exception("Failed to delete group");
            }
        }

        private void ProcessChildGroups(Group group, string webUrl, string accessToken)
        {
            // relink all child groups; if the root group is being deleted, set children parent ids to 0; if
            // a sub-group is deleted, set child parent ids to parent id of sub-group
            var childGroups = _groupRepository.GetAll(webUrl, accessToken, "&$filter=(ParentId eq " + group.Id + ")");
            var childClauses = _clauseRepository.GetAll(webUrl, accessToken, "&$filter=(GroupId eq " + group.Id + ")");
            var newChildParentId = group.ParentId > 0 ? group.ParentId : 0;

            foreach (var childGroup in childGroups)
            {
                var ownerId = childGroup.Owner.Id;
                childGroup.ParentId = newChildParentId;
                childGroup.ToClient = false;
                childGroup.OwnerId = ownerId;
                _groupRepository.Update(webUrl, accessToken, childGroup);
            }

            foreach (var childClause in childClauses)
            {
                var ownerId = childClause.Owner.Id;
                childClause.GroupId = newChildParentId;
                childClause.ToClient = false;
                childClause.OwnerId = ownerId;
                _clauseRepository.Update(webUrl, accessToken, childClause);
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