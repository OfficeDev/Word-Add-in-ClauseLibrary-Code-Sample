// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    var app = angular.module('clauseLibraryApp');
    notificationController['$inject'] = ['$scope', 'notificationService'];
    app.controller('notificationController', notificationController);

    function notificationController($scope, notificationService) {

        $scope.notifications = [];
        $scope.allVisible = false;
        $scope.isVisible = false;

        $scope.toggleAllVisible = function() {
            $scope.allVisible = !$scope.allVisible;
            if ($scope.allVisible) {

                // adds a click handler to the entire document so that when the user
                // clicks outside of the notifications container the notifications (if all visible)
                // will close.
                $('html').addClass('notification-clickeater')
                .click(function (e) {
                    // checks to ensure the click does not happen inside of the
                    // notifications handler to allow the user to click around as
                    // needed in the notifications; with the exception of the 
                    // "# Notifications" link that will toggle the visiblity state
                    if (!$(e.target).closest('#notification-container').length ||
                        $(e.target).hasClass('.notifications-count')) {

                        // hide everything
                        $scope.allVisible = false;

                        // $apply, since we're inside of the jQuery context
                        $scope.$apply();

                        // clean up - unbinds the click handler to avoid multiple-binding
                        $(this).unbind('click');

                        $(this).removeClass('notification-clickeater');
                    } else {
                        // do nothing
                    }
                });
            } else {
                // toggling the notifications to invisible, make sure there's
                // no existing clickeater handler or classes lying around.
                $('html.notification-clickeater')
                    .removeClass('notification-clickeater')
                    .unbind('click');
            }
        }

        // hides a notification, does not clear it from the notifications list
        $scope.dismiss = function(notification) {
            notification.isActive = false;
        }

        $scope.isPending = function(notification) {
            return notification.type == notificationService.types.pending;
        }

        // clears a notification from the notifications list
        $scope.clearNotification = function (notification, index) {
            notificationService.clearOne(index);
        }

        // commands the notification service to remove all notifications
        $scope.clearAllNotifications = function () {
            notificationService.clearAll();
        }

        function initialise() {
            $scope.notifications = notificationService.notifications;
            notificationService.subscribe(function (updatedNotifications) {
                $scope.isVisible = true;
                $scope.notifications = updatedNotifications || [];
                //console.log($scope.notifications);
            });
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