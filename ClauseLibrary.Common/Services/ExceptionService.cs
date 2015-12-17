// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Net;

namespace ClauseLibrary.Common.Services
{
    /// <summary>
    /// An exception handling service.
    /// </summary>
    public class ExceptionService
    {
        private readonly LoggingService _log = new LoggingService();

        /// <summary>
        /// A generic error.
        /// </summary>
        public const string GENERIC = "Something went wrong with your request:";
        /// <summary>
        /// The not found error.
        /// </summary>
        public const string NOT_FOUND = "The resource you requested could not be found:";
        
        /// <summary>
        /// The unauthorized error.
        /// </summary>
        public const string UNAUTHORIZED = "You are not authorized to make this request:";
        
        /// <summary>
        /// The internal error.
        /// </summary>
        public const string INTERNAL_ERROR = "An error occurred on the server:";
        
        /// <summary>
        /// The invalid url or token error.
        /// </summary>
        public const string INVALID_URL_OR_TOKEN = "The SharePoint URL or access token is invalid:";

        /// <summary>
        /// Returns an error message for a specified <see param="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public string WebResponseMessage(HttpStatusCode statusCode)
        {
            return WebResponseMessage(statusCode, string.Empty);
        }

        /// <summary>
        /// Returns an error message for a specified <see param="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public string WebResponseMessage(HttpStatusCode statusCode, string reason)
        {
            var code = (int) statusCode;
            string str;
            if (reason == string.Empty)
            {
                switch (code)
                {
                    case 401:
                        str = UNAUTHORIZED;
                        break;
                    case 404:
                        str = NOT_FOUND;
                        break;
                    case 500:
                        str = INTERNAL_ERROR;
                        break;
                    default:
                        str = GENERIC;
                        break;
                }
            }
            else
            {
                str = reason;
            }
            return string.Format("{0}: ({1}) {2}", str, code, statusCode);
        }

        /// <summary>
        /// Logs a web exception.
        /// </summary>
        public WebException WebExceptionHandler(WebException exception)
        {
            return WebExceptionHandler(exception, null);
        }

        /// <summary>
        /// Logs a web exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="reason">The reason.</param>
        public WebException WebExceptionHandler(WebException exception, string reason)
        {
            if (exception == null) return InternalServerError();

            var response = (HttpWebResponse) exception.Response;
            if (response == null) return InternalServerError();

            var statusCode = response.StatusCode;
            _log.LogException(exception, response);
            return new WebException(WebResponseMessage(statusCode, reason));
        }

        /// <summary>
        /// Handles an internal server error.
        /// </summary>
        private WebException InternalServerError()
        {
            return new WebException(WebResponseMessage(HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// Handles an exception when the url or token is invalid.
        /// </summary>
        /// <returns></returns>
        public ArgumentException InvalidUrlOrToken()
        {
            return new ArgumentException(INVALID_URL_OR_TOKEN);
        }

        /// <summary>
        /// Gets the failure reason.
        /// </summary>
        public string GetFailureReason(WebException exception)
        {
            if (exception == null) return string.Empty;

            var response = (HttpWebResponse) exception.Response;
            if (response == null) return string.Empty;

            var headers = response.Headers;
            if (headers == null) return string.Empty;

            var diagnostics = headers.Get("x-ms-diagnostics");
            if (diagnostics == null) return string.Empty;

            var arr = diagnostics.Split(';');
            if (arr.Length == 0) return string.Empty;

            var reason = string.Empty;
            foreach (var item in arr)
            {
                if (item == null || !item.Contains("=")) continue;
                var innerArr = item.Split('=');

                if (innerArr.Length < 2) continue;
                var key = innerArr[0];

                if (key != "reason") continue;

                if (innerArr[1] == null) continue;
                reason = innerArr[1].Replace("\"", string.Empty);
            }
            return reason;
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