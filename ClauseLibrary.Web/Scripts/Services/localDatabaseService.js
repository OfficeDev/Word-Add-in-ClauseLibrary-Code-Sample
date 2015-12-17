// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('localDatabase.service', [])
        .service('localDatabaseService', localDatabaseService);

    localDatabaseService['$inject'] = [
        '$q', '$http', '$sce', '$indexedDB', 'sessionService', 'notificationService', 'apiService'
    ];

    function localDatabaseService($q, $http, $sce, $indexedDB, sessionService, notificationService, apiService) {

        var self = this;
        var indexDbVersion = 1;
        // Define indexes for the database here
        var indices = {
            // store name
            'clauses': {
                // index name: object property
                'search_index': 'SearchIndices',
                // do not change parentGroup_index; if you must, make sure you
                // also update the 'groups' store index as well otherwise the
                // getRootFromStore() method will break because it expects the presence
                // of this key
                'parentGroup_index': 'GroupId'
            },

            'groups': {
                // do not change parentGroup_index; if you must, make sure you
                // also update the 'clauses' store index as well otherwise the
                // getRootFromStore() method will break because it expects the presence
                // of this key.
                'parentGroup_index': 'ParentId'
            },

            'tags': {
                'title_index': 'Title'    
            },

            'externalLinks': {}
        }


        sessionService.onSignOut(function() {
            // clears the local DB
            return self.deleteLocalDB();
        });

        // Creates a new local DB with the given name
        this.connect = function (databaseName) {
            var deferred = $q.defer();
            // initialize the indexedDB storage
            $indexedDB.connection(databaseName)
                .upgradeDatabase(indexDbVersion, function (event, db) {
                    console.log('connecting to database: ', databaseName);

                    // iterates over straightforward, generic store/index mapping defined above.
                    // If you need to add additional stores with definitions or specifications
                    // other than those declared in the loop below, you can define those before
                    // or after this loop.
                    for (var storeName in indices) {
                        var objectStore = db.createObjectStore(storeName, { keyPath: 'Id' });
                        for (var indexName in indices[storeName]) {
                            objectStore.createIndex(indexName, indices[storeName][indexName]);
                        }
                    }

                });

            return $indexedDB.open(databaseName);
        }

        // retrieves all items within the given store
        this.getAllFromStore = function (storeName) {
            var deferred = $q.defer();
            $indexedDB.openStore(storeName, function(store) {
                store.getAll().then(function (items) {
                    console.log('db items:', items);
                    items.forEach(function(item) {
                        item.Id = item.Id.replace(self.generateItemPrefix(store), '');
                    });
                    if (storeName == 'clauses')
                        items.forEach(function(item) { return $sce.trustAsHtml(item.Text); });
                    deferred.resolve(items);

                }).catch(function (err) {
                    console.error(err);
                    deferred.reject(err);

                });
            });
            return deferred.promise;
        }

        this.getByGroupId = function(storeName, groupId) {
            var deferred = $q.defer();
            $indexedDB.openStore(storeName, function (store) {
                var matchingGroupId = isNaN(groupId) ? 0 : parseInt(groupId);
                var query = $indexedDB.getQuery('parentGroup_index', '$eq', matchingGroupId);

                store.eachWhere(query).then(function (items) {
                    console.log('db items:', items);
                    items.forEach(function (item) {
                        item.Id = item.Id.replace(self.generateItemPrefix(store), '');
                    });

                    if(storeName == 'clauses')
                        items.forEach(function (item) { return $sce.trustAsHtml(item.Text); });

                    deferred.resolve(items);

                }).catch(function (err) {
                    console.error(err);
                    deferred.reject(err);

                });
            });
            return deferred.promise;
        }

        this.getRootFromStore = function(storeName) {
            return this.getByGroupId(storeName, 0);
        }

        // Stores clauses into a local indexed DB for quick search and offline retrieval
        this.storeItems = function (items, storeName, showNotification) {

            var deferred = $q.defer();

            // connect to the currently selected library's corresponding
            // local database
            var currentLocalDB = sessionService.getCurrentLocalDB();
            this.connect(currentLocalDB).then(function() {
                if (items && items.length > 0) {
                    console.log('syncing local database ', storeName);

                    try {
                        $indexedDB.openStore(storeName, function (store) {

                            store.count().then(function (val) {
                                console.log(['count [', storeName, ']: '].join(''), val);

                                if (items && items.length > 0) {
                                    self.upsertItems(items, store).then(function () {

                                        deferred.resolve(items);

                                    }).catch(function (err) {
                                        notificationService.notify(['Sync failed (', storeName, ')'],
                                            notificationService.types.error);
                                        deferred.reject(err);
                                    });
                                } else {
                                    notificationService.notify(
                                        ['No items to sync (', storeName, ')'].join(''),
                                        notificationService.types.info);
                                }

                            }).catch(function (err) {
                                notificationService.notify(
                                    ['Local sync failed (', storeName, ')'].join(''),
                                    notificationService.types.error);
                                deferred.reject(err);
                            });

                        });
                    } catch (exception) {
                        notificationService.notify(
                            ['Local sync failed (', storeName, ')'].join(''),
                            notificationService.types.error);
                    }
                } else {
                    // no sync needed, carry on
                    deferred.resolve(items);
                }
            }).catch(function(err) {
                console.error(err);
            });
            
            return deferred.promise;
        }

        // Updates the given item in the given object store
        this.updateItem = function (item, storeName) {
            var deferred = $q.defer();
            $indexedDB.openStore(storeName, function (store) {
                var itemClone = angular.copy(item);
                itemClone.Id = self.generateUniqueIndexId(itemClone.Id, store);
                store.upsert(itemClone).then(function (results) {
                    //notificationService.notify(
                    //    ['Item successfully updated (', storeName, ')'].join(''),
                    //    notificationService.types.success);
                    deferred.resolve(results);
                }).catch(function(err) {
                    notificationService.notify(
                        ['Failed to update item (', storeName, ')'].join(''),
                        notificationService.types.error);
                    deferred.reject(err);
                });
            });
            return deferred.promise;
        }

        // Updates the item Id to a globally unique Id for the entire IndexDb irrespective of store.
        // This resolves an iOS issue where objects in different stores were being removed where they had the same Id.
        // Reference: http://www.raymondcamden.com/2014/09/25/IndexedDB-on-iOS-8-Broken-Bad, and
        // http://stackoverflow.com/questions/26019147/primary-key-issue-on-ios8-implementation-of-indexeddb
        this.generateUniqueIndexId = function(itemKey, store) {
            return self.generateItemPrefix(store) + itemKey;
        }

        this.generateItemPrefix = function(store) {
            return store.storeName + '-';
        }

        // Updates the given items in the local DB if they exist,
        // otherwise they are inserted
        this.upsertItems = function (items, store) {
            var deferred = $q.defer();

            // uses an async control loop to maintain async control
            // over upsert method which is async, although indexedDB locks
            // the table during each transaction
            async.each(items, function (item, goToNextItem) {
                var itemClone = angular.copy(item);
                itemClone.Id = self.generateUniqueIndexId(itemClone.Id, store);
                store.upsert(itemClone).then(function () {
                    goToNextItem();
                }).catch(function(err) {
                    goToNextItem(err);
                });
            }, function (err) {
                if (err)
                    deferred.reject(err);
                else
                    deferred.resolve();
            });
            return deferred.promise;
        }

        this.deleteItem = function(itemKey, storeName) {
            var deferred = $q.defer();
            $indexedDB.openStore(storeName, function (store) {
                var indexKey = self.generateUniqueIndexId(itemKey, store);
                store.delete(indexKey).then(function (results) {
                    console.log('Item deleted from local database');
                    deferred.resolve(results);
                }).catch(function(err) {
                    deferred.reject(err);
                });
            });
            return deferred.promise;
        }

        this.searchItems = function (queryText, searchItems) {
            var deferred = $q.defer();

            if (!queryText)
                queryText = "";
            var queryItems = queryText.split(" ");
            console.log('items: ', searchItems);
            var matchingClauses = [];
            for (var i = 0; i < searchItems.length; i++) {
                var item = searchItems[i];

                if (!item.SearchIndices) continue;

                for (var j = 0; j < item.SearchIndices.length; j++) {
                    var searchIndex = item.SearchIndices[j];
                    
                    item.SearchIndices[j] = searchIndex ? searchIndex.toLowerCase() : "";
                }

                var match = true;
                for (var k = 0; k < queryItems.length; k++) {
                    var result = item.SearchIndices.filter(function (item) {
                        return typeof item == 'string' && item.indexOf(queryItems[k].toLowerCase()) > -1;
                    });
                    match = match && result.length > 0;
                    //match = match && (item.SearchIndices.indexOf(queryItems[k].toLowerCase()) > -1);
                    // must match ALL key words.
                }
                if (match) 
                    matchingClauses.push(item);
                // test whether the query text matches a substring of the item title
                //if (item.SearchIndices.indexOf(queryText.toLowerCase()) > -1)
                //    matchingClauses.push(item);
            }

            if (matchingClauses.length > 0) {
                $indexedDB.openStore('clauses', function (store) {
                    var dbMatches = [];
                    for (var i = 0; i < matchingClauses.length; i++) {
                        var matchingKey = self.generateUniqueIndexId(matchingClauses[i].Id, store);
                        store.find(matchingKey).then(function (result) {
                            console.log('search result', result);
                            dbMatches.push(result);

                        }).catch(function (err) {
                            console.error(err);
                            deferred.reject(err);
                        });
                    }
                    deferred.resolve(dbMatches);
                });
            } else {
                deferred.resolve(matchingClauses);
            }

            return deferred.promise;
        }

        this.searchFavorites = function (searchItems) {
            var deferred = $q.defer();
                                
            console.log('items: ', searchItems);
            var matchingClauses = [];
            for (var i = 0; i < searchItems.length; i++) {
                var item = searchItems[i];

                // test whether the query text matches a substring of the item title
                if (item.Favourite)
                    matchingClauses.push(item);
            }

            console.log(matchingClauses);
            //deferred.resolve(matchingClauses);
            if (matchingClauses.length > 0) {
                $indexedDB.openStore('clauses', function (store) {
                    var dbMatches = [];
                    for (var i = 0; i < matchingClauses.length; i++) {
                        var matchingKey = self.generateUniqueIndexId(matchingClauses[i].Id, store);
                        store.find(matchingKey).then(function (result) {
                            console.log('search result', result);
                            dbMatches.push(result);

                        }).catch(function (err) {
                            console.error(err);
                            deferred.reject(err);
                        });
                    }
                    deferred.resolve(dbMatches);
                });
            } else {
                deferred.resolve(matchingClauses);
            }

            return deferred.promise;
        }

        this.searchMyClauses = function (searchItems) {
            var deferred = $q.defer();
            var currentUserEmail = sessionService.getUserEmail();
            console.log('items: ', searchItems);
            var matchingClauses = [];
            for (var i = 0; i < searchItems.length; i++) {
                var item = searchItems[i];

                // test whether the query text matches a substring of the item title
                if (item.Owner.EMail == currentUserEmail)
                    matchingClauses.push(item);

                else if(item.DesigneesList) {
                    for (var j = 0; j < item.DesigneesList.length; j++) {
                        var designee = item.DesigneesList[j];
                        if (designee && designee.EMail == currentUserEmail) {
                            matchingClauses.push(item);
                        }
                    }
                }
            }

            console.log(matchingClauses);
            //deferred.resolve(matchingClauses);
            if (matchingClauses.length > 0) {
                $indexedDB.openStore('clauses', function (store) {
                    var dbMatches = [];
                    for (var i = 0; i < matchingClauses.length; i++) {
                        var matchingKey = self.generateUniqueIndexId(matchingClauses[i].Id, store);
                        store.find(matchingKey).then(function (result) {
                            console.log('search result', result);
                            dbMatches.push(result);

                        }).catch(function (err) {
                            console.error(err);
                            deferred.reject(err);
                        });
                    }
                    deferred.resolve(dbMatches);
                });
            } else {
                deferred.resolve(matchingClauses);
            }

            return deferred.promise;
        }

        this.deleteLocalDB = function () {
            var deferred = $q.defer();
            if ($indexedDB.connectionExists()) {
                $indexedDB.deleteDatabase().then(function(done) {
                    console.log('deleted database', done);
                    deferred.resolve(done);
                }).catch(function(error) {
                    console.error('deleting database failed,', error);
                    deferred.reject(error);
                });
            } else {
                deferred.resolve(true);
            }
            return deferred.promise;
        }

        this.deleteAllLocalDBs = function() {
            var deferred = $q.defer();
            
            $indexedDB.deleteAllDatabases().then(function (done) {
                console.log('deleted all databases', done);
                deferred.resolve(done);
            }).catch(function (err) {
                console.error('failed to delete all databases', error);
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