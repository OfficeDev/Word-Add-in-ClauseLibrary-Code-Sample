// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('clauseItem.service', [])
        .service('clauseItemService', clauseItemService);

    clauseItemService['$inject'] = [
        '$q',
        '$http',
        '$sce',
        'localDatabaseService',
        'constantsService',
        'localStorageService',
        'sessionService',
        'notificationService',
        'apiService'
    ];

    function clauseItemService(
        $q, $http, $sce, localDatabaseService,
        constantsService, localStorageService,
        sessionService, notificationService, apiService) {
        
        this.get = function(action, params, storeLocally, showNotification) {

            var clausesNotification;
            if (showNotification === true) {
                clausesNotification = notificationService.notify(
                'Retrieving data (this may take a few moments)...', notificationService.types.pending);
            }

            var deferred = $q.defer();
            apiService.get('clauses', action, params).then(function(clauses) {
                // clean up some data before it gets consumed.
                $.each(clauses, function (index, item) {
                    item.ModifiedInLocale = new Date(item.Modified).toLocaleString();
                });

                if (showNotification === true) {
                    notificationService.update(clausesNotification,
                    [clauses.length, ' clauses loaded successfully'].join(''),
                    notificationService.types.success);
                }

                // persist the data locally unless explicitly directed to refresh
                if (storeLocally) {

                    // display a notification
                    var storingLocallyNotification;
                    if (showNotification === true) {
                        storingLocallyNotification = notificationService.notify(
                            ['Syncing data, please wait...'].join(''),
                            notificationService.types.pending);
                    }
                    localDatabaseService.storeItems(clauses, 'clauses', true).then(function (localData) {

                        if (showNotification === true) {
                            notificationService.update(
                                storingLocallyNotification,
                                ["Your library is ready for use"].join(''),
                                notificationService.types.success);
                        }

                        deferred.resolve(localData);
                    }).catch(function (err) {
                        deferred.reject(err);
                    });

                } else {
                    deferred.resolve(clauses);
                }
            }).catch(function(err) {
                notificationService.update(
                        clausesNotification,
                        'Failed to load all clauses.',
                        notificationService.types.error);

                deferred.reject(err);
            });

            return deferred.promise;
        }

        this.getRoot = function (storeLocally) {
            return this.get('GetRoot', null, storeLocally, false);
        }

        this.getAll = function (storeLocally) {
            return this.get('GetAllClauses', null, storeLocally, true);
        }

        this.getByGroupId = function (groupId, storeLocally) {
            return this.get('GetByGroupId', { 'groupId': groupId }, storeLocally);
        }

        // Creates a new clause on the server
        this.save = function (newClause) {
            return apiService.save('clauses', 'create', newClause);
        }

        // Takes an updated clause and sends it to the server to be updated
        this.update = function (updatedClause) {
            return apiService.update('clauses', 'update', updatedClause);
        }

        // Takes an Id of a clause to delete and makes the request
        this.delete = function (clause) {
            var deferred = $q.defer();
            apiService.delete('clauses', 'delete', {Clause: clause}).then(function() {
                // update local db
                localDatabaseService.deleteItem(clause.Id, 'clauses').then(function(results) {
                    deferred.resolve(results);
                }).catch(function(err) {
                    deferred.reject(err);
                });
            }).catch(function(err) {
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