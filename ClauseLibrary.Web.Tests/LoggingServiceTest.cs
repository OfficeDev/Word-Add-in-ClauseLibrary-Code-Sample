// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Diagnostics;
using System.IO;
using ClauseLibrary.Common.Services;
using FluentAssertions;
using NUnit.Framework;

namespace ClauseLibrary.Web.Tests
{
    [TestFixture]
    internal class LoggingServiceTest
    {
        [Test]
        public void CanCreateFile()
        {
            var filepath = new LoggingService().CreateLog();
            var fileExists = File.Exists(filepath);

            filepath.Should().NotBeNull();
            Assert.IsTrue(fileExists);
        }

        [Test]
        public void CanWriteLog()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(@"..\..\..\ClauseLibrary.Web\App_Data\Logs"));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Entering Main");
            Trace.WriteLine("Exiting Main");
            Trace.Unindent();
        }

        [Test]
        public void HasCorrectDateTime()
        {
            var filename = Path.GetFileName(new LoggingService().CreateLog());
            var currentDateTime = DateTime.Now.ToString("yy-MM-dd");
            filename.Should().Be("Log_" + currentDateTime + ".txt");
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