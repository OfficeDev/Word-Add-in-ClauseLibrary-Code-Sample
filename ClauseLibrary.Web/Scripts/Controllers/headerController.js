// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    var app = angular.module('clauseLibraryApp');

    headerController['$inject'] = [
        '$scope',
        '$location',
        'sessionService',
        'navigationService',
        'offlineService'
    ];

    app.controller('headerController', headerController);

    function headerController($scope, $location, sessionService, navigationService, offlineService) {

        $scope.headerTitle = navigationService.getPageTitle();
        $scope.userEmail = sessionService.getUserEmail();
        $scope.isUserAdmin = sessionService.isUserAdmin();
        $scope.isOnline = offlineService.isOnline();

        navigationService.subscribe(function (path, state, title) {
            $scope.headerTitle = title;
        });

        offlineService.subscribe(function(status) {
            $scope.isOnline = status;
        });

        $scope.updateTitle = function(title) {
            $scope.headerTitle = navigationService.setPageTitle(title);
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