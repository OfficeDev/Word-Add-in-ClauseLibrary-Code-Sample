// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE in the project root for license information.

/**
 * Spinner Component
 *
 * An animating activity indicator.
 *
 */

/**
 * @namespace fabric
 */
var fabric = fabric || {};

/**
 * @param {DOMElement} holderElement - The element the Spinner will attach itself to.
 * @param {string} type - The type of spinner. Set this property to "sixteen" for sixteen dot version. The default is eight.
 */

fabric.Spinner = function(holderElement, spinnerType) {

    var _holderElement = holderElement;
    var _spinnerType = spinnerType || "eight";
    var eightSize = 0.18;
    var sixteenSize = 0.1;
    var circleObjects = [];
    var animationSpeed = 80;
    var interval;
    var spinner;
    var numCircles;
    var offsetSize;

    /**
     * @function start - starts or restarts the animation sequence
     * @memberOf fabric.Spinner
     */
    function start() {
        interval = setInterval(function() {
            var i = circleObjects.length;
            while(i--) {
                _fade(circleObjects[i]);
            }
        }, animationSpeed);
    }

    /**
     * @function stop - stops the animation sequence
     * @memberOf fabric.Spinner
     */
    function stop() {
        clearInterval(interval);
    }

    //private methods

    function _init() {
        if(_spinnerType === "sixteen") {
            offsetSize = sixteenSize;
            numCircles = 16;
        } else {
            offsetSize = eightSize;
            numCircles = 8;
        }
        _createCirclesAndArrange();
        _initializeOpacities();
        start();
    }

    function _initializeOpacities() {
        var i = numCircles, j;
        while(i--) {
            j = circleObjects.length;
            while(j--) {
                _fade(circleObjects[j]);
            }
        }
    }

    function _fade(circleObject) {
        var opacity;
        if(circleObject.j < numCircles) {
            if(Math.floor(circleObject.j / (numCircles / 2))) {
                opacity = _getOpacity(circleObject.element) - 2 / numCircles;
            } else{
                opacity = _getOpacity(circleObject.element) + 2 / numCircles;
            }
        } else {
            circleObject.j = 0;
            opacity = 2/ numCircles;
        }
        _setOpacity(circleObject.element, opacity);
        circleObject.j++;
    }

    function _getOpacity(element) {
        return parseFloat(window.getComputedStyle(element).getPropertyValue("opacity"));
    }

    function _setOpacity(element, opacity) {
        element.style.opacity = opacity;
    }

    function _createCircle() {
        var circle = document.createElement('div');
        var parentWidth = parseInt(window.getComputedStyle(spinner).getPropertyValue("width"), 10);
        circle.className = "ms-Spinner-circle";
        circle.style.width = circle.style.height = parentWidth * offsetSize + "px";
        return circle;
    }

    function _createCirclesAndArrange() {
        spinner = document.createElement("div");
        spinner.className = "ms-Spinner";
        _holderElement.appendChild(spinner);
        var width = spinner.clientWidth;
        var height = spinner.clientHeight;
        var angle = 0;
        var offset = width * offsetSize;
        var step = (2 * Math.PI) / numCircles;
        var i = numCircles;
        var circleObject;
        var radius = (width- offset) * 0.5;
        while(i--) {
            var circle = _createCircle();
            var x = Math.round(width * 0.5 + radius * Math.cos(angle) - circle.clientWidth * 0.5) - offset * 0.5;
            var y = Math.round(height * 0.5 + radius * Math.sin(angle) - circle.clientHeight * 0.5) - offset * 0.5;
            spinner.appendChild(circle);
            circle.style.left = x + 'px';
            circle.style.top = y + 'px';
            angle += step;
            circleObject = {element:circle, j:i};
            circleObjects.push(circleObject);
        }
    }

    _init();

    return {
        start:start,
        stop:stop
    };
};