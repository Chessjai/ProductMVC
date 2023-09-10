/*/*employee.js*//** /*/

let gl = "";

$(document).ready(function () {
    loadData();
    $('#Country').on('change', function () {
        loadStatesByCountry();
    });
    getCountries();
    $("#uploadButton").click(function () {
        var fileInput = $("#file")[0].files[0]; // Get the selected file
        var formData = new FormData(); // Create a FormData object

        if (fileInput) {
            formData.append("file", fileInput); // Add the file to the FormData object

            $.ajax({
                url: '/Employee/UploadExcel', // Replace 'YourControllerName' with your actual controller name
                type: 'POST',
                data: formData,
                processData: false, // Don't process the data
                contentType: false, // Don't set content type
                success: function (data) {
                    // Handle success response from the server
                    $("#resultMessage").html(data);
                },
                error: function (xhr, status, error) {
                    // Handle error response from the server
                    $("#resultMessage").html("Error: " + error);
                }
            });
        } else {
            $("#resultMessage").html("Please select a file to upload.");
        }
    });

    $('#downloadExcelButton').click(function () {
        downloadExcel();
    });


});
function downloadExcel() {
    $('#downloadExcelSubmit').click();
}

function getCountries() {
    $.ajax({
        url: "/Employee/getCountries",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (countries) {
            var countryDropdown = $('#Country');
            countryDropdown.empty();
            countryDropdown.append($('<option>').val('').text('Select Country'));
            $.each(countries, function (key, country) {
                countryDropdown.append($('<option>').val(country.CountryID).text(country.CountryName));
            });
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function loadStatesByCountry() {
    var selectedCountry = $('#Country').val();

    if (selectedCountry) {
        $.ajax({
            url: "/Employee/GetStatesByCountry?countryId=" + selectedCountry,
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (states) {
                var stateDropdown = $('#State');
                stateDropdown.empty();
                stateDropdown.append($('<option>').val('').text('Select State'));
                $.each(states, function (key, state) {
                    stateDropdown.append($('<option>').val(state.StateID).text(state.StateName));
                });
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    } else {
        $('#State').empty();
    }
}
function loadData() {
    $.ajax({
        url: "/Employee/List",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {


            var html = '';
            var counter = 1;

            $.each(result, function (key, item) {
                console.log(result);
                console.log(item.EmployeeID);
                html += '<tr>';
                html += '<td>' + counter + '</td>';
                counter++;
                html += '<td>' + item.Name + '</td>';
                html += '<td>' + item.Age + '</td>';
                html += '<td>' + item.Designation + '</td>';
                html += '<td>' + item.Email + '</td>';
                html += '<td>' + formatDate(item.JoinedDate) + '</td>';
                html += '<td>' + item.IsMarried + '</td>';
                html += '<td>' + item.Salary + '</td>';
                html += '<td>' + item.Role + '</td>';
                html += '<td>' + item.StateName + '</td>';
                html += '<td>' + item.CountryName + '</td>';
                html += '<td>' + formatDate(item.DateOfBirth) + '</td>';
                html += '<td>' + item.MobileNumber + '</td>';

                html += '<td><a href="#" onclick="return getbyID(' + item.EmployeeID + ')">Edit</a> | <a href="#" onclick="Delele(' + item.EmployeeID + ')">Delete</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function formatDate(dateString) {
    var date = new Date(parseInt(dateString.substr(6)));
    return date.toISOString().substr(0, 10);
}


function Add() {
    var res = validate();
    if (res == false) {
        return false;
    }
    getCountries();


    gblEmployeeeId = $("#EmployeeID").val();
    var empObj = {
        EmployeeID: $('#EmployeeID').val(),
        Name: $('#Name').val(),
        Age: $('#Age').val(),
        Designation: $('#Designation').val(),
        Email: $('#Email').val(),
        JoinedDate: $('#JoinedDate').val(),
        IsMarried: $('#IsMarried').val() === "true",
        Salary: $('#Salary').val(),
        Role: $('#Role').val(),
        StateID: $('#State').val(),
        CountryID: $('#Country').val(),
        DateOfBirth: $('#DateOfBirth').val(),
        MobileNumber: $('#MobileNumber').val()
    };

    $.ajax({
        url: "/Employee/Add",
        data: JSON.stringify(empObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadStatesByCountry();

            loadData();
            $('#myModal').modal('hide');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}



function getbyID(EmpID) {

    console.log('EmployeeID in getbyID:', EmpID);

    $.ajax({
        url: "/Employee/GetbyID/?EmployeeID=" + EmpID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#EmployeeID').val(result.EmployeeID);
            $('#Name').val(result.Name);
            $('#Age').val(result.Age);

            $('#Country').val(result.CountryID);
            $('#IsMarried').val(result.IsMarried ? "true" : "false");
            $('#Designation').val(result.Designation);
            $('#Email').val(result.Email);
            $('#JoinedDate').val(result.JoinedDate ? formatDate(result.JoinedDate) : "");
            $('#Salary').val(result.Salary);
            $('#Role').val(result.Role);
            $('#DateOfBirth').val(result.DateOfBirth ? formatDate(result.DateOfBirth) : "");
            $('#MobileNumber').val(result.MobileNumber);
            loadStatesByCountry();
            $('#State').val(result.StateID);
            $('#myModal').modal('show');
            $('#btnUpdate').show();
            $('#btnAdd').hide();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}


function Update() {
    var res = validate();
    if (res == false) {
        return false;
    }

    var empId = $('#EmployeeID').val();
    console.log('EmployeeID:', empId)
    var empObj = {
        EmployeeID: $('#EmployeeID').val(),
        Name: $('#Name').val(),
        Age: $('#Age').val(),
        StateID: $('#State').val(),
        CountryID: $('#Country').val(),
        Designation: $('#Designation').val(),
        Email: $('#Email').val(),
        JoinedDate: $('#JoinedDate').val(),
        IsMarried: $('#IsMarried').val(),
        Salary: $('#Salary').val(),
        Role: $('#Role').val(),
        DateOfBirth: $('#DateOfBirth').val(),
        MobileNumber: $('#MobileNumber').val()
    };
    $.ajax({
        url: "/Employee/Update",
        data: JSON.stringify(empObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            $('#myModal').modal('hide');

            $('#Name').val("");
            $('#Age').val("");
            $('#State').val("");
            $('#Country').val("");
            $('#Designation').val("");
            $('#JoinedDate').val("");
            $('#Salary').val("");
            $('#Role').val("");
            $('#IsMarried').val("");
            $('#DateOfBirth').val("");
            $('#MobileNumber').val("");
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}



function Delele(ID) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/Employee/Delete/?EmployeeID=" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadData();
                console.log(result.EmployeeID);
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}


function exportToExcel() {
    $.ajax({
        url: '/Employee/ExportToExcel', // Replace with your actual URL
        type: 'POST',
        success: function (data) {
            $("#resultMessage").html(data);
            // Handle success response, if needed
        },
        error: function (xhr, status, error) {
            // Handle error response, if needed
        }
    });
}


function clearTextBox() {

    $('#Name').val("");
    $('#Age').val("");
    $('#State').val("");
    $('#Country').val("");
    $('#Designation').val("");
    $('#JoinedDate').val("");
    $('#Salary').val("");
    $('#Role').val("");
    $('#IsMarried').val("");
    $('#Email').val("");
    $('#DateOfBirth').val("");
    $('#MobileNumber').val("");





    $('#btnUpdate').hide();
    $('#btnAdd').show();

}

function validate() {

    // Clear error messages
    $('#NameError').text("");
    $('#AgeError').text("");
    $('#StateError').text("");
    $('#CountryError').text("");
    $('#DesignationError').text("");
    $('#JoinedDateError').text("");
    $('#SalaryError').text("");
    $('#RoleError').text("");
    $('#IsMarriedError').text("");
    $('#EmailError').text("");
    $('#DobError').text("");
    $('#MobError').text("");

    var isValid = true;
    var emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

    if ($('#Name').val().trim() === "") {
        $('#Name').css('border-color', 'Red');

        $('#NameError').text('Please enter name');
        isValid = false;
    } else {
        $('#Name').css('border-color', 'lightgrey');
    }

    var mobileNumber = $('#MobileNumber').val().trim();
    var mobileNumberPattern = /^\d{10}$/; // Regular expression for 10-digit number

    if (mobileNumber === "") {
        $('#MobileNumber').css('border-color', 'Red');
        $('#MobError').text('Please enter a mobile number');
        isValid = false;
    } else if (!mobileNumberPattern.test(mobileNumber)) {
        $('#MobileNumber').css('border-color', 'Red');
        $('#MobError').text('Please enter a valid 10-digit mobile number');
        isValid = false;
    } else {
        $('#MobileNumber').css('border-color', 'lightgrey');
    }


    var ageValue = $('#Age').val().trim();
    if (ageValue === "") {
        $('#Age').css('border-color', 'Red');
        $('#AgeError').text('Please enter age');
        isValid = false;
    } else {
        // Use parseInt to check if the input is a valid integer
        var parsedAge = parseInt(ageValue);
        if (isNaN(parsedAge)) {
            $('#Age').css('border-color', 'Red');
            $('#AgeError').text('Please enter a valid integer for age');
            isValid = false;
        } else {
            $('#Age').css('border-color', 'lightgrey');
        }
    }


    if ($('#Country').val().trim() === "") {
        $('#Country').css('border-color', 'Red');
        $('#CountryError').text('Please select Country');
        isValid = false;
    } else {
        $('#Country').css('border-color', 'lightgrey');
    }


    if ($('#State').val().trim() === "") {
        $('#State').css('border-color', 'Red');
        $('#StateError').text('Please Select State');
        isValid = false;
    } else {
        $('#State').css('border-color', 'lightgrey');
    }



    if ($('#Designation').val().trim() === "") {
        $('#Designation').css('border-color', 'Red');
        $('#DesignationError').text('Please enter Designation');
        isValid = false;
    } else {
        $('#Designation').css('border-color', 'lightgrey');
    }


    if ($('#JoinedDate').val().trim() === "") {
        $('#JoinedDate').css('border-color', 'Red');
        $('#JoinedDateError').text('Please select JoinedDate');
        isValid = false;

    } else {
        $('#JoinedDate').css('border-color', 'lightgrey');
    }


    var salaryValue = $('#Salary').val().trim();

    if (salaryValue === "") {
        $('#Salary').css('border-color', 'Red');
        $('#SalaryError').text('Please enter salary.');
        isValid = false;
    } else if (isNaN(salaryValue)) {
        $('#Salary').css('border-color', 'Red');
        $('#SalaryError').text('Please enter a valid number for salary.');
        isValid = false;
    } else {
        $('#Salary').css('border-color', 'lightgrey');
    }


    if ($('#Role').val().trim() === "") {
        $('#Role').css('border-color', 'Red');
        $('#RoleError').text('Please enter Role');
        isValid = false;
    } else {
        $('#Role').css('border-color', 'lightgrey');
    }


    if ($('#IsMarried').val().trim() === "") {
        $('#IsMarried').css('border-color', 'Red');
        $('#IsMarriedError').text('Please select IsMarried');
        isValid = false;
    } else {
        $('#IsMarried').css('border-color', 'lightgrey');
    }


    if ($('#DateOfBirth').val().trim() === "") {
        $('#DateOfBirth').css('border-color', 'Red');
        $('#DobError').text('Please select JoinedDate');
        isValid = false;

    } else {
        $('#DateOfBirth').css('border-color', 'lightgrey');
    }


    var email = $('#Email').val().trim();
    if (email === "") {
        $('#Email').css('border-color', 'Red');
        $('#EmailError').text('Please enter an email address.');
        isValid = false;
    } else if (!emailRegex.test(email)) {
        $('#Email').css('border-color', 'Red');
        $('#EmailError').text('Please enter a valid email address.');
        isValid = false;
    } else {
        $('#Email').css('border-color', 'lightgrey');
        $('#EmailError').text('');
    }
    return isValid;
}