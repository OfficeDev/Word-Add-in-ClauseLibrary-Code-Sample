// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('settings.service', [])
        .service('settingsService', settingsService);

    settingsService['$inject'] = ['$q', '$http', 'apiService', 'sessionService', 'localDatabaseService', 'navigationService'];

    function settingsService($q, $http, apiService, sessionService, localDatabaseService, navigationService) {
        console.log('settings service');

        this.get = function (action, params, overrideDefaultUrl) {
            var deferred = $q.defer();
            apiService.get('settings', action, params, overrideDefaultUrl).then(function (results) {
                deferred.resolve(results);
            }).catch(function (err) {
                deferred.reject(err);
            });
            return deferred.promise;
        }

        this.getAllExistingLibraries = function () {
            // set overrideDefaultUrl to true so that the api service
            // knows to pass the tenant web url rather than the default web url
            var userId = sessionService.getUserId();
            var webUrl = sessionService.getSpWebUrl();
            return this.get('GetAllExistingLibraries', {
                'userId': encodeURIComponent(userId),
                'tenantWebUrl': encodeURIComponent(webUrl)
            }, true);
        }

        this.getPotentialLibraryLocations = function () {
            // set overrideDefaultUrl to true so that the api service
            // knows to pass the tenant web url rather than the default web url
            var webUrl = sessionService.getSpWebUrl();
            return this.get('GetPotentialLibraryLocations', {
                'tenantWebUrl': encodeURIComponent(webUrl)
            }, true);
        }

        this.spUpdateRequired = function() {
            var webUrl = sessionService.getHostWebUrl();
            return this.get('SpUpdateRequired', {
                'webUrl': encodeURIComponent(webUrl)
            }, true);
        }

        this.upgrade = function() {
            var webUrl = sessionService.getHostWebUrl();
            return this.get('Upgrade', {
                'webUrl': encodeURIComponent(webUrl),
                'tenantId': sessionService.getTenantId()
            }, true);
        }

        this.create = function (createLibraryUrl, libraryModel) {
            return apiService.save(
                'settings', 'create', libraryModel,
                { 'webUrl': encodeURIComponent(createLibraryUrl) }, true);
        }

        this.connect = function (libraryModel) {
            var deferred = $q.defer();
            var webUrl = sessionService.getSpWebUrl();
            var userId = sessionService.getUserId();
            apiService.update('settings', 'connect', libraryModel, {
                'userId': encodeURIComponent(userId),
                'tenantWebUrl': encodeURIComponent(webUrl)
            }, true).then(function (data) {
                // take IsAdmin and update session
                if (data.AccessInfo) {
                    sessionService.setIsUserAdmin(data.AccessInfo.IsAdmin);
                    sessionService.setUser(data.AccessInfo.User);
                } else {
                    console.error('AccessInfo was not returned with the connect request', data);
                }

                // connect to the local DB
                var databaseName = ['db', '-', libraryModel.HostWebUrl].join('');

                // delete the current one first
                localDatabaseService.deleteLocalDB().then(function () {
                    localDatabaseService.connect(databaseName).then(function() {
                        console.log('connected successfully to local db: ', databaseName);
                        sessionService.setCurrentLocalDB(databaseName);
                        navigationService.goToManage();
                    });
                    // pass along the library
                    deferred.resolve(data.Library);

                }).catch(function(err) {
                    console.error('failed to delete db', err);
                    deferred.reject(err);
                });
            })
            .catch(function (err) {
                deferred.reject(err);
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