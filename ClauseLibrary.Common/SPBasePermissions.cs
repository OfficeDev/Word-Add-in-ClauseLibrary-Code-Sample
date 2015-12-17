// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// SharePoint permission flags.
    /// </summary>
    [Flags]
    public enum SPBasePermissions : long
    {
        /// <summary>
        /// empty mask
        /// </summary>
        EmptyMask = 0,
        /// <summary>
        /// view list items
        /// </summary>
        ViewListItems = 1,
        /// <summary>
        /// add list items
        /// </summary>
        AddListItems = 2,
        /// <summary>
        /// edit list items
        /// </summary>
        EditListItems = 4,
        /// <summary>
        /// delete list items
        /// </summary>
        DeleteListItems = 8,
        /// <summary>
        /// approve items
        /// </summary>
        ApproveItems = 16,
        /// <summary>
        /// open items
        /// </summary>
        OpenItems = 32,
        /// <summary>
        /// view versions
        /// </summary>
        ViewVersions = 64,
        /// <summary>
        /// delete versions
        /// </summary>
        DeleteVersions = 128,
        /// <summary>
        /// cancel checkout
        /// </summary>
        CancelCheckout = 256,
        /// <summary>
        /// manage personal views
        /// </summary>
        ManagePersonalViews = 512,
        /// <summary>
        /// manage lists
        /// </summary>
        ManageLists = 2048,
        /// <summary>
        /// view form pages
        /// </summary>
        ViewFormPages = 4096,
        /// <summary>
        /// open
        /// </summary>
        Open = 65536,
        /// <summary>
        /// view pages
        /// </summary>
        ViewPages = 131072,
        /// <summary>
        /// add and customize pages
        /// </summary>
        AddAndCustomizePages = 262144,
        /// <summary>
        /// apply theme and border
        /// </summary>
        ApplyThemeAndBorder = 524288,
        /// <summary>
        /// apply style sheets
        /// </summary>
        ApplyStyleSheets = 1048576,
        /// <summary>
        /// view usage data
        /// </summary>
        ViewUsageData = 2097152,
        /// <summary>
        /// create SSC site
        /// </summary>
        CreateSSCSite = 4194304,
        /// <summary>
        /// manage subwebs
        /// </summary>
        ManageSubwebs = 8388608,
        /// <summary>
        /// create groups
        /// </summary>
        CreateGroups = 16777216,
        /// <summary>
        /// manage permissions
        /// </summary>
        ManagePermissions = 33554432,
        /// <summary>
        /// browse directories
        /// </summary>
        BrowseDirectories = 67108864,
        /// <summary>
        /// browse user information
        /// </summary>
        BrowseUserInfo = 134217728,
        /// <summary>
        /// add delete private web parts
        /// </summary>
        AddDelPrivateWebParts = 268435456,
        /// <summary>
        /// update personal web parts
        /// </summary>
        UpdatePersonalWebParts = 536870912,
        /// <summary>
        /// manage web
        /// </summary>
        ManageWeb = 1073741824,
        /// <summary>
        /// use remote ap is
        /// </summary>
        UseRemoteAPIs = 137438953472,
        /// <summary>
        /// manage alerts
        /// </summary>
        ManageAlerts = 274877906944,
        /// <summary>
        /// create alerts
        /// </summary>
        CreateAlerts = 549755813888,
        /// <summary>
        /// edit my user information
        /// </summary>
        EditMyUserInfo = 1099511627776,
        /// <summary>
        /// enumerate permissions
        /// </summary>
        EnumeratePermissions = 4611686018427380000,
        /// <summary>
        /// full mask
        /// </summary>
        FullMask = 9223372036854770000
    };
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