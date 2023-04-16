app.controller("SalesController", function ($scope, $http, $location, $window, $timeout) {
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
        if (customerName != undefined || customerName !="") {
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
            $scope.TotalAmount = grandTotal;
        }
        if ($scope.Discount) {
            var discountedAmount = ($scope.Discount / 100) * $scope.TotalAmount;
            $scope.GrandTotal = $scope.TotalAmount - discountedAmount;

        }
        else {
            $scope.GrandTotal = grandTotal;
        }
    };

    $scope.updateDiscountAmount = function () {
        if ($scope.Discount) {
            if ($scope.TotalAmount) {
                var discountedAmount = ($scope.Discount / 100) * $scope.TotalAmount;
                $scope.GrandTotal = $scope.TotalAmount - discountedAmount;
            }
            else {
                var grandTotal = 0;
                for (var i = 0; i < $scope.medicines.length; i++) {
                    grandTotal += $scope.medicines[i].TotalAmount;
                    $scope.TotalAmount = grandTotal;
                }
                var discountedAmount = ($scope.Discount / 100) * $scope.TotalAmount;
                $scope.GrandTotal = $scope.TotalAmount - discountedAmount;
            }

        }
        else if (!$scope.Discount && $scope.TotalAmount) {
            $scope.GrandTotal = $scope.TotalAmount
        }
        
    }

    $scope.submitForm = function (action) {
        if (($scope.CustomerName == undefined && $scope.customer == undefined) || $scope.SalesDate == undefined || $scope.MobileNumber == undefined || $scope.Address == undefined || $scope.Email == undefined) {
            $scope.showSalesError = true;
            $scope.showSalesSuccess = false;
            $scope.salesErrorMessage = "Please Fill out all the fields";
        }
        else {
            //Validating table data
            var valid = $scope.medicines.every(function (medicine) {
                return medicine.MedicineId && medicine.PackingType  && medicine.Quantity && medicine.Price && medicine.TotalAmount;

            });
            if (!valid) {
                $scope.showSalesError = true;
                $scope.showSalesSuccess = false;
                $scope.salesErrorMessage = "Please Fill out all the fields of table";
            }
            else {
              
               /* if ($scope.NewCustomer) {*/

                Customer = {
                    "CustomerId": $scope.CustomerId,
                    "CustomerName": $scope.customer,
                    "MobileNumber": $scope.MobileNumber,
                    "Email": $scope.Email,
                    "Address": $scope.Address
                    }
                //}
                //else {
                //    Customer = {
                //        "CustomerId": $scope.CustomerId,
                //    }
                //}
                var CustomerPurchase = {
                    "CustomerPurchaseId": $scope.CustomerPurchaseId,
                    "CustomerId": $scope.CustomerId,
                    "Discount": $scope.Discount,
                    "GrandTotal": $scope.GrandTotal,
                    "PurchasedDate": $scope.SalesDate
                }

                var CustomerPurchasedMedicine = $scope.medicines;

                var model = {
                    "Customer": Customer,
                    "CustomerPurchase": CustomerPurchase,
                    "CustomerPurchasedMedicine": CustomerPurchasedMedicine,
                    "IsNewCustomer": $scope.NewCustomer
                }
                if (action == "add") {
                    $http({
                        method: 'Post',
                        url: '/Sales/SaveSales',
                        data: model
                    }).then(function (response) {
                        if (response.data == "True") {
                            $scope.customer = "";
                            $scope.CustomerId = "";
                            $scope.MobileNumber = "";
                            $scope.Address = "";
                            $scope.Email = "";
                            $scope.Discount = "";
                            $scope.GrandTotal = "";
                            $scope.SalesDate = "";
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
                        url: '/Sales/EditSales',
                        data: model
                    }).then(function (response) {
                        if (response.data == "True") {
                            $scope.showSalesSuccess = true;
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

    $scope.GetSalesData = function () {
        $http({
            method: 'Get',
            url: '/Sales/GetSalesData'
        }).then(function (response) {

            $scope.Sales = response.data;
          

        }, function (response) {
            $scope.showSucess = true;
            $scope.showError = true;
            $scope.salesMessage = "Something Went Wrong. Contact Admin";
        });
    }

    $scope.editSales= function (id) {
        $window.location.href = '/Sales/EditSales?id=' + parseInt(id);
    }

    $scope.sortAsc = function () {
        $scope.purchaseDate = 'PurchasedDate';
        //$scope.reverse = false;
    };

    $scope.sortDesc = function () {
        $scope.purchaseDate = '-PurchasedDate';
        //$scope.reverse = true;
    };

    $scope.GetSalesEditData = function () {
        //getting id from url
        var url = $location.$$absUrl;
        var CustomerPurchaseId = url.split('=').pop();
        $http({
            method: 'Post',
            url: '/Sales/GetSalesDetail',
            data: { CustomerPurchaseId: parseInt(CustomerPurchaseId) }
        }).then(function (response) {

            var data = response.data;
            $scope.CustomerId = data.Customer.CustomerId;
            $scope.customer = data.Customer.CustomerName;
            $scope.CustomerName = data.Customer.CustomerName;
            $scope.MobileNumber = data.Customer.MobileNumber;
            $scope.Address = data.Customer.Address;
            $scope.Email = data.Customer.Email;
            $scope.CustomerPurchaseId = data.CustomerPurchase.CustomerPurchaseId;
            $scope.Discount = data.CustomerPurchase.Discount;
            $scope.SalesDate = new Date(data.CustomerPurchase.PurchasedDate);
            $scope.GrandTotal =data.CustomerPurchase.GrandTotal;
            $scope.medicines = data.CustomerPurchasedMedicine;
            $scope.NewCustomer = false;
            


        }, function (response) {
            $scope.showPurchaseError = true;
            $scope.showPurchaseSuccess = false;
            $scope.purchaseErrorMessage = "Something Went Wrong. Contact Admin";
        });
    }

    $scope.printSalesBill = function (purchaseData) {
        $http({
            method: 'Post',
            url: '/Sales/SalesInvoiceTemplate',
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

    $scope.deleteSalesBill = function (CustomerPurchaseId) {
        $http({
            method: 'Post',
            url: '/Sales/DeleteSales',
            data: { CustomerPurchaseId: parseInt(CustomerPurchaseId) }
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
    }
    
});