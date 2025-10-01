var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue.List = (function ($) {
    'use strict';

    function addFilterDropdown(column, containerSelector, placeholder, map = null) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        let select = document.createElement("select");
        select.className = "form-select select2 text-capitalize";
        select.innerHTML = `<option value="">${placeholder}</option>`;
        container.appendChild(select);

        if (map && Array.isArray(map)) {
            map.forEach(item => {
                let option = document.createElement("option");
                option.value = item.id;
                option.textContent = item.title;
                select.appendChild(option);
            });
        } else {
            column.data().unique().sort().each(function (value) {
                if (value !== null && value !== undefined && value !== "") {
                    let option = document.createElement("option");
                    option.value = value;
                    option.textContent = value;
                    select.appendChild(option);
                }
            });
        }

        if (window.jQuery && $(select).select2) {
            $(select).select2({
                placeholder: placeholder,
                allowClear: true,
                width: "resolve"
            });


            $(select).on("change", function () {
                const val = this.value || "";
                column.search(val, false, false).draw();
            });
        } else {
            select.addEventListener("change", function () {
                const val = select.value ? `^${select.value}$` : "";
                column.search(val, true, false).draw();
            });
        }

    }
    function addFilterText(column, containerSelector, placeholder) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        // input elementini oluştur
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control";
        input.placeholder = placeholder;

        container.appendChild(input);

        // her yazımda filtre uygula (debounce ile optimize edebilirsin)
        input.addEventListener("keyup", function () {
            const val = input.value || "";
            column.search(val, false, true).draw();
        });
    }
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
                    url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
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
                    { data: 'ProductAttributeId' },
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
                                 <a href="javascript:void(0);"
                                   class="btn btn-text-secondary rounded-pill waves-effect btn-icon edit-attributeValue"
                                   data-id="${row.Id}">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                </a>
                                <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                </button>
                                <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="javascript:void(0);" class="dropdown-item edit-attributeValue" data-id="${row.Id}">Güncelle</a>
                                    <a href="javascript:void(0);" class="dropdown-item text-danger delete-attributeValue" data-id="${row.Id}">Sil</a>
                                </div>
                            </div>`
                    },
                    {
                        targets: 2, // ProductAttributeId kolonu
                        visible: false,
                        searchable: false
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
                                    //action: function () {
                                    //    window.location.href = "/ProductAttributeValue/Create";
                                    //}
                                    action: function () {
                                        Entegro.AttributeValue.create();
                                    }
                                }
                            ]
                        }]
                    },
                    bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
                    bottomEnd: "paging"
                },
                initComplete: function () {
                    this.api().columns().every(function () {
                        
                        if (this.dataSrc() === "ProductAttributeId") {
                            var column = this;
                            $('<select id="productAttributeFilter" style="width:200px"></select>')
                                .appendTo(".variantAttributeFilterVariant")
                                .on("change", function () {
                                    const val = $(this).val();
                                    column.search(val ? "^" + val + "$" : "", true, false).draw();
                                });
                            $('#productAttributeFilter').select2({
                                placeholder: "Varyant seçin",
                                allowClear: true,
                                ajax: {
                                    url: '/ProductAttribute/AllProductAttribute',
                                    type: 'POST',
                                    dataType: 'json',
                                    delay: 250,
                                    data: function (params) {
                                        return {
                                            term: params.term || "",
                                            page: params.page || 1
                                        };
                                    },
                                    processResults: function (data, params) {
                                        return {
                                            results: data.results,
                                            pagination: { more: data.pagination.more }
                                        };
                                    }
                                }
                            });
                        } else { }
                    });
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
                                        window.location.href = '/ProductAttributeValue/list';
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
        addFilterText: addFilterText,
        addFilterDropdown: addFilterDropdown,
        init
    };

})(jQuery);
