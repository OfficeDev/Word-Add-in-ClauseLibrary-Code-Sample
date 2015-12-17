// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('modal.service', [])
        .service('modalService', modalService);

    modalService['$inject'] = [];

    function modalService() {

        this.modals = {
            generic: {
                templateUrl: 'genericConfirmModal.html',
                controller: 'genericModalController',
                resolve: {
                    message: function() {
                        return "Generic dialog message";
                    },
                    countdown: function () { return null; }
                }
            },
            info: {
                templateUrl: 'genericInfoModal.html',
                controller: 'genericModalController',
                resolve: {
                    message: function() {
                        return "Generic info message";
                    },
                    countdown: function () { return null; }
                }
            },
            inactivityPrompt: {
                templateUrl: 'inactivityPromptModal.html',
                controller: 'genericModalController',
                resolve: {
                    message: function() { return null; },
                    countdown: function (t) {
                        return t;
                    }
                }
            },
            inactivityLogout: {
                templateUrl: 'inactivityLogoutModal.html',
                controller: 'genericModalController',
                resolve: {
                    message: function() { return null; },
                    countdown: function() { return null; }
                }
            },
            sessionError: {
                templateUrl: 'sessionErrorModal.html',
                controller: 'genericModalController',
                resolve: {
                    message: function() { return null; },
                    countdown: function() { return null; }
                }
            }
        }

        this.getModalProperties = function(key, options) {
            return $.extend(this.modals[key], options);
        }

        this.getGenericModal = function(message) {
            var rules = this.modals.generic;
            rules.resolve.message = function() {
                return message;
            }
            return rules;
        }

        this.getInfoModal = function(message) {
            var rules = this.modals.info;
            rules.resolve.message = function() {
                return message;
            }
            return rules;
        }

        this.getInactivityPromptModal = function (countdown) {
            var rules = this.modals.inactivityPrompt;
            rules.resolve.countdown = function () {
                return countdown;
            }
            return rules;
        }

        this.getInactivityLogoutModal = function () {
            return this.modals.inactivityLogout;
        }

        this.getSessionErrorModal = function() {
            return this.modals.sessionError;
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