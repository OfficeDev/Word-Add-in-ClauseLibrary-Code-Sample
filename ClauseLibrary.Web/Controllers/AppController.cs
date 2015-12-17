// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Web.Mvc;
using ClauseLibrary.Web.ViewModels;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// A controller for the app.  Surfaces views for Angular (with some server side binding as necessary).
    /// </summary>
    public class AppController : Controller
    {
        /// <summary>
        /// The root page for the application.
        /// Get: /app
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// The error view.
        /// </summary>
        public ActionResult Error(Exception exception)
        {
            return
                View(new IndexViewModel(exception, "App", "Unknown")
                {
                    Error =
                        string.Format("Unable to acquire access token to SharePoint services.  The error was: {0}",
                            exception)
                });
        }

        /// <summary>
        /// The initial landing page when unauthenticated.
        /// </summary>
        public ActionResult Welcome()
        {
            return View(new WelcomeViewModel {SessionId = Session.SessionID});
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