// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using Newtonsoft.Json;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Abstract ListItem class contains fields required for any ListItem Data Model.
    /// </summary>
    public abstract class ListItem
    {
        /// <summary>
        /// Comma separated list of fields to request for ListItem.
        /// </summary>
        [JsonIgnore]
        public string Fields { get; set; }

        /// <summary>
        /// Comma seperated list of fields to expand for ListItem
        /// </summary>
        [JsonIgnore]
        public string ExpandFields { get; set; }

        /// <summary>
        /// Indicates whether object is being transmitted to client (vs. to SharePoint). Used for Serialization
        /// </summary>
        [JsonIgnore]
        public bool ToClient { get; set; }

        /// <summary>
        /// Contains type property of ListItem for Creating and Upating ListItem.
        /// </summary>
        public MetaData __metadata { get; set; }

        /// <summary>
        /// Id of ListItem for Updating and Deleting ListItem.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fields">Comma separated list of fields to request for ListItem.</param>
        /// <param name="listItemType">Type of ListItem in the form 'SP.Data.[:List Name]ListItem'</param>
        /// <param name="expandFields">Comma seperated list of fields to expand for ListItem</param>
        protected ListItem(string fields, string listItemType, string expandFields = null)
        {
            Fields = "Id," + fields;
            ExpandFields = expandFields;
            ToClient = false;
            __metadata = new MetaData(listItemType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItem"/> class.
        /// </summary>
        protected ListItem(bool toClient)
        {
            ToClient = toClient;
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