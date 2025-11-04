var Entegro = Entegro || {};
Entegro.import = Entegro.import || {};

Entegro.import.list = (function ($) {
    function initList() {
        const table = $('#ImportTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json'
            },
            serverSide: true,
            order: [[3, 'asc']],  // Profil Adı
            ajax: {
                url: '/Import/ImportProfileList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', orderable: false },         
                { data: 'Id', visible: false },            
                { data: 'ProfileName' },                    
                { data: 'MediaFileType' },                  
                { data: 'MediaFileUrl' },                   
                { data: 'Id' }                              
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                
                {
                    targets: 2,
                    responsivePriority: 1,
                    render: (data, type, row) => {
                        const image = row.MediaFileUrl ? `<img src="${row.MediaFileUrl}" class="rounded me-2" width="32" height="32" alt="Medya">` : '';
                        return `<div class="d-flex align-items-center"><span>${data}</span></div>`;
                    }
                },
                {
                    targets: 4,
                    render: data => {
                        if (!data) return '';
                        return `<a href="${data}" target="_blank">${data}</a>`;
                    }
                },
                {
                    targets: 5,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    className: "text-center",
                    render: (data, type, row) => `

                        <div class="d-inline-block text-nowrap">
                            <a href="/Import/Xml?profileId=${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                                <i class="icon-base ti ti-pencil icon-22px"></i>
                            </a>
                            <button type="button" onclick="Entegro.import.list.runJob('ImportJob',${row.TaskId})" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="İçe Aktar">
                                <i class="icon-base ti ti-player-play icon-22px"></i>
                            </button>
                            <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                            </button>
                            <div class="dropdown-menu dropdown-menu-end m-0">
                            <a href="ImportAllProductsFromXml?profileId=${row.Id}" class="dropdown-item">Bütün Ürünleri Kaydet</a>
                         
                                <div class="dropdown-divider"></div>
                                <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Sil</a>
                            </div>
                        </div>`
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
                                        exportOptions: { columns: [2, 3, 4, 5, 6] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5, 6] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5, 6] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`,
                                        exportOptions: { columns: [2, 3, 4, 5, 6] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`,
                                        exportOptions: { columns: [2, 3, 4, 5, 6] }
                                    }
                                ]
                            },
                            //{
                            //    text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                            //            <span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                            //    className: "add-new btn btn-primary",
                            //    action: function () {
                            //        window.location.href = "Create";
                            //    }
                            //}
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

        // Görsel sınıf düzeltmeleri
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

        // Silme işlemi
        $(document).on('click', '.delete-record', function () {
            const recordId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu kayıt silinecek!',
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
                        url: '/Import/DeleteProfile',
                        type: 'POST',
                        data: { id: recordId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Kayıt başarıyla silindi.',
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

    function runJob(type, taskId) {
        if (!type) {
            toastr.error("Geçersiz job tipi.");
            return;
        }

        $.ajax({
            url: '/Scheduling/Run',
            type: 'POST',
            data: { type: type, taskId: taskId },
            success: function (res) {
                if (res.success) {
                    toastr.success(type + " job başarıyla çalıştırıldı.");
                    $('#SchedulingTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(res.error || "İşlem başarısız.");
                }
            },
            error: function (xhr) {
                const msg = xhr.responseText || "Bilinmeyen hata.";
                toastr.error("Hata: " + msg);
            }
        });
    }

    return {
        init: initList,
        runJob: runJob
    };

})(jQuery);
