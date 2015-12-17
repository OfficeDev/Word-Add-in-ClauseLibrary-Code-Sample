// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace ClauseLibrary.Common.Services
{
    /// <summary>
    /// Provides logging.
    /// </summary>
    public class LoggingService : ExceptionLogger
    {
        private const string LOG_PATH = @"Logs\";
        private const int RETRY_COUNT = 10;
        private const int DEFAULT_TAB = 4;
        private readonly string _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingService"/> class.
        /// </summary>
        public LoggingService()
        {
            _root = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : GetProjectDirectory();
        }

        /// <summary>
        /// Gets the project's root directory if not in an HttpContext (i.e. if we are testing)
        /// </summary>
        /// <returns>Returns the projects root directory as a string</returns>
        private static string GetProjectDirectory()
        {
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            return directoryInfo != null ? directoryInfo.FullName : null;
        }

        /// <summary>
        /// Writes an exception message and stack trace to the log
        /// </summary>
        /// <param name="e"></param>
        public void LogException(Exception e)
        {
            LogException(e, null);
        }

        /// <summary>
        /// An overrride which takes an HttpWebResponse as a parameter
        /// </summary>
        /// <param name="e"></param>
        /// <param name="response"></param>
        public void LogException(Exception e, HttpWebResponse response)
        {
            LogException(e, response, 0);
        }

        /// <summary>
        /// Takes an exception and a HttpWebResponse and logs the exception message
        /// and stack trace along with the web response
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="response">The response.</param>
        /// <param name="retry">The retry.</param>
        public void LogException(Exception e, HttpWebResponse response, int retry)
        {
            try
            {
                using (var stream = new StreamWriter(GetCurrentLogPath(), true))
                {
                    stream.WriteLine(LogLineDateSeparator());
                    stream.WriteLine("MESSAGE: ");
                    stream.WriteLine(e.Message);
                    stream.WriteLine(Environment.NewLine);
                    if (response != null && response.Headers != null)
                    {
                        stream.WriteLine("Response: ");
                        foreach (var headerItem in response.Headers.Keys)
                        {
                            stream.WriteLine(Tab() + headerItem + ": ");
                            stream.WriteLine(Tab(6) + response.Headers.Get(headerItem.ToString()));
                        }
                        stream.WriteLine(Environment.NewLine);
                    }
                    stream.WriteLine("Stack trace: ");
                    stream.WriteLine(e.StackTrace);
                    stream.WriteLine(LogLineDateSeparator());
                }
            }
            catch (IOException)
            {
                if (retry < RETRY_COUNT)
                {
                    LogException(e, response, ++retry);
                }
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogMessage(string message)
        {
            LogMessage(message, 0);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="retry">The retry.</param>
        public void LogMessage(string message, int retry)
        {
            try
            {
                using (var stream = new StreamWriter(GetCurrentLogPath(), true))
                {
                    stream.WriteLine(LogLineDateSeparator());
                    stream.WriteLine(message);
                    stream.WriteLine(LogLineDateSeparator());
                }
            }
            catch (IOException)
            {
                if (retry < RETRY_COUNT)
                    LogMessage(message, ++retry);
            }
        }

        private string LogLineDateSeparator()
        {
            return Environment.NewLine +
                   "------------------" +
                   DateTime.Now +
                   "------------------" +
                   Environment.NewLine;
        }

        private string Tab(int number = DEFAULT_TAB)
        {
            var str = "";
            for (var i = 0; i < number; i++)
            {
                str += " ";
            }
            return str;
        }

        /// <summary>
        /// Generates a .txt file in the project root App_Data\Logs directory
        /// </summary>
        /// <returns>Returns the created filepath</returns>
        public string CreateLog()
        {
            var filepath = GetCurrentLogPath();
            if (!Directory.Exists(GetLogPath()))
            {
                Directory.CreateDirectory(GetLogPath());
            }
            File.Create(filepath).Close();
            return filepath;
        }

        /// <summary>
        /// Builds a path from the root directory to the logs directory
        /// </summary>
        /// <returns>Returns the path to the logs directory as a string</returns>
        public string GetLogPath()
        {
            return Path.Combine(_root, LOG_PATH);
        }

        /// <summary>
        /// Takes the current date in the format 'yy-MM-dd' and generates the
        /// full path to the log with the given date
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns>returns the full path to the log with the given date</returns>
        public string GetLogPath(string dateString)
        {
            var filename = "Log_" + dateString + ".txt";
            var projectRoot = GetLogPath();
            return Path.Combine(projectRoot, filename);
        }

        /// <summary>
        /// Generates a path to today's log
        /// </summary>
        /// <returns>Returns a path to the current log as a string</returns>
        public string GetCurrentLogPath()
        {
            return GetLogPath(GetCurrentDate(DateTime.Now));
        }

        /// <summary>
        /// Gets a formatted date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns the current year month day</returns>
        private string GetCurrentDate(DateTime date)
        {
            return date.ToString("yy-MM-dd");
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