/* Common app functionality */

(function () {
    "use strict";

    var app = {};

    // define angular app and dependencies.
    app.clauseLibraryApp = angular.module('clauseLibraryApp', [
        'clauselibrary.welcome.services',
        'ui.removeClass',
        'angularSpinner'
    ]);

    app.clauseLibraryApp.config(['usSpinnerConfigProvider', function (usSpinnerConfigProvider) {

        // Initialize the spinner
        usSpinnerConfigProvider.setDefaults({
            lines: 9, // The number of lines to draw
            length: 0, // The length of each line
            width: 4, // The line thickness
            radius: 6, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 0, // The rotation offset
            direction: 1, // 1: clockwise, -1: counterclockwise
            color: '#000', // #rgb or #rrggbb or array of colors
            speed: 1.3, // Rounds per second
            trail: 50, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: '-9px', // Top position relative to parent
            left: '100%', // Left position relative to parent
            position: 'relative'
        });

    }]);

})();