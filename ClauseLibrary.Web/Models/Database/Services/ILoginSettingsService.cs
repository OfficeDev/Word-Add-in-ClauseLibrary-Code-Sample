// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using ClauseLibrary.Web.Models.Database.LoginSettings;

namespace ClauseLibrary.Web.Models.Database.Services
{
    /// <summary>
    /// A contract for providing login setting services.
    /// </summary>
    public interface ILoginSettingsService : IDisposable
    {
        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        User GetUserById(Guid userId);

        /// <summary>
        /// Gets the tenant by identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        Tenant GetTenantById(Guid tenantId);

        /// <summary>
        /// Gets the library by identifier.
        /// </summary>
        /// <param name="libraryId">The library identifier.</param>
        Library GetLibraryById(Guid libraryId);
        
        /// <summary>
        /// Adds the specified new entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newEntity">The new entity.</param>
        void Add<T>(T newEntity) where T : class;
        
        /// <summary>
        /// Deletes the specified entity to delete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete<T>(T entityToDelete) where T : class;

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();
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