var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue.List = (function ($) {
    'use strict';

    const init = function () {
        $(function () {
            const dt = $('#ProductAttributeValueTable').DataTable({
                language: {
                    paginate: {
                        next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                        previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                        first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                        last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                    },
                    url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
                },
                serverSide: true,
                ajax: {
                    url: '/ProductAttributeValue/ProductAttributeValueList',
                    type: 'POST',
                    contentType: 'application/json',
                    data: d => JSON.stringify(d),
                },
                columns: [
                    { data: 'Id' },
                    { data: 'Id', orderable: false, render: DataTable.render.select() },
                    { data: 'ProductAttributeName' },
                    { data: 'Name' },
                    { data: 'DisplayOrder' },
                    { data: 'Id' }
                ],
                columnDefs: [
                    { className: "control", searchable: false, orderable: false, responsivePriority: 2, targets: 0, render: () => "" },
                    {
                        targets: 1,
                        orderable: false,
                        searchable: false,
                        responsivePriority: 3,
                        checkboxes: { selectAllRender: '<input type="checkbox" class="form-check-input">' },
                        render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                    },
                    {
                        targets: -1,
                        title: "İşlemler",
                        searchable: false,
                        orderable: false,
                        render: (data, type, row) => `
                            <div class="d-inline-block text-nowrap">
                                <a href="/ProductAttributeValue/Edit/${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                </a>
                                <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                </button>
                                <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="/ProductAttributeValue/Edit/${row.Id}" class="dropdown-item">Güncelle</a>
                                    <a href="javascript:void(0);" class="dropdown-item text-danger delete-attributeValue" data-id="${row.Id}">Sil</a>
                                </div>
                            </div>`
                    }
                ],
                select: {
                    style: "multi",
                    selector: "td:nth-child(2)"
                },
                order: [[4, "asc"]],
                displayLength: 7,
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
                                menu: [7, 10, 25, 50, 100],
                                text: "_MENU_"
                            },
                            buttons: [
                                {
                                    extend: "collection",
                                    className: "btn btn-label-secondary dropdown-toggle me-4",
                                    text: `
                                        <span class="d-flex align-items-center gap-1">
                                            <i class="icon-base ti ti-upload icon-xs"></i>
                                            <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                                        </span>`,
                                    buttons: [
                                        { extend: "print", className: "dropdown-item", exportOptions: { columns: [2, 3, 4] } },
                                        { extend: "csv", className: "dropdown-item", exportOptions: { columns: [2, 3, 4] } },
                                        { extend: "excel", className: "dropdown-item", exportOptions: { columns: [2, 3, 4] } },
                                        { extend: "pdf", className: "dropdown-item", exportOptions: { columns: [2, 3, 4] } },
                                        { extend: "copy", className: "dropdown-item", exportOptions: { columns: [2, 3, 4] } }
                                    ]
                                },
                                {
                                    text: `
                                        <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                        <span class="d-none d-sm-inline-block">Yeni Kayıt</span>`,
                                    className: "add-new btn btn-primary",
                                    action: function () {
                                        window.location.href = "/ProductAttributeValue/Create";
                                    }
                                }
                            ]
                        }]
                    },
                    bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
                    bottomEnd: "paging"
                }
            });

            $(document).on('click', '.delete-attributeValue', function () {
                const attributeValueId = $(this).data('id');
                Swal.fire({
                    title: 'Emin misiniz?',
                    text: 'Bu ürün özelliği silinecek!',
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
                            url: '/ProductAttributeValue/Delete',
                            type: 'POST',
                            data: { id: attributeValueId },
                            success: function (response) {
                                if (response.success) {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Silindi!',
                                        text: 'Özellik başarıyla silindi.',
                                        confirmButtonText: 'Tamam',
                                        customClass: { confirmButton: 'btn btn-success' },
                                        buttonsStyling: false
                                    }).then(() => {
                                        location.reload();
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
        });
    };

    return {
        init
    };

})(jQuery);
