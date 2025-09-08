var Entegro = Entegro || {};
Entegro.Attribute = Entegro.Attribute || {};

Entegro.Attribute.List = (function ($) {
    'use strict';

    const init = function () {
        $(function () {
            const dt = $('#ProductAttributeTable').DataTable({
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
                    url: '/ProductAttribute/ProductAttributeList',
                    type: 'POST',
                    contentType: 'application/json',
                    data: (d) => JSON.stringify(d),
                },
                columns: [
                    { data: 'Id' },
                    { data: 'Id', orderable: false, render: DataTable.render.select() },
                    { data: 'Name' },
                    { data: 'DisplayOrder' },
                    { data: 'Id' },
                ],
                columnDefs: [
                    { className: "control", searchable: false, orderable: false, responsivePriority: 2, targets: 0, render: () => "" },
                    {
                        targets: 1, orderable: false, searchable: false, responsivePriority: 3,
                        checkboxes: { selectAllRender: '<input type="checkbox" class="form-check-input">' },
                        render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                    },
                    {
                        targets: -1, title: "İşlemler", searchable: false, orderable: false,
                        render: (data, type, row) => `
                            <div class="d-inline-block text-nowrap">
                                <a href="javascript:void(0);" class="btn btn-text-secondary rounded-pill waves-effect btn-icon edit-attribute" data-id="${row.Id}">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                </a>
                                <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                </button>
                                <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="javascript:void(0);" class="dropdown-item edit-attribute" data-id="${row.Id}">Güncelle</a>
                                    <a href="javascript:void(0);" class="dropdown-item text-danger delete-attribute" data-id="${row.Id}">Sil</a>
                                </div>
                            </div>`
                    }
                ],
                select: { style: "multi", selector: "td:nth-child(2)" },
                order: [3, "asc"],
                displayLength: 7,
                layout: {
                    topStart: {
                        rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                        features: [{ search: { className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0", placeholder: "Ara..", text: "_INPUT_" } }]
                    },
                    topEnd: {
                        rowClass: "row m-3 my-0 justify-content-between",
                        features: [{
                            pageLength: { menu: [7, 10, 25, 50, 100], text: "_MENU_" },
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
                                        { extend: "print", className: "dropdown-item", text: `<i class="icon-base ti tabler-printer me-1"></i> Print`, exportOptions: { columns: [2, 3] } },
                                        { extend: "csv", className: "dropdown-item", text: `<i class="icon-base ti tabler-file me-1"></i> CSV`, exportOptions: { columns: [2, 3] } },
                                        { extend: "excel", className: "dropdown-item", text: `<i class="icon-base ti tabler-upload me-1"></i> Excel`, exportOptions: { columns: [2, 3] } },
                                        { extend: "pdf", className: "dropdown-item", text: `<i class="icon-base ti tabler-file-text me-1"></i> PDF`, exportOptions: { columns: [2, 3] } },
                                        { extend: "copy", className: "dropdown-item", text: `<i class="icon-base ti tabler-copy me-1"></i> Kopyala`, exportOptions: { columns: [2, 3] } }
                                    ]
                                },
                                {
                                    text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i> <span class="d-none d-sm-inline-block">Yeni Kayıt</span>`,
                                    className: "add-new btn btn-primary",
                                    action: function () {
                                        Entegro.Attribute.create();
                                    }
                                }
                            ]
                        }]
                    },
                    bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
                    bottomEnd: "paging"
                }
            });

            $(document).on('attributeTable.reload', () => {
                dt.ajax.reload(null, false);
            });
        });
    };

    return { init };
})(jQuery);
