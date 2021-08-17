$(document).ready(function () {
    $("#data-table").DataTable({
        "ajax": {
            "url": '/ManagementSystem/GetAllDepartments',
            "type": 'get',
            "datatype": 'json'
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            {
                "data": "id",
                "render": function (data, type) {
                    return '<button class="btn btn-primary" onclick="Edit(' + data + ')">Edit</button>'
                }
            },
            {
                "data": "id",
                "render": function (data, type) {
                    return '<button class="btn btn-danger" onclick="Delete(' + data + ')">Delete</button>'
                }
            }
        ],
        success: function (data) {
            data.name.foreach()
        }
    });
});
function Edit(id) {
    $.ajax({
        type: "POST",
        url: "/ManagementSystem/GetEditDepartment",
        data: { Id: id },
        dataType: "json",
        success: function (data) {

            if (data.resulte) {
                $("#EditDepartmentId").val(data.resulte.id);
                $("#EditDepartmentName").val(data.resulte.name);
                $("#EditDepartment").modal('show');
                
            }
        },
        error: function (req, status, error) {
            alert(error);
        }
    });
}

function Delete(id) {
    bootbox.confirm({
        size: "small",
        message: "Are you sure to Delete This Department?",
        callback: function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/ManagementSystem/DeleteDepartment",
                    data: { Id: id },
                    dataType: "json",
                    success: function (data) {

                        if (data.resulte) {
                            toastr.success(data.msg, 'Done');
                            $('#data-table').DataTable().ajax.reload();
                        }

                    },
                    error: function (req, status, error) {
                        alert(error);
                    }
                });
            }
        }
    });
}