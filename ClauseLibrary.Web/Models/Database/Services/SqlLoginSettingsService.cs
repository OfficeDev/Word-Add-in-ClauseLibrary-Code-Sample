// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Data.Entity;
using System.Linq;
using ClauseLibrary.Web.Models.Database.LoginSettings;

namespace ClauseLibrary.Web.Models.Database.Services
{
    /// <summary>
    /// Provides access to the login settings.
    /// </summary>
    public class SqlLoginSettingsService : ILoginSettingsService
    {
        private readonly LoginSettingsContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLoginSettingsService"/> class.
        /// </summary>
        public SqlLoginSettingsService()
        {
            _context = new LoginSettingsContext();
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public User GetUserById(Guid userId)
        {
            return _context.Users
                .Include(u => u.DefaultLibrary)
                .Include(u => u.Tenant)
                .FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Gets the tenant by identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        public Tenant GetTenantById(Guid tenantId)
        {
            return _context.Tentants.FirstOrDefault(t => t.TenantId == tenantId);
        }

        /// <summary>
        /// Gets the library by identifier.
        /// </summary>
        /// <param name="libraryId">The library identifier.</param>
        public Library GetLibraryById(Guid libraryId)
        {
            return _context.Libraries.FirstOrDefault(l => l.LibraryId == libraryId);
        }

        /// <summary>
        /// Adds the specified new entity.
        /// </summary>
        public void Add<T>(T newEntity) where T : class
        {
            _context.Set<T>().Add(newEntity);
        }

        /// <summary>
        /// Deletes the specified entity to delete.
        /// </summary>
        public void Delete<T>(T entityToDelete) where T : class
        {
            _context.Set<T>().Remove(entityToDelete);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
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