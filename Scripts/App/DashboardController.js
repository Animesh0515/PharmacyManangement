app.controller("DashBoardController", function ($scope, $http) {

    $scope.GetDashboardData = function () {
        $http({
            method: 'GET',
            url: '/Home/GetDashboardData'
        }).then(function (response) {
            $scope.data = response.data;
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Error while data fetch. Contact Admin.";
        });
    }
})