// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

namespace ClauseLibrary.Common
{
    /// <summary>
    /// A list of constants used in the messaging.
    /// </summary>
    public static class SpApiConstants
    {
        /// <summary>
        /// Request headers.
        /// </summary>
        public static class RequestHeaders
        {
            /// <summary>
            /// The json content
            /// </summary>
            public const string JSON_CONTENT = "application/json;odata=verbose";
            
            /// <summary>
            /// The authorization
            /// </summary>
            public const string AUTHORIZATION = "Bearer ";
           
            /// <summary>
            /// The authorization template
            /// </summary>
            public const string AUTHORIZATIONTEMPLATE = AUTHORIZATION + "{0}";
            
            /// <summary>
            /// The XHTTP method
            /// </summary>
            public const string X_HTTP_METHOD = "X-HTTP-Method";
           
            /// <summary>
            /// The merge
            /// </summary>
            public const string MERGE = "MERGE";
            
            /// <summary>
            /// The delete
            /// </summary>
            public const string DELETE = "DELETE";
        }

        /// <summary>
        /// Webs
        /// </summary>
        public static class Webs
        {
            /// <summary>
            /// The base
            /// </summary>
            private const string BASE = "{0}/_api/web/webs/";
            
            /// <summary>
            /// The single site
            /// </summary>
            private const string SINGLE_SITE = "{0}/_api/web/";
            
            /// <summary>
            /// The permission query
            /// </summary>
            private const string PERMISSION_QUERY = "?$select=title,id,serverrelativeurl,url";
            
            /// <summary>
            /// The create
            /// </summary>
            public const string CREATE = "{0}/_api/web/webs/add";
            
            /// <summary>
            /// The get
            /// </summary>
            public const string GET = BASE + "?$select=title,url";
            
            /// <summary>
            /// The get all properties
            /// </summary>
            public const string GET_ALLPROPERTIES = SINGLE_SITE + "allproperties";
            
            /// <summary>
            /// The get permission trimmed
            /// </summary>
            public const string GET_PERMISSION_TRIMMED = BASE + PERMISSION_QUERY;
            
            /// <summary>
            /// The get single permission trimmed
            /// </summary>
            public const string GET_SINGLE_PERMISSION_TRIMMED = SINGLE_SITE + PERMISSION_QUERY;
            
            /// <summary>
            /// The get subsites
            /// </summary>
            public const string GET_SUBSITES = "{0}/{1}/_api/web/webinfos?$select=title,id,serverrelativeurl,url";

            /// <summary>
            /// The search sites
            /// </summary>
            public const string SEARCH_SITES =
                @"{0}/_api/search/query?querytext='contentclass:STS_Web'&trimduplicates=false&rowlimit=500";
        }

        /// <summary>
        /// Lists
        /// </summary>
        public static class Lists
        {
            /// <summary>
            /// The base
            /// </summary>
            private const string BASE = "{0}/_api/web/lists/";
            
            /// <summary>
            /// The post
            /// </summary>
            public const string POST = BASE;
            
            /// <summary>
            /// The get by title
            /// </summary>
            public const string GET_BY_TITLE = BASE + "GetByTitle('{1}')";
            
            /// <summary>
            /// The create field
            /// </summary>
            public const string CREATE_FIELD = GET_BY_TITLE + "/Fields";
            
            /// <summary>
            /// The list titles
            /// </summary>
            public const string LIST_TITLES = BASE + "?$top=10000&$select=Title&orderBy=Title";
            
            /// <summary>
            /// The clause expand
            /// </summary>
            public const string CLAUSE_EXPAND = "Author,Designees,Owner,Editor";
            
            /// <summary>
            /// The group expand
            /// </summary>
            public const string GROUP_EXPAND = "Author,Designees,Owner";
        }

        /// <summary>
        /// List items
        /// </summary>
        public static class ListItems
        {
            /// <summary>
            /// The base
            /// </summary>
            private const string BASE = "{0}/_api/lists/getbytitle('{1}')/items";
            
            /// <summary>
            /// The expand
            /// </summary>
            public const string EXPAND = "$expand={0}";
            
            /// <summary>
            /// The get all
            /// </summary>
            public const string GET_ALL = BASE + "?$top=10000&$select={2}&orderBy=Title";
           
            /// <summary>
            /// The get
            /// </summary>
            public const string GET = BASE + "({2})?$select={3}";
           
            /// <summary>
            /// The get expanded
            /// </summary>
            public const string GET_EXPANDED = GET + "&$expand={4}";
            
            /// <summary>
            /// The post
            /// </summary>
            public const string POST = BASE;
            
            /// <summary>
            /// The merge
            /// </summary>
            public const string MERGE = BASE + "({2})";
            
            /// <summary>
            /// The delete
            /// </summary>
            public const string DELETE = BASE + "({2})";
            
            /// <summary>
            /// The count
            /// </summary>
            public const string COUNT = "{0}/_api/lists/getbytitle('{1}')/itemcount";
        }

        /// <summary>
        /// Users
        /// </summary>
        public static class Users
        {
            /// <summary>
            /// The base
            /// </summary>
            private const string BASE = "{0}/_api/web/siteusers";
            
            /// <summary>
            /// The select fields
            /// </summary>
            private const string SELECT_FIELDS = "$select=Id,Title";
            
            /// <summary>
            /// The get all
            /// </summary>
            public const string GET_ALL = BASE + "?" + SELECT_FIELDS;
            
            /// <summary>
            /// The get by identifier
            /// </summary>
            public const string GET_BY_ID = BASE + "/getbyid({1})?" + SELECT_FIELDS;
            
            /// <summary>
            /// The get by accountname
            /// </summary>
            public const string GET_BY_ACCOUNTNAME = BASE + "(@v)?@v='{1}'&" + SELECT_FIELDS;
            
            /// <summary>
            /// The get current
            /// </summary>
            public const string GET_CURRENT = "{0}/_api/web/CurrentUser?" + SELECT_FIELDS;
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