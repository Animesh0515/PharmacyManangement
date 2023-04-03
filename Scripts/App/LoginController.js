app.controller("LoginController", function ($scope, $http, $window) {

    $scope.message = "Successfulll";
    
    $scope.submitForm = function () {
        var username = $scope.username;
        var password = $scope.password;
        var model = {
            "Username": username,
            "Password": password
        }
        if (username != undefined || password != undefined) {
            $http({
                method: 'POST',
                url: '/Account/Login',
                data: model
            }).then(function (response) {
                if (response.data=="True") {
                    $window.open('/Home/Index', '_self');
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Invalid username or password";
                }
            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Error while Login. Contact Admin.";
            });
        }
        else {
            $scope.showError = true;
            $scope.errorMessage = "Please Enter Username and Password.";
        }
    };
});