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
                                <button onclick="LockUnlock('${data.id}')" class="btn btn-danger btn-sm me-2">
                                    <i class="bi bi-lock-fill"></i> Lock
                                </button>
                                <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-secondary btn-sm">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `
                    }
                    else {
                        return `
                            <div class="text-center">
                                <button onclick="LockUnlock('${data.id}')" class="btn btn-success btn-sm me-2">
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </button>
                                <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-secondary btn-sm">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `
                    }
                },
                "width": "25%"
            }
        ]
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
