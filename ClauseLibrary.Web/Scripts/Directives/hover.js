// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";

    // Thanks @broofa!
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    var app = angular.module('ui.hover', []);
    app.controller('hoverController', [
        '$scope', function($scope) {
            $scope.hoverIn = function() {
                //console.log('hover in');
            }
            $scope.hoverOut = function() {
                //console.log('hover out');
            }
        }
    ]).directive('hover', function() {
        return {
            restrict: 'A',
            controller: 'hoverController',
            link: function ($scope, $element) {

                // applies a unique guid to all hover elements related
                // to this control; provides a way to scope child / sibling /
                // target elements.
                var uniqueId = guid();
                $element.attr('hover-guid', uniqueId);
                $element.find('[hover-class]').attr('hover-guid', uniqueId);

                $element.mouseover(function (e) {
                    e.stopPropagation();

                    $(this).addClass('hover-in');

                    var targetGuid = $(this).attr('hover-guid');

                    console.log(targetGuid);

                    // add hover-in class to child elements with hoverClass
                    // directive applied
                    $('[hover-class]').each(function () {
                        var $item = $(this);
                        if ($item.attr('hover-guid') == targetGuid) {
                            $item.addClass('hover-in');
                        } else {
                            $item.removeClass('hover-in');
                        }
                    });

                    $scope.hoverIn();

                }).mouseout(function (e) {
                    e.stopPropagation();

                    $(this).removeClass('hover-in');

                    // remove hover-in class from child elements with hoverClass
                    // directive applied
                    $(this).find('.hover-in').removeClass('hover-in');

                    $scope.hoverOut();

                });
            }
        }
    }).directive('hoverClass', function() {
        return {
            restrict: 'A',
            controller: 'hoverController',
            link: function() {
            }
        }
    }).directive('hoverTarget', ['$timeout', function () {

        var attributeName = 'hoverTarget';

        return {
            restrict: 'A',
            controller: 'hoverController',
            link: function ($scope, $element, attrs) {

                var handle = function() {

                    if (!attrs || !attrs[attributeName] || typeof attrs[attributeName] != 'string') {
                        console.error('Invalid value passed to hover-target; value must be a valid selector string');
                        return;
                    }

                    // applies a unique guid to all hover elements related
                    // to this control; provides a way to scope child / sibling /
                    // target elements.
                    var uniqueId = guid();
                    $element.attr('hover-guid', uniqueId);


                    var targetSelector = attrs[attributeName];
                    var $target = $element.find(targetSelector);
                    $target.attr('hover-guid', uniqueId);
                    console.log(targetSelector, $target);

                    // handle-target should be used on the parent-most element; this way
                    // we can search the current parent's children and maintain scope
                    $element.find('[hover-class]').attr('hover-guid', uniqueId);

                    $target.mouseover(function(e) {
                        e.stopPropagation();

                        $(this).addClass('hover-in');

                        var parentGuid = $element.attr('hover-guid');

                        // add hover-in class to child elements with hoverClass
                        // directive applied
                        $('[hover-class]').each(function() {
                            var $item = $(this);
                            if ($item.attr('hover-guid') == parentGuid) {
                                $item.addClass('hover-in');
                            } else {
                                $item.removeClass('hover-in');
                            }
                        });

                        $scope.hoverIn();

                    }).mouseout(function (e) {
                        e.stopPropagation();

                        $(this).removeClass('hover-in');

                        // remove hover-in class from child elements with hoverClass
                        // directive applied
                        $('.hover-in').removeClass('hover-in');

                        $scope.hoverOut();

                    });

                }

                //var timer = $timeout(handle, 0);
                handle();
            }
        }
    }]);

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