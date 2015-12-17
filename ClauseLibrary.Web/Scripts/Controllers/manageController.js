// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    var app = angular.module('clauseLibraryApp');
    manageController['$inject'] = [
        '$scope',
        '$q',
        '$modal',
        '$filter',
        '$indexedDB',
        '$analytics',
        'clauseItemService',
        'groupItemService',
        'tagService',
        'userService',
        'favouriteItemService',
        'navigationService',
        'officeService',
        'sessionService',
        'notificationService',
        'modalService',
        'externalLinksService',
        'manageViewService',
        'usSpinnerService',
        'offlineService',
        'dataService',
        'localDatabaseService',
        'psAccordionService',
        'settingsService'
    ];
    app.controller('manageController', manageController);

    function manageController(
        $scope, $q, $modal, $filter, $indexedDB, $analytics,
        clauseItemService, groupItemService, tagService, userService,
        favouriteItemService, navigationService, officeService,
        sessionService, notificationService, modalService,
        externalLinksService, manageViewService, usSpinnerService,
        offlineService, dataService, localDatabaseService, psAccordionService, settingsService) {

        $scope.hasInitialised = false;
        $scope.defaultLibrary = {};
        $scope.Groups = [];
        $scope.Clauses = [];
        $scope.searchResults = [];
        $scope.navItems = getNavItems();
        $scope.loggedInUser = null;
        $scope.userEmail = sessionService.getUserEmail();
        $scope.currentUserIsOwner = false;
        $scope.forcibleRefresh = false;
        $scope.collapseExternalLinks = true;
        $scope.manageLoading = true;
        $scope.getActiveViewName = manageViewService.getActiveViewName();
        $scope.clauseSearchText = manageViewService.getActiveSearchQuery();
        $scope.hasTagsChecked = false;
        $scope.isUserAdmin = sessionService.isUserAdmin();
        $scope.isOnline = offlineService.isOnline();
        $scope.noLocalData = false;
        $scope.searchReady = false;
        $scope.searchLoading = false;
        var allClauses = [];

        offlineService.subscribe(function (status) {
            // we are going from offline to online; redirect the user to the welcome page
            navigationService.goToWelcome();
            
            $scope.isOnline = status;
        });

        var resetFilterFlags = function() {
            $scope.showFavourites = false;
            $scope.showMyClauses = false;
        }
        resetFilterFlags();

        $scope.openAccordion = function(item) {
            var deferred = $q.defer();

            item.isLoading = true;
            item.isActive = true;
            usSpinnerService.spin('group-load-' + item.Id);

            // dirty way of checking whether the current item is a group (has a Clauses property)
            // or a clause (does not have a Clauses property)
            if (item.hasOwnProperty('Clauses')) {

                // add group to list of active items
                manageViewService.setActiveItem(item, 'group');

                // Populate the contents of the open item
                async.parallel([
                    // Get all child clauses
                    function(callback) {
                        // retrieve from local DB
                        console.log('retrieving clauses for group ', item.Id);
                        localDatabaseService.getByGroupId('clauses', item.Id).then(function (clauses) {
                            // clauses will be results[0] in the final callback
                            callback(null, clauses);
                        }).catch(function(err) {
                            notificationService.notify(
                                'Failed to retrieve clauses from local DB',
                                notificationService.types.error);
                            callback(err, null);
        });
                    },

                    // Get all child groups
                    function(callback) {
                        // Retrieve from local DB
                        console.log('retrieving groups for group ', item.Id);
                        localDatabaseService.getByGroupId('groups', item.Id).then(function (groups) {
                            // groups will be results[1] in the final callback
                            callback(null, groups);
                        }).catch(function(err) {
                            notificationService.notify(
                                'Failed to retrieve groups from local DB',
                                notificationService.types.error);
                            callback(err, null);
                        });
                    }
                ],

                // all contents have been retrieved..
                function (error, results) {
                    if (error) {
                        console.error(error);
                        deferred.reject(error);
                    } else {
                        if (results)
                            item.Clauses = results[0], item.Groups = results[1];

                        item.isLoaded = true;
                        usSpinnerService.stop('group-load-' + item.Id);

                        deferred.resolve(results);
                    }
                });
            } else {
                // the item opened is a clause (the item does not have a Clauses property;
                // see comments above about dirty type checking)

                // add clause to list of active items
                manageViewService.setActiveItem(item, 'clause');

                deferred.resolve(item);
            }

            return deferred.promise;
        }

        $scope.closeAccordion = function (item) {
            var deferred = $q.defer();

            (function () {
                item.isActive = false;
                // item has Clauses property? type: group, otherwise: clause (dirty type checking)
                if (item.hasOwnProperty('Clauses'))
                    manageViewService.removeActiveItem(item, 'group');
                else
                    manageViewService.removeActiveItem(item, 'clause');

                deferred.resolve(item);
            })();

            return deferred.promise;
        }

        $scope.goToSettings = function() {
            navigationService.goToState(clApp.navigation.states.settings);
        }

        // reload the manage view data
        $scope.refreshClauseLibrary = function () {
            dataService.setSyncRequired(true);
            initialize();
        }

        // Navigate to new clause view
        $scope.addNewClause = function (group) {
            // persists the clause's parent group for use in the
            // add clause state
            group && dataService.persistParentGroup(group);

            navigationService.goToState(clApp.navigation.states.addClause);
        }

        // Navigate to new group view
        $scope.addNewGroup = function (group) {
            group && dataService.setCurrentEditGroup(group);
            navigationService.goToState(clApp.navigation.states.addGroup);
        }

        // Navigate to edit clause view
        $scope.editClause = function (clause) {
            // persist the current clause to access between states
            dataService.setCurrentEditClause(clause);

            // activate the clause so that it is opened when we return
            manageViewService.setActiveItem(clause, 'clause');

            // navigate to the edit clause view
            navigationService.goToState(clApp.navigation.states.editClause);
        }

        // Delete clause
        $scope.deleteClause = function (clause, group) {
            // disable the clause while the delete method runs
            clause.isDisabled = true;

            var deleteConfirm = $modal.open(modalService.getGenericModal("Are you sure you want to delete this item?"));

            deleteConfirm.result.then(function(confirmed) {
                if (confirmed) {
                    // user confirmed delete...deleting clause

                    var notification = notificationService.notify("Deleting clause...", notificationService.types.pending);

                    clauseItemService.delete(clause)
                        .then(function () {
                            // create a successful notification
                            notificationService.update(notification, 'Clause successfully deleted', notificationService.types.success);

                            if (group) {
                                group.Clauses.splice(group.Clauses.indexOf(clause), 1);
                            } else {
                                $scope.Clauses.splice($scope.Clauses.indexOf(clause), 1);
                            }

                            localDatabaseService.getAllFromStore('clauses').then(function (clauses) {
                                allClauses = clauses;
                            }).catch(function () {
                            });

                        })
                        .catch(function (error) {
                            //something went wrong; if the clause hasn't been deleted, re-enable it
                            clause.isDisabled = false;

                            // display error notification
                            notificationService.update(notification, error, notificationService.types.error);
                        });
                }
            }, function() {
                console.log('delete cancelled');
                clause.isDisabled = false;
            });
        }
        
        // Navigate to edit group view
        $scope.editGroup = function (group) {
            // persist the current group to access between states
            dataService.setCurrentEditGroup(group);

            // navigate to the edit group view
            navigationService.goToState(clApp.navigation.states.editGroup);
        }

        // Delete group
        $scope.deleteGroup = function (group, index) {
            // disable the group while the delete method runs
            group.isDisabled = true;

            var deleteConfirm = $modal.open(modalService.getGenericModal("Are you sure you want to delete this item?"));

            deleteConfirm.result.then(function (confirmed) {
                if (confirmed) {
                    // user confirmed delete...deleting group

                    var notification = notificationService.notify("Deleting group...", notificationService.types.pending);

                    groupItemService.delete(group)
                        .then(function (deletedGroup) {
                            // create a successful notification
                            notificationService.update(notification, "Group '"+group.Title+"' has been successfully deleted", notificationService.types.success);

                            $scope.Groups.splice($scope.Groups.indexOf(group), 1);

                            dataService.setSyncRequired(true);
                            initialize();

                        })
                        .catch(function (error) {
                            //something went wrong; if the clause hasn't been deleted, re-enable it
                            group.isDisabled = false;

                            // display error notification
                            notificationService.update(notification, error, notificationService.types.error);
                        });
                }
            }, function () {
                console.log('delete cancelled');
                group.isDisabled = false;
            });
        }

        $scope.toggleFavouriteClause = function (clause) {
            toggleFavourite(clause, { ClauseId: clause.Id }).then(function(updatedClause) {
                localDatabaseService.updateItem(updatedClause, 'clauses').then(function () {
                    console.log('updated clause in local DB');
                    localDatabaseService.getAllFromStore('clauses').then(function(clauses) {
                        allClauses = clauses;
                    }).catch(function(err) {
                        console.log(err);
                    });
                });
            }).catch(function(err) {
                console.error(err);
            });
        }

        $scope.toggleFavouriteGroup = function (group) {
            toggleFavourite(group, { GroupId: group.Id });
        }

        var toggleFavourite = function (listItem, favouriteTemplate) {
            var deferred = $q.defer();

            var clauseLibrarySourceUrl = sessionService.getClauseLibrarySourceUrl();
            
            if (listItem.Favourite) {
                // do not pass along Author information
                var favouriteToDelete = {
                    Id: listItem.Favourite.Id,
                    ClauseId: listItem.Favourite.ClauseId,
                    GroupId: listItem.Favourite.GroupId
                }
                favouriteItemService.deleteItem(clauseLibrarySourceUrl, favouriteToDelete);

                listItem.Favourite = null;
                notificationService.notify(listItem.Title + " is no longer a favorite.", notificationService.types.success);
                setFavouriteFilterFlags();
                deferred.resolve(listItem);
            } else {
                var favouriteToAdd = {
                    Id: -1, //This needs to be -1 to be set by sharepoint
                    ClauseId: favouriteTemplate.ClauseId
                }

                favouriteItemService.saveItem(clauseLibrarySourceUrl, favouriteToAdd)
                .then(function (responseData) {
                    notificationService.notify(listItem.Title + " set as favorite.", notificationService.types.success);
                    listItem.Favourite = responseData;
                    setFavouriteFilterFlags();
                    deferred.resolve(listItem);
                })
                .catch(function (error) {
                    notificationService.notify(error, notificationService.types.error);
                    deferred.reject(error);
                });
            }
            return deferred.promise;
        }

        // Add clause to document
        $scope.addClauseToDocument = function (clause) {
            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Nov', 'Dec'];
            var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thurs', 'Fri', 'Sat']
            var now = new Date();
            var fullHours = now.getHours();
            var modifier = fullHours >= 12 ? 'PM' : 'AM';
            var timeHours = fullHours > 12 ? fullHours - 12 : fullHours;
            var timeMins = now.getMinutes();
            var time = [timeHours, ':', (timeMins < 10 ? '0'+timeMins: timeMins), ' ', modifier].join('');
            var month = months[now.getMonth()];
            var d = now.getDate();
            var day = days[now.getDay()];
            var date = [time, ' ', day, ' ', month, ' ', (d < 10 ? '0' + d : d), ', ', now.getFullYear()].join('');
            var userEmail = $scope.userEmail;
            var user = sessionService.getUser();
            var event = ['Clause "', clause.Title, '" used'].join('');
            var label = [(user && user.Title ? user.Title : userEmail), ' | ', userEmail, ' | ', date].join('');
            
            console.log(event, label);

            if (clause) {
                $analytics.eventTrack(event, { category: 'Clause Usage', label: label});
                officeService.addClauseToDocument(clause.Text);
            }
        }

        // Toggle collapse state of the item
        $scope.headerClick = function() {
        }

        // Toggles the view-state of the actions (visible if the item
        // is open or the header is hovered)
        $scope.toggleActions = function(item) {
            item.actionsVisible = !item.actionsVisible;
        }

        $scope.getDesigneeEmails = function(designees) {
            if (designees && designees.length > 0) {
                var designeeEmails = [designees.shift().EMail];
                designeeEmails.concat(designees.map(function (item) {
                    console.log(item.EMail);
                    return item.EMail;
                }));
                console.table(designeeEmails);
                return designeeEmails.join(", ");
            }
        }

        $scope.$watch('clauseSearchText', function (newValue) {
            if (!newValue || newValue == '') {
                // display the manage view when the search field goes empty
                $scope.getActiveViewName = 'manage';
                manageViewService.setActiveView('manage');
            }
            manageViewService.setActiveSearchQuery(newValue);
        });

        // queries the local DB for matching clauses
        $scope.searchClauses = function() {

            $scope.searchLoading = true;
            localDatabaseService.searchItems($scope.clauseSearchText, allClauses).then(function (results) {
                $scope.searchResults = results;
                $scope.getActiveViewName = manageViewService.setActiveView('search');
                usSpinnerService.stop('search-load');
                $scope.searchLoading = false;
            }).catch(function (error) { });

        }

        // Inner-view navigation for different manage states
        $scope.clickManageNavItem = function (navItem) {
            for (var i = 0; i < $scope.navItems.length; i++) {
                $scope.navItems[i].isActive = false;
            }
            navItem.isActive = true;
            navItem.action();
        }

        // Return a set of view-navigation items
        function getNavItems() {
            var navItems = [
                {
                    label: "All Clauses",
                    href: "#manage-content-container",
                    isActive: true,
                    action: showManageView
                },
                {
                    label: "My Clauses",
                    href: "#manage-content-container",
                    isActive: false,
                    action: getMyClauses
                },
                {
                    label: "Favorites",
                    href: "#manage-content-container",
                    isActive: false,
                    action: getFavorites
                }
            ];
            return navItems;
        }

        function showManageView() {
            resetFilterFlags();
            $scope.clauseSearchText = null;
            $scope.getActiveViewName = manageViewService.setActiveView('manage');
            }

        function getMyClauses() {
            //resetFilterFlags();
            //$scope.showMyClauses = true;
            $scope.searchLoading = true;
            $scope.getActiveViewName = manageViewService.setActiveView('search');
            localDatabaseService.searchMyClauses(allClauses).then(function (myClauses) {
                $scope.searchResults = myClauses;
                usSpinnerService.stop('search-load');
                $scope.searchLoading = false;
            }).catch(function (err) {
                console.error(err);
                });
        }

        function getFavorites() {

            //resetFilterFlags();
            //$scope.showFavourites = true;
            $scope.searchLoading = true;
            $scope.getActiveViewName = manageViewService.setActiveView('search');
            localDatabaseService.searchFavorites(allClauses).then(function(favorites) {
                $scope.searchResults = favorites;
                usSpinnerService.stop('search-load');
                $scope.searchLoading = false;
            }).catch(function(err) {
                console.error(err);
            });

            //Set filtering flags
            //setFavouriteFilterFlags();
        }

        //helper functions for favourites filtering:
        var setFavouriteFilterFlags = function () {
            var filterFavouritesCondition = function (item) {
                return item.Favourite;
            };
            setFavouriteFilterFlagOnSubItems($scope.Groups, filterFavouritesCondition);
            setFavouriteFilterFlagOnSubItems($scope.Clauses, filterFavouritesCondition);
        }

        //Go through all of the items in the groups and clauses
        var setFavouriteFilterFlagOnSubItems = function (items, filterCondition) {
            if (!items) {
                return;
            }
            for (var i = 0; i < items.length; i++) {
                var showItemInFavouritesList = filterCondition(items[i]);
                //If this is a favourite then don't do any further filtering (we want to see everything in the subtree)
                //Set the filtering function to always return true for items in the subtree
                var newfilterCondition = showItemInFavouritesList ? function () { return true; } : filterCondition;
                setFavouriteFilterFlagOnSubItems(items[i].Groups, newfilterCondition);
                setFavouriteFilterFlagOnSubItems(items[i].Clauses, newfilterCondition);
                items[i].showInFavourites = showItemInFavouritesList || hasFlaggedFavouriteItemInSubTree(items[i].Groups) || hasFlaggedFavouriteItemInSubTree(items[i].Clauses);
            }
        }

        var hasFlaggedFavouriteItemInSubTree = function (items) {
            if (!items) {
                return false;
            }
            for (var i = 0; i < items.length; i++) {
                if (items[i].showInFavourites) {
                    return true;
                }
            }
            return false;
        };

        // Initializes the view and its data
        function initialize() {

            if ($scope.isOnline) {

                if (!sessionService.isLoggedIn()) {
                    console.log('user not logged in');
                    return;
                }

                // Request a new access token just to keep it fresh
                sessionService.updateAccessToken().then(function () {

                    settingsService.spUpdateRequired().then(function(response) {
                        if (response) {
                            var notification = notificationService.notify("A SharePoint upgrade is required. Performing upgrade...", notificationService.types.pending);
                            settingsService.upgrade().then(function() {
                                notificationService.update(notification, "SharePoint upgrade successful.", notificationService.types.success);
                                $scope.defaultLibrary = sessionService.getDefaultLibrary();
                                if (!$scope.defaultLibrary) {
                                    console.log('no default library found');
                                    return;
                                }

                                // Determines whether the library is built-out from data retrieved
                                // from sharepoint, or, if the local DB is already sync'd, to
                                // use the local DB data to populate the library instead
                                if (dataService.isSyncRequired()) {
                                    // A data sync is required; this is true the first time a user
                                    // logs into the app. Subsequent data requests are made from
                                    // the local database. User must be online.
                                    initializeRemoteData();
                                } else {
                                    // No data sync is required with SharePoint; go ahead and
                                    // use local DB data to populate the library
                                    initializeLocalData();
                                }
                            }).catch(function(err) {
                                notificationService.update(notification, "SharePoint upgrade not successful: " + err, notificationService.types.error);
                            });
                        } else {
                            $scope.defaultLibrary = sessionService.getDefaultLibrary();
                            if (!$scope.defaultLibrary) {
                                console.log('no default library found');
                                return;
                            }

                            // Determines whether the library is built-out from data retrieved
                            // from sharepoint, or, if the local DB is already sync'd, to
                            // use the local DB data to populate the library instead
                            if (dataService.isSyncRequired()) {
                                // A data sync is required; this is true the first time a user
                                // logs into the app. Subsequent data requests are made from
                                // the local database. User must be online.
                                initializeRemoteData();
                            } else {
                                // No data sync is required with SharePoint; go ahead and
                                // use local DB data to populate the library
                                initializeLocalData();
                            }
                        }
                    }).catch(function(err) {
                        notificationService.notify("A SharePoint update check failed: " + err, notificationService.types.error);
                        $scope.goToSettings();
                    });
                    
                    
                    
                }).catch(function() {
                    notificationService.notify(
                        'Something went wrong with your login. Please login again.',
                        notificationService.types.error);
                });
            } else {
                if (!sessionService.getCurrentLocalDB()) {
                    $scope.noLocalData = true;
                                $scope.manageLoading = false;
                    console.log('user is offline and local database is empty');
                    return;
                    }
                    
                // user is offline retrieve the clause library from a local indexedDB
                initializeLocalData();
                }
        }

        function initializeRemoteData() {

            // NOTE: there is a nested async.parallel chain. Please read the comments
            // herein carefully.
            async.parallel([
                function (outerCallback) {

                    // The inner async.parallel method is used for retrieving
                    // the smaller data: root-level groups, root-level clauses, and tags.
                    // The purpose of putting it into its own async.parallel method is so
                    // that when the async methods in this portion of the chain are completed
                    // we can hide the loading spinner and indicate that the main manage view
                    // loading is complete (even though we are still waiting on the BULK 
                    // request for all clauses might still be running. See comments on that below)
                    async.parallel([

                        // Retrieve all groups
                        function (callback) {
                            groupItemService.getAll(true).then(function (groups) {
                                // Assign top level groups to scope
                                $scope.Groups = groups.filter(function (item) { return !item.ParentId; });

                                callback(null, groups);
                            }).catch(function (err) {
                                callback(err, null);
                            });
                        },

                        // Retrieve root-level clauses - for display in UI
                        // storeLocally boolean set to false --> getRoot(false)
                        // because we are already requesting all clauses in this async
                        // parallel chain.
                        function (callback) {
                            clauseItemService.getRoot(false).then(function (clauses) {
                                $scope.Clauses = clauses;
                                callback(null, clauses);
                            }).catch(function (error) {
                                callback(error, null);
                            });
                        },

                        // Retreive tags
                        function (callback) {
                            tagService.getAll(true).then(function (tags) {
                                $scope.Tags = tags;
                                callback(null, tags);
                            }).catch(function (error) {
                                notificationService.notify(
                                    'Failed to load tags',
                                    notificationService.types.error);
                                callback(error, null);
                            });
                        }
                    ],

                    // Done function that gets called when the inner async.parallel
                    // method completes; when this fires, our UI is ready for use,
                    // even though we are still waiting on the BULK request of all clauses
                    // to use for search (see comments below)
                    function (err, results) {
                        if (err) {
                            outerCallback(err, results);
                        } else {
                            // stop the spinner, acknowledge that loading
                            // the root items (groups/clauses) is complete
                            usSpinnerService.stop('manage-load');
                            $scope.manageLoading = false;
                            outerCallback(null, results);
                        }
                    });
                },

                // Retrieve all clauses - FOR SEARCH.
                // This request will likely take the longest and especially if there
                // are several hundred or thousands of clauses; it is kept out of the
                // inner async.parallel method because otherwise the manage view
                // would continue to display the loading spinner until the BULK request
                // completes, which can cause the app to timeout.
                function (outerCallback) {
                    clauseItemService.getAll(true).then(function (clauses) {
                        console.log(clauses);
                        allClauses = clauses;
                        $scope.searchReady = true;
                        outerCallback(null, clauses);
                    }).catch(function (error) {
                        notificationService.notify(
                            'Failed to load clauses',
                            notificationService.types.error);
                        outerCallback(error, null);
                    });
                }
            ],

            // This is the done method for the outer async.parallel method;
            // this way we can keep track of when all async calls have been
            // completed.
            function (err, results) {
                if (err) {
                    notificationService.notify(
                        'Library initialization failed',
                        notificationService.types.error);
                } else {
                    dataService.setSyncRequired(false);
                    initializationSuccessful();
                }
            });
        }

        function initializeLocalData() {
            async.parallel([
                function (callback) {
                    localDatabaseService.getRootFromStore('clauses').then(function (clauses) {
                        $scope.Clauses = clauses;
                        callback(null, clauses);
                    }).catch(function (err) {
                        notificationService.notify(
                            'Failed to retrieve clauses from local DB',
                            notificationService.types.error);
                        callback(err, null);
                    });
                },

                function (callback) {
                    localDatabaseService.getAllFromStore('clauses').then(function (clauses) {
                        allClauses = clauses;
                        $scope.searchReady = true;
                        callback(null, clauses);
                    }).catch(function (err) {
                        notificationService.notify(
                            'Failed to retrieve clauses from local DB',
                            notificationService.types.error);
                        callback(err, null);
                    });
                },

                function (callback) {
                    localDatabaseService.getRootFromStore('groups').then(function (groups) {
                        $scope.Groups = groups;
                        //manageViewService.findActive($scope.Groups, 'group', null, true);
                        callback(null, groups);
                    }).catch(function (err) {
                        notificationService.notify(
                            'Failed to retrieve groups from local DB',
                            notificationService.types.error);
                        callback(err, null);
                    });
                }
            ], function (err) {
                if (err) {
                    notificationService.notify(
                        'Failed to initialize library',
                        notificationService.types.error);
                } else {
                    dataService.setSyncRequired(false);
                    usSpinnerService.stop('manage-load');
                    $scope.manageLoading = false;
                    initializationSuccessful();
                }
            });
        }

        function initializationSuccessful() {
            console.info('Initialization completed successfully');
            
            psAccordionService.onready(function(item, toggleFn) {
                var type = item.hasOwnProperty('Clauses') ? 'group' : 'clause';
                if (manageViewService.isItemActive(item, type)) {
                    toggleFn(item, true);
                }
            });
        }

        initialize();


       //adding JS for the pivot link 
        (function ($) {
  $.fn.Pivot = function () {

      /** Go through each pivot we've been given. */
    return this.each(function () {

      var $pivotContainer = $(this);

          /** When clicking/tapping a link, select it. */
      $pivotContainer.on('click', '.ms-Pivot-link', function(event) {
        event.preventDefault();
        $(this).siblings('.ms-Pivot-link').removeClass('is-selected');
        $(this).addClass('is-selected');
      });

        });

        };
        }) (jQuery);

        if ($.fn.Pivot) {
            $('.ms-Pivot').Pivot();
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