// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Response Package contains Items when returned from SharePoint REST API (Used in serialization)
    /// </summary>
    /// <typeparam name="T">Type of Item contained in response</typeparam>
    public class RestItemsResponse<T>
    {
        /// <summary>
        /// Contains array of Items
        /// </summary>
        public ResultsContainer<T> d { get; set; }
    }

    /// <summary>
    /// The rest sites response.
    /// </summary>
    public class RestSitesResponse
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public SitesResultsContainer d { get; set; }

        /// <summary>
        /// Gets the sites list.
        /// </summary>
        public List<Site> GetSitesList()
        {
            return d != null && d.query != null && d.query.PrimaryQueryResult != null &&
                   d.query.PrimaryQueryResult.RelevantResults != null &&
                   d.query.PrimaryQueryResult.RelevantResults.Table != null &&
                   d.query.PrimaryQueryResult.RelevantResults.Table.Rows != null
                ? d.query.PrimaryQueryResult.RelevantResults.Table.Rows.GetSitesList()
                : new List<Site>();
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