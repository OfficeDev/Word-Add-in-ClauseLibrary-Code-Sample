// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Web;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;

namespace ClauseLibrary.Web.Models.DataModel
{
    /// <summary>
    /// A clause.
    /// </summary>
    public class Clause : ListItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// Gets or sets the tags list.
        /// </summary>
        public List<Tag> TagsList { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public int? GroupId { get; set; }
        /// <summary>
        /// Gets or sets the group title.
        /// </summary>
        public string GroupTitle { get; set; }


        /// <summary>
        /// Gets or sets the usage guidelines.
        /// </summary>
        public string UsageGuidelines { get; set; }
        /// <summary>
        /// Gets or sets the item hover text.
        /// </summary>
        public string ItemHoverText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [user can modify].
        /// </summary>
        public bool UserCanModify { get; set; }
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public SharePointUser Owner { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is owner.
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// Gets or sets the favourite.
        /// </summary>
        public Favourite Favourite { get; set; }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public SharePointUser Author { get; set; }
        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        public SharePointUser Editor { get; set; }
        /// <summary>
        /// Gets or sets the designees.
        /// </summary>
        public SharePointUserResults Designees { get; set; }
        /// <summary>
        /// Gets or sets the designees list.
        /// </summary>
        public List<SharePointUser> DesigneesList { get; set; }
        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// Gets or sets the external links.
        /// </summary>
        public List<ExternalLink> ExternalLinks { get; set; }
        /// <summary>
        /// Gets or sets the search indices.
        /// </summary>
        public List<string> SearchIndices { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Clause"/> class.
        /// </summary>
        public Clause()
            : base(
                "Title,Tags,Text,UsageGuidelines,GroupId,IsLocked,Owner/Title,Owner/EMail,Owner/Id,Designees/Title,Designees/EMail,Designees/Id,Author/Title,Author/Id,Author/EMail,Editor/EMail,Editor/Title,Editor/Id,Modified",
                "SP.Data.ClausesListItem", "Author,Editor,Designees,Owner")
        {
            Title = HttpUtility.UrlDecode(Title);
            Text = HttpUtility.UrlDecode(Text);
            UserCanModify = false;
            IsOwner = false;
            DesigneesList = Designees != null ? Designees.results : new List<SharePointUser>();
            GroupId = 0;
        }

        /// <summary>
        /// Returns whether or not the author should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAuthor()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the editor should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeEditor()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the designees list should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDesigneesList()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the designees should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDesignees()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the tags list should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeTagsList()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the group title should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGroupTitle()
        {
            return ToClient;
        }


        /// <summary>
        /// Returns whether or not the user can modify field should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeUserCanModify()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the modified field should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeModified()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the owner should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeOwner()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the is owner field should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeIsOwner()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the search indices should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSearchIndices()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the external links should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeExternalLinks()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the favourites should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeFavourite()
        {
            return ToClient;
        }

        /// <summary>
        /// Returns whether or not the item hover text should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeItemHoverText()
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