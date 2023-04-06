app.controller("SupplierController", function ($scope, $http, $window, $timeout) {

    
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
           
                $scope.SupplierList = response.data;
            
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };

   
    $scope.isEdit = false;

    $scope.editSupplier = function (supplier) {
        // Set the editing status of the clicked row to true
        supplier.editing = true;
        $scope.isEdit = true;
        // Set the editing status of all other rows to false
        angular.forEach($scope.SupplierList, function (item) {
            if (item !== supplier) {
                item.editing = false;
            }
        });
    };

    $scope.cancelEdit = function (supplier) {
        supplier.editing = false;
        $scope.isEdit = false;
        $window.location.reload();

    };

    $scope.saveSupplier = function (supplier) {
        $scope.showError = false;
        $scope.showError = false;
        //Checking if any value is not passed in input text
        if (supplier.SupplierName == "" || supplier.ContactNumber == "" || supplier.Address == "" || supplier.Email == "") {
            $scope.showError = true;
            $scope.errorMessage = "Missing Field ! Fill out all fields.";
        }
        else {
            var contactnumber = parseInt(supplier.ContactNumber);
            //Checking if ContactNumber enter is number
            if (isNaN(contactnumber)) {
                $scope.showError = true;
                $scope.errorMessage = "Not a valid contact number.";
            } else if (supplier.ContactNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
                $scope.showError = true;
                $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
            }
            else {
                var model = {
                    "SupplierName": supplier.SupplierName,
                    "ContactNumber": supplier.ContactNumber,
                    "Email": supplier.Email,
                    "Address": supplier.Address
                }
                $http({
                    method: 'POST',
                    url: '/Supplier/EditSupplier',
                    data: supplier
                }).then(function (response) {
                    if (response.data == "True") {
                        $scope.showSuccess = true;
                        supplier.editing = false;
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
        }
    }

    $scope.reverse = false;
    $scope.createdDate = 'CreatedDate';

    $scope.sortAsc = function () {
        $scope.createdDate = 'CreatedDate';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.createdDate = '-CreatedDate';
        //$scope.reverse = true;
    };


   
});