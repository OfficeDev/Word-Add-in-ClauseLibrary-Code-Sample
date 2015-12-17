// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;

namespace ClauseLibrary.Web.Models.DataModel
{
    /// <summary>
    /// A favourited clause.
    /// </summary>
    public class Favourite : ListItem
    {
        /// <summary>
        /// Gets or sets the clause identifier.
        /// </summary>
        public int? ClauseId { get; set; }
        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public int? GroupId { get; set; }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public SharePointUser Author { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Favourite"/> class.
        /// </summary>
        public Favourite()
            : base("ClauseId,GroupId,Author/Id,Author/Title,Author/EMail", "SP.Data.FavouritesListItem", "Author")
        {
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