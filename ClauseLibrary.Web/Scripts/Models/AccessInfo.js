// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

window.ClauseLibrary = {};

ClauseLibrary.AccessInfo = function(accessInfo) {

    var namespace = "clauselibrary.";

    // ACCESS INFO EXISTS
    this.IsSet = accessInfo ? true : false;

    // CONNECTION INFO
    this.HostWebUrl     = accessInfo && accessInfo.HostWebUrl || ""; // URL to the library in sharepoint
    this.SPTenantUrl    = accessInfo && accessInfo.SPTenantUrl || ""; // Root URL of sharepoint tenant
    this.TenantId       = accessInfo && accessInfo.TenantId || ""; // unique GUID of tenant
    this.DefaultLibrary =  accessInfo && accessInfo.DefaultLibrary ? {
        Name            : accessInfo.DefaultLibrary.Name || "", // name of library
        Description     : accessInfo.DefaultLibrary.Description || "", // description of library
        HostWebUrl      : accessInfo.DefaultLibrary.HostWebUrl || "", // URL to sharepoint resource
        LibraryId       : accessInfo.DefaultLibrary.LibraryId || "", // unique GUID
        TenantId        : accessInfo.DefaultLibrary.TenantId || "" // tenant GUID
    }: null;

    // USER INFO
    this.IsAdmin        = accessInfo && accessInfo.IsAdmin || false; // whether the current user is a site collection admin
    this.User = {
        EMail           : accessInfo && accessInfo.User && accessInfo.User.EMail || "", // user's email address
        Id              : accessInfo && accessInfo.User && accessInfo.User.Id || "", // user's sharepoint id
        Title           : accessInfo && accessInfo.User && accessInfo.User.Title || "", // Display name of user
        _metadata       : accessInfo && accessInfo.User && accessInfo.User._metadata || {} // sharepoint user metadata
    }
    this.UserEmail      = accessInfo && accessInfo.UserEmail || ""; // email of currently logged in user
    this.UserId         = accessInfo && accessInfo.UserId || ""; // unique GUID of currently logged in user

    // AUTHENTICATION INFO
    this.AccessToken    = accessInfo && accessInfo.AccessToken || ""; // access token
    this.RefreshToken = accessInfo && accessInfo.RefreshToken || ""; // refresh token
    this.ExpiresOn = accessInfo && accessInfo.ExpiresOn || ""; // datetime of access token expiration

    // LOCAL DB
    this.CurrentLocalDB = accessInfo && accessInfo.DefaultLibrary && 'db-' + accessInfo.DefaultLibrary.HostWebUrl;

    // Stores all of the current AccessInfo data in local storage
    this.store = function() {

        // NOTE: complex objects must be stringified before insertion
        localStorage.setItem(namespace + "IsSet", this.IsSet);
        localStorage.setItem(namespace + "HostWebUrl", this.HostWebUrl);
        localStorage.setItem(namespace + "SPTenantUrl", this.SPTenantUrl);
        localStorage.setItem(namespace + "TenantId", this.TenantId);
        localStorage.setItem(namespace + "DefaultLibrary", (this.DefaultLibrary ? JSON.stringify(this.DefaultLibrary) : null));
        localStorage.setItem(namespace + "IsAdmin", this.IsAdmin);
        localStorage.setItem(namespace + "User", JSON.stringify(this.User));
        localStorage.setItem(namespace + "UserEmail", this.UserEmail);
        localStorage.setItem(namespace + "UserId", this.UserId);
        localStorage.setItem(namespace + "AccessToken", this.AccessToken);
        localStorage.setItem(namespace + "RefreshToken", this.RefreshToken);
        localStorage.setItem(namespace + "ExpiresOn", this.ExpiresOn);
        localStorage.setItem(namespace + "CurrentLocalDB", this.CurrentLocalDB);
    }

    return this;
}

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