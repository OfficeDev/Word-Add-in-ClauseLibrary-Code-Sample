// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClauseLibrary.Common.Models
{
    /// <summary>
    /// A SharePoint user.
    /// </summary>
    [JsonObject]
    public class SharePointUser
    {
        /// <summary>
        /// Gets or sets the __metadata.
        /// </summary>
        public MetaData __metadata { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the e mail.
        /// </summary>
        public string EMail { get; set; }
    }

    /// <summary>
    /// The SharePoint user results.
    /// </summary>
    public class SharePointUserResults
    {
        /// <summary>
        /// The results
        /// </summary>
        public List<SharePointUser> results;
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