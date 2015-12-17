// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;

namespace ClauseLibrary.Web.Models.DataModel
{
    /// <summary>
    /// A collection of clauses.
    /// </summary>
    public class Group : ListItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Gets or sets the clauses.
        /// </summary>
        public List<Clause> Clauses { get; set; }
        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public List<Group> Groups { get; set; }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public SharePointUser Author { get; set; }
        /// <summary>
        /// Gets or sets the designees.
        /// </summary>
        public SharePointUserResults Designees { get; set; }
        /// <summary>
        /// Gets or sets the designees list.
        /// </summary>
        public List<SharePointUser> DesigneesList { get; set; }
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public SharePointUser Owner { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// Gets or sets the favourite.
        /// </summary>
        public Favourite Favourite { get; set; }
        /// <summary>
        /// Gets or sets the clause count.
        /// </summary>
        public int ClauseCount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is owner.
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [user can modify].
        /// </summary>
        public bool UserCanModify { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// Gets or sets the parent group title.
        /// </summary>
        public string ParentGroupTitle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
            : base(
                "Title,Id,ParentId,IsLocked,Owner/Title,Owner/EMail,Owner/Id,Author/Title,Author/EMail,Author/Id,Designees/Title,Designees/EMail,Designees/Id",
                "SP.Data.GroupsListItem", "Author,Designees,Owner")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group(IEnumerable<Favourite> favourites) : this()
        {
            Title = HttpUtility.UrlDecode(Title);
            if (favourites != null)
            {
                Favourite = favourites.FirstOrDefault(f => f.GroupId == Id);
            }
            Clauses = new List<Clause>();
            Groups = new List<Group>();
            DesigneesList = Designees.results ?? new List<SharePointUser>();
            UserCanModify = false;
            IsOwner = false;
            ClauseCount = 0;
            ParentId = 0;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize owner.
        /// </summary>
        public bool ShouldSerializeOwner()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize author.
        /// </summary>
        public bool ShouldSerializeAuthor()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize clause count.
        /// </summary>
        public bool ShouldSerializeClauseCount()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize is new.
        /// </summary>
        public bool ShouldSerializeIsNew()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize user can modify.
        /// </summary>
        public bool ShouldSerializeUserCanModify()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize is owner.
        /// </summary>
        public bool ShouldSerializeIsOwner()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize designees.
        /// </summary>
        public bool ShouldSerializeDesignees()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns a value indicating if we can serialize parent group title.
        /// </summary>
        public bool ShouldSerializeParentGroupTitle()
        {
            return ToClient;
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