app.controller("ReportController", function ($scope) {

    $scope.getSales = function () {
        $http({
            method: 'Get',
            url: '/Report/GetSalesReport'
        }).then(function (response) {
            $scope.SalesList = response.data;
        }, function (response) {
            $scope.showSucess = false;
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    }
})