// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;
using ClauseLibrary.Web.Models;
using ClauseLibrary.Web.Models.DataModel;
using WebGrease.Css.Extensions;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// Manages CRUD operations for clauses.
    /// </summary>
    public class ClauseService : ListItemRequestService<Clause>
    {
        private readonly IListItemsRepository<Clause> _clauseRepository;
        private readonly IListItemsRepository<Group> _groupRepository;
        private readonly IListItemsRepository<Tag> _tagRepository;
        private readonly IListItemsRepository<ExternalLink> _externalLinksRepository;
        private readonly IListItemsRepository<Favourite> _favouritesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClauseService"/> class.
        /// </summary>
        public ClauseService(
            IListItemsRepository<Clause> clauseRepository,
            IListItemsRepository<Group> groupRepository,
            IListItemsRepository<Tag> tagRepository,
            IListItemsRepository<ExternalLink> externalLinksRepository,
            IListItemsRepository<Favourite> favouritesRepository) : base(clauseRepository)
        {
            _clauseRepository = clauseRepository;
            _groupRepository = groupRepository;
            _tagRepository = tagRepository;
            _externalLinksRepository = externalLinksRepository;
            _favouritesRepository = favouritesRepository;
        }

        private IEnumerable<Clause> BuildClauses(IEnumerable<Clause> clauses, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // retrieve tags
            var tags = _tagRepository.GetAll(webUrl, accessToken, "").ToList();

            // retrieve favorites
            var favourites = _favouritesRepository.GetAll(webUrl, accessToken, "").ToList();

            // retrieve external links
            var externalLinks = _externalLinksRepository.GetAll(webUrl, accessToken, "").ToList();

            // retrieve groups
            var groups = _groupRepository.GetAll(webUrl, accessToken, "").ToList();

            var buildClauses = clauses as IList<Clause> ?? clauses.ToList();
            return buildClauses.Select(clause =>
            {
                var tempSearchKeys = new List<string>();
                var titleTokens = clause.Title.Split(' ');
                tempSearchKeys.Add(clause.Title);
                titleTokens.ForEach(tempSearchKeys.Add);
                var tagIds = clause.Tags ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(tagIds))
                {
                    //split and parse
                    var tagList =
                        tagIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList();
                    clause.TagsList = tags.Where(t => tagList.Contains(t.Id)).ToList();
                    foreach (var tag in clause.TagsList)
                    {
                        tempSearchKeys.Add(tag.Title);
                        var tagTokens = tag.Title.Split(' ');
                        tagTokens.ForEach(tempSearchKeys.Add);
                    }
                }
                else
                {
                    clause.TagsList = new List<Tag>();
                }

                if (favourites.Any())
                {
                    clause.Favourite = favourites.FirstOrDefault(f => f.ClauseId == clause.Id);
                }

                if (externalLinks.Any())
                {
                    clause.ExternalLinks = externalLinks.Where(e => e.ClauseId == clause.Id).ToList();
                    foreach (var externalLink in clause.ExternalLinks)
                    {
                        externalLink.Text = HttpUtility.UrlDecode(externalLink.Text);
                    }
                }
                else
                {
                    clause.ExternalLinks = new List<ExternalLink>();
                }

                clause.SearchIndices = tempSearchKeys;

                var clauseGroup = groups.SingleOrDefault(g => g.Id == clause.GroupId);

                List<Group> parentGroups = new List<Group>();
                // find parents recursive, to find level 1 & 2 group
                if (clauseGroup?.ParentId != null && clauseGroup.ParentId.Value > 0)
                {
                    var parentGroup = groups.SingleOrDefault(g => g.Id == clauseGroup.ParentId.Value);
                    while (parentGroup != null)
                    {
                        parentGroups.Add(parentGroup);
                        if (parentGroup.ParentId.HasValue && parentGroup.ParentId.Value > 0)
                        {
                            parentGroup = groups.SingleOrDefault(g => g.Id == parentGroup.ParentId.Value);
                        }
                        else
                        {
                            parentGroup = null;
                        }
                    }
                }

                clause = ProcessGroup(clause, clauseGroup, parentGroups);
                clause.ToClient = true;
                clause.Text = HttpUtility.UrlDecode(clause.Text);
                clause.Title = HttpUtility.UrlDecode(clause.Title);
                clause.DesigneesList = clause.Designees != null ? clause.Designees.results : new List<SharePointUser>();
                return SetClauseOwnership(clause, clauseGroup, currentUserEmail, isUserAdmin);
            });
        }

        /// <summary>
        /// Gets all clauses.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="query">The query.</param>
        /// <param name="currentUserEmail">The current user email.</param>
        /// <param name="isUserAdmin">if set to <c>true</c> [is user admin].</param>
        public override IEnumerable<Clause> GetAll(string webUrl, string accessToken, string query, string currentUserEmail, bool isUserAdmin)
        {
            var clauses = _clauseRepository.GetAll(webUrl, accessToken, query).Select(t =>
            {
                t.ToClient = true;
                return t;
            }).ToList();
            return BuildClauses(clauses, webUrl, accessToken, currentUserEmail, isUserAdmin);
        }


        /// <summary>
        /// Creates the clause (and optionally a group).
        /// </summary>
        public override Clause Create(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // User cannot create a clause in a group they do not own
            //if(clauseRequest.Group != null && !IsOwnerOrDesignee(clauseRequest.Group))
            //    throw new Exception("You cannot create a clause in a group you do not own.");

            // Non-admin cannot create a clause in a locked group
            if (itemRequestModel.Group != null && itemRequestModel.Group.IsLocked && !isUserAdmin)
                throw new Exception("You cannot modify a locked group");

            try
            {
                var preparedClause = PrepareClause(itemRequestModel, webUrl, accessToken);

                var createdClause = _clauseRepository.Create(webUrl, accessToken, preparedClause);

                // need to explicitly retrieve the new clause in order to get its full set of properties
                var retrievedNewClause = _clauseRepository.Get(webUrl, createdClause.Id, accessToken,
                    SpApiConstants.Lists.CLAUSE_EXPAND);

                // now that the clause has been created, create any new external links; there is a dependency
                // on the clause id (which is not set before the clause is created)
                ProcessExternalLinks(retrievedNewClause, itemRequestModel.ExternalLinks, webUrl, accessToken);

                return
                    BuildClauses(new List<Clause> {retrievedNewClause}, webUrl, accessToken, currentUserEmail,
                        isUserAdmin).First();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create clause", e);
            }
        }

        /// <summary>
        /// Updates the clause.
        /// </summary>
        public override Clause Update(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            // User cannot edit a clause they do not own
            if (!IsOwnerOrDesignee(itemRequestModel.Clause, currentUserEmail) && !isUserAdmin)
                throw new Exception("You do not have permission to edit this clause.");

            // User cannot add a clause to a group they do not own
            //if(clauseRequest.Group != null && !IsOwnerOrDesignee(clauseRequest.Group))
            //    throw new Exception("You do not have permission to modify the selected group.");

            // User cannot edit a locked clause unless they are admin
            if (itemRequestModel.Clause.IsLocked && !isUserAdmin)
                throw new Exception("You cannot modify a locked clause.");

            try
            {
                var preparedClause = PrepareClause(itemRequestModel, webUrl, accessToken);

                _clauseRepository.Update(webUrl, accessToken, preparedClause);

                // need to explicitly retrieve the new clause in order to get its full set of properties
                var updatedClause = _clauseRepository.Get(webUrl, itemRequestModel.Clause.Id, accessToken,
                    SpApiConstants.Lists.CLAUSE_EXPAND);

                // now that the clause has been created, create any new external links; there is a dependency
                // on the clause id (which is not set before the clause is created)
                ProcessExternalLinks(updatedClause, itemRequestModel.ExternalLinks, webUrl, accessToken);

                return
                    BuildClauses(new List<Clause> {updatedClause}, webUrl, accessToken, currentUserEmail, isUserAdmin)
                        .First();
            }
            catch (Exception)
            {
                throw new Exception("Failed to update clause");
            }
        }

        /// <summary>
        /// Deletes the specified clause.
        /// </summary>
        public override Clause Delete(ItemRequestModel itemRequestModel, string webUrl, string accessToken,
            string currentUserEmail, bool isUserAdmin)
        {
            var clause = itemRequestModel.Clause;

            // User cannot delete a clause they do not own unless they are admin
            if (!IsOwnerOrDesignee(clause, currentUserEmail) && !isUserAdmin)
                throw new Exception("You do not have permission to delete this clause.");

            if (clause.IsLocked && !isUserAdmin)
                throw new Exception("You cannot delete a locked item.");

            try
            {
                var deletedClause = _clauseRepository.Get(webUrl, clause.Id, accessToken,
                    SpApiConstants.Lists.CLAUSE_EXPAND);
                _clauseRepository.Delete(webUrl, accessToken, clause.Id);
                return deletedClause;
            }
            catch (Exception)
            {
                throw new Exception("Failed to delete clause");
            }
        }

        private Clause ProcessTags(Clause clause, string webUrl, string accessToken)
        {
            // watch for null pointer exception
            if (clause.TagsList == null)
                clause.TagsList = new List<Tag>();

            var storedTags = _tagRepository.GetAll(webUrl, accessToken, "");

            // Create new tags, if any; ensure there are no tags with the same title in SharePoint
            var createdTags = clause.TagsList
                .Where(t => t.Id <= 0 && storedTags.All(storedTag => storedTag.Title != t.Title))
                .Select(tag => _tagRepository.Create(webUrl, accessToken, tag)).ToList();

            // concatenate any newly created tags onto the list of existing tags; will be an empty list
            // if their are neither
            var existingTagList = clause.TagsList.Where(t => t.Id > 0);
            clause.TagsList = new List<Tag>(createdTags.Concat(existingTagList));

            // Update clause.Tags to match the TagsList; regardless of whether an add, remove, or both
            // occur, this will ensure the clause.Tag stays current; will be an empty string if no tags
            clause.Tags = string.Join(",", clause.TagsList.Select(t => t.Id));

            return clause;
        }

        private Clause ProcessGroup(Clause clause, Group group, List<Group> parentGroups)
        {
            if (parentGroups != null && parentGroups.Any())
            {
                clause.ItemHoverText = parentGroups.Count == 1
                    ? $"{clause.Title} - [{parentGroups[parentGroups.Count - 1].Title}]"
                    : $"{clause.Title} - [{parentGroups[parentGroups.Count - 1].Title} > {parentGroups[parentGroups.Count - 2].Title}]";
            }

            // create new group if needed
            if (@group != null)
            {
                if (string.IsNullOrEmpty(clause.ItemHoverText))
                    clause.ItemHoverText = $"{clause.Title} - [{group.Title}]";
                //if (!@group.IsNew && @group.Id >= 0) return clause; // no new group info

                //@group = _groupRepository.Create(webUrl, accessToken, @group);
                clause.GroupId = @group.Id;
                clause.IsLocked = @group.IsLocked;
                clause.GroupTitle = @group.Title;

                return clause;
            }

            if (string.IsNullOrEmpty(clause.ItemHoverText))
                clause.ItemHoverText = $"{clause.Title}";
            clause.IsLocked = false;
            clause.GroupTitle = "Ungrouped";
            return clause;
        }

        private void ProcessExternalLinks(Clause clause, List<ExternalLink> externalLinks, string webUrl,
            string accessToken)
        {
            // create new external links if needed
            if (!externalLinks.Any()) return;

            var externalLinksToCreate = externalLinks.Where(externalLink => externalLink.Id < 0);
            foreach (var externalLink in externalLinksToCreate)
            {
                externalLink.ClauseId = clause.Id;
                _externalLinksRepository.Create(webUrl, accessToken, externalLink);
            }
        }

        private Clause PrepareClause(ItemRequestModel itemRequestModel, string webUrl, string accessToken)
        {
            var clause = itemRequestModel.Clause;
            var group = itemRequestModel.Group;

            clause = ProcessTags(clause, webUrl, accessToken);
            clause = ProcessGroup(clause, group, null);

            return clause;
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