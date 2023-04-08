app.controller("PurchaseController", function ($scope, $http) {
    
    //used document .ready for typeahead
    //this gets executed at the time page loads
    $(document).ready(function () {
        $http({
            method: 'Get',
            url: '/Supplier/GetSupplier'
        }).then(function (response) {

            $scope.SupplierList = response.data;
            $('#inputElement').typeahead({
                source: $scope.SupplierList,
                displayText: function (supplier) {
                    return supplier.SupplierName;
                },                
                afterSelect: function (supplier) {
                    $scope.SupplierName = supplier.SupplierName;
                    $scope.SupplierId = supplier.SupplierId;
                    $scope.$apply();
                }
            });
            
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });

      
            $http({
                method: 'Get',
                url: '/Medicine/GetMedicine'
            }).then(function (response) {
                $scope.MedicineList = response.data;
            }, function (response) {
                $scope.showError = true;
                $scope.errorMessage = "Something Went Wrong. Contact Admin";
            });
        
        
    });

   
   //gets triggerd when ever user types or removes while entering supplier name
    $scope.setSupplierId = function (supplierName) {
        var supplier =$scope.SupplierList.find(function (s) {
            return s.SupplierName === supplierName;
        });

        if (supplier) {
            $scope.SupplierId = supplier.SupplierId;
            $scope.supplier = supplierName;
        } else {
            $scope.SupplierId = undefined;
        }
    };


    $scope.medicines = [
        { MedicineName: "", ExpiryDate: "", Quantity: "", Price:"",TotalAmount:""}
    ];

    $scope.addMedicine = function () {
        $scope.medicines.push({ MedicineName: "", ExpiryDate: "", Quantity: "", Price: "", TotalAmount: "" });
    };
    

    $scope.removeMedicine = function (index) {
        $scope.medicines.splice(index, 1);
    };

    $scope.submitForm = function () {

        if ($scope.SupplierId== undefined || $scope.PurchasedDate == undefined || $scope.BatchNumber == undefined || $scope.PaymentType == undefined) {
            $scope.showPurchaseError = true;
            $scope.showPurchaseSuccess = false;
            $scope.purchaseErrorMessage = "Please Fill out all the fields";
        }
        else {
            //Validating table data
            var valid = $scope.medicines.every(function (medicine) {
                return medicine.MedicineId && medicine.PackingType && medicine.ExpiryDate && medicine.Quantity && medicine.Price && medicine.TotalAmount;

            });
            if (!valid) {
                $scope.showPurchaseError = true;
                $scope.showPurchaseSuccess = false;
                $scope.purchaseErrorMessage = "Please Fill out all the fields of table";
            }
            else {
                var model = {
                    "SupplierId": $scope.SupplierId,
                    "BatchNumber": $scope.BatchNumber,
                    "PaymentType": $scope.PaymentType,
                    "GrandTotal": $scope.GrandTotal,
                    "PurchasedDate": $scope.PurchasedDate,
                    "MedicinePurchasedModels": $scope.medicines
                }

                $http({
                    method: 'Post',
                    url: '/Purchase/SavePurchase',
                    data: model
                }).then(function (response) {
                    if (response.data == "True") {
                        $scope.supplier = "";
                        $scope.SupplierId = "";
                        $scope.BatchNumber = "";
                        $scope.PaymentType = "";
                        $scope.GrandTotal = "";
                        $scope.PurchasedDate = "";
                        $scope.medicines = [
                            { MedicineName: "", ExpiryDate: "", Quantity: "", Price: "", TotalAmount: "" }
                        ];
                    }
                    else {
                        $scope.showPurchaseSuccess = false;
                        $scope.showPurchaseError = true;
                        $scope.purchaseErrorMessage = "Error while Adding Purchase. Contact Admin";
                    }
                        
                }, function (response) {
                    $scope.showPurchaseSuccess = false;
                    $scope.showPurchaseError = true;
                    $scope.purchaseErrorMessage = "Something Went Wrong. Contact Admin";
                });

            }
        }
    };

    //gettting the packing type after selecting medicine
    $scope.getMedicinePackingType = function (medicineId) {
        for (var i = 0; i < $scope.MedicineList.length; i++) {
            if ($scope.MedicineList[i].MedicineId == medicineId) {
                return $scope.MedicineList[i].PackingType;
            }
        }
        return "";
    };

    //getting the Total Amount of row as inputed by user
    $scope.updateTotalAmount = function (medicine) {
        medicine.TotalAmount = (medicine.Price || 0) * (medicine.Quantity || 0);
        //Getting the grand total
        var grandTotal = 0;
        for (var i = 0; i < $scope.medicines.length; i++) {
            grandTotal += $scope.medicines[i].TotalAmount;
        }
        $scope.GrandTotal = grandTotal;
    };

    
});