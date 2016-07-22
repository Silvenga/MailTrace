import {
    bootstrap
} from 'aurelia-bootstrapper-webpack';

var Promise = require('bluebird').config({
    longStackTraces: false,
    warnings: false
});

bootstrap(function(aurelia) {
    aurelia.use
        .standardConfiguration()
        .developmentLogging();

    aurelia.start().then(() => aurelia.setRoot('app/app', document.body));
});