// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using ClauseLibrary.Common.Models;
using ClauseLibrary.Web.Models.DataModel;

namespace ClauseLibrary.Web.Tests.Builders
{
    public class FluentClauseBuilder
    {
        private SharePointUser _author = new FluentSharePointUserBuilder().WithTitle("Author").Build();
        private SharePointUser _editor = new FluentSharePointUserBuilder().WithTitle("Editor").Build();
        private int _groupId;
        private DateTime _modified = DateTime.Now;
        private string _tags = "1,2,3";
        private string _text = "text";
        private string _title = "title";
        private static int _uniqueId = 1;

        public FluentClauseBuilder WithAuthor(SharePointUser author)
        {
            _author = author;
            return this;
        }

        public FluentClauseBuilder WithAuthorCalled(string title)
        {
            _author = new FluentSharePointUserBuilder().WithTitle(title).BuildExisting();
            return this;
        }

        public FluentClauseBuilder WithEditor(SharePointUser editor)
        {
            _editor = editor;
            return this;
        }

        public FluentClauseBuilder WithEditorCalled(string title)
        {
            _editor = new FluentSharePointUserBuilder().WithTitle(title).BuildExisting();
            return this;
        }

        public FluentClauseBuilder ForGroup(int groupId)
        {
            _groupId = groupId;
            return this;
        }

        public FluentClauseBuilder WithLastModified(DateTime lastModified)
        {
            _modified = lastModified;
            return this;
        }

        public FluentClauseBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public FluentClauseBuilder WithText(string text)
        {
            _text = text;
            return this;
        }

        public FluentClauseBuilder WithTags(string tags)
        {
            _tags = tags;
            return this;
        }

        public Clause Build()
        {
            return new Clause
            {
                Author = _author,
                Editor = _editor,
                GroupId = _groupId,
                Modified = _modified,
                Tags = _tags,
                Text = _text,
                Title = _title
            };
        }

        public Clause BuildExisting(int? id = null)
        {
            var clause = Build();
            clause.Id = id.GetValueOrDefault(_uniqueId++);
            return clause;
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