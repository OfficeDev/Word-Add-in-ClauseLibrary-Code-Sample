// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Net;
using ClauseLibrary.Common.Models;
using ClauseLibrary.Common.Services;
using Newtonsoft.Json;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Service to interact with SharePoint
    /// </summary>
    public class SharePointService : ISharePointService
    {
        private static readonly ExceptionService ExceptionService = new ExceptionService();

        #region Web Methods

        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <param name="tenantUrl">The tenant URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        public List<Site> GetSites(string tenantUrl, string accessToken, SPBasePermissions effectiveBasePermissions)
        {
            var url = String.Format(SpApiConstants.Webs.SEARCH_SITES, tenantUrl);
            var rawResponse = GetString(url, accessToken);
            var response = JsonConvert.DeserializeObject<RestSitesResponse>(rawResponse);
            if (response == null || response.d == null)
                return null;

            return response.GetSitesList();
        }

        /// <summary>
        /// Gets the web.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="effectiveBasePermissions">The effective base permissions.</param>
        public Web GetWeb(string webUrl, string accessToken, SPBasePermissions effectiveBasePermissions)
        {
            var url = string.Format(SpApiConstants.Webs.GET_SINGLE_PERMISSION_TRIMMED, webUrl, (int) effectiveBasePermissions);
            var response = JsonConvert.DeserializeObject<RestItemResponse<Web>>(GetString(url, accessToken));

            return response.d;
        }

        #endregion

        #region List Methods

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="listJson">The list json.</param>
        public string CreateList(string webUrl, string accessToken, string listJson)
        {
            var url = String.Format(SpApiConstants.Lists.POST, webUrl);
            return PostString(url, accessToken, listJson);
        }

        /// <summary>
        /// Creates the list field.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listTitle">The list title.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="fieldJson">The field json.</param>
        public string CreateListField(string webUrl, string listTitle, string accessToken, string fieldJson)
        {
            var url = String.Format(SpApiConstants.Lists.CREATE_FIELD, webUrl, listTitle);
            return PostString(url, accessToken, fieldJson);
        }

        #endregion

        #region ListItem Methods

        /// <summary>
        /// Gets the list items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="query">The query.</param>
        public IEnumerable<T> GetListItems<T>(string webUrl, string listName, string accessToken, string query = "")
            where T : ListItem, new()
        {
            var t = new T();
            var url = String.Format(SpApiConstants.ListItems.GET_ALL + query, webUrl, listName, t.Fields);

            if (!String.IsNullOrEmpty(t.ExpandFields))
                url += String.Format("&" + SpApiConstants.ListItems.EXPAND, t.ExpandFields);

            var results = GetString(url, accessToken);
            if (results == null) return null;

            var response = JsonConvert.DeserializeObject<RestItemsResponse<T>>(results);
            return response.d.results;
        }

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        public T GetListItem<T>(string webUrl, string listName, int id, string accessToken) where T : ListItem, new()
        {
            return GetListItem<T>(webUrl, listName, id, accessToken, "");
        }

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expand">The expand.</param>
        public T GetListItem<T>(string webUrl, string listName, int id, string accessToken, string expand)
            where T : ListItem, new()
        {
            var url = string.IsNullOrWhiteSpace(expand)
                ? String.Format(SpApiConstants.ListItems.GET, webUrl, listName, id, new T().Fields)
                : String.Format(SpApiConstants.ListItems.GET_EXPANDED, webUrl, listName, id, new T().Fields, expand);

            return JsonConvert.DeserializeObject<RestItemResponse<T>>(GetString(url, accessToken)).d;
        }

        /// <summary>
        /// Gets the lists.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        public List<SharePointList> GetLists(string webUrl, string accessToken)
        {
            var url = String.Format(SpApiConstants.Lists.LIST_TITLES, webUrl);
            var response = JsonConvert.DeserializeObject<RestItemsResponse<SharePointList>>(GetString(url, accessToken));
            if (response == null || response.d == null || response.d.results == null) return null;
            return response.d.results;
        }

        /// <summary>
        /// Posts the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="listItem">The list item.</param>
        public T PostListItem<T>(string webUrl, string listName, string accessToken, T listItem)
            where T : ListItem, new()
        {
            var url = String.Format(SpApiConstants.ListItems.POST, webUrl, listName);
            var serialisedData = JsonConvert.SerializeObject(
                listItem, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            var results = PostString(url, accessToken, serialisedData);
            var deserializedResults = JsonConvert.DeserializeObject<ListItemResponse<T>>(results);
            return deserializedResults.d;
        }

        /// <summary>
        /// Merges the list item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="listItem">The list item.</param>
        public string MergeListItem<T>(string webUrl, string listName, string accessToken, T listItem)
            where T : ListItem
        {
            var url = String.Format(SpApiConstants.ListItems.MERGE, webUrl, listName, listItem.Id);
            return MergeString(url, accessToken, JsonConvert.SerializeObject(
                listItem, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }

        /// <summary>
        /// Deletes the list item.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="id">The identifier.</param>
        public string DeleteListItem(string webUrl, string listName, string accessToken, int id)
        {
            var url = String.Format(SpApiConstants.ListItems.DELETE, webUrl, listName, id);
            return Delete(url, accessToken);
        }

        /// <summary>
        /// Gets the list item count.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="accessToken">The access token.</param>
        public int GetListItemCount(string webUrl, string listName, string accessToken)
        {
            var url = String.Format(SpApiConstants.ListItems.COUNT, webUrl, listName);
            return
                JsonConvert.DeserializeObject<RestItemResponse<ItemCountResponse>>(GetString(url, accessToken))
                    .d.ItemCount;
        }

        #endregion

        #region User Methods

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="accessToken">The access token.</param>
        public SharePointUser GetCurrentUser(string webUrl, string accessToken)
        {
            var url = String.Format(SpApiConstants.Users.GET_CURRENT, webUrl);
            return JsonConvert.DeserializeObject<RestItemResponse<SharePointUser>>(GetString(url, accessToken)).d;
        }

        #endregion

        private static string GetString(string url, string accessToken)
        {
            using (var c = new WebClient())
            {
                c.Headers[HttpRequestHeader.Authorization] =
                    String.Format(SpApiConstants.RequestHeaders.AUTHORIZATIONTEMPLATE, accessToken);
                c.Headers[HttpRequestHeader.Accept] = SpApiConstants.RequestHeaders.JSON_CONTENT;

                try
                {
                    var results = c.DownloadString(url);
                    return results;
                }
                catch (WebException exception)
                {
                    var reason = ExceptionService.GetFailureReason(exception);
                    throw ExceptionService.WebExceptionHandler(exception, reason);
                }
            }
        }

        private static string PostString(string url, string accessToken, string data)
        {
            using (var c = new WebClient())
            {
                c.Headers[HttpRequestHeader.Authorization] =
                    String.Format(SpApiConstants.RequestHeaders.AUTHORIZATIONTEMPLATE, accessToken);
                c.Headers[HttpRequestHeader.Accept] = SpApiConstants.RequestHeaders.JSON_CONTENT;
                c.Headers[HttpRequestHeader.ContentType] = SpApiConstants.RequestHeaders.JSON_CONTENT;
                try
                {
                    return c.UploadString(url, data);
                }
                catch (WebException exception)
                {
                    var reason = ExceptionService.GetFailureReason(exception);
                    throw ExceptionService.WebExceptionHandler(exception, reason);
                }
            }
        }

        private static string MergeString(string url, string accessToken, string data)
        {
            using (var c = new WebClient())
            {
                c.Headers[HttpRequestHeader.Authorization] =
                    String.Format(SpApiConstants.RequestHeaders.AUTHORIZATIONTEMPLATE, accessToken);
                c.Headers[HttpRequestHeader.ContentType] = SpApiConstants.RequestHeaders.JSON_CONTENT;
                c.Headers[HttpRequestHeader.IfMatch] = "*";
                c.Headers[SpApiConstants.RequestHeaders.X_HTTP_METHOD] = SpApiConstants.RequestHeaders.MERGE;

                try
                {
                    return c.UploadString(url, data);
                }
                catch (WebException exception)
                {
                    var reason = ExceptionService.GetFailureReason(exception);
                    throw ExceptionService.WebExceptionHandler(exception, reason);
                }
            }
        }

        private static string Delete(string url, string accessToken)
        {
            using (var c = new WebClient())
            {
                c.Headers[HttpRequestHeader.Authorization] =
                    String.Format(SpApiConstants.RequestHeaders.AUTHORIZATIONTEMPLATE, accessToken);
                c.Headers[HttpRequestHeader.IfMatch] = "*";
                c.Headers[SpApiConstants.RequestHeaders.X_HTTP_METHOD] = SpApiConstants.RequestHeaders.DELETE;

                try
                {
                    return c.UploadString(url, String.Empty);
                }
                catch (WebException exception)
                {
                    var reason = ExceptionService.GetFailureReason(exception);
                    throw ExceptionService.WebExceptionHandler(exception, reason);
                }
            }
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