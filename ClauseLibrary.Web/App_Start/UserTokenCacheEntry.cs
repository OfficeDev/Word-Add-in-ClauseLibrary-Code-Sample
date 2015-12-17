// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ClauseLibrary.Web
{
    /// <summary>
    /// A simple database for token access.
    /// </summary>
    public class TokenDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDbContext"/> class.
        /// </summary>
        public TokenDbContext()
            : base("DefaultConnection")
        {
        }

        /// <summary>
        /// Gets or sets the user token cache list.
        /// </summary>
        public DbSet<UserTokenCacheEntry> UserTokenCacheList { get; set; }
    }

    /// <summary>
    /// A user token entry for caching.
    /// </summary>
    public class UserTokenCacheEntry
    {
        /// <summary>
        /// Gets or sets the user token cache identifier.
        /// </summary>
        [Key]
        public int UserTokenCacheId { get; set; }

        /// <summary>
        /// Gets or sets the web user unique identifier.
        /// </summary>
        public string WebUserUniqueId { get; set; }

        /// <summary>
        /// Gets or sets the cache bits.
        /// </summary>
        public byte[] CacheBits { get; set; }

        /// <summary>
        /// Gets or sets the last write.
        /// </summary>
        public DateTime LastWrite { get; set; }
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