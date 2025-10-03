var Entegro = Entegro || {};
Entegro.notification = Entegro.notification || {};

Entegro.notification.list = (function ($) {

    function initList() {
        const table = $('#NotificationTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            order: [[4, 'desc']],
            ajax: {
                url: '/Notification/NotificationList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                {
                    data: 'Type',
                    render: function (data) {
                        const types = {
                            1: { text: 'Bilgi', class: 'badge bg-info' },
                            2: { text: 'Başarılı', class: 'badge bg-success' },
                            3: { text: 'Uyarı', class: 'badge bg-warning' },
                            4: { text: 'Hata', class: 'badge bg-danger' }
                        };
                        const type = types[data] || { text: 'Bilinmiyor', class: 'badge bg-secondary' };
                        return `<span class="${type.class}">${type.text}</span>`;
                    }
                },
                { data: 'Title' },
                { data: 'Message' },
                {
                    data: 'NotificationDate',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.yyyy HH:mm");
                    }
                },
                {
                    data: 'IsRead',
                    className: "text-center",
                    render: function (data) {
                        const checked = data ? "checked" : "";
                        const title = data ? "Okundu" : "Okunmadı";
                        return `
                            <div class="form-check d-inline-flex justify-content-center">
                                <input class="form-check-input" type="checkbox" ${checked} disabled title="${title}">
                            </div>`;
                    }
                },
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: {
                style: "multi",
                selector: "td:nth-child(1)"
            },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                        buttons: [
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1">
                                        <i class="icon-base ti ti-upload icon-xs"></i>
                                        <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                                      </span>`,
                                buttons: [
                                    {
                                        extend: "print",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Print</span>`,
                                        exportOptions: { columns: [2, 3, 4] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`,
                                        exportOptions: { columns: [2, 3, 4] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [2, 3, 4] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`,
                                        exportOptions: { columns: [2, 3, 4] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`,
                                        exportOptions: { columns: [2, 3, 4] }
                                    }
                                ]
                            }
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            }
        });

        // Silme işlemi
        $(document).on('click', '.delete-record', function () {
            const notificationId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu bildirim silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Notification/Delete',
                        type: 'POST',
                        data: { id: notificationId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Bildirim başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    table.ajax.reload(null, false);
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Sunucu Hatası!',
                                text: 'İstek gönderilirken bir hata oluştu.',
                                confirmButtonText: 'Tamam',
                                customClass: { confirmButton: 'btn btn-danger' },
                                buttonsStyling: false
                            });
                        }
                    });
                }
            });
        });
    }

    return {
        init: initList
    };

})(jQuery);
