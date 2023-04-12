app.controller("SalesController", function ($scope, $http) {
    $scope.NewCustomer = true;
    //used document .ready for typeahead
    //this gets executed at the time page loads
    $(document).ready(function () {
        $http({
            method: 'Get',
            url: '/Customer/GetCustomer'
        }).then(function (response) {

            $scope.CustomerList = response.data;
            $('#inputElement').typeahead({
                source: $scope.CustomerList,
                displayText: function (customer) {
                    return customer.CustomerName;
                },
                afterSelect: function (customer) {                    
                    //getting customer details after selecting customer
                    $scope.NewCustomer = false;
                    $scope.MobileNumber = customer.MobileNumber;
                    $scope.Address = customer.Address;
                    $scope.Email = customer.Email;
                    $scope.CustomerName = customer.CustomerName;
                    $scope.CustomerId = customer.CustomerId;
                    $scope.$apply();
                }
            });

        }, function (response) {
            $scope.showSalesSucess = true;
            $scope.showSalesError = true;
            $scope.salesErrorMessage = "Something Went Wrong. Contact Admin";
        });


        $http({
            method: 'Get',
            url: '/Medicine/GetMedicine'
        }).then(function (response) {
            $scope.MedicineList = response.data;
        }, function (response) {
            $scope.showSalesSucess = false;
            $scope.showSalesError = true;
            $scope.errorMessage = "Something Went Wrong. Contact Admin";
        });
        


    });

    $scope.setCustomerId = function (customerName) {
        $scope.NewCustomer = true;
        var customer = $scope.CustomerList.find(function (s) {
            return s.CustomerName === customerName;
        });

        if (customer) {
            $scope.CustomerId = customer.CustomerId;
            $scope.customer = customerName;
        } else {
            $scope.CustomerId = undefined;
        }
    };

    $scope.medicines = [
        { MedicineName: "", ExpiryDate: "", Quantity: "", Price: "", TotalAmount: "" }
    ];

    $scope.addMedicine = function () {
        $scope.medicines.push({ MedicineName: "", ExpiryDate: "", Quantity: "", Price: "", TotalAmount: "" });
    };


    $scope.removeMedicine = function (index) {
        $scope.medicines.splice(index, 1);
    };

    //gettting the packing type and price after selecting medicine
    $scope.setMedicineDetails = function (medicine) {
        // Find the selected medicine object from the MedicineList
        var selectedMedicine = $scope.MedicineList.find(function (med) {
            return med.MedicineId === medicine.MedicineId;
        });

        // Set the PackingType and Price fields of the medicine object
        if (selectedMedicine) {
            medicine.PackingType = selectedMedicine.PackingType;
            medicine.Price = selectedMedicine.Price;
        }
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

    $scope.submitForm = function (action) {
        if ($scope.SupplierId == undefined || $scope.PurchasedDate == undefined || $scope.BatchNumber == undefined || $scope.PaymentType == undefined) {
            $scope.showSalesError = true;
            $scope.showSalesSuccess = false;
            $scope.salesErrorMessage = "Please Fill out all the fields";
        }
        else {
            //Validating table data
            var valid = $scope.medicines.every(function (medicine) {
                return medicine.MedicineId && medicine.PackingType && medicine.ExpiryDate && medicine.Quantity && medicine.Price && medicine.TotalAmount;

            });
            if (!valid) {
                $scope.showSalesError = true;
                $scope.showSalesSuccess = false;
                $scope.salesErrorMessage = "Please Fill out all the fields of table";
            }
            else {
                var Customer = null;
                if ($scope.NewCustomer) {
                     Customer = {
                        "CustomerName": $scope.CustomerName,
                        "MobileNumber": $scope.MobileNumber,
                        "Email": $scope.Email,
                        "Address": $scope.Address
                    }
                }
                var CustomerPurchase = {
                    "CustomerId": $scope.CustomerId,
                    "Discount": $scope.Discount,
                    "GrandTotal": $scope.GrandTotal,
                    "PurchasedDate": $scope.SalesDate
                }

                var CustomerPurchasedMedicine = $scope.medicines;

                var model = {
                    "Customer": Customer,
                    "CustomerPurchase": CustomerPurchase,
                    "CustomerPurchasedMedicine": CustomerPurchasedMedicine
                }
                if (action == "add") {
                    $http({
                        method: 'Post',
                        url: '/Sales/SaveSales',
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
                            $scope.showSalesSuccess = true;
                            $scope.showSalesError = false;
                        }
                        else {
                            $scope.showSalesSuccess = false;
                            $scope.showSalesError = true;
                            $scope.salesErrorMessage = "Error while Adding Purchase. Contact Admin";
                        }

                    }, function (response) {
                        $scope.showSalesSuccess = false;
                        $scope.showSalesError = true;
                        $scope.salesErrorMessage = "Something Went Wrong. Contact Admin";
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
                            $scope.showSalesSuccess = false;
                            $scope.showSalesError = true;
                            $scope.salesErrorMessage = "Error while Adding Purchase. Contact Admin";
                        }

                    }, function (response) {
                        $scope.showSalesSuccess = false;
                        $scope.showSalesError = true;
                        $scope.salesErrorMessage = "Something Went Wrong. Contact Admin";
                    });
                }

            }
        }
    }
});