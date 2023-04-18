app.controller("ProfileController", function ($scope, $http, $window, $timeout) {
      
    //Confirmed
    $scope.getProfile = function () {
        $http({
            method: 'Get',
            url: '/Profile/GetProfile'
        }).then(function (response) {

            $scope.Profile = response.data;

        }, function () {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };

    // In your controller:
    $scope.onFileSelect = function (event) {
        const files = event.target.files;
        const fileList = [];

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            fileList.push({
                name: file.name,
                size: file.size,
                type: file.type,
                lastModifiedDate: file.lastModifiedDate
            });
        }

        $scope.$emit('filesSelected', fileList);
    };


    $scope.isEdit = false;

    $scope.editProfile = function (user) {
        // Set the editing status of the clicked row to true
        user.editing = true;
        $scope.isEdit = true;
        // Set the editing status of all other rows to false
        angular.forEach($scope.Profile, function (item) {
            if (item !== user) {
                item.editing = false;
            }
        });
    };


    $scope.cancelEdit = function (user) {
        user.editing = false;
        $scope.isEdit = false;
        $window.location.reload();

    };

    $scope.saveProfile = function (user) {


        $scope.showError = false;
        $scope.showError = false;

        //Checking if any value is not passed in input text
        if (user.Name == "" || user.MobileNumber == "" || user.Address == "" || user.Email == "" || user.Username == "" || user.Password == "") {
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
                "Name": user.Name,
                "MobileNumber": user.MobileNumber,
                "Address": user.Address,
                "Email": user.Email,
                "Username": user.Username,
                "Password": user.Password
            }


            $http({
                method: 'POST',
                url: '/Profile/EditProfile',
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

    $scope.onSelect = function (event) {
        console.log(event);
    }

});
