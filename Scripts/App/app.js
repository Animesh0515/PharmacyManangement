/// <reference path="../angular.min.js" />
var app = angular.module('myApp', ["ngRoute"]);

app.config(function ($routeProvider) {
    $routeProvider
        .when('/EditPurchase/:id', {
            templateUrl: '/Purchase/EditPurchase.html',
            controller: 'EditPurchase'
        });
});