// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using ClauseLibrary.Common.Models;
using ClauseLibrary.Web.Models.DataModel;

namespace ClauseLibrary.Web.Tests.Builders
{
    public class FluentGroupBuilder
    {
        private SharePointUser _author = new FluentSharePointUserBuilder().WithTitle("Author").Build();
        private string _title = "title";
        private static int _uniqueId = 1;
        private int _parentId;

        public FluentGroupBuilder WithAuthor(SharePointUser author)
        {
            _author = author;
            return this;
        }

        public FluentGroupBuilder WithAuthorCalled(string title)
        {
            _author = new FluentSharePointUserBuilder().WithTitle(title).BuildExisting();
            return this;
        }

        public FluentGroupBuilder WithParent(int parentId)
        {
            _parentId = parentId;
            return this;
        }

        public FluentGroupBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public Group Build()
        {
            return new Group
            {
                Author = _author,
                ParentId = _parentId,
                Title = _title
            };
        }

        public Group BuildExisting(int? id = null)
        {
            Group group = Build();
            group.Id = id.GetValueOrDefault(_uniqueId++);
            return group;
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