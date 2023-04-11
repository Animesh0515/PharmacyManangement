app.controller("PurchaseController", function ($routeParams, $scope, $http, $window, $timeout, $location) {
    
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

    $scope.submitForm = function (action) {

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
                    "PurchaseId": $scope.PurchaseId,
                    "SupplierId": $scope.SupplierId,
                    "BatchNumber": $scope.BatchNumber,
                    "PaymentType": $scope.PaymentType,
                    "GrandTotal": $scope.GrandTotal,
                    "PurchasedDate": $scope.PurchasedDate,
                    "MedicinePurchasedModels": $scope.medicines
                }
                if (action == "add") {
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
                else {
                    $http({
                        method: 'Post',
                        url: '/Purchase/EditPurchase',
                        data: model
                    }).then(function (response) {
                        if (response.data == "True") {
                            $window.location.href = '/Purchase/GetPurchase';
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

    $scope.getPurchases = function () {
        $http({
            method: 'Get',
            url: '/Purchase/GetAllPurchase'
        }).then(function (response) {
            $scope.Purchases = response.data;
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    }

    $scope.printPurchaseBill = function (purchaseData) {
        $http({
            method: 'Post',
            url: '/Purchase/PurchaseInvoiceTemplate',
            data: purchaseData
        }).then(function (response) {
            $scope.Purchases = response.data;
            //printing the invoice
            var printWindow = window.open('', '_blank', 'height=500,width=800');
            var htmlContent = response.data;
            printWindow.document.write(htmlContent);
            printWindow.print();
            printWindow.close();
            $window.location.reload();
        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });       
    };

    $scope.deletePurchaseBill = function (PurchaseId) {
        $http({
            method: 'Post',
            url: '/Purchase/DeletePurchase',
            data: { PurchaseId: parseInt(PurchaseId) }
        }).then(function (response) {
            if (response.data == "True") {
                $scope.showSuccess = true;
                $scope.showError = false;
                $timeout(function () {
                    $window.location.reload();
                }, 2000);
            }
            else {
                $scope.showSuccess = false;
                $scope.showError = true;
                $scope.errorMessage = "Error while deleting Purchase. Contact Admin";
            }

        }, function (response) {
            $scope.showError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
    };

    $scope.sortAsc = function () {
        $scope.createdDate = 'PurchasedDate';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.createdDate = '-PurchasedDate';
        //$scope.reverse = true;
    };

    $scope.editPurchase = function (id) {       
        $window.location.href = '/Purchase/EditPurchase?id=' + parseInt(id);
    }
    
    $scope.GetPurchaseEditData = function () {
        //getting id from url
        var url = $location.$$absUrl;       
        var PurchaseIdForEdit=url.split('=').pop();
        $http({
            method: 'Post',
            url: '/Purchase/GetPurchaseDetail',
            data: { PurchaseId: parseInt(PurchaseIdForEdit) }
        }).then(function (response) {

            var data = response.data;
            $scope.SupplierId = data.SupplierId;
            $scope.supplier = data.SupplierName;
            $scope.PurchasedDate = new Date(data.PurchasedDate);
            $scope.PaymentType = data.PaymentType;
            $scope.BatchNumber = data.BatchNumber;
            angular.forEach(data.MedicinePurchasedModels, function (medicine) {
                medicine.ExpiryDate = new Date(medicine.ExpiryDate)
            });
            $scope.medicines = data.MedicinePurchasedModels;


        }, function (response) {
            $scope.showPurchaseError = true;
            $scope.showPurchaseSuccess = false;
            $scope.purchaseErrorMessage = "Something Went Wrong. Contact Admin";
        });
    };
});