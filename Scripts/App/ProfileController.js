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

        //Checking if any value is not passed in input text
        if (user.Name == "" || user.MobileNumber == "" || user.Address == "" || user.Email == "" || user.Username == "" || user.Password == "") {
            $scope.showError = true;
            $scope.showSuccess = false;
            $scope.errorMessage = "Missing Field ! Fill out all fields.";
        }
        else if (isNaN(user.MobileNumber)) {
            $scope.showError = true;
            $scope.showSuccess = false;
            $scope.errorMessage = "Not a valid contact number.";
        } else if (user.MobileNumber.toString().length != 10) {  /*Checking Length of Number to be 10 digit*/
            $scope.showSuccess = false;
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
                    $scope.showError = false;
                    $scope.successMessage = "Profile Edited Sucessfully! Reloading Please Wait.";
                    user.editing = false;
                    $scope.isEdit = false;
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000); // reload after 3 seconds
                }
                else {
                    $scope.showError = true;
                    $scope.showSuccess = false;
                    $scope.errorMessage = "Error while updating. Please Try Again.";
                }



            }, function (response) {
                $scope.showError = true;
                $scope.showSuccess = false;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
        }
    };

    $scope.onSelect = function (event) {
        console.log(event);
    }
    $scope.removePicture = function (UserId)
    {
        $http({
            method: 'POST',
            url: '/Profile/DeleteProfileImage',
            data: { UserId: UserId}
        }).then(function (response) {
            if (response.data == "True") {
                $scope.showSuccess = true;
                $scope.showError = false;
                $scope.successMessage = "Profile Image Removed Sucessfully! Reloading Please Wait.";                
                $timeout(function () {
                    $window.location.reload();
                }, 3000); // reload after 3 seconds
            }
            else {
                $scope.showError = true;
                $scope.showSuccess = false;
                $scope.errorMessage = "Error while Removing. Please Try Again.";
            }



        }, function (response) {
            $scope.showError = true;
            $scope.showSuccess = false;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    }
    $scope.file = null;

    $scope.uploadPicture = function (UserId) {
        if ($scope.file) {
            var fileData = new FormData();
            fileData.append("file", $scope.file);
            fileData.append("userId", UserId);

            $http.post("/Profile/UploadImage", fileData, {
                transformRequest: angular.identity,
                headers: { "Content-Type": undefined }
            }).then(function (response) {
                if (response.data = "True") {
                    $scope.showError = false;
                    $scope.showSuccess = true;
                    $scope.successMessage = "Successfully Uploaded Image. Please wait reloading...";
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000);
                }
                else {
                    $scope.showError = true;
                    $scope.showSuccess = false;
                    $scope.errorMessage = "Error while uploading Image! Contact admin";
                }
            }, function (error) {
                $scope.showError = true;
                $scope.showSuccess = false;
                $scope.errorMessage = "Something went wrong! Contact admin";
            });
        }
        else {
            $scope.showError = true;
            $scope.showSuccess = false;
            $scope.errorMessage = "Select File First!";
        }
    };


    //event listerner that gets invoked when file is chosen
    function addFileListener() {
        angular.element(document.querySelector("#formFile")).on("change", function (event) {
            $scope.$apply(function () {
                var file = event.target.files[0];
                var validImageTypes = ["image/jpeg", "image/png", "image/gif"];
                if (validImageTypes.indexOf(file.type) === -1) {
                    $scope.showError = true;
                    $scope.showSuccess = false;
                    $scope.errorMessage = "Please select a valid image file.";                    
                    event.target.value = null; // Clear the file input to allow selecting another file
                    $scope.file = null; // Reset the $scope.file variable
                } else {
                    $scope.file = file;
                }
            });
        });
    }

    angular.element(document).ready(function () {
        addFileListener();
    });
});
