// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('localStorage.service', [])
        .service('localStorageService', localStorageService);

    localStorageService['$inject'] = [];

    function localStorageService() {

        this.rootKey = 'clauselibrary';

        this.getFullyQualifiedKey = function(key) {
            return this.rootKey + '.' + key;
        }

        this.getItem = function(key, dataTransformCallback) {
            var fullyQualifiedKey = this.getFullyQualifiedKey(key);
            var storedObject = window.localStorage.getItem(fullyQualifiedKey);

            if (storedObject) {
                if (/^[\],:{}\s]*$/.test(storedObject.replace(/\\["\\\/bfnrtu]/g, '@').
                replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
                replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                    storedObject = JSON.parse(storedObject);
                }
            }

            if (dataTransformCallback == null) {
                dataTransformCallback = function(data) {
                    return data;
                }
            }
            return dataTransformCallback(storedObject);
        }

        this.setItem = function(key, objectToStore) {
            if (objectToStore === null) {
                this.removeItem(key);
                return;
            }
            var fullyQualifiedKey = this.getFullyQualifiedKey(key);

            var formattedInput = objectToStore;
            if (typeof(objectToStore) !== 'string') {
                formattedInput = JSON.stringify(objectToStore);
            }
            window.localStorage.setItem(fullyQualifiedKey, formattedInput);
        }

        this.removeItem = function(key) {
            var fullyQualifiedKey = this.getFullyQualifiedKey(key);
            window.localStorage.removeItem(fullyQualifiedKey);
        }

        this.clearAllItemsStartingWith = function(keyPrefix) {
            var prefix = this.getFullyQualifiedKey(keyPrefix);
            var myLength = prefix.length;

            Object.keys(localStorage).forEach(function (key) {
                if (key.substring(0, myLength) == prefix) {
                    window.localStorage.removeItem(key);
                }
            });
        }

        this.clearAllItems = function() {
            return this.clearAllItemsStartingWith('');
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