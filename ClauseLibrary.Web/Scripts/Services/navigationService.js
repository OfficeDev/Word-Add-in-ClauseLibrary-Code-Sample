// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('navigation.service', [])
        .service('navigationService', navigationService);

    navigationService['$inject'] = ['$location', '$window', 'clauseItemService'];

    function navigationService($location, $window, clauseItemService) {

        var currentPath = clApp.navigation.states.blank.url;
        var currentState = clApp.navigation.states.blank;
        var currentTitle = clApp.navigation.states.blank.title;

        var navigationStateCallback = function() {}

        this.getPageTitle = function() {
            return currentTitle;
        }
        this.setPageTitle = function(title) {
            currentTitle = title;
            return title;
        }

        this.goToPath = function (path) {
            currentPath = path;
            $location.path(path);
        }

        this.back = function() {
            this.goToManage();
        }

        this.goToState = function (state) {
            currentState = state;
            currentPath = state.url;
            currentTitle = state.title;
            navigationStateCallback(currentPath, currentState, currentTitle);
            return this.goToPath(state.url);
        }

        this.goToManage = function () {
            return this.goToState(clApp.navigation.states.manage);
        }

        this.goToWelcome = function() {
            window.location = '/app/welcome';
        }

        this.subscribe = function(callback) {
            navigationStateCallback = callback;
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