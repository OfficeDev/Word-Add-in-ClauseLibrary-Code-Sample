// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

namespace ClauseLibrary.Common.Models
{
    /// <summary>
    /// An access token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the token_type.
        /// </summary>
        public string token_type { get; set; }
        
        /// <summary>
        /// Gets or sets the expires_in.
        /// </summary>
        public string expires_in { get; set; }
        
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string scope { get; set; }
        
        /// <summary>
        /// Gets or sets the expires_on.
        /// </summary>
        public string expires_on { get; set; }
        
        /// <summary>
        /// Gets or sets the not_before.
        /// </summary>
        public string not_before { get; set; }
        
        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public string resource { get; set; }
        
        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        public string access_token { get; set; }
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