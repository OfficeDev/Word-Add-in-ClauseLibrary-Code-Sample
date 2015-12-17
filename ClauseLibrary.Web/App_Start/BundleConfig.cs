// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Web.Optimization;

namespace ClauseLibrary.Web
{
    /// <summary>
    /// Provides bundling services.
    /// </summary>
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-signalr").Include(
                "~/Scripts/jquery-1.9.1.min.js", 
                "~/Scripts/jquery.signalR-2.2.0.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/appdependencies").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/angular.min.js",
                "~/Scripts/angular-ui-router.min.js",
                "~/Scripts/angular-sanitize.min.js",
                "~/Scripts/ui-bootstrap-tpls-0.12.1.min.js",
                "~/Scripts/spin.min.js",
                "~/Scripts/angular-spinner.min.js",
                "~/Scripts/angular-messages.min.js",
                "~/Scripts/angular-indexed-db.js",
                "~/Scripts/async.min.js",
                "~/Scripts/select.min.js",
                "~/Scripts/angulartics.min.js",
                "~/Scripts/angulartics-ga.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/App.js",
                "~/Scripts/Controllers/*.js",
                "~/Scripts/Services/*.js",
                "~/Scripts/Directives/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/welcome").Include(
                 "~/Scripts/welcome.js"
                ,"~/Scripts/Services/clauseLibraryWelcomeServices.js"
                ,"~/Scripts/Services/localStorageService.js"
                ,"~/Scripts/Services/authenticationHubService.js"
                ,"~/Scripts/Services/offlineService.js"
                ,"~/Scripts/Services/notificationService.js"
                ,"~/Scripts/Services/modalService.js"
                ,"~/Scripts/Services/analyticsService.js"
                ,"~/Scripts/Directives/removeClass.js"
                ,"~/Scripts/Controllers/welcomeController.js"
                ,"~/Scripts/Controllers/notificationController.js"
                ,"~/Scripts/Models/AccessInfo.js"
                ));
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