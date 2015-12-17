// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    var app = angular.module('clauseLibraryApp');

    welcomeController['$inject'] = [
        '$scope',
        '$http',
        'usSpinnerService',
        'offlineService',
        'notificationService',
        'localStorageService',
        'authenticationHubService',
        'analyticsService'
    ];

    app.controller('welcomeController', welcomeController);

    function welcomeController(
        $scope, $http, usSpinnerService, offlineService, notificationService,
        localStorageService, authenticationHubService, analyticsService) {

        // online() and offline() functions are called at the
        // very bottom of this controller upon initialization
        $scope.isOnline = offlineService.isOnline();
        console.log($scope.isOnline);

        $scope.loggingIn = $scope.isOnline;

        offlineService.subscribe(function (status) {
            console.log('status changed', status);
            $scope.isOnline = status;

            $scope.$apply();

            $scope.isOnline ? online() : offline();
        });

        $scope.signOut = function () {
            localStorageService.clearAllItems();

            authenticationHubService.signout(function () {
                window.location = authenticationHubService.redirectUrl;
            });
        }

        $scope.cancelLogin = function() {
            $scope.loggingIn = false;
        }

        function online() {
            console.log('online');

            var appUrl = "/app";
            var signInUrl = "/authentication/signin?parentid=" + 0;
            var hub = $.connection.authenticationHub;

            //setup signalR hub
            hub.client.onLoginSuccess = function (id, accessInfo) {

                if (id === $.connection.hub.id) {

                    var parsedAccessInfo = JSON.parse(accessInfo);

                    // processes the access info and initializes local storage
                    // with its values
                    new ClauseLibrary.AccessInfo(parsedAccessInfo).store();

                    window.location = appUrl;
                }
            };

            // handle login error / exception thrown during login
            hub.client.onLoginError = function (id, error) {
                notificationService.notify(error, notificationService.types.error);
                $scope.signOut();
            }

            // Start the connection.
            $.connection.hub.start().done(function () {

                //get the parentId off the hub
                var parentId = $.connection.hub.id;

                //wire the event to launch popup (passing the parentId)
                signInUrl = "/authentication/signin?parentid=" + parentId;

                $('#signInLink').click(function () {
                    $scope.loggingIn = true;
                    usSpinnerService.spin('login-pending');
                    $scope.$apply();
                    window.open(signInUrl);
                });

                var spWebUrl    = localStorage.getItem('clauselibrary.SPTenantUrl');
                var tenantId        = localStorage.getItem('clauselibrary.TenantId');
                var refreshToken = localStorage.getItem('clauselibrary.RefreshToken');

                if (refreshToken && spWebUrl && tenantId) {
                    $.post('/Authentication/RefreshAccessToken', {
                        refreshToken: refreshToken,
                        spWebUrl: spWebUrl,
                        tenantId: tenantId
                    }).done(function (accessToken) {
                        console.log(accessToken);

                        localStorage.setItem('clauselibrary.AccessToken', accessToken);
                        // we've initialized the access token; navigate to index
                        window.location = appUrl;
                    }).fail(function (err) {
                        console.log(err);
                        $scope.loggingIn = false;
                        usSpinnerService.stop('login-pending');
                        notificationService.notify('An error occurred while logging in.  Please try again.', notificationService.types.error);
                        $scope.$apply();
                    });
                } else {
                    console.log('no access info');
                    $scope.loggingIn = false;
                    usSpinnerService.stop('login-pending');
                    $scope.$apply();
                }
            });
        }

        function offline() {
            console.log('offline');
            $scope.loggingIn = false;
            $('#signInOffline').click(function () {
                window.location = '/app';
            });
        }

        // first-time initialization
        $scope.isOnline ? online() : offline();
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