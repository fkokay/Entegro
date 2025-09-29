var Entegro = Entegro || {};
Entegro.log = Entegro.log || {};

Entegro.log.list = (function ($) {

    function initList() {
        const table = $('#LogTable').DataTable({
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
                url: '/Log/LogList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Level' },
                {
                    data: 'Message',
                    render: function (data, type, row) {
                        if (type === "display") {
                            return `<a href="javascript:void(0);" class="view-log-detail" data-log='${JSON.stringify(row)}'>${data}</a>`;
                        }
                        return data;
                    }
                },
                { data: 'MessageTemplate' },
                {
                    data: 'TimeStamp',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.yyyy HH:mm:ss");
                    }
                },
                { data: 'Exception' }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 1,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                {
                    targets: -1,
                    title: 'İşlem',
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <button class="btn btn-text-danger btn-icon delete-log" title="Sil" data-id="${row.Id}">
                                <i class="icon-base ti ti-trash icon-22px"></i>
                            </button>`;
                    }
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
                            placeholder: "Log ara...",
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
                                        <span class="d-none d-sm-inline-block">Dışa Aktar</span>
                                      </span>`,
                                buttons: [
                                    {
                                        extend: "print",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Yazdır</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> CSV</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> PDF</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Kopyala`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    }
                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-trash icon-16px me-0 me-sm-1"></i>
                                       <span class="d-none d-sm-inline-block">Tümünü Sil</span>`,
                                className: "btn btn-danger delete-all-logs",
                                action: function () {
                                    Swal.fire({
                                        title: 'Emin misiniz?',
                                        text: 'Tüm loglar kalıcı olarak silinecek!',
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
                                            $.post('/Log/Delete', { id: 0 }, function (response) {
                                                if (response.success) {
                                                    Swal.fire({
                                                        icon: 'success',
                                                        title: 'Silindi!',
                                                        text: 'Tüm loglar başarıyla silindi.',
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
                                            });
                                        }
                                    });
                                }
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

        // Görsel düzeltmeler
        setTimeout(() => {
            const adjustments = [
                { selector: ".dt-buttons .btn", classToRemove: "btn-secondary" },
                { selector: ".dt-buttons.btn-group", classToAdd: "mb-md-0 mb-6" },
                { selector: ".dt-search .form-control", classToRemove: "form-control-sm", classToAdd: "ms-0" },
                { selector: ".dt-search", classToAdd: "mb-0 mb-md-6" },
                { selector: ".dt-length .form-select", classToRemove: "form-select-sm" },
                { selector: ".dt-layout-end", classToAdd: "gap-md-2 gap-0 mt-0" },
                { selector: ".dt-layout-start", classToAdd: "mt-0" },
                { selector: ".dt-layout-table", classToRemove: "row mt-2" },
                { selector: ".dt-layout-full", classToRemove: "col-md col-12", classToAdd: "table-responsive" }
            ];
            adjustments.forEach(({ selector, classToRemove, classToAdd }) => {
                document.querySelectorAll(selector).forEach(el => {
                    if (classToRemove) classToRemove.split(" ").forEach(cls => el.classList.remove(cls));
                    if (classToAdd) classToAdd.split(" ").forEach(cls => el.classList.add(cls));
                });
            });
        }, 100);

        // Tek log sil
        $(document).on('click', '.delete-log', function () {
            const logId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu log silinecek!',
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
                    $.post('/Log/Delete', { id: logId }, function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Silindi!',
                                text: 'Log başarıyla silindi.',
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
                    });
                }
            });
        });

        // Detay göster
        $(document).on('click', '.view-log-detail', function () {
            const log = $(this).data('log');
            const detailHtml = `
                <div class="text-start">
                    <p><strong>Mesaj:</strong> ${log.Message}</p>
                    <p><strong>Şablon:</strong> ${log.MessageTemplate || '-'}</p>
                    <p><strong>Exception:</strong><br><pre>${log.Exception || '-'}</pre></p>
                    <p><strong>Özellikler:</strong><br><pre>${log.Properties || '-'}</pre></p>
                    <p><strong>Log Event:</strong> ${log.LogEvent || '-'}</p>
                </div>
            `;
            Swal.fire({
                title: `Log Detayı (ID: ${log.Id})`,
                html: detailHtml,
                width: '60%',
                confirmButtonText: 'Kapat',
                customClass: { confirmButton: 'btn btn-secondary' },
                buttonsStyling: false
            });
        });
    }

    return {
        init: initList
    };

})(jQuery);
