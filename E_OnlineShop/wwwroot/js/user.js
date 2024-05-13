var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                            <div class="text-center">
                                <button onclick="LockUnlock('${data.id}')" type="button" class="btn btn-danger btn-sm me-1 mb-1">
                                    <i class="bi bi-lock-fill"></i>
                                </button>
                                <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-secondary btn-sm me-1 mb-1">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                                <button onclick="Delete('/admin/user/delete/${data.id}')" class="btn btn-danger btn-sm">
                                   <i class="bi bi-trash-fill"></i> 
                                </button>
                            </div>
                        `
                    }
                    else {
                        return `
                            <div class="text-center">
                                <button onclick="LockUnlock('${data.id}')" type="button" class="btn btn-success btn-sm me-1 mb-1">
                                    <i class="bi bi-unlock-fill"></i>
                                </button>
                                <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-secondary btn-sm me-1 mb-1">
                                    <i class="bi bi-pencil-square"></i> 
                                </a>
                                <button onclick="Delete('/admin/user/delete/${data.id}')" class="btn btn-danger btn-sm">
                                   <i class="bi bi-trash-fill"></i> 
                                </button>
                            </div>
                        `
                    }
                },
                "width": "25%"
            }
        ],
        "responsive": true
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: data.message,
                    showConfirmButton: false,
                    timer: 1500
                });
                dataTable.ajax.reload();
            }
        }
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();

                    Swal.fire({
                        title: 'Success',
                        text: data.message,
                        icon: 'success'
                    });
                }
            })
        }
    })
}