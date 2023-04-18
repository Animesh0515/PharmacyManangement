app.controller("UserController", function ($scope, $http, $window, $timeout) {

        $scope.submitForm = function () {
            var mobilenumber = parseInt($scope.MobileNumber);
            //Checking if ContactNumber enter is number
            if (isNaN(mobilenumber)) {
                $scope.showError = true;
                $scope.errorMessage = "Not a valid contact number.";
            } else if ($scope.MobileNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
                $scope.showError = true;
                $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
            }
            else if ($scope.Role == undefined) {
                $scope.showError = true;
                $scope.errorMessage = "Please Select Role";
            }
            else {
                var model = {
                    "Name": $scope.Name,
                    "MobileNumber": $scope.MobileNumber,
                    "Address": $scope.Address,
                    "Email": $scope.Email,
                    "Username": $scope.Username,
                    "Password": $scope.Password,
                    "Role": $scope.Role,
                };

                //sending data to controller
                $http({
                    method: 'POST',
                    url: '/User/AddUser',
                    data: model
                }).then(function (response) {
                    if (response.data == "True") {
                        $scope.showSuccess = true;
                        $timeout(function () {
                            $window.location.reload();
                        }, 2000);
                    }
                    else {
                        $scope.showSuccess = false;
                        $scope.showError = true;
                        $scope.errorMessage = "Error while adding user. Contact Admin";
                }
                }, function (response) {
                    $scope.showSuccess = false;
                    $scope.showError = true;
                    $scope.errorMessage = "Something Went Wrong. Contact Admin";
                });
            }
    };

    //Confirmed
    $scope.getUser = function () {
        $http({
            method: 'Get',
            url: '/User/GetUser'
        }).then(function (response) {

            $scope.UserList = response.data;

        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };


    $scope.isEdit = false;

    $scope.editUser = function (user) {
        // Set the editing status of the clicked row to true
        user.editing = true;
        $scope.isEdit = true;
        // Set the editing status of all other rows to false
        angular.forEach($scope.UserList, function (item) {
            if (item !== user) {
                item.editing = false;
            }
        });
    };


    $scope.removeUser = function (user) {
        $scope.showError = false;
        $scope.showError = false;

            var model = {
                "userid": user.UserId
            }

            $http({
                method: 'POST',
                url: '/User/RemoveUser',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    user.editing = false;
                    $scope.isEdit = false;
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000); // reload after 3 seconds
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Error while removing. Please Try Again.";
                }



            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
    };


    $scope.cancelEdit = function (user) {
        user.editing = false;
        $scope.isEdit = false;
        $window.location.reload();

    };

    $scope.saveUser = function (user) {
        $scope.showError = false;
        $scope.showError = false;

        //Checking if any value is not passed in input text
        if (user.Name == "" || user.MobileNumber == "" || user.Address == "" || user.Email == "" || user.Username == "" || user.Password == "" || user.Role == "") {
            $scope.showError = true;
            $scope.errorMessage = "Missing Field ! Fill out all fields.";
        }
        else if (isNaN(user.MobileNumber)) {
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number.";
        } else if (user.MobileNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
            $scope.showError = true;
            $scope.errorMessage = "Not a valid contact number. Must be 10 digit";
        }
        else {
            var model = {
                "userid": user.UserId,
                "Name": user.Name,
                "MobileNumber": user.MobileNumber,
                "Address": user.Address,
                "Email": user.Email,
                "Username": user.Username,
                "Password": user.Password,
                "Role": user.Role
            }


            $http({
                method: 'POST',
                url: '/User/EditUser',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    user.editing = false;
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


    $scope.Name = 'Name';

    $scope.sortAsc = function () {
        $scope.Name = 'Name';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.Name = '-Name';
        //$scope.reverse = true;
    };


});