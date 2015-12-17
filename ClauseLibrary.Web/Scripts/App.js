/* Common app functionality */

var clApp = (function () {
    "use strict";

    var app = {};

    // define angular app and dependencies.
    app.clauseLibraryApp = angular.module('clauseLibraryApp', [
        'ui.router',
        'ui.select',
        'ui.ps',
        'ui.hover',
        'ui.designeeList',
        'ui.removeClass',
        'ui.trustHtml',
        'angularSpinner',
        'ui.tree',
        'ui.bootstrap',
        'ngSanitize',
        'clauselibrary.core.services',
        'angulartics',
        'angulartics.google.analytics',
        'ngMessages',
        'indexedDB',
        'fabric.ui.components'
    ]);
    
    app.navigation = {
        states: {
            blank:      { name: 'blank', title: 'Clause Library', url: '/', templateUrl: 'manage.html', controller: 'manageController' },
            account:    { name: 'account', title: 'account', url: '/app/account', templateUrl: 'account.html', controller: 'accountController' },
            manage:     { name: 'manage', title: '', url: '/app/manage', templateUrl: 'manage.html', controller: 'manageController' },
            addClause:  { name: 'addclause', title: 'create clause', url: '/app/addclause', templateUrl: 'editclause.html', controller: 'addClauseController' },
            editClause: { name: 'editclause', title: 'edit clause', url: '/app/editclause', templateUrl: 'editclause.html', controller: 'editClauseController' },
            editGroup:  { name: 'editgroup', title: 'edit group', url: '/app/editgroup', templateUrl: 'editgroup.html', controller: 'editGroupController' },
            addGroup:   { name: 'addgroup', title: 'create group', url: '/app/addgroup', templateUrl: 'editgroup.html', controller: 'addGroupController' },
            settings:   { name: 'settings', title: 'settings', url: '/app/settings', templateUrl: 'settings.html', controller: 'settingsController' }
        }
    }

    app.clauseLibraryApp.config([
        '$stateProvider', '$urlRouterProvider', 'uiSelectConfig', 'usSpinnerConfigProvider',
        function ($stateProvider, $urlRouterProvider, uiSelectConfig, usSpinnerConfigProvider) {
            
            // Initialise routes
            $urlRouterProvider.otherwise('/');
            angular.forEach(app.navigation.states, function (state) {
                $stateProvider.state(state.name, state);
            });

            // Initialse select behaviour
            uiSelectConfig.theme = 'bootstrap';
            uiSelectConfig.resetSearchInput = true;

            // Initialize the spinner
            usSpinnerConfigProvider.setDefaults({
                lines: 9, // The number of lines to draw
                length: 0, // The length of each line
                width: 4, // The line thickness
                radius: 6, // The radius of the inner circle
                corners: 1, // Corner roundness (0..1)
                rotate: 0, // The rotation offset
                direction: 1, // 1: clockwise, -1: counterclockwise
                color: '#000', // #rgb or #rrggbb or array of colors
                speed: 1.3, // Rounds per second
                trail: 50, // Afterglow percentage
                shadow: false, // Whether to render a shadow
                hwaccel: false, // Whether to use hardware acceleration
                className: 'spinner', // The CSS class to assign to the spinner
                zIndex: 0, // The z-index (defaults to 2000000000)
                top: '-9px', // Top position relative to parent
                left: '100%', // Left position relative to parent
                position: 'relative'
            });

        }]);

    // Perform first run functions.
    app.clauseLibraryApp.run([
        '$http', '$q', '$injector', 'sessionService', 'navigationService', 'offlineService', 'localDatabaseService', 
        function ($http, $q, $injector, sessionService, navigationService, offlineService, localDatabaseService) {
            // Initialize app activity
            //sessionService.initializeActivityTracking(Date.now());

            // update activity on any click or keypress
            //$('body').click(function() {
            //    sessionService.setTimeOfLastActivity(Date.now());
            //}).keypress(function() {
            //    sessionService.setTimeOfLastActivity(Date.now());
            //});

            var isOnline = offlineService.isOnline();


            // Apply token if rendered by server for app to consume.
            if (typeof (sessionContext) === 'undefined') {
                window.sessionContext = {};
            }

            // if local storage hasn't been initialized after login,
            // something went wrong. Return the user through the auth
            // flow again.
            if (!localStorage.getItem('clauselibrary.IsSet') && isOnline) {
                window.location = '/app/welcome';
            } else {
                // we are offline...
            }

            $injector.get("$http").defaults.transformRequest = function (data, headersGetter) {
                var hostWebUrl = sessionService.getHostWebUrl();

                // If hostWebUrl is null, we have not configured a clause library and
                // we are entering the app as a first-time user.
                if (hostWebUrl) {
                    if (sessionService.isLoggedIn()) {
                        headersGetter()['Authorization'] = "Bearer " + sessionService.getAccessToken();
                        headersGetter()['Expiration'] = sessionService.getExpiration();
                        headersGetter()['CurrentUserEmail'] = sessionService.getUserEmail();
                        headersGetter()['IsAdmin'] = sessionService.isUserAdmin() && sessionService.isUserAdmin().toString();
                    }
                    if (data) {
                        return angular.toJson(data);
                    }
                }
            };

            // connect to default local DB if one exists
            var localDB = sessionService.getCurrentLocalDB();
            if (localDB) {
                localDatabaseService.connect(localDB);
            }

            navigationService.goToState(app.navigation.states.manage);

        }]);

    app.clauseLibraryApp.filter('clauseSearch', function() {
        return function() {
            
        }
    });

    // Custom filter for composite objects.

    /**
     * AngularJS default filter with the following expression:
     * "person in people | filter: {name: $select.search, age: $select.search}"
     * performs a AND between 'name: $select.search' and 'age: $select.search'.
     * We want to perform a OR.
     */
    app.clauseLibraryApp.filter('propertiesOrFilter', function () {
        return function (items, props) {
            var out = [];

            if (angular.isArray(items)) {
                items.forEach(function (item) {
                    var itemMatches = false;

                    var keys = Object.keys(props);
                    for (var i = 0; i < keys.length; i++) {
                        var prop = keys[i];
                        var text = props[prop].toLowerCase();
                        if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                            itemMatches = true;
                            break;
                        }
                    }

                    if (itemMatches) {
                        out.push(item);
                    }
                });
            } else {
                // Let the output be the input untouched
                out = items;
            }

            return out;
        };
    });

    //Filter the topmost layer by some condition
    //Return a new collection with only the items that satisfy the filter
    var filterCollection = function (unfilteredItems, filterCondition) {
        var filteredItems = [];
        for (var i = 0; i < unfilteredItems.length; i++) {
            if (filterCondition(unfilteredItems[i])) {
                filteredItems.push(unfilteredItems[i]);
            }
        }
        return filteredItems;
    }

    app.clauseLibraryApp.filter('hasTags', function () {

        var filterTaggedOnly = function (item) {
            return item.hasOwnProperty("Tags") && item.Tags && item.Tags.length > 0;
        };

        return function (unfilteredItems, hasTagsChecked) {
            if (!hasTagsChecked) {
                return unfilteredItems;
            }
            return filterCollection(unfilteredItems, filterTaggedOnly);
        }
    });

    app.clauseLibraryApp.filter('isMyClause', ['sessionService', function (sessionService) {

        return function (unfilteredItems, showMyClauses) {
            if (!unfilteredItems) return [];

            if (showMyClauses) {
                var currentUserEmail = sessionService.getUserEmail();

                var filteredClauses = unfilteredItems.filter(function(item) {
                    var designeeMatches = item.DesigneesList && item.DesigneesList.filter(function(designee) {
                        return designee && designee.EMail == currentUserEmail;
                    });

                    // returns match if user's email matches clause's author email or if
                    // user is found in the list of designees for the clause
                    return (item.Author && item.Author.EMail == currentUserEmail) ||
                    (designeeMatches && designeeMatches.length > 0);
                });

                return filteredClauses;
            } else {
                return unfilteredItems;
            }
        }
    }]);

    app.clauseLibraryApp.filter('isFavourite', function () {

        var filterFavourites = function (item) {
            return item.showInFavourites;
        }

        return function (unfilteredItems, showFavourites) {
            if (!showFavourites) {
                return unfilteredItems;
            }
            return filterCollection(unfilteredItems, filterFavourites);
        }
    });

    angular.module('authenticationHub.service', []).factory('authenticationHubService', function () {

        var hub = $.connection.authenticationHub;
        var parentId = '';
        var signoutUrl = '/authentication/signout?signOutId=';
        var redirectUrl = '/app/welcome';

        hub.client.onSignOutSuccess = function (hubId) {
            if (hubId === $.connection.hub.id) {
                window.location = redirectUrl;
            }
        };

        return {
            //setup signalR hub
            signout: function () {
                $.connection.hub.start().done(function () {
                    parentId = $.connection.hub.id;
                    signoutUrl += parentId;
                    window.open(signoutUrl);
                });
            }
        }
    });

    return app;
})();