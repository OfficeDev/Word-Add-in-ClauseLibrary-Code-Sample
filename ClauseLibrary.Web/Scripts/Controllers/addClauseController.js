// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    var app = angular.module('clauseLibraryApp');
    addClauseController['$inject'] = [
        '$scope',
        '$window',
        '$controller',
        'dataService',
        'localDatabaseService',
        'clauseItemService',
        'groupItemService',
        'navigationService',
        'sessionService',
        'officeService',
        'notificationService',
        'modalService'
    ];

    app.controller('addClauseController', addClauseController);

    function addClauseController(
        $scope, $window, $controller,
        dataService, localDatabaseService,
        clauseItemService, groupItemService,
        navigationService, sessionService,
        officeService, notificationService,
        modalService) {

        //Scope variables and functions shared between the add and edit views are set in the base controller
        $controller('baseClauseController',
        {
            $scope: $scope,
            $window: $window,
            $controller: $controller,
            dataService: dataService,
            clauseItemService: clauseItemService,
            groupItemService: groupItemService,
            navigationService: navigationService,
            sessionService: sessionService,
            officeService: officeService,
            notificationService: notificationService,
            modalService: modalService
        });

        $scope.submitText = "Create clause";

        // Create a clause
        $scope.submitForm = function () {

            $scope.formSubmitted = true;

            if ($scope.clauseForm.$invalid) {
                return;
            }

            var defaultOwner = sessionService.getUser();

            var group = $scope.groups.length > 0 &&
                $scope.groups.selected &&
                $scope.groups.selected.hasOwnProperty('Id') ? $scope.groups.selected : null;
            var data = {
                Clause: {
                    Id: -1,
                    Title: $scope.title,
                    Text: encodeURIComponent($scope.text),
                    UsageGuidelines: $scope.usageGuidelines,
                    TagsList: $scope.getSelectedTagsWithoutDuplicates(),
                    OwnerId: parseInt(defaultOwner.Id)
                },
                Group: group,
                ExternalLinks: $scope.externalLinks
            };

            // add external links
            console.log('TODO: create external links');

            var notification = notificationService.notify("Creating clause '" + data.Clause.Title + "'...", notificationService.types.pending);

            clauseItemService.save(data)
                .then(function (createdClause) {
                    // forcibly refresh clause library stored data
                    notificationService.update(notification, "Clause '" + data.Clause.Title + "' has been created", notificationService.types.success);

                    // sync the local DB
                    localDatabaseService.updateItem(createdClause, 'clauses');

                    navigationService.goToManage();
                })
                .catch(function(error) {
                    notificationService.update(notification, error, notificationService.types.error);
                });
        }

        function initialise() {

            // get data from highlighted section inside the word document
            var deferDataFromSelection = officeService.getDataFromSelection();
            deferDataFromSelection.then(function (data) {
                if ($(data).children().text().trim()) {
                    $scope.text = data;
                } else {
                    $scope.text = 'To update the clause, please select text within your document and then click on the Update Contents button.';
                }
            })
            .catch(function (error) {
                notificationService.notify(error, notificationService.types.error);
            });


            // retrieve tags from local DB
            localDatabaseService.getAllFromStore('tags').then(function (tags) {
                $scope.tags = tags;
            }).catch(function(err) {
                console.error(err);
                notificationService.notify('Failed to retrieve all tags', notificationService.types.error);
            });


            // Retrieve all clause groups from the api
            localDatabaseService.getAllFromStore('groups').then(function (groups) {
                $scope.groups = groups;
                $scope.groups.selected = dataService.getParentGroup();
            }).catch(function (err) {
                console.error(err);
                notificationService.notify('Failed to retrieve all groups', notificationService.types.error);
            });

            $.when(deferDataFromSelection, deferDataFromSelection).done(function () {
                $scope.hasInitialised = true;
            });

            $scope.tags.selected = [];
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