app.controller("ReportController", function ($scope, $http, $window, $filter) {


    //For Sales Report part
    $scope.getSales = function () {
        $http({
            method: 'Get',
            url: '/Report/GetSalesReport'
        }).then(function (response) {
            $scope.SalesList = response.data;
            var total = 0;
            angular.forEach($scope.SalesList, function (sales) {
               total += parseInt(sales.CustomerPurchase.GrandTotal);
            });
            $scope.GrandTotal = total;
        }, function (response) {
            $scope.showSalesSuccess= false;
            $scope.showSalesError = true;
            $scope.salesErrorMessage = "Something Went Wrong. Contact Admin";
        });
    }
    $scope.calculateSalesTotalAmount = function (model) {
        var total = 0;
        angular.forEach($scope.SalesList, function (sales) {
            if (sales.CustomerPurchase.CustomerPurchaseId == model.CustomerPurchase.CustomerPurchaseId) {
                angular.forEach(sales.CustomerPurchasedMedicine, function (medicine) {
                    total += medicine.TotalAmount;
                });
            }
        });
        return total;
    };

    $scope.sortSalesAsc = function () {
        $scope.salesPurchasedDate = 'CustomerPurchase.PurchasedDate';
        //$scope.reverse = false;
    };

    $scope.sortSalesDesc = function () {
        $scope.salesPurchasedDate = '-CustomerPurchase.PurchasedDate';
        //$scope.reverse = true;
    };
    $scope.filterSalesTable = function () {
        $scope.salesDateFilter = $filter('salesDateRangeFilter')($scope.SalesList, $scope.fromDate, $scope.toDate);
        var total = 0;
        angular.forEach($scope.salesDateFilter, function (sales) {
            total += sales.CustomerPurchase.GrandTotal;
        });
        $scope.GrandTotal = total;
    };


    //For Purchase Report Part
    $scope.getPurchases = function () {
        $http({
            method: 'Get',
            url: '/Report/GetPurchaseReport'
        }).then(function (response) {
            $scope.PurchaseList = response.data;
            var total = 0;
            angular.forEach($scope.PurchaseList, function (purchase) {
                total += parseInt(purchase.GrandTotal);
            });
            $scope.GrandTotal = total;
        }, function (response) {
            $scope.showPurchaseSuccess = false;
            $scope.showPurchaseError = true;
            $scope.purchaseErrorMessage = "Something Went Wrong. Contact Admin";
        });
    }

    $scope.calculatePurchaseTotalAmount = function (model) {
        var total = 0;
        angular.forEach($scope.PurchaseList, function (purchase) {
            if (purchase.PurchaseId == model.PurchaseId) {
                angular.forEach(purchase.MedicinePurchasedModels, function (medicine) {
                    total += medicine.TotalAmount;
                });
            }
        });
        return total;
    };

    $scope.sortPurchaseAsc = function () {
        $scope.purchasePurchasedDate = 'PurchasedDate';
        //$scope.reverse = false;
    };

    $scope.sortPurchaseDesc = function () {
        $scope.purchasePurchasedDate = '-PurchasedDate';
        //$scope.reverse = true;
    };

    $scope.filterPurchaseTable = function () {
        $scope.purchaseDateFilter = $filter('purchaseDateRangeFilter')($scope.PurchaseList, $scope.fromDate, $scope.toDate);
        var total = 0;
        angular.forEach($scope.purchaseDateFilter, function (purchase) {
            total += purchase.GrandTotal;
        });
        $scope.GrandTotal = total;
    }
});

//Date Filter for Sales
app.filter('salesDateRangeFilter', function () {
    return function (SalesList, startDate, endDate) {
        if (!SalesList) return;
        if (!startDate && !endDate) return SalesList;
        var filtered = [];
        var start = new Date(startDate);
        var end = new Date(endDate);
        angular.forEach(SalesList, function (item) {
            var purchasedDate = new Date(item.CustomerPurchase.PurchasedDate);
            if (isNaN(start.getTime()) || purchasedDate.getDate() >= start.getDate()) {
                if (isNaN(end.getTime()) || purchasedDate.getDate() <= end.getDate()) {
                    filtered.push(item);
                }
            }
        });
        return filtered;
    };
});


//Date Sales For Purchase
app.filter('purchaseDateRangeFilter', function () {
    return function (PurchaseList, startDate, endDate) {
        if (!PurchaseList) return;
        if (!startDate && !endDate) return PurchaseList;
        var filtered = [];
        var start = new Date(startDate);
        var end = new Date(endDate);
        angular.forEach(PurchaseList, function (item) {
            var purchasedDate = new Date(item.PurchasedDate);
            if (isNaN(start.getTime()) || purchasedDate.getDate() >= start.getDate()) {
                if (isNaN(end.getTime()) || purchasedDate.getDate() <= end.getDate()) {
                    filtered.push(item);
                }
            }
        });
        return filtered;
    };
});