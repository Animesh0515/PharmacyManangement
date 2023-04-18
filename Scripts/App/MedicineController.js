app.controller("MedicineController", function ($scope, $http, $window, $timeout) {

    
    $scope.submitForm = function () {
            var model = {
                "MedicineName": $scope.Name,
                "Description": $scope.Description,
                "Price": $scope.Price,
                "PackingType": $scope.PackingType
            }

            //sending data to controller
            $http({
                method: 'POST',
                url: '/Medicine/AddMedicine',
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
                    $scope.errorMessage = "Error while adding medicine. Contact Admin";
                }
            }, function (response) {
                $scope.showSuccess = false;
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
    };


    //Confirmed
    $scope.getMedicine = function () {
        $http({
            method: 'Get',
            url: '/Medicine/GetMedicine'
        }).then(function (response) {           
           
                $scope.MedicineList = response.data;
            
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };

   
    $scope.isEdit = false;

    $scope.editMedicine = function (medicine) {
        // Set the editing status of the clicked row to true
        medicine.editing = true;
        $scope.isEdit = true;
        // Set the editing status of all other rows to false
        angular.forEach($scope.MedicineList, function (item) {
            if (item !== medicine) {
                item.editing = false;
            }
        });
    };

    $scope.cancelEdit = function (medicine) {
        medicine.editing = false;
        $scope.isEdit = false;
        $window.location.reload();

    };

    $scope.saveMedicine = function (medicine) {
        $scope.showError = false;
        $scope.showError = false;
        //Checking if any value is not passed in input text
        if (medicine.MedicineName == "" || medicine.Description == "" || medicine.Price == "" || medicine.PackingType == "") {
            $scope.showError = true;
            $scope.errorMessage = "Missing Field ! Fill out all fields.";
        }
        else {

            var model = {
                "MedicineId": medicine.MedicineId,
                "MedicineName": medicine.MedicineName,
                "Description": medicine.Description,
                "Price": medicine.Price,
                "PackingType": medicine.PackingType,
                "Quantity": medicine.Quantity
                
            }
            $http({
                method: 'POST',
                url: '/Medicine/EditMedicine',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    medicine.editing = false;
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

    $scope.removeMedicine = function (medicine) {

        $scope.showError = false;
        $scope.showError = false;

            var model = {
                "MedicineId": medicine.MedicineId
            }
            $http({
                method: 'POST',
                url: '/Medicine/RemoveMedicine',
                data: model
            }).then(function (response) {
                if (response.data == "True") {
                    $scope.showSuccess = true;
                    medicine.editing = false;
                    $scope.isEdit = false;
                    $timeout(function () {
                        $window.location.reload();
                    }, 3000); // reload after 3 seconds
                }
                else {
                    $scope.showError = true;
                    $scope.errorMessage = "Cannot remove medicine. Please Try Again.";
                }



            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
    };

    
    $scope.createdDate = 'CreatedDate';

    $scope.sortAsc = function () {
        $scope.createdDate = 'CreatedDate';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.createdDate = '-CreatedDate';
        //$scope.reverse = true;
    };

    //$scope.sortExp = function () {
    //    // Filter out records with ExpiryDate greater than or equal to current time
    //    var filteredList = $scope.MedicineList.filter(function (item) {
    //        return new Date(item.ExpiryDate) < new Date();
    //    });
    //    // Sort the filtered list by ExpiryDate
    //    filteredList.sort(function (a, b) {
    //        return new Date(a.ExpiryDate) - new Date(b.ExpiryDate);
    //    });
    //    // Replace the existing MedicineList with the sorted and filtered list
    //    $scope.MedicineList = filteredList;
    //};


   
});