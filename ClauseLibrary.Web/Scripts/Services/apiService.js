// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('api.service', [])
        .service('apiService', apiService);

    apiService['$inject'] = ['$q', '$http', 'sessionService'];

    function apiService($q, $http, sessionService) {

        function buildRequestUrl(controller, action, params, useOverrideUrl) {
            var additionalParams = [];
            for (var p in params) {
                additionalParams.push(['&', p, '=', params[p]].join(''));
            }

            // most requests require a webUrl endpoint to use in the request;
            // the URL is usually the URL to the currently connected library.
            // however, some requests may require a different url (such as the
            // root tenant web url). Therefore, a useOverrideUrl boolean is
            // given as an optional parameter, which, if set, allows the client
            // to override the default webUrl with the specified url. The override
            // url should be passed in through the params object.
            var defaultUrlParam = '';
            if (!useOverrideUrl)
                defaultUrlParam = [
                    'webUrl',
                    '=',
                    encodeURIComponent(
                        sessionService.getClauseLibrarySourceUrl())
                ].join('');

           return [
                '/api/',
                controller,
                '/',
                action,
                '?',
                defaultUrlParam,
                additionalParams.join('')
            ].join('');
        }

        function makeRequest(requestUrl, data, method) {
            var deferred = $q.defer();

            (function() {
                return data ?
                    $http[method](requestUrl, data) :
                    $http[method](requestUrl);
            })().success(function (results) {
                deferred.resolve(results);

            }).error(function (err) {
                var error = err;
                if (err && err.hasOwnProperty('ExceptionMessage'))
                    error = err['ExceptionMessage'];

                deferred.reject(error);
            });

            return deferred.promise;
        }

        this.get = function (controller, action, params, useOverrideUrl) {
            
            var requestUrl = buildRequestUrl(controller, action, params, useOverrideUrl);
            return makeRequest(requestUrl, null, 'get');

        }

        this.save = function (controller, action, objectToStore, params, useOverrideUrl) {

            var requestUrl = buildRequestUrl(controller, action, params, useOverrideUrl);
            return makeRequest(requestUrl, objectToStore, 'post');

        }

        this.update = function (controller, action, objectToStore, params, useOverrideUrl) {
            var requestUrl = buildRequestUrl(controller, action, params, useOverrideUrl);
            return makeRequest(requestUrl, objectToStore, 'put');
        }

        // the itemRequestModel - see ItemRequestModel.cs; the clients of this service are
        // responsible for passing their request data in the proper format.
        this.delete = function (controller, action, itemRequestModel, params, useOverrideUrl) {
            // It might seem strange to call `save` internally here, but in reality their
            // method signatures are basically exactly the same.
            return this.save(controller, action, itemRequestModel, params, useOverrideUrl);
        }


    }
})(angular);


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