app.controller("SignupController", function ($scope, $http, $window) {

    $scope.submitForm = function () {
        var mobilenumber = parseInt($scope.MobileNumber);
        if (isNaN(mobilenumber)) {
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number.";
        } else if ($scope.MobileNumber.toString().length != 10) {
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
        }
        else if ($scope.Role == undefined)
        {
            $scope.showError = true;
            $scope.errorMessage = "Please Select Role";
        }
        else
        {
            var model = {
                "Name": $scope.Name,
                "MobileNumber": $scope.MobileNumber,
                "Address": $scope.Address,
                "Email": $scope.Email,
                "Username": $scope.Username,
                "Password": $scope.Password,
                "Role": $scope.Role,
            };
            $http({
                method: 'POST',
                url: '/Account/Signup',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $window.open('/Account/Login', '_self');
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Error while Signup. Contact Admin";
                }
            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Error while Login, Contact Admin";
            });
        }
    };
});