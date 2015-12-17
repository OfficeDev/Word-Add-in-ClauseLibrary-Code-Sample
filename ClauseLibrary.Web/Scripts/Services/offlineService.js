// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('offline.service', [])
        .service('offlineService', offlineService);

    offlineService['$inject'] = [
        '$q', '$http', '$window', 'notificationService'
    ];

    function offlineService($q, $http, $window, notificationService) {

        var self = this;

        // listen for offline
        var isOnline = $window.navigator.onLine !== undefined && $window.navigator.onLine !== null ? $window.navigator.onLine : true;

        var statusChangeCallbacks = [
            function (status) {
                self.log(status);
            }
        ];

        this.isOnline = function() {
            return isOnline;
        }

        this.subscribe = function (callback) {
            statusChangeCallbacks.push(callback);
        }

        this.notify = function (status) {
            for (var i = 0; i < statusChangeCallbacks.length; i++) {
                var singleCallback = statusChangeCallbacks[i];
                if (singleCallback && typeof singleCallback == 'function') {
                    singleCallback(status);
                }
            }
            if (!status)
                notificationService.notify('You are offline', notificationService.types.warning);
            else
                notificationService.notify('You are back online', notificationService.types.info);
        }

        this.log = function(status) {
            var method = status ? 'info' : 'warn';
            var color = ['color: ', (status ? 'green' : 'red'), ';'].join('');
            var message = ['%c', 'Network status: ', (status ? 'ONLINE' : 'OFFLINE')].join('');
            console[method](message, color);
        }

        $window.addEventListener('online', function () {
            isOnline = true;
            self.notify(isOnline);
        });

        $window.addEventListener('offline', function () {
            isOnline = false;
            self.notify(isOnline);
        });

        // run log immediately upon loading
        this.log(this.isOnline);



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