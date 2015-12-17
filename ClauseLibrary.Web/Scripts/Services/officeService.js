// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('office.service', [])
        .service('officeService', officeService)
        .run([function () {
            
            if (typeof(Office) === 'undefined') {
                console.warn('Office JS is unavailable');

                // create a fake context for debugging out of office.
                window.Office = {};
            }

            Office.initialize = function (reason) {
                // Placeholder function for Office to run.
            };
        }]);

    officeService['$inject'] = ['$http', '$q', 'notificationService'];

    function officeService($http, $q, notificationService) {
        var useDebugOfficeContext = !window.Office.ActiveView;
        if (useDebugOfficeContext) {
            window.Office = {
                context: {
                    document: {
                        setSelectedDataAsync: function(text, options, callback) {
                            callback({ status: Office.AsyncResultStatus.Succeeded });
                        },
                        getSelectedDataAsync: function(coercionType, callback) {
                            callback({
                                value: 'demo value',
                                status: Office.AsyncResultStatus.Succeeded
                            });
                        }
                    }
                }
            };
        }

        this.addClauseToDocument = function(text) {
            if (Office && Office.context && Office.context.document && Office.context.document.setSelectedDataAsync) {
                Office.context.document.setSelectedDataAsync(text, {
                    coercionType: Office.CoercionType.Html
                },
                function (asyncResult) {
                    if (asyncResult.status === "failed") {
                        notificationService.notify("" + asyncResult.error, notificationService.types.error);
                    }
                });
            }

        }

        // Reads data from current document selection
        this.getDataFromSelection = function() {
            var deferred = $q.defer();

            if (Office && Office.context && Office.context.document && Office.context.document.getSelectedDataAsync) {

                Office.context.document.getSelectedDataAsync(Office.CoercionType.Html,
                    function(result) {
                        if (result.status === Office.AsyncResultStatus.Succeeded) {
                            deferred.resolve(result.value);
                        } else {
                            deferred.reject(result.error.message);
                        }
                    }
                );
            } else {
                deferred.reject("Office JS is not initialized");
            }

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