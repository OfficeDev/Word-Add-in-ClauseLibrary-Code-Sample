// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    'use strict';

    var DEBUG = true;

    /*
	*	INCLUDING IN THE PROJECT:
	*	# add tree.css and tree.js to the project
	* 	# add 'ui.tree' as a dependency to the parent app module
	*	# include the tree-view directive in the HTML
	*		# <tree-view tree='data' ng-controller="treeController"></tree-view>
	*
	*/

    /* 
	 * An Angular service which helps with creating recursive directives.
	 * @author Mark Lagendijk
	 * @license MIT
	 */
    angular.module('RecursionHelper', [])
	.factory('RecursionHelper', ['$compile', function ($compile) {
	    return {
	        /**
			 * Manually compiles the element, fixing the recursion loop.
			 * @param element
			 * @param [link] A post-link function, or an object with function(s) registered via pre and post properties.
			 * @returns An object containing the linking functions.
			 */
	        compile: function (element, link) {
	            // Normalize the link parameter
	            if (angular.isFunction(link)) {
	                link = { post: link };
	            }

	            // Break the recursion loop by removing the contents
	            var contents = element.contents().remove();
	            var compiledContents;
	            return {
	                pre: (link && link.pre) ? link.pre : null,
	                /**
					 * Compiles and re-adds the contents
					 */
	                post: function (scope, element) {
	                    // Compile the contents
	                    if (!compiledContents) {
	                        compiledContents = $compile(contents, "foo");
	                    }
	                    // Re-add the compiled contents to the element
	                    compiledContents(scope, function (clone) {
	                        element.append(clone);
	                    });

	                    // Call the post-linking function, if any
	                    if (link && link.post) {
	                        link.post.apply(null, arguments);
	                    }
	                }
	            };
	        }
	    };
	}]);

    var app = angular.module('ui.tree', ['RecursionHelper']);

    app.controller('treeController', ['$scope', function ($scope) {
        function toggleTreeRecurse(items) {
            if (items && items.length > 0) {
                for (var i = 0; i < items.length; i++) {
                    if (items[i].isCollapsed)
                        $scope.defaults.open(items[i]);
                    else
                        $scope.defaults.close(items[i]);

                    items[i].isCollapsed = $scope.defaults.collapsed;

                    toggleTreeRecurse(items[i][$scope.defaults.items]);
                }
            }
            return;
        }

        $scope.defaults = {
            collapsed: true,
            emptyMessage: "Empty",
            items: "items",
            contents: "contents",
            label: "label",
            radioValue: 'radioValue',
            radio: function(item) {
                DEBUG && console.log('item radio selected', item);
            },
            toggle: function (item) {
                DEBUG && console.log('item toggled (default handler)', item.isCollapsed);
            },
            open: function (item, callback) {
                DEBUG && console.log('item opened (default handler)');
                if (callback && typeof callback === 'function') {
                    callback();
                }
            },
            close: function (item) {
                DEBUG && console.log('item closed (default handler)');
            },
            dynamic: false,
            loaded: function(item, data) {
                DEBUG && console.log('TREE loaded callback', item, data);
                item.isLoading = false; // done loading! hide the spinner
                item.doNotLoad = true;
            },
            error: function (item, error) {
                // uh-oh
                DEBUG && console.log('TREE error :[', item, error);
                item.isLoading = false; // done loading
            }
        };

        $scope.radioValue = null;

        $scope.tree = {};

        $scope.setTree = function (data) {
            $scope.tree = data;
        }

        $scope.toggle = function (item) {
            // call the toggle handler (either default or user-specified)
            if (!item.isCollapsed) {
                // call close handler
                $scope.defaults.close(item);
            } else {
                // do not display loading spinner if item is already loaded
                if ($scope.defaults.dynamic && !item.doNotLoad) {
                    item.isLoading = true; // making a dynamic request for content, display loading spinner
                }
                $scope.defaults.open(item, function(data) {
                    $scope.defaults.loaded(item, data);
                }, function(error) {
                    $scope.defaults.error(item, error);
                });
            }

            item.isCollapsed = !item.isCollapsed;

            $scope.defaults.toggle(item);
        }

        
        $scope.toggleTree = function () {
            $scope.defaults.collapsed = !$scope.defaults.collapsed;
            if ($scope.tree && $scope.tree[$scope.defaults.items]) {
                toggleTreeRecurse($scope.tree[$scope.defaults.items]);
            }
        }
    }])
	.directive('treeView', function () {

	    return {
	        restrict: 'E',
	        template:
			'<div>' +
				//'<span ng-click="toggleTree()" class="tree-icon"' +
				//	'ng-class="{\'tree-expand-all\': defaults.collapsed, \'tree-collapse-all\': !defaults.collapsed}">' +
				//'</span>' +
				//'<span class="tree-title">{{tree["defaults.label"]}}</span>' +
				'<tree data="tree"></tree>' +
			'</div>',
	        link: {
	            pre: function ($scope, element, attrs) {

	                function updateController() {
	                    // update the treeController tree to render the UI
	                    $scope.tree = $scope && fields['data'] ? $scope[fields['data']] : {};
	                };

	                console.log('treeView');

	                if (!attrs['tree'])
	                    console.error('You must specify a "tree" attribute whose value is equal to the property name containing your tree data');

	                if (attrs && attrs['tree']) {



	                    var fields = JSON.parse(JSON.stringify(eval("(" + attrs['tree'] + ")")));

	                    $scope.defaults.contents = fields['contents'] && fields['contents'];
	                    $scope.defaults.items = fields['items'] && fields['items'];
	                    $scope.defaults.label = fields['label'] && fields['label'];
	                    $scope.defaults.radioValue = fields['radioValue'] && fields['radioValue'];

	                    // check whether the dynamic field is set and ensure it is a boolean							
	                    if (
						fields['dynamic'] !== undefined && typeof fields['dynamic'] !== 'boolean') {
	                        console.error('"dynamic" must be a boolean');
	                    } else {
	                        $scope.defaults.dynamic = fields['dynamic'];
	                    }

	                    // attach toggle, open, close event handlers, if specified in fields
	                    var handlers = {
	                        toggle: fields['toggle'],
	                        open: fields['open'],
	                        close: fields['close'],
                            radio: fields['radio']
	                    };

	                    for (var handler in handlers) {
	                        DEBUG && console.log(handler);
	                        var handlerName = handlers[handler];
	                        if (handlerName) {
	                            // check whether the toggle method is set and ensure it is a function
	                            if (handlerName !== undefined && typeof handlerName !== 'string') {
	                                console.error('The value for "' + handler + '" must be a function name as a string');
	                            } else {
	                                // ensure the function exists in the client controller scope
	                                // and that it is a function
	                                if ($scope[handlerName] &&
									typeof $scope[handlerName] === 'function') {
	                                    $scope.defaults[handler] = $scope[handlerName];

	                                    DEBUG && console.log('attached: ' + handlerName);
	                                } else {
	                                    console.error('No function named "' + handlerName + '" exists');
	                                }
	                            }
	                        }
	                    }

	                    // only apply the watch if the user explicitly sets 'dynamic: true'.
	                    // by default, dynamic is false and a watch is not set up to dynamically refresh the tree
	                    if ($scope.defaults.dynamic) {
	                        $scope.$watch(fields['data'], function (updated, previous) {
	                            DEBUG && console.log('Tree dynamically updating...');
	                            updateController();
	                        }, true);
	                    }

	                    updateController();
	                }
	            }
	        }
	    }
	})
	.directive('tree', function (RecursionHelper) {

	    var loadingImageSrc = "data:image/gif;base64,R0lGODlhDwAPALMPAMrKygwMDJOTkz09PZWVla+vr3p6euTk5M7OzuXl5TMzMwAAAJmZmWZmZszMzP///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgAPACwAAAAADwAPAAAEQvDJaZaZOIcV8iQK8VRX4iTYoAwZ4iCYoAjZ4RxejhVNoT+mRGP4cyF4Pp0N98sBGIBMEMOotl6YZ3S61Bmbkm4mAgAh+QQFCgAPACwAAAAADQANAAAENPDJSRSZeA418itN8QiK8BiLITVsFiyBBIoYqnoewAD4xPw9iY4XLGYSjkQR4UAUD45DLwIAIfkEBQoADwAsAAAAAA8ACQAABC/wyVlamTi3nSdgwFNdhEJgTJoNyoB9ISYoQmdjiZPcj7EYCAeCF1gEDo4Dz2eIAAAh+QQFCgAPACwCAAAADQANAAAEM/DJBxiYeLKdX3IJZT1FU0iIg2RNKx3OkZVnZ98ToRD4MyiDnkAh6BkNC0MvsAj0kMpHBAAh+QQFCgAPACwGAAAACQAPAAAEMDC59KpFDll73HkAA2wVY5KgiK5b0RRoI6MuzG6EQqCDMlSGheEhUAgqgUUAFRySIgAh+QQFCgAPACwCAAIADQANAAAEM/DJKZNLND/kkKaHc3xk+QAMYDKsiaqmZCxGVjSFFCxB1vwy2oOgIDxuucxAMTAJFAJNBAAh+QQFCgAPACwAAAYADwAJAAAEMNAs86q1yaWwwv2Ig0jUZx3OYa4XoRAfwADXoAwfo1+CIjyFRuEho60aSNYlOPxEAAAh+QQFCgAPACwAAAIADQANAAAENPA9s4y8+IUVcqaWJ4qEQozSoAzoIyhCK2NFU2SJk0hNnyEOhKR2AzAAj4Pj4GE4W0bkJQIAOw==";

	    return {
	        restrict: 'E', // element only
	        scope: {
	            data: '='
	        },

	        template:
			'<ul>' +
                '<li ng-repeat="item in data[defaults.items]" ng-class="{collapsed: item.isCollapsed}">' +
					'<div class="tree-label" ng-click="toggle(item)">' +
						'{{item[defaults.label]}}' +
					'</div>' +

					'<div ng-show="!item.isCollapsed" class="tree-item-contents">' +
                        '<input type="radio" ng-model="defaults.radioValue" ng-change="defaults.radio(item)"' +
                            'name="createLibraryRadio" value="{{item[defaults.contents]}}" />' +
                        '{{item[defaults.contents]}}' +
                    '</div>' +
                    '<span ng-show="!item.isCollapsed && item.isLoading" class="tree-item-loading">' +
                        '<img class="tree-item-loading-icon" src="' + loadingImageSrc + '"/>' +
                        '<em>Loading items...</em>' +
                    '</span>' +
					'<tree data="item"></tree>' +
					'<em ng-show="item.isEmpty && !item.isCollapsed" class="tree-empty">{{item.emptyMessage}}</em>' +
                '</li>' +
            '</ul>',

	        compile: function (element, foo) {
	            return RecursionHelper.compile(element, function ($scope, element, attrs) {

	                if ($scope.data) {
	                    $scope.defaults = $scope.$parent.defaults;

	                    $scope.toggle = $scope.$parent.toggle;

	                    if (!$scope.data.hasOwnProperty('isCollapsed')) {
	                        $scope.data.isCollapsed = $scope.defaults.collapsed;
	                    }

	                    $scope.data.emptyMessage = $scope.defaults.emptyMessage;

	                    if ((!$scope.data[$scope.defaults.contents] ||
						$scope.data[$scope.defaults.contents] == "") &&
						($scope.data[$scope.defaults.items] &&
						$scope.data[$scope.defaults.items].length == 0)) {
	                        $scope.data.isEmpty = true;
	                    } else {
	                        $scope.isEmpty = false;
	                    }
	                }
	            });
	        }
	    }
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