// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    var app = angular.module('clauseLibraryApp');
    editClauseController['$inject'] = [
        '$scope',
        '$window',
        '$controller',
        'localDatabaseService',
        'clauseItemService',
        'groupItemService',
        'navigationService',
        'officeService',
        'sessionService',
        'notificationService',
        'dataService',
        'modalService'
    ];
    app.controller('editClauseController', editClauseController);

    function editClauseController(
        $scope, $window, $controller, localDatabaseService,
        clauseItemService, groupItemService, navigationService,
        officeService, sessionService, notificationService, dataService, modalService) {

        //Scope variables and functions shared between the add and edit views are set in the base controller
        $controller('baseClauseController',
        {
            $scope: $scope,
            $window: $window,
            $controller: $controller,
            localDatabaseService: localDatabaseService,
            clauseItemService: clauseItemService,
            groupItemService: groupItemService,
            navigationService: navigationService,
            sessionService: sessionService,
            officeService: officeService,
            notificationService: notificationService,
            modalService: modalService
        });

        $scope.submitText = "Submit";
        $scope.isEditMode = true;
        $scope.clause = {};

        //Add clause to document for editing
        $scope.addClauseToDocument = function() {
             officeService.addClauseToDocument($scope.text);
        }

        $scope.submitForm = function () {

            $scope.formSubmitted = true;

            if ($scope.clauseForm.$invalid) {
                return;
            }

            // if no owner is set, default to the author
            var owner = $scope.clause.Owner ? $scope.clause.Owner : $scope.clause.Author;

            var group = $scope.groups.selected || null;
            var data = {
                Clause: {
                    Id: $scope.clauseId,
                    Title: $scope.title,
                    Text: encodeURIComponent($scope.text),
                    UsageGuidelines: $scope.usageGuidelines,
                    TagsList: $scope.getSelectedTagsWithoutDuplicates(),
                    Author: $scope.clause.Author,
                    GroupId: group ? group.Id : 0,
                    Owner: owner,
                    OwnerId: owner.Id,
                    DesigneesList: $scope.clause.DesigneesList
                },
                Group: group,
                ExternalLinks: $scope.externalLinks
            };

            var notification = notificationService.notify(
                "Updating clause " + $scope.title + "...",
                notificationService.types.pending
            );

            clauseItemService.update(data)
                .then(function (updatedClause) {
                    notificationService.update(notification, "Clause has been successfully updated", notificationService.types.success);
                        
                    // sync the local DB
                    localDatabaseService.updateItem(updatedClause, 'clauses');

                    navigationService.goToManage();
                })
                .catch(function (error) {
                    notificationService.update(notification, error, notificationService.types.error);
                });
        }

        // sets $scope.groups.selected based on the clause's matching GroupId
        function setSelectedGroupByGroupId(groups, selectedGroupId) {
            var matchingGroup = groups.filter(function(item) {
                return item.Id == selectedGroupId;
            });

            if (matchingGroup && matchingGroup.length > 0) {
                $scope.groups.selected = matchingGroup[0];
            }
        }

        function initialise() {
            // get current clause
           var clause = dataService.getCurrentEditClause();
           
            // get all groups
            groupItemService.getFromLocalDB().then(function (groups) {
                $scope.groups = groups;
                setSelectedGroupByGroupId(groups, clause.GroupId);
            })
            .catch(function () {
                notificationService.notify("Failed to retrieve all groups", notificationService.types.error);
            });

            localDatabaseService.getAllFromStore('tags').then(function(tags) {
                $scope.tags = tags;
                $scope.tags.selected = clause.TagsList;
            }).catch(function() {
                notificationService.notify("Failed to retrieve tags", notificationService.types.error);
            });

            //Everything is in the stored on the scope for consistancy
            //Since the selected tags have to be stored on the tags collection for the ui-select plugin
            $scope.clause = clause;
            $scope.clauseId = clause.Id;
            $scope.usageGuidelines = clause.UsageGuidelines;
            $scope.title = clause.Title;
            $scope.text = clause.Text;
            $scope.author = clause.Author;
            $scope.externalLinks = clause.ExternalLinks;
            $scope.hasInitialized = true;
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