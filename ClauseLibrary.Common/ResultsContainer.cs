// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Holds array of Items (Used in serialization)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultsContainer<T>
    {
        /// <summary>
        /// Array of Items
        /// </summary>
        public List<T> results { get; set; }

        /// <summary>
        /// Gets or sets the __next url for paged results.
        /// </summary>
        public string __next { get; set; }
    }

    /// <summary>
    /// A container for site results.
    /// </summary>
    public class SitesResultsContainer
    {
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        public SiteQueryResult query { get; set; }
    }

    /// <summary>
    /// The site query result.
    /// </summary>
    public class SiteQueryResult
    {
        /// <summary>
        /// Gets or sets the primary query result.
        /// </summary>
        public PrimaryQueryResult PrimaryQueryResult { get; set; }
    }

    /// <summary>
    /// The primary query result.
    /// </summary>
    public class PrimaryQueryResult
    {
        /// <summary>
        /// Gets or sets the relevant results.
        /// </summary>
        public RelevantResults RelevantResults { get; set; }
    }

    /// <summary>
    /// The results of a query.
    /// </summary>
    public class RelevantResults
    {
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        public RelevantResultsTable Table { get; set; }
    }

    /// <summary>
    /// A table of <see cref="RelevantResults"/>.
    /// </summary>
    public class RelevantResultsTable
    {
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public RelevantResultsRows Rows { get; set; }
    }

    /// <summary>
    /// The relevant result rows.
    /// </summary>
    public class RelevantResultsRows
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public List<RelevantResultsRowResults> results { get; set; }

        /// <summary>
        /// Gets the sites list.
        /// </summary>
        public List<Site> GetSitesList()
        {
            return (
                from result in results
                where
                    result != null &&
                    result.Cells != null &&
                    result.Cells.results != null &&
                    result.Cells.results.Any()
                let url = result.Cells.results.Find(prop => prop.Key == "SPWebUrl").Value
                let id = result.Cells.results.Find(prop => prop.Key == "UniqueId").Value
                let title = result.Cells.results.Find(prop => prop.Key == "Title").Value
                where
                    !string.IsNullOrWhiteSpace(url) &&
                    !string.IsNullOrWhiteSpace(id) &&
                    !string.IsNullOrWhiteSpace(title)
                select
                    new Site(url, id, title)
                )
                .ToList();
        }
    }

    /// <summary>
    /// The relevant result row results.
    /// </summary>
    public class RelevantResultsRowResults
    {
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        public RelevantResultsCells Cells { get; set; }
    }

    /// <summary>
    /// The relevant result cells.
    /// </summary>
    public class RelevantResultsCells
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public List<SiteProperty> results { get; set; }
    }

    /// <summary>
    /// A property of a site.
    /// </summary>
    public class SiteProperty
    {
        /// <summary>
        /// Gets or sets the __metadata.
        /// </summary>
        public MetaData __metadata { get; set; }
        
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// A representation of a SharePoint site.
    /// </summary>
    public class Site
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
       
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }
       
        /// <summary>
        /// Gets or sets the server relative URL.
        /// </summary>
        public string ServerRelativeUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance has library.
        /// </summary>
        public bool HasLibrary { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Site"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="title">The title.</param>
        public Site(string url, string id, string title)
        {
            Id = id != null ? id.Replace("{", string.Empty).Replace("}", string.Empty) : string.Empty;
            Title = HttpUtility.UrlDecode(title);
            Url = HttpUtility.HtmlDecode(url);
            ServerRelativeUrl = HttpUtility.UrlDecode(new Uri(Url).AbsolutePath);
            HasLibrary = false;
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