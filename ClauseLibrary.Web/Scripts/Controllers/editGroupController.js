// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    var app = angular.module('clauseLibraryApp');

    editGroupController['$inject'] = [
        '$scope',
        '$window',
        'sessionService',
        'navigationService',
        'groupItemService',
        'clauseItemService',
        'notificationService',
        'localDatabaseService',
        'dataService'
    ];

    app.controller('editGroupController', editGroupController);

    function editGroupController(
        $scope, $window, sessionService,
        navigationService, groupItemService,
        clauseItemService, notificationService, localDatabaseService, dataService) {

        //Group data
        $scope.groupId = null;
        $scope.title = '';
        $scope.author = {};
        $scope.isLocked = false;
        $scope.groups = [];
        $scope.groups.selected = {};
        $scope.isUserAdmin = sessionService.isUserAdmin();

        //Form data
        $scope.hasInitialised = false;
        $scope.maximumTitleLength = 255;
        $scope.submitButtonText = "Save changes";
        $scope.groupSelectionLabelText = "Move to new or existing group that you own";

        //Form functions
        $scope.cancelEdit = function() {
            //navigationService.back();
        }

        $scope.submitForm = function () {

            $scope.formSubmitted = true;

            //The form is discovered by angular using the name attribute
            if ($scope.groupForm.$invalid) {
                return;
            }

            var data = {
                Group: {
                    Id: $scope.groupId,
                    Title: $scope.title,
                    ParentId: $scope.groups.selected ? $scope.groups.selected.Id : 0,
                    IsLocked: $scope.isLocked,
                    Author: $scope.author,
                    OwnerId: $scope.owner.Id,
                    Owner: $scope.owner
                },
                Parent: $scope.groups.selected
            };

            var notification = notificationService.notify(
                "Updating group " + $scope.title + "...", notificationService.types.pending);

            groupItemService.update(data)
                .then(function (updatedGroup) {
                    notificationService.update(notification, "Group has been successfully updated", notificationService.types.success);
                    localDatabaseService.updateItem(updatedGroup, 'groups');
                    navigationService.goToManage();
                })
                .catch(function (error) {
                    console.error(error);
                    notificationService.update(notification, error, notificationService.types.error);
                });
        }

        $scope.cancel = function () {
            navigationService.back();
        }

        // sets $scope.groups.selected based on the clause's matching GroupId
        var setSelectedGroupByGroupId = function(groups, selectedGroupId) {
            var matchingGroup = groups.filter(function (item) {
                return item.Id == selectedGroupId;
            });

            if (matchingGroup && matchingGroup.length > 0) {
                $scope.groups.selected = matchingGroup[0];
            }
        }

        $scope.showControlError = function (formControl) {
            return (formControl.$dirty || $scope.formSubmitted) && formControl.$invalid;
        }

        var initialise = function() {
            // retrive the locally-persisted group from the clause item service
            var group = dataService.getCurrentEditGroup();

            $scope.groupId = group.Id;
            $scope.title = group.Title;
            $scope.author = group.Author;
            $scope.isLocked = group.IsLocked;
            $scope.ownerId = group.OwnerId;
            $scope.owner = group.Owner;

            // populate the list of other groups
            localDatabaseService.getAllFromStore('groups')
            .then(function (groups) {
                // do not add THIS group to the list of possible groups
                for (var i = 0; i < groups.length; i++) {
                    var g = groups[i];
                    if (g && g.Id == $scope.groupId) {
                        groups.splice(i, 1);
                        break;
                    }
                }
                $scope.groups = groups;
                setSelectedGroupByGroupId(groups, group.ParentId);
                $scope.hasInitialised = true;
            })
            .catch(function () {
                notificationService.notify("An error occurred while trying to retrieve groups", notificationService.types.error);
            });
        }

        initialise();
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