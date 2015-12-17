// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
/*
 * UI components
 */
    angular.module("ui.ps", [
        "ui.ps.tpls",
        "ui.ps.transition",
        "ui.ps.collapse",
        "ui.ps.accordion"
    ]);


    angular.module("ui.ps.tpls", [
        "template/ps-accordion/ps-accordion-group.html",
        "template/ps-accordion/ps-accordion.html"
    ]);

    angular.module("ui.ps.transition", [])
        .factory("$transition", [
            "$q", "$timeout", "$rootScope", function (a, b, c) {
                function d(a) {
                    for (var b in a)
                        if (void 0 !== f.style[b]) return a[b]
                }

                var e = function (d, f, g) {
                    g = g || {};
                    var h = a.defer(),
                        i = e[g.animation ? "animationEndEventName" : "transitionEndEventName"],
                        j = function () {
                            c.$apply(function () {
                                d.unbind(i, j), h.resolve(d);
                            })
                        };
                    return i && d.bind(i, j), b(function () {
                        angular.isString(f) ? d.addClass(f) : angular.isFunction(f) ? f(d) : angular.isObject(f) && d.css(f), i || h.resolve(d)
                    }), h.promise.cancel = function () {
                        i && d.unbind(i, j), h.reject("Transition cancelled")
                    }, h.promise
                },
                    f = document.createElement("trans"),
                    g = {
                        WebkitTransition: "webkitTransitionEnd",
                        MozTransition: "transitionend",
                        OTransition: "oTransitionEnd",
                        transition: "transitionend"
                    },
                    h = {
                        WebkitTransition: "webkitAnimationEnd",
                        MozTransition: "animationend",
                        OTransition: "oAnimationEnd",
                        transition: "animationend"
                    };
                return e.transitionEndEventName = d(g), e.animationEndEventName = d(h), e
            }
        ]);

    angular.module("ui.ps.collapse", ["ui.ps.transition"]).directive("collapse", [
        "$transition", function ($transition) {
            return {
                link: function ($scope, $element, d) {

                    function animate(options) {
                        function d() {
                            j === e && (j = void 0);
                        }

                        var e = $transition($element, options);
                        return j && j.cancel(), j = e, e.then(d, d), e;
                    }

                    function expand() {
                        k ? (k = false, expandDone()) : ($element.removeClass("collapse").addClass("collapsing"), animate({
                            height: $element[0].scrollHeight + "px"
                        }).then(expandDone));
                    }

                    function expandDone() {
                        $element.removeClass("collapsing"), $element.addClass("collapse in"), $element.css({
                            height: "auto"
                        });
                    }

                    function collapse() {
                        if (k)
                            k = false, collapseDone(), $element.css({
                                height: 0
                            });
                        else {
                            $element.css({
                                height: $element[0].scrollHeight + "px"
                            });
                            {
                                $element[0].offsetWidth
                            }
                            $element.removeClass("collapse in").addClass("collapsing");

                            animate({
                                height: 0
                            }).then(collapseDone);
                        }
                    }

                    function collapseDone() {
                        $element.removeClass("collapsing");
                        $element.addClass("collapse");
                    }

                    var j, k = true;
                    $scope.$watch(d.collapse, function (shouldCollapse) {
                        shouldCollapse ? collapse() : expand();
                    });
                }
            }
        }
    ]);

    angular.module("ui.ps.accordion", ["ui.ps.collapse"]).constant("psAccordionConfig", {
        closeOthers: true
    }).controller("psAccordionController", [
        "$scope", '$q', "$attrs", "psAccordionConfig", function ($scope, $q, $attrs, psAccordionConfig) {

            this.groups = [];

            this.closeOthers = function (openGroup) {
                var closeOthers = angular.isDefined($attrs.closeOthers) ? $scope.$eval($attrs.closeOthers) : psAccordionConfig.closeOthers;
                if (closeOthers) {
                    angular.forEach(this.groups, function (group) {
                        if (group !== openGroup) {
                            group.isOpen = false;
                        }
                    });
                }
            }

            this.addGroup = function (group) {
                var self = this;
                this.groups.push(group);
                group.$on("$destroy", function () {
                    self.removeGroup(group);
                });
            }

            this.removeGroup = function (groupToRemove) {
                var matchingGroup = this.groups.indexOf(groupToRemove);
                -1 !== matchingGroup && this.groups.splice(matchingGroup, 1);
            }

            this.onToggle = function (toOpen, item, openCallback, closeCallback, toggleFn) {
                var self = this;
                var deferred = $q.defer();
                if (toOpen) {
                    self.isOpen = true;
                    openCallback(item, toggleFn).then(function (results) {
                        deferred.resolve(results);
                    }).catch(function (err) {
                        deferred.reject(err);
                    });
                } else {
                    self.isOpen = false;
                    closeCallback(item, toggleFn).then(function (data) {
                        deferred.resolve(data);
                    }).catch(function (err) {
                        deferred.reject(err);
                    });
                }

                return deferred.promise;
            }

        }
    ]).directive("psAccordion", function () {
        return {
            restrict: "EA",
            controller: "psAccordionController",
            transclude: true,
            replace: false,
            templateUrl: "template/ps-accordion/ps-accordion.html"
        }
    }).directive("psAccordionGroup", ['$q', 'psAccordionService', function ($q, psAccordionService) {
        return {
            require: "^psAccordion",
            restrict: "EA",
            transclude: true,
            replace: true,
            templateUrl: "template/ps-accordion/ps-accordion-group.html",
            scope: {
                heading: "@",
                isOpen: "=?",
                isDisabled: "=?",
                onOpen: "=?",
                onClose: "=?",
                data: "=?"
            },
            controller: function () {
                this.setHeading = function (fn) {
                    this.heading = fn;
                }
            },
            link: function ($scope, $element, attrs, controller) {
                controller.addGroup($scope);

                if (!$scope.onOpen || typeof $scope.onOpen !== 'function')
                    $scope.onOpen = function () { };
                if (!$scope.onClose || typeof $scope.onClose !== 'function')
                    $scope.onClose = function () { };

                $scope.$watch('isOpen', function (value) {
                    if (value) {
                        controller.closeOthers($scope);
                    }
                });

                $scope.toggleOpen = function (itm, forceOpen) {
                    var deferred = $q.defer();
                    var item = itm || $scope.data;
                    var isOpen = !$scope.isOpen;
                    var toOpen = forceOpen != undefined ? forceOpen : isOpen;
                    //$scope.isDisabled || (isOpen = !isOpen);
                    controller.onToggle(toOpen, item, $scope.onOpen, $scope.onClose, $scope.toggleOpen).then(function (data) {
                        deferred.resolve(data);
                    }).catch(function (err) {
                        deferred.reject(err);
                    });
                    return deferred.promise;
                }

                psAccordionService.ready($scope.data, $scope.toggleOpen);
            }
        }
    }]).directive("psAccordionHeading", function () {
        return {
            restrict: "EA",
            transclude: true,
            template: "",
            replace: true,
            require: "^psAccordionGroup",
            link: function ($scope, $element, attrs, psAccordionGroupController, transclude) {
                psAccordionGroupController.setHeading(transclude($scope, function () { }));
            }
        }
    }).directive("psAccordionTransclude", function () {
        return {
            require: "^psAccordionGroup",
            link: function ($scope, $element, attrs, psAccordionGroupController) {
                $scope.$watch(function () {
                    // value
                    return psAccordionGroupController[attrs.psAccordionTransclude];
                }, function (newValue) {
                    // listener
                    newValue && ($element.html(""), $element.append(newValue));
                    if ($scope.data && $scope.data.hasOwnProperty('rendered') &&
                        typeof $scope.data.rendered === 'function') {
                        $scope.data.rendered('child loaded');
                    }
                });
            }
        }
    }).service('psAccordionService', function () {
        var readyCallback = function(item, toggleFn) {
            //console.log('ps-accordion ready');
        };

        this.onready = function (callback) {
            readyCallback = callback;
        }

        this.ready = function (item, toggleFn) {
            //console.log('ready callback', item.Id);
            readyCallback(item, toggleFn);
        }
    });

    angular.module("template/ps-accordion/ps-accordion-group.html", []).run([
        "$templateCache", function ($templateCache) {
            $templateCache.put(
                "template/ps-accordion/ps-accordion-group.html", [
                    '<div class="panel panel-default">\n',
                    '  <div class="panel-heading">\n',
                    '    <h4 class="panel-title">\n',
                    '      <div class="accordion-toggle" ng-click="toggleOpen(group)" ps-accordion-transclude="heading">',
                    '        <span ng-class="{\'text-muted\': isDisabled}">{{heading}}</span>',
                    '      </div>\n',
                    '    </h4>\n',
                    '  </div>\n',
                    '  <div class="panel-collapse" collapse="!isOpen">\n',
                    '    <div class="panel-body" ng-transclude></div>\n',
                    '  </div>\n',
                    '</div>\n'
                ].join('')
            );
        }
    ]),

    angular.module("template/ps-accordion/ps-accordion.html", []).run([
        "$templateCache", function ($templateCache) {
            $templateCache.put("template/ps-accordion/ps-accordion.html", '<div class="panel-group" ng-transclude></div>');
        }
    ]);
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