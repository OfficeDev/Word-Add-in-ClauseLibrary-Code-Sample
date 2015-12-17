// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Configuration;

namespace ClauseLibrary.Web
{
    /// <summary>
    /// A helper class to read through settings.
    /// </summary>
    public class SettingsHelper
    {
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"] ??
                                          ConfigurationManager.AppSettings["ida:ClientID"];

        private static string _clientKey = ConfigurationManager.AppSettings["ida:ClientKey"] ??
                                           ConfigurationManager.AppSettings["ida:Password"];

        private static string _useSqlForLoginSettings = ConfigurationManager.AppSettings["UseSqlForLoginSettings"] ?? bool.FalseString;
        private static string _useCookieForTokenStorage = ConfigurationManager.AppSettings["UseCookieForTokenStorage"] ?? bool.FalseString;

        /// <summary>
        /// The graph resource identifier
        /// </summary>
        public const string GRAPH_RESOURCE_ID = "https://graph.windows.net";
        /// <summary>
        /// The authority
        /// </summary>
        public const string AUTHORITY = "https://login.windows.net/";
        /// <summary>
        /// The discovery svc resource identifier
        /// </summary>
        public const string DISCOVERY_SVC_RESOURCE_ID = "https://api.office.com/discovery/";
        /// <summary>
        /// The discovery svc endpoint URI
        /// </summary>
        public const string DISCOVERY_SVC_ENDPOINT_URI = "https://api.office.com/discovery/v1.0/me/";
        /// <summary>
        /// The groups list name
        /// </summary>
        public const string GROUPS_LIST_NAME = "Groups";
        /// <summary>
        /// The clauses list name
        /// </summary>
        public const string CLAUSES_LIST_NAME = "Clauses";
        /// <summary>
        /// The favourites list name
        /// </summary>
        public const string FAVOURITES_LIST_NAME = "Favourites";
        /// <summary>
        /// The external links list name
        /// </summary>
        public const string EXTERNAL_LINKS_LIST_NAME = "ExternalLinks";
        
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public static string ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        /// <summary>
        /// Gets or sets the client key.
        /// </summary>
        public static string ClientKey
        {
            get { return _clientKey; }
            set { _clientKey = value; }
        }

        /// <summary>
        /// Gets the discovery service endpoint URI.
        /// </summary>
        /// <value>
        /// The discovery service endpoint URI.
        /// </value>
        public static Uri DiscoveryServiceEndpointUri
        {
            get { return new Uri(DISCOVERY_SVC_ENDPOINT_URI); }
        }

        /// <summary>
        /// Gets or sets the use SQL for login setting.
        /// </summary>
        public static bool UseSqlForLoginSettings
        {
            get
            {
                bool result;
                Boolean.TryParse(_useSqlForLoginSettings, out result);
                return result;
            }
            set { _useSqlForLoginSettings = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use cookie for token storage.
        /// </summary>
        public static bool UseCookieForTokenStorage
        {
            get
            {
                bool result;
                Boolean.TryParse(_useCookieForTokenStorage, out result);
                return result;
            }
            set { _useCookieForTokenStorage = value.ToString(); }
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