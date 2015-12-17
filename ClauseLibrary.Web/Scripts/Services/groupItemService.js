// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('groupItem.service', [])
        .service('groupItemService', groupItemService);

    groupItemService['$inject'] = ['$q', '$http', 'localDatabaseService', 'sessionService', 'apiService'];

    function groupItemService($q, $http, localDatabaseService, sessionService, apiService) {

        this.get = function (action, params, storeLocally) {

            var deferred = $q.defer();
            apiService.get('groups', action, params).then(function(groups) {
                // persist the data locally unless explicitly directed to refresh
                if (storeLocally) {

                    localDatabaseService.storeItems(groups, 'groups').then(function (localData) {
                        deferred.resolve(localData);
                    }).catch(function (err) {
                        deferred.reject(err);
                    });

                } else {
                    deferred.resolve(groups);
                }
            }).catch(function(err) {
                deferred.reject(err);
            });

            return deferred.promise;
        }

        this.getRoot = function(storeLocally) {
            return this.get('GetRoot', null, storeLocally);
        }

        this.getAll = function (storeLocally) {
            return this.get('GetAllGroups', null, storeLocally);
        }

        this.getByParentId = function (parentId, storeLocally) {
            return this.get('GetByParentId', { 'parentId': parentId }, storeLocally);
        }

        this.getFromLocalDB = function() {
            return localDatabaseService.getAllFromStore('groups');
        }

        // Creates a new group with the given group data
        this.save = function (newGroup) {
            return apiService.save('groups', 'create', newGroup);
        }

        // Takes an updated group and sends it to the server to be updated
        this.update = function (updatedGroup) {
            return apiService.update('groups', 'update', updatedGroup);
        }

        // Takes an Id of a clause to delete and makes the request
        this.delete = function (group) {
            var deferred = $q.defer();
            apiService.delete('groups', 'delete', { Group: group }).then(function (results) {
                // update local db
                localDatabaseService.deleteItem(group.Id, 'groups').then(function (results) {
                    deferred.resolve(results);
                }).catch(function (err) {
                    deferred.reject(err);
                });
            }).catch(function (err) {
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