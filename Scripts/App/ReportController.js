app.controller("ReportController", function ($scope, $http, $window, $filter) {

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
            $scope.showSucess = false;
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    }
    $scope.calculateTotalAmount = function (model) {
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

    $scope.sortAsc = function () {
        $scope.purchasedDate = 'CustomerPurchase.PurchasedDate';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.purchasedDate = '-CustomerPurchase.PurchasedDate';
        //$scope.reverse = true;
    };
    $scope.filterTable = function () {
        $scope.dateFilter = $filter('dateRangeFilter')($scope.SalesList, $scope.fromDate, $scope.toDate);
        var total = 0;
        angular.forEach($scope.dateFilter, function (sales) {
            total += sales.CustomerPurchase.GrandTotal;
        });
        $scope.GrandTotal = total;
    };

  
});

app.filter('dateRangeFilter', function () {
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