app.controller("SupplierController", function ($scope, $http) {


    $scope.submitForm = function () {
        var contactnumber = parseInt($scope.ContactNumber);
        //Checking if ContactNumber enter is number
        if (isNaN(contactnumber)) {
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number.";
        } else if ($scope.ContactNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
        }
        else {
            var model = {
                "SupplierName": $scope.Name,
                "ContactNumber": $scope.ContactNumber,
                "Email": $scope.Email,
                "Address": $scope.Address
            }

            //sending data to controller
            $http({
                method: 'POST',
                url: '/Supplier/AddSupplier',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Error while adding supplier. Contact Admin";
                }
            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
        }
    };

    $scope.getSupplier = function () {
        $http({
            method: 'Get',
            url: '/Supplier/GetSupplier'
        }).then(function (response) {
            var data = response.data;
           
                $scope.SupplierList = response.data;
            
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };
});