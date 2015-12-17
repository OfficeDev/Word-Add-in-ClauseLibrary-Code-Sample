// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Collections.Generic;
using ClauseLibrary.Web.Models;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// A contract for accessing list items generically.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IListItemRequestService<T>
    {
        /// <summary>
        /// Creates items in the given item request model
        /// </summary>
        /// <param name="itemRequestModel"></param>
        /// <param name="webUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        T Create(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail,
            bool isUserAdmin);

        /// <summary>
        /// Updates items in the given item request model
        /// </summary>
        /// <param name="itemRequestModel"></param>
        /// <param name="webUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        T Update(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail,
            bool isUserAdmin);

        /// <summary>
        /// Deletes the item with the given id
        /// </summary>
        /// <param name="itemRequestModel"></param>
        /// <param name="webUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        T Delete(ItemRequestModel itemRequestModel, string webUrl, string accessToken, string currentUserEmail,
            bool isUserAdmin);

        /// <summary>
        /// Retrieves all items from SharePoint
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="query"></param>
        /// <param name="accessToken"></param>
        /// <param name="currentUserEmail"></param>
        /// <param name="isUserAdmin"></param>
        /// <returns></returns>
        IEnumerable<T> GetAll(string webUrl, string query, string accessToken, string currentUserEmail, bool isUserAdmin);
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