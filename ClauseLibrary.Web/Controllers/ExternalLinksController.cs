// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System.Web.Http;
using ClauseLibrary.Common;
using ClauseLibrary.Web.Models.DataModel;
using ClauseLibrary.Web.Services;

namespace ClauseLibrary.Web.Controllers
{
    /// <summary>
    /// Manages access to external links.
    /// </summary>
    [ExceptionHandling]
    public class ExternalLinksController : ListItemsController<ExternalLink>
    {
        private readonly IExternalLinksService _externalLinksService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalLinksController"/> class.
        /// </summary>
        public ExternalLinksController(IListItemsRepository<ExternalLink> externalLinksRepository)
            : base(externalLinksRepository)
        {
            _externalLinksService = new ExternalLinksService(externalLinksRepository);
        }

        /// <summary>
        /// Creates the external link.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="externalLink">The external link.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpPost]
        public ExternalLink Create(string webUrl, [FromBody] ExternalLink externalLink, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            return _externalLinksService.CreateExternalLink(externalLink, webUrl, accessToken);
        }

        /// <summary>
        /// Deletes the external link.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="externalLink">The external link.</param>
        /// <param name="accessToken">The access token.</param>
        [HttpPost]
        public string Delete(string webUrl, [FromBody] ExternalLink externalLink, string accessToken = "")
        {
            accessToken = GetAccessToken(accessToken);
            var id = externalLink.Id;
            return _externalLinksService.DeleteExternalLink(id, webUrl, accessToken);
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