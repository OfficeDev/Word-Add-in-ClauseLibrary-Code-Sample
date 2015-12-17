// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('data.service', [])
        .service('dataService', dataService);

    dataService['$inject'] = [
        '$q', '$http', '$indexedDB'
    ];

    /**
     * Store data in memory for persistence between views
     * Access local indexedDB
     * make requests for bulk data, such as for initial loading
     */
    function dataService($q, $http, $indexedDB) {

        // maintains in-memory persistence for quick retrieval when
        // navigating between views
        var persistedData = {
            parentGroup: null
        };

        // maintains an array of sync handler callback functions that are
        // called when a sync is required
        var syncCallbacks = [];

        // maintains whether a remote sync is needed between the client
        // and sharepoint data. Will request data from SP if true, otherwise
        // retrieves data from indexedDB.
        // True by default; when the app loads, a sync will be required
        var syncRequired = true;

        var useLocalData = false;

        //Contains the clause we currently want to edit
        var currentEditClause = null;
        
        //Sets the currect Clause to edit
        this.setCurrentEditClause = function (clause) {
            currentEditClause = clause;
        }

        //Gets the currect clause to edit
        this.getCurrentEditClause = function () {
            return currentEditClause;
        }

        //Contains the Current clause Group
        var currentEditGroup = null;

        //Sets the Clause Group
        this.setCurrentEditGroup = function (group) {
            currentEditGroup = group;
        }
        //Gets the clause group
        this.getCurrentEditGroup = function () {
            return currentEditGroup;
        }



        // sets a boolean flag to indicate whether to retrieve data from
        // local DB (true) or retrieve data remotely from SP (false)
        this.setFromLocalDB = function(useLocal) {
            useLocalData = useLocal;
        }

        this.getFromLocalDB = function () {
            return useLocalData;
        }

        this.persist = function(key, value) {
            persistedData[key] = value;
        }

        this.persistParentGroup = function (group) {
            this.persist('parentGroup', group);
        }

        this.getParentGroup = function() {
            return persistedData['parentGroup'];
        }


        // sets sync required to true/false; allows different modules to
        // notify the app if data is changed and needs to be updated.
        // fires all sync event listeners throughout the app so proper
        // sync handling will occur
        this.setSyncRequired = function(isRequired) {
            syncRequired = isRequired;

            // if syncRequired is set to true, invoke all sync handlers
            syncRequired && invokeSyncHandlers();
        }

        this.isSyncRequired = function() {
            return syncRequired;
        }

        this.subscribe = function(callback) {
            this.syncCallbacks.push(callback);
        }

        // fires all sync handlers
        function invokeSyncHandlers() {
            // iterate over all sync handlers and invoke them as they
            // are popped off the sync callback array
            while(syncCallbacks.length > 0) {
                syncCallbacks.pop()();
            }
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