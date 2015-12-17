// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('favouriteItem.service', [])
        .service('favouriteItemService', favouriteItemService);

    favouriteItemService['$inject'] = ['$q', '$http', 'sessionService'];

    function favouriteItemService($q, $http, sessionService) {

        // Creates a new favourite on the server
        this.saveItem = function (webUrl, favorite) {
            var requestUrl = [
                '/api/favourites/post?webUrl=', encodeURIComponent(webUrl)
            ].join('');

            var deferred = $q.defer();
            $http.post(requestUrl, favorite)
                .success(function (data) {
                    deferred.resolve(data);
                })
                .error(function (err) {
                    var error = err;
                    if (err && err.hasOwnProperty('ExceptionMessage'))
                        error = err['ExceptionMessage'];

                    deferred.reject(error);
                });
            return deferred.promise;
        }

        // Takes an Id of a favourite to delete and makes the request
        this.deleteItem = function (webUrl, favourite) {
            var userEmail = sessionService.getUserEmail();
            var requestUrl = [
                '/api/favourites/delete?webUrl=', encodeURIComponent(webUrl),
                '&Id=', favourite.Id,
                '&userEmail=', userEmail
            ].join('');

            var deferred = $q.defer();
            $http.delete(requestUrl)
                .success(function (data) {
                    deferred.resolve(data);
                })
                .error(function (err) {
                    var error = err;
                    if (err && err.hasOwnProperty('ExceptionMessage'))
                        error = err['ExceptionMessage'];

                    deferred.reject(error);
                });
            return deferred.promise;
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