// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
    "use strict";
    angular
        .module('manageView.service', [])
        .service('manageViewService', manageViewService);

    manageViewService['$inject'] = ['sessionService', 'psAccordionService'];

    function manageViewService(sessionService, psAccordionService) {
        var activeItems = [];
        var activeItemData = {};
        var activeView = 'manage'; // manage by default
        var activeSearchQuery = ''; // empty by default

        this.openEachActiveItem = function() {
            async.eachSeries(activeItems, function(key, callback) {
                console.log('opening: ', key);
                var item = activeItemData[key].item;
                var toggleFn = activeItemData[key].toggle;

                // open handler must return a promise:
                // $q.defer().promise
                toggleFn(item, true).then(function() {
                    callback();
                }).catch(function(err) {
                    callback(err);
                });

            }, function (err) {
                if (err)
                    console.error('Failed to open');
                else
                    console.log('opened item...');
            });
        }

        this.forEachActiveItem = function(callback) {
            for (var i = 0; i < activeItems.length; i++) {
                var item = activeItems[i];
                var components = item.split('|');
                var type = components[0];
                var id = components[1];
                var handlers = activeItemData[item];
                callback(id, type, handlers.open, handlers.close);
            }
        }

        this.getActiveItems = function() {
            return activeItems;
        }

        this.setActiveItem = function (item, type) {
            var element = [type, '|', item.Id].join('');
            if (activeItems.indexOf(element) > -1) return;

            var parentIdKey = type == 'clause' ? 'GroupId' : 'ParentId';
            var parentElement = ['group|', item[parentIdKey]].join('');
            var parentElementIndex = activeItems.indexOf(parentElement);

            // if no parent for this item exists in the list, we're trying
            // to open an accordion that's outside of our current nested context.
            // in this case, clear the array entirely
            parentElementIndex < 0 && this.clearActiveItems();

            // if the parent element is not the last in the list, then another
            // sibling item is opened; slice off all elements in the list after
            // THIS current it's parent.
            if (parentElementIndex < activeItems.length - 1)
                activeItems = this.clearItemsAfterIndex(parentElementIndex);

            activeItems.push(element);

            // keep track of the item's open/close handlers
            //activeItemData[element] = {
            //    open: openHandler || function() { console.log('item opened', element); },
            //    close: closeHandler || function () { console.log('item closed', element); },
            //    toggle: toggleFn,
            //    item: item
            //}

            console.log('added item: ', '"' + element.toString() + '" -> ', activeItems);
        }

        this.removeActiveItem = function (item, type) {
            var element = [type, '|', item.Id].join('');

            activeItems.splice(activeItems.indexOf(element), activeItems.length);

            // unlink the handlers associated with the item
            //delete activeItemData[element];

            console.log('removed item: ', '"' + element.toString() + '" -> ', activeItems);
        }

        this.clearItemsAfterIndex = function(index) {
            return activeItems.slice(0, index + 1);
        }

        this.clearActiveItems = function () {
            activeItems = [];
        }

        this.isItemActive = function (item, type) {
            return activeItems.indexOf([type, '|', item.Id].join('')) > -1;
        }

        this.setActiveView = function(viewName) {
            activeView = viewName;
            return viewName;
        }

        this.getActiveViewName = function() {
            return activeView;
        }

        this.setActiveSearchQuery = function(searchQuery) {
            activeSearchQuery = searchQuery;
            return searchQuery;
        }

        this.getActiveSearchQuery = function() {
            return activeSearchQuery;
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