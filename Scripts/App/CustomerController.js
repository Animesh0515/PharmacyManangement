app.controller("CustomerController", function ($scope, $http, $window, $timeout) {
  
    //Confirmed
    $scope.getCustomer = function () {
        $http({
            method: 'Get',
            url: '/Customer/GetCustomer'
        }).then(function (response) {

            $scope.CustomerList = response.data;

        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };


    $scope.isEdit = false;

    $scope.editCustomer = function (customer) {
        // Set the editing status of the clicked row to true
        customer.editing = true;
        $scope.isEdit = true;
        // Set the editing status of all other rows to false
        angular.forEach($scope.CustomerList, function (item) {
            if (item !== customer) {
                item.editing = false;
            }
        });
    };


    $scope.removeCustomer = function (customer) {
        $scope.showError = false;
        $scope.showError = false;

            var model = {
                "CustomerId": customer.CustomerId
            }


            $http({
                method: 'POST',
                url: '/Customer/RemoveCustomer',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    customer.editing = false;
                    $scope.isEdit = false;
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000); // reload after 3 seconds
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Error while removing customer. Please Try Again.";
                }



            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
    };


    $scope.cancelEdit = function () {
        customer.editing = false;
        $scope.isEdit = false;
        $window.location.reload();

    };

    $scope.saveCustomer = function (customer) {
        $scope.showError = false;
        $scope.showError = false;

        //Checking if any value is not passed in input text
        if (customer.CustomerName == "" || customer.MobileNumber == "" || customer.Address == "" || customer.Email == "") {
            $scope.showError = true;
            $scope.errorMessage = "Missing Field ! Fill out all fields.";
        }
        else if (isNaN(customer.MobileNumber)) {
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number.";
        } else if (customer.MobileNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
        }
        else {
            var model = {
                "CustomerId": customer.CustomerId,
                "CustomerName": customer.CustomerName,
                "MobileNumber": customer.MobileNumber,
                "Address": customer.Address,
                "Email": customer.Email
            }


            $http({
                method: 'POST',
                url: '/Customer/EditCustomer',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    customer.editing = false;
                    $scope.isEdit = false;
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000); // reload after 3 seconds
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Error while updating. Please Try Again.";
                }



            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
        }
    };


    $scope.CustomerName = 'CustomerName';

    $scope.sortAsc = function () {
        $scope.Name = 'CustomerName';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.Name = '-CustomerName';
        //$scope.reverse = true;
    };


});