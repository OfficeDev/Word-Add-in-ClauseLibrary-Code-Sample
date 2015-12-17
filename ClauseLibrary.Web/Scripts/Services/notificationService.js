// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('notifications.service', [])
        .service('notificationService', notificationService);

    notificationService['$inject'] = ['$timeout'];

    function notificationService($timeout) {

        this.notifications = [];
        
        // provides a callback within the controller, used to update the view,
        // by passing back the array of updated notifications.
        this.controllerCallback = function () { };

        // Keep track of how many seconds a notification is visible before it disappears
        var TIMER_LENGTH = 2000; // 2 seconds

        function notification(message, type, isActive) {
            return {
                message: message,
                type: type,
                isActive: isActive
            }
        }

        this.types = {
            success: "success",
            error: "error",
            warning: "warning",
            info: "info",
            pending: "pending"
        }

        this.clearAll = function () {
            // empty the list of notifications
            this.notifications = [];
            this.controllerCallback(this.notifications);
        }

        this.clearOne = function (index) {
            // remove the notification at the given index
            this.notifications.splice(index, 1);
            this.controllerCallback(this.notifications);
        }

        this.notify = function (message, type) {
            // pushes a new notification onto the front of the notification list
            var newNotification = notification(message, type, true);
            this.notifications.unshift(newNotification);

            // sets a timer to remove the notification after a given interval
            if (newNotification.type != this.types.pending) {
                this.eventuallyHide(newNotification);
            }

            // notifies the controller that the notifications have changed
            this.controllerCallback(this.notifications);

            return newNotification;
        }

        this.update = function (updatedNotification, message, type, isActive) {
            updatedNotification.message = message || updatedNotification.message;
            updatedNotification.type = type || updatedNotification.type;
            updatedNotification.isActive = isActive || updatedNotification.isActive;

            if (updatedNotification.type != this.types.pending) {
                this.eventuallyHide(updatedNotification);
            }
        }

        this.eventuallyHide = function(theNotification) {
            var self = this;
            $timeout(function () {
                theNotification.isActive = false;
                self.controllerCallback(self.notifications);
            }, TIMER_LENGTH);
        }

        // We need a way for the notificaiton controller to subscribe to
        // changes in the notifications array, so that we can push notifications
        // to this array from anywhere and be able to invoke a method
        // in notifications controller that will update the notifications view.
        // The notifications controller calls .subscribe(callback), which passes
        // in a callback from within the controller's scope, allowing us to
        // invoke a method directly in the controller.
        this.subscribe = function (controllerCallback) {
            this.controllerCallback = controllerCallback;
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