// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    var app = angular.module('clauseLibraryApp');
    baseClauseController['$inject'] = [
        '$scope',
        '$window',
        '$modal',
        'localDatabaseService',
        'clauseItemService',
        'groupItemService',
        'externalLinksService',
        'navigationService',
        'sessionService',
        'officeService',
        'notificationService',
        'modalService'
    ];

    app.controller('baseClauseController', baseClauseController);

    function baseClauseController(
        $scope, $window, $modal, localDatabaseService, clauseItemService, groupItemService, externalLinksService,
        navigationService, sessionService, officeService, notificationService, modalService) {

        //Clause data
        $scope.title = '';
        $scope.text = '';
        $scope.usageGuidelines = '';
        $scope.clauseId = 0;
        $scope.author = {};
        $scope.tags = [];
        $scope.tags.selected = [];
        $scope.groups = [];
        $scope.groups.selected = {};
        $scope.externalLinks = [];
        $scope.newExternalLink = externalLinksService.getEmptyExternalLink();
        $scope.newExternalLinkTextSet = false;
        $scope.newExternalLinkUrlSet = false;
        $scope.collapseExternalLinks = true;
        $scope.isUserAdmin = sessionService.isUserAdmin();

        //Form data
        $scope.hasInitialised = false;
        $scope.maximumTitleLength = 255;
        $scope.maximumTextLength = 64000;
        $scope.maximumUsageGuidelinesLength = 2000;
        $scope.maximumTagLength = 255;
        $scope.maximumUrlLength = 2000;
        $scope.tagsOutput = '';
        $scope.formSubmitted = false;
        $scope.isEditMode = false;
        $scope.hasInvalidTags = false;
        $scope.addClauseButtonText = "Add to document";
        $scope.updateContentsButtonText = "Update contents";

        $scope.$watch('newExternalLink', function(updatedLink) {
            $scope.newExternalLinkTextSet = updatedLink.Text != null && updatedLink.Text != undefined && updatedLink.Text != '' || false;
            $scope.newExternalLinkUrlSet = updatedLink.Url != null && updatedLink.Url != undefined && updatedLink.Url != '' || false;
        }, true);

        //Tag related functions
        var containsInvalidCharacters = function (tagName) {
            var regex = new RegExp("^[A-Za-z0-9 _()-]*$");
            return !regex.test(tagName);
        };

        //This function will return a collection that has no tags with duplicate names
        //and using the existing version of any newly added tags.
        $scope.getSelectedTagsWithoutDuplicates = function () {

            var nonDuplicateTags = [];

            angular.forEach($scope.tags.selected, function (tag) {

                var matchingNonDuplicateTags = nonDuplicateTags.filter(function(nonDuplicateTag) {
                    return tag.Title == nonDuplicateTag.Title;
                });

                //If there are matching non duplicate tags then this is a duplicate so do nothing with it
                if (matchingNonDuplicateTags.length) {
                    return;
                }

                //Match up the newly added non duplicate tag with tags that are in the list of options
                var matchingExistingTags = $scope.tags.filter(function(existingTag) {
                    return tag.Title == existingTag.Title;
                });

                //If there is matching tag in the list of available tags then make sure the current tag is set to this
                //So there we are not adding duplicate options server side
                if (matchingExistingTags.length) {
                    tag = matchingExistingTags[0];
                }

                nonDuplicateTags.push(tag);
            });

            return nonDuplicateTags;
        };

        $scope.updateInvalidTags = function () {
            var hasTooLongTag = false;
            var hasTagWithInvalidCharacters = false;
            var hasInvalidTags = false;

            angular.forEach($scope.tags.selected, function (tag) {
                tag.isTooLong = tag.Title.length > $scope.maximumTagLength;
                tag.hasInvalidCharacters = containsInvalidCharacters(tag.Title);
                tag.isInvalid = tag.isTooLong || tag.hasInvalidCharacters;

                hasTooLongTag = hasTooLongTag || tag.isTooLong;
                hasTagWithInvalidCharacters = hasTagWithInvalidCharacters || tag.hasInvalidCharacters;
                hasInvalidTags = hasInvalidTags || tag.isInvalid;
            });

            $scope.hasInvalidTags = hasInvalidTags;
            $scope.clauseForm.tags.$setValidity("length", !hasTooLongTag);
            $scope.clauseForm.tags.$setValidity("characters", !hasTagWithInvalidCharacters);
        };

        $scope.tagTransform = function (newTag) {
            var newGroup = {
                Title: newTag,
                Id: -1
            };
            return newGroup;
        };

        $scope.createTag = function (newTagTitle) {
            return { Title: newTagTitle, Id: -1 };
        }

        //Form functions
        $scope.showControlError = function (formControl) {
            return (formControl.$dirty || $scope.formSubmitted) && formControl.$invalid;
        }

        // Cancel - return to previous view
        $scope.cancel = function () {
            navigationService.back();
        }

        $scope.updateClause = function () {
            // get data from highlighted section inside the word document
            var deferDataFromSelection = officeService.getDataFromSelection();
            deferDataFromSelection.then(function (data) {
                if ($(data).children().text().trim()) {
                    $scope.text = data;
                } else {
                    $modal.open(modalService.getInfoModal("To update the clause, please select text within your document and then click on the Update Contents button."));
                }
            })
            .catch(function (error) {
                // there was an error attempting to get data from the word document
                notificationService.notify(error, notificationService.types.error);
            });
        }

        $scope.addExternalLink = function () {
            $scope.newExternalLink.ClauseId = $scope.clauseId;
            $scope.externalLinks.push($scope.newExternalLink);
            $scope.newExternalLink = externalLinksService.getEmptyExternalLink();
        }

        $scope.removeExternalLink = function (item) {
            var index = $scope.externalLinks.indexOf(item);

            if ($scope.isEditMode && item.Id > 0) {

                var deleteConfirm = $modal.open(modalService.getGenericModal("Are you sure you want to remove this link? This will happen immediately."));

                deleteConfirm.result.then(function(confirmed) {
                    if (confirmed) {
                        // user confirmed delete...deleting group

                        var notification = notificationService.notify("Deleting link...", notificationService.types.pending);

                        externalLinksService.delete(item).then(function() {
                            $scope.externalLinks.splice(index, 1);
                            console.log('remove external link', item, index, $scope.externalLinks);
                            $scope.collapseExternalLinks = $scope.externalLinks.length == 0 && true;

                            notificationService.update(notification, "External link deleted", notificationService.types.success);
                        }).catch(function(error) {
                            console.error(error);

                            notificationService.update(notification, "An error occurred when creating the link", notificationService.types.error);
                        });
                    }
                }, function() {
                    console.log('delete cancelled');
                });
            } else {
                $scope.externalLinks.splice(index, 1);
            }
        }
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