// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    'use strict';

    var app = angular.module('ui.designeeList', []);
    app.controller("designeeListController", function ($scope) {
        console.log('designee list controller');

        $scope.emails = [];
        $scope.designees = [];

    }).directive("designeeList", function () {
        return {
            restrict: "A",
            link: function($scope, $element, attrs) {
                if (attrs && attrs.hasOwnProperty('designeeList')) {
                    var item = JSON.parse(attrs['designeeList']);
                    if (item && item.DesigneesList && item.DesigneesList.length > 0) {
                        var designees = item.DesigneesList;
                        if (designees && designees.length > 0) {
                            $scope.designees = designees;
                            $scope.emails = designees.map(function (designee) {
                                return designee.EMail;
                            });
                        }
                        $scope.emails && $scope.emails.length > 0 &&
                            $element.attr({
                                'title': $scope.emails.join(', '),
                                'href': [
                                    'mailto:',
                                    $scope.emails.join('; '),
                                    '?subject=Clause Library - ',
                                    item.Title,
                                    '&cc=',
                                    item.Author.EMail
                                ].join('')
                            });
                    }
                }

                
            }
        };
    });

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