// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

(function (angular) {
/*
 * UI components
 */
    angular.module("fabric.ui.components", [
        "fabric.ui.components.button",
        "fabric.ui.components.label",
        "fabric.ui.components.textfield"
    ]);

    //-----------------------Button-------------------------
    var ButtonDirective = (function () {
        function ButtonDirective() {
            this.template = '<button ng-click="action()" ng-class="{\'ms-Button\':true, \'ms-Button--primary\': isPrimary}">' +
                '<span class="ms-Button-label">{{label}}</span>' +
                '</button>';

            this.scope = {
                label: '@',
                action: '&',
                isPrimary: '='
            };
        }
        ButtonDirective.prototype.link = function (scope, elem, attrs) {
        };
        ButtonDirective.factory = function () {
            var directive = function () { return new ButtonDirective(); };
            return directive;
        };
        return ButtonDirective;
    })();

    angular.module("fabric.ui.components.button", [])
        .directive("uifButton", ButtonDirective.factory());

//-----------------------Label-------------------------
    var LabelDirective = (function () {
        function LabelDirective() {
            this.restrict = 'A';

            this.scope = {
                isRequired: '='
            };
        }
        LabelDirective.prototype.link = function (scope, elem, attrs) {
            elem.addClass('control-label ms-Label ms-fontWeight-semibold');
            if (scope.$eval(attrs.isRequired)) {
                elem.html(elem.html() + '*');
            }
        };
        LabelDirective.factory = function () {
            var directive = function () { return new LabelDirective(); };
            return directive;
        };
        return LabelDirective;
    })();

    angular.module("fabric.ui.components.label", [])
        .directive("uifLabel", LabelDirective.factory());;

//-----------------------textbox-------------------------
    var TextfieldDirective = (function () {
        function TextfieldDirective() {
            this.restrict = 'A';

            this.scope = {
                isRequired: '='
            };
        }
        TextfieldDirective.prototype.link = function (scope, elem, attrs) {
            elem.addClass('form-control ms-TextField-field');
            if (scope.$eval(attrs.isRequired)) {
                elem.html(elem.html() + '*');
            }
        };
        TextfieldDirective.factory = function () {
            var directive = function () { return new TextfieldDirective(); };
            return directive;
        };
        return TextfieldDirective;
    })();

    angular.module("fabric.ui.components.textfield", [])
        .directive("uifTextfield", TextfieldDirective.factory());;



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