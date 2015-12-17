// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    var app = angular.module('clauseLibraryApp');
    settingsController['$inject'] = [
        '$scope',
        '$filter',
        '$window',
        '$modal',
        'sessionService',
        'settingsService',
        'notificationService',
        'clauseItemService',
        'usSpinnerService',
        'navigationService',
        'dataService',
        'modalService',
        'localDatabaseService'
    ];
    app.controller('settingsController', settingsController);

    function settingsController($scope, $filter, $window, $modal, sessionService, settingsService,
        notificationService, clauseItemService, usSpinnerService, navigationService, dataService, modalService, localDatabaseService) {
        $scope.hasInitialised = false;
        $scope.formSubmitted = false;
        $scope.maximumNameLength = 255;
        $scope.maximumDescriptionLength = 1000;

        $scope.libraries = [];
        $scope.libraryUrl = {
            createLibraryUrl: sessionService.getSpWebUrl().replace(/\/$/, '') + '/clauselibrary'
        };
        $scope.newLibrary = { Name: "Clause Library", Description: "My clauses" };

        $scope.connectableLibraries = [];
        $scope.connectableLibraries.selected = {};
        $scope.connectLibrariesLabel = 'libraries';
        $scope.webUrlConnectedTo = sessionService.getDefaultLibrary();

        $scope.accordion = {
            connect: {
                open: $scope.connectableLibraries.length > 0,
                disabled: $scope.connectableLibraries.length === 0
            },
            create: {
                open: $scope.connectableLibraries.length === 0
            }
        }

        $scope.createLibrarySelectHandler = function(item) {
            $scope.libraryUrl.createLibraryUrl = item.Url;
            console.log('settings library selected', $scope.libraryUrl.createLibraryUrl, item.Url);
        }

        // Used for connecting to libraries
        // Returns an async promise
        $scope.getAllExistingLibraries = function () {

            // start the loading spinner (running by default)
            // usSpinnerService.spin('connect-load');
            return settingsService.getAllExistingLibraries()
                .then(function (data) {

                    $scope.connectableLibraries = data;
                
                    // open the connect panel by default if there are options,
                    // otherwise close and disable
                    var hasConnectableLibraries = data && data.length > 0;
                    $scope.accordion.connect.open = hasConnectableLibraries;
                    $scope.accordion.connect.disabled = !hasConnectableLibraries;
                    $scope.connectLibrariesLabel = data && data.length == 1 ? 'library' : 'libraries';

                    // stop the spinner
                    usSpinnerService.stop('connect-load');

                    console.log('existing', data);

                }).catch(function(error) {
                    console.error(error);

                    notificationService.notify(error, notificationService.types.error);

                    // stop the spinner
                    usSpinnerService.stop('connect-load');
                });
            
        }

        // Used for creating libraries
        // Returns an async promise
        $scope.getPotentialLibraryLocations = function () {
            console.log('requesting libraries');
            
            return settingsService.getPotentialLibraryLocations()
                .then(function (data) {
                    $scope.libraries = data;

                    console.log('getPotentialLibraryLocations', data);

                    // stop the spinner
                    usSpinnerService.stop('create-load');
                })
                        .catch(function(error) {
                    console.error(error);
                    notificationService.notify(error, notificationService.types.error);

                    // stop the spinner
                    usSpinnerService.stop('create-load');
                });
        }

        $scope.create = function (form) {

            $scope.formSubmitted = true;

            // weburl
            var clauseLibrarySourceUrl = $scope.libraryUrl.createLibraryUrl;

            if (form.$invalid) {
                return;
            }

            if ($scope.connectableLibraries.filter(function(library) { return library.HostWebUrl.toLowerCase() === clauseLibrarySourceUrl.toLowerCase() }).length >= 1) {
                notificationService.notify('Library already exists, please enter another URL', notificationService.types.error);
                return;
            }

            // library model
            var library = {
                Name: $scope.newLibrary.Name,
                Description: $scope.newLibrary.Description,
                TenantId: sessionService.getTenantId()
            };

            var notification = notificationService.notify(
                "Creating library '" + library.Name + "' (this may take several moments)...",
                notificationService.types.pending
            );


            settingsService.create(clauseLibrarySourceUrl, library)
                .then(function (data) {
                    var libraryGuid = data;
                    console.log('create library', data);

                    notificationService.update(
                        notification,
                        "Library '" + library.Name + "' successfully created",
                        notificationService.types.success
                    );

                    // Update the settings page with fresh library data
                    $scope.getAllExistingLibraries().then(function() {
                            $scope.connectableLibraries.selected = $filter('filter')($scope.connectableLibraries, { LibraryId: libraryGuid }, true)[0];
                        $scope.connect();
                    });
                })
                .catch(function (error) {
                    console.error(error);
                    notificationService.update(
                        notification,
                        "Failed to create new library '" + library.Name + "'",
                        notificationService.types.error
                    );
                });
        }

        $scope.showControlError = function (formControl) {
            return (formControl.$dirty || $scope.formSubmitted) && formControl.$invalid;
        }

        $scope.cancel = function() {
            navigationService.back();
        }

        $scope.connect = function () {

            var selectedLibraryToConnect = $scope.connectableLibraries.selected;

            if (selectedLibraryToConnect) {
                // library model - only need library id (copied from db)
                var library = {
                    LibraryId: selectedLibraryToConnect.LibraryId,
                    TenantId: selectedLibraryToConnect.TenantId,
                    Name: selectedLibraryToConnect.Name,
                    Description: selectedLibraryToConnect.Description,
                    HostWebUrl: selectedLibraryToConnect.HostWebUrl
                };

                var notification = notificationService.notify(
                    "Connecting to library '" + library.Name + "'...",
                    notificationService.types.pending
                );

                // connects to an existing library for which the current user has sufficient permissions
                // and yeilds a populated library object upon successful connection
                settingsService.connect(library)
                    .then(function(connectedLibrary) {

                        if (connectedLibrary) {
                            // update session service with new web url (will update localStorage)
                            var isUpdated = sessionService.setHostWebUrl(connectedLibrary.HostWebUrl);

                            // update clause item service so that new library data can be displayed in manage view
                            if (isUpdated) {

                                // update local storage
                                sessionService.setNewLibraryConnection(connectedLibrary);

                                // update the visual connected library url in the view
                                $scope.webUrlConnectedTo = connectedLibrary;

                                // we've connected to a new library; 
                                // forcibly refresh the manage view
                                dataService.setSyncRequired(true);

                                notificationService.update(
                                    notification,
                                    "Connected to '" + library.Name + "' successfully",
                                    notificationService.types.success
                                );

                                navigationService.goToManage();
                            } else {
                                notificationService.update(
                                    notification,
                                    "You are already connected to '" + library.Name + "'.",
                                    notificationService.types.info);
                            }
                        } else {
                            notificationService.update(
                                notification,
                                "Something went wrong when trying to connect to " + library.Name,
                                notificationService.types.error
                            );
                        }
                    })
                    .catch(function(error) {
                        console.error(error);
                        notificationService.update(
                            notification,
                            "Failed to connect to library '" + library.Name + "'",
                            notificationService.types.error
                        );
                    });
            } else {
                notificationService.notify("No Library has been selected", notificationService.type.warning);
            }
            
        }

        $scope.clearCache = function() {
        
            var deleteConfirm = $modal.open(modalService.getGenericModal("Are you sure you want to clear the local data from this library?"));

            deleteConfirm.result.then(function(confirmed) {
                if (!confirmed) {
                    return;
                }
                // user confirmed cache clear
                var notification = notificationService.notify("Resetting local store...", notificationService.types.pending);
                localDatabaseService.deleteLocalDB().then(function () {
                    if (sessionService.getHostWebUrl() === $scope.connectableLibraries.selected.HostWebUrl) {
                        dataService.setSyncRequired(true);
                    }

                    notificationService.update(notification, 'Local data store cleared', notificationService.types.success);
                });
            }, function() {
                console.log('Clear cache cancelled');
            });
        }    

        function initialise() {
            $scope.hasInitialised = true;

            $scope.getAllExistingLibraries();
            $scope.getPotentialLibraryLocations();
        }

        initialise();
    };
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