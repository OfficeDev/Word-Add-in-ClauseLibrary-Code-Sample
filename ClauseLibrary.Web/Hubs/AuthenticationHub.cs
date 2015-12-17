// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ClauseLibrary.Web.Hubs
{
    /// <summary>
    /// A hub which broadcasts notifications.
    /// </summary>
    [HubName("authenticationHub")]
    public class AuthenticationHub : Hub
    {
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        public void SendMessage(string clientId, string message)
        {
            //send the message to the specific client passed in
            Clients.Client(clientId).sendMessage(message);
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