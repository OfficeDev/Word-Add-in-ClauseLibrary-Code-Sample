// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Web;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Models;

namespace ClauseLibrary.Web.Models.DataModel
{
    /// <summary>
    /// An external link.
    /// </summary>
    public class ExternalLink : ListItem
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the clause identifier.
        /// </summary>
        public int ClauseId { get; set; }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public SharePointUser Author { get; set; }
        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        public SharePointUser Editor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalLink"/> class.
        /// </summary>
        public ExternalLink()
            : base(
                "Text,Url,ClauseId,Id,Author/Title,Author/Id,Author/EMail,Editor/EMail,Editor/Title,Editor/Id,Modified",
                "SP.Data.ExternalLinksListItem", "Author,Editor")
        {
            Text = HttpUtility.UrlDecode(Text);
            Url = HttpUtility.UrlDecode(Url);
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