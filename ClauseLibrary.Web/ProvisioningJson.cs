// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

namespace ClauseLibrary.Web
{
    /// <summary>
    /// Predefined strings for provisioning lists and fields.
    /// </summary>
    public static class ProvisioningJson
    {
        /// <summary>
        /// Webs
        /// </summary>
        public static class Webs
        {
            /// <summary>
            /// The clause library format
            /// </summary>
            public static string ClauseLibraryFormat =
                "{{'parameters':{{'__metadata':{{'type':'SP.WebCreationInformation'}},'Title':'{0}','Url':'{1}','WebTemplate':'STS'}}}}";
                // ,'UseSamePermissionsAsParentSite': true
        }

        /// <summary>
        /// Lists
        /// </summary>
        public static class Lists
        {
            /// <summary>
            /// The clauses
            /// </summary>
            public static string Clauses =
                "{ '__metadata': { 'type': 'SP.List' }, 'BaseTemplate': 100, 'Description': 'Clauses', 'Title': 'Clauses' }";

            /// <summary>
            /// The groups
            /// </summary>
            public static string Groups =
                "{ '__metadata': { 'type': 'SP.List' }, 'BaseTemplate': 100, 'Description': 'Clause Library Groups List', 'Title': 'Groups' }";

            /// <summary>
            /// The tags
            /// </summary>
            public static string Tags =
                "{ '__metadata': { 'type': 'SP.List' }, 'BaseTemplate': 100, 'Description': 'Clause Library Tags List', 'Title': 'Tags' }";

            /// <summary>
            /// The favourites
            /// </summary>
            public static string Favourites =
                "{ '__metadata': { 'type': 'SP.List' }, 'BaseTemplate': 100, 'Description': 'Clause Library Favourites List', 'Title': 'Favourites' }";

            /// <summary>
            /// The external links
            /// </summary>
            public static string ExternalLinks =
                "{ '__metadata': { 'type': 'SP.List' }, 'BaseTemplate': 100, 'Description': 'Clause Library External Links List', 'Title': 'ExternalLinks' }";
        }

        /// <summary>
        /// Fields
        /// </summary>
        public static class Fields
        {
            /// <summary>
            /// The text
            /// </summary>
            public static string Text = "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'Text', 'FieldTypeKind': 3}";
            /// <summary>
            /// The URL
            /// </summary>
            public static string Url = "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'Url', 'FieldTypeKind': 3}";
            /// <summary>
            /// The tags
            /// </summary>
            public static string Tags = "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'Tags', 'FieldTypeKind': 3}";

            /// <summary>
            /// The clause identifier
            /// </summary>
            public static string ClauseId =
                "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'ClauseId', 'FieldTypeKind': 9}";

            /// <summary>
            /// The parent identifier
            /// </summary>
            public static string ParentId =
                "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'ParentId', 'FieldTypeKind': 9}";

            /// <summary>
            /// The group identifier
            /// </summary>
            public static string GroupId =
                "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'GroupId', 'FieldTypeKind': 9 }";

            /// <summary>
            /// The designees
            /// </summary>
            public static string Designees =
                "{ '__metadata': { 'type': 'SP.FieldUser' }, 'Title': 'Designees', 'FieldTypeKind': 20, 'AllowMultipleValues': true, 'SelectionMode': 0 }";

            /// <summary>
            /// The is locked
            /// </summary>
            public static string IsLocked =
                "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'IsLocked', 'FieldTypeKind': 8 }";

            /// <summary>
            /// The owner
            /// </summary>
            public static string Owner =
                "{ '__metadata': { 'type': 'SP.FieldUser' }, 'Title': 'Owner', 'FieldTypeKind': 20, 'SelectionMode': 0 }";

            /// <summary>
            /// The usage guidelines
            /// </summary>
            public static string UsageGuidelines =
                "{ '__metadata': { 'type': 'SP.Field' }, 'Title': 'UsageGuidelines', 'FieldTypeKind': 3}";
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