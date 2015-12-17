// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('session.service', [])
        .service('sessionService', sessionService);

    sessionService['$inject'] = [
        '$q', '$http', '$interval', '$timeout', '$modal',
        'localStorageService',
        'constantsService',
        'modalService',
        'authenticationHubService',
        'offlineService'
    ];

    function sessionService(
        $q, $http, $interval, $timeout, $modal, localStorageService,
        constantsService, modalService, authenticationHubService, offlineService) {

        // Defines a reference to the object inner scope for
        // use inside of anonymous (and "private") methods
        var self = this;

        // takes an integer as a number of minutes and returns
        // the value in milliseconds
        function toMilliseconds(minutes) {
            return minutes * 60 * 1000;
        }

        var ACCESS_INFO_VALID = false;
        var EXPIRATION_CHECK_INTERVAL = toMilliseconds(5); // update access token every 5 minutes
        var IS_ONLINE = offlineService.isOnline();
        var expirationInterval;

        // <Not in use>
        var INACTIVITY_CHECK_INTERVAL = toMilliseconds(0.33); 
        var INACTIVITY_DURATION_LIMIT = toMilliseconds(5); 
        var INACTIVITY_PROMPT_TIMEOUT = toMilliseconds(1);
        var INACTIVITY_TIMER_PAUSED = false;
        var timeOfLastActivity = -1;
        var timeOfLogin = -1;
        var inactivityInterval;
        var inactivityPromptTimeout;
        // </Not in use>

        var signOutCallbacks = [];

        // subscribe to changes from offline service
        offlineService.subscribe(function(status) {
            IS_ONLINE = status;
        });

        this.initializeActivityTracking = function (unixTimestamp) {
            timeOfLogin = unixTimestamp;
            this.setTimeOfLastActivity(unixTimestamp, 'LOGIN: ');
        }

        this.getTimeOfLastActivity = function() {
            return timeOfLastActivity;
        }

        this.setTimeOfLastActivity = function (unixTimestamp, message) {
            console.log((message || 'Activity: '), unixTimestamp);
            timeOfLastActivity = unixTimestamp;
        }

        this.setNewLibraryConnection = function (newLibrary) {
            localStorageService.setItem(constantsService.storage.session.tenantId, newLibrary.TenantId);
            localStorageService.setItem(constantsService.storage.session.hostWebUrl, newLibrary.HostWebUrl);
            localStorageService.setItem(constantsService.storage.session.defaultLibrary, newLibrary);
        }

        this.getHostWebUrl = function() {
            return localStorageService.getItem(constantsService.storage.session.hostWebUrl);
        }

        this.setHostWebUrl = function (url) {
            var hostWebUrlKey = constantsService.storage.session.hostWebUrl;
            if (localStorageService.getItem(hostWebUrlKey) == url) return false;

            else localStorageService.setItem(hostWebUrlKey, url);

            return true;
        }

        this.getClauseLibrarySourceUrl = function () {
            // append /clauselibrary api to sharepoint host url endpoint
            return this.getHostWebUrl();
        };

        this.getSpWebUrl = function() {
            return localStorageService.getItem(constantsService.storage.session.spWebUrl);
        };

        this.getUserId = function() {
            //retrieve the userId (guid) of the logged in user
            return localStorageService.getItem(constantsService.storage.session.userId);
        };
        this.getTenantId = function () {
            //retrieve the userId (guid) of the logged in user
            return localStorageService.getItem(constantsService.storage.session.tenantId);
        };

        this.getUserEmail = function() {
            return localStorageService.getItem(constantsService.storage.session.userEmail);
        }

        this.getUser = function() {
            return localStorageService.getItem(constantsService.storage.session.user);
        }

        this.setUser = function(updatedUser) {
            localStorageService.setItem(constantsService.storage.session.user, updatedUser);
        }

        this.isUserAdmin = function() {
            return localStorageService.getItem(constantsService.storage.session.isAdmin);
        }

        this.setIsUserAdmin = function(isUserAdmin) {
            localStorageService.setItem(constantsService.storage.session.isAdmin, isUserAdmin);
        }

        // retrieves the access token expiration date
        this.getExpiration = function () {
            return localStorageService.getItem(constantsService.storage.authentication.accessToken);
        }

        this.getAccessToken = function() {
            return localStorageService.getItem(constantsService.storage.authentication.accessToken);
        }

        this.getRefreshToken = function () {
            return localStorageService.getItem(constantsService.storage.authentication.refreshToken);
        }

        this.setAccessToken = function(accessToken) {
            localStorageService.setItem(constantsService.storage.authentication.accessToken, accessToken);
        }

        this.setCurrentLocalDB = function(databaseName) {
            localStorageService.setItem(constantsService.storage.session.currentLocalDB, databaseName);
        }

        this.getCurrentLocalDB = function() {
            return localStorageService.getItem(constantsService.storage.session.currentLocalDB);
        }

        this.setDefaultLibrary = function(defaultLibrary) {
            localStorageService.setItem(constantsService.storage.session.defaultLibrary, defaultLibrary);
        }

        this.getDefaultLibrary = function () {
            return localStorageService.getItem(constantsService.storage.session.defaultLibrary);
        }

        this.checkForExpiration = function () {
            expirationInterval = $interval(function () {
                if (!INACTIVITY_TIMER_PAUSED && IS_ONLINE) {
                    // retrieve a new access token
                    self.updateAccessToken();
                }
            }, EXPIRATION_CHECK_INTERVAL);
        }

        this.checkForInactivity = function () {
            inactivityInterval = $interval(checkForInactivityInterval, INACTIVITY_CHECK_INTERVAL);
        }

        this.updateAccessToken = function () {
            var tenantId        = self.getTenantId();
            var spWebUrl        = self.getSpWebUrl();
            var refreshToken    = self.getRefreshToken();

            var deferred = $q.defer();
            $http({
                method: 'POST',
                url: '/Authentication/RefreshAccessToken',
                headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                transformRequest: function(obj) {
                    var str = [];
                    for(var p in obj)
                        str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                    return str.join("&");
                },
                data: {
                        refreshToken: refreshToken,
                        spWebUrl: spWebUrl,
                        tenantId: tenantId
                    }
            })
            .success(function (accessToken) {
                // update the access token in local storage
                self.setAccessToken(accessToken);

                console.info('updated access token at: ', new Date());

                deferred.resolve(accessToken);
            })
            .error(function (err) {
                var error = err;
                if (err && err.hasOwnProperty('ExceptionMessage'))
                    error = err['ExceptionMessage'];

                var dialog = $modal.open(modalService.getSessionErrorModal());
                INACTIVITY_TIMER_PAUSED = true;
                dialog.result.then(function (confirmed) {
                    $interval.cancel(expirationInterval);
                    self.signOut();
                }, function () {
                    $interval.cancel(expirationInterval);
                    self.signOut();
                });

                deferred.reject(error);
            });

            return deferred.promise;
        }

        function getTimeToExpiration() {
                // check whether we are within 10 minutes of access token expiration
                var expiration = localStorageService.getItem(constantsService.storage.authentication.expiresOn);
                var expirationDatetime = new Date(expiration).getTime(); // unix timestamp
                var now = Date.now();
                var timeDiff = expirationDatetime - now;
            var totalSecs = Math.floor(timeDiff / 1000);
            var totalMins = totalSecs / 60;
            var mins = Math.round(totalMins);
            var secs = Math.round((totalMins % 1) * 60);
            secs = secs < 10 ? '0' + secs : secs;
            var hours = new Number(mins / 60).toFixed(4);

            var minsSecs2 = [mins, ':', secs, ' (m, s)'].join('');
            console.log('expiration in: ', minsSecs2);
        }

        function checkForInactivityInterval() {
            if (!INACTIVITY_TIMER_PAUSED) {
                console.log('checking for inactivity...');
                var diff = Date.now() - timeOfLastActivity;
                if (INACTIVITY_DURATION_LIMIT < diff) {
                    // open dialog
                    // stop interval
                    displayInactivityPrompt();
                }
            }
        }

        function displayInactivityPrompt() {

            var promptDialog = $modal.open(modalService.getInactivityPromptModal(INACTIVITY_PROMPT_TIMEOUT));
            INACTIVITY_TIMER_PAUSED = true;

            promptDialog.result.then(function (confirmed) {
                if (confirmed) {
                    INACTIVITY_TIMER_PAUSED = false;
                } else {
                    console.log('cancelled');
                    // carry on with the interval timer
                    INACTIVITY_TIMER_PAUSED = false;
                }
            }, function (result) {
                if (result == 'escape key pressed' || result == 'backdrop click')
                    INACTIVITY_TIMER_PAUSED = false;
                else {
                    console.log('inactive');
                    displayLogoutModal();
                }
            });
        }

        function displayLogoutModal() {
            INACTIVITY_TIMER_PAUSED = true;
            var dialog = $modal.open(modalService.getInactivityLogoutModal());

            dialog.result.then(function (confirmed) {
                $interval.cancel(inactivityInterval);
                $interval.cancel(expirationInterval);
                self.signOut();
            }, function () {
                $interval.cancel(inactivityInterval);
                $interval.cancel(expirationInterval);
                self.signOut();
            });
        }

        this.checkForExpiration();

        // disabled for now
        //this.checkForInactivity();


        this.isLoggedIn = function () {
            return Boolean(this.getAccessToken());
        }

        this.signOut = function () {
            self.clearSessionCookies();
            localStorageService.clearAllItems();

            async.each(signOutCallbacks, function(callback, next) {
                    signOutCallbacks.pop()().then(function(r) {
                        next();
                    }).catch(function(err) {
                        console.error(err);
                        next(err);
                    });
                },
            function(err) {
                if (err) {
                    console.error(err);
                } else {
                    authenticationHubService.signout(function () {
                        window.location = authenticationHubService.redirectUrl;
                    });
                }
            });
        }

        this.onSignOut = function (callback) {
            if (typeof callback == 'function')
                signOutCallbacks.push(callback);
            else
                console.error('Callback must be a function');
        }

        this.getDocumentCookiesAsJson = function () {
            var d = document.cookie && document.cookie.split(';');
            var k = {};
            for (var i = 0; i < d.length; i++) {
                if (d[i]) {
                    var s = d[i].split('=');
                    if (s && s.length > 1) {
                        var n = s[0] && s[0].trim();
                        var o = s[1] && s[1].split('|');
                        if (o) {
                            k[n] = {};
                            if (o.length > 1) {
                                var v = o[0];
                                var e = o[1];
                                k[n]['value'] = v.trim();
                                k[n]['expires'] = e.trim();
                            } else if (o.length == 1) {
                                k[n]['value'] = o[0] && o[0].trim();
                            }
                        }
                    }
                }
            }
            return k;
        }

        this.clearSessionCookies = function () {
            var c = this.getDocumentCookiesAsJson();

            // set expiration date to the past
            var expires = new Date();
            expires.setMonth(expires.getMonth() - 1);

            for (var k in c) {
                document.cookie = k + '=|' + expires.toGMTString() + ';';
            }
        }

        this.isAuthenticated = function () {
            return ACCESS_INFO_VALID;
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
