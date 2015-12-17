// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Linq;
using System.Web.Http;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// A base shared controller for API calls.
    /// </summary>
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        protected string GetAccessToken(string defaultAccessToken)
        {
            if (string.IsNullOrWhiteSpace(defaultAccessToken))
            {
                var bearerToken = Request.Headers.Authorization != null ? Request.Headers.Authorization.ToString() : "";
                defaultAccessToken = bearerToken.Replace(SpApiConstants.RequestHeaders.AUTHORIZATION, "");
            }
            return defaultAccessToken;
        }

        /// <summary>
        /// Gets the current user email.
        /// </summary>
        protected string GetCurrentUserEmail()
        {
            try
            {
                return Request.Headers.GetValues("CurrentUserEmail").First() ?? "";
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred. Please sign out and back in.", e);
            }
        }

        /// <summary>
        /// Determines whether the current user is an admin.
        /// </summary>
        protected bool IsUserAdmin()
        {
            try
            {
                return Request.Headers.GetValues("IsAdmin").First() == "true";
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred. Please sign out and back in.",e);
            }
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