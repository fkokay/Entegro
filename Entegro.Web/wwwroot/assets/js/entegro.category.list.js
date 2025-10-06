var Entegro = Entegro || {};
Entegro.category = Entegro.category || {};

Entegro.category.list = (function () {

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
    function init() {
        const table = $('#CategoryTable').DataTable({
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
                url: '/Category/CategoryList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id' }, // 0 - control
                {
                    data: 'Id',
                    orderable: false,
                    render: DataTable.render.select()
                }, // 1 - checkbox
                { data: 'Id', visible: false }, // 2 - Id
                { data: 'ParentId', visible: false }, // 3 - ParentId (gizli)
                { data: 'Name' }, // 4
                { data: 'DisplayOrder' }, // 5
                {
                    data: 'UpdatedOn',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.yyyy HH:mm");
                    }
                }, // 6
                {
                    data: 'Published',
                    render: data => {
                        const checked = data ? "checked" : "";
                        const titleText = data ? "Yayında" : "Yayında Değil";
                        return `
                        <div class="form-check d-inline-flex justify-content-center">
                          <input class="form-check-input" type="checkbox" ${checked} onclick="return false;" title="${titleText}">
                        </div>`;
                    }
                },
                { data: 'Id' } // 8 - işlemler
            ],
            columnDefs: [
                {
                    className: "control",
                    searchable: false,
                    orderable: false,
                    responsivePriority: 2,
                    targets: 0,
                    render: () => ""
                },
                {
                    targets: 1,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 4,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                {
                    targets: 4,
                    responsivePriority: 1,
                    render: (data, type, row) => {
                        const image = row.MediaFile == null ? '' : `<img src="${row.MediaFile.Url}" class="rounded">`;

                        return `<div class="d-flex justify-content-start align-items-center product-name">
                           <div class="avatar-wrapper">
                             <div class="avatar me-2 me-sm-4 rounded-2 bg-label-secondary">
                               ${image}
                             </div>
                           </div>
                           <div class="d-flex flex-column">
                             <h6 class="text-nowrap mb-0">${row.Name}</h6>
                             ${row.Breadcrumb}
                           </div>
                         </div>`;
                    }
                },
                {
                    targets: -1,
                    title: "İşlemler",
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                                <div class="d-inline-block text-nowrap">
                                  <a href="Edit?id=${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                  </a>
                                  <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                  </button>
                                  <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="Details?id=${row.Id}" class="dropdown-item">Detaylar</a>
                                    <a href="Archive?id=${row.Id}" class="dropdown-item">Arşiv</a>
                                    <div class="dropdown-divider"></div>
                                    <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Kategori Sil</a>
                                  </div>
                                </div>`
                },
                {
                    targets: 3, // ParentId kolonunu gizliyoruz
                    visible: false,
                    searchable: false
                }
            ],

            select: {
                style: "multi",
                selector: "td:nth-child(2)"
            },
            order: [[3, "asc"]],
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
                            menu: [7, 10, 25, 50, 100],
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
                                        exportOptions: { columns: [3, 4, 5] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`,
                                        exportOptions: { columns: [3, 4, 5] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [3, 4, 5] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`,
                                        exportOptions: { columns: [3, 4, 5] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`,
                                        exportOptions: { columns: [3, 4, 5] }
                                    }
                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                   <span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    window.location.href = "Create";
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
            },
            initComplete: function () {
                this.api().columns().every(function () {
                    ; if (this.dataSrc() === "Name") {
                        Entegro.category.list.addFilterText(this, ".categoryFilterName", "Kategori Adı");
                    } else if (this.dataSrc() === "Published") {
                        Entegro.category.list.addFilterDropdown(this, ".categoryFilterPublished", "Durum", [{ title: "Yayınlandı", id: true }, { title: "Yayınlanmadı", id: false }]);
                    } else if (this.dataSrc() === "ParentId") {
                        var column = this;

                        $('<select id="categoryFilter" style="width:200px"></select>')
                            .appendTo(".categoryFilterParent")
                            .on("change", function () {
                                const val = $(this).val();
                                column.search(val ? "^" + val + "$" : "", true, false).draw();
                            });

                        $('#categoryFilter').select2({
                            width: '100%',
                            placeholder: 'Kategori seçiniz',
                            allowClear: true,
                            dropdownParent: $('#categoryFilter').parent(),
                            minimumInputLength: 0,
                            language: {
                                inputTooShort: () => 'Daha fazla karakter yazın',
                                searching: () => 'Aranıyor...',
                                noResults: () => 'Sonuç bulunamadı'
                            },
                            ajax: {
                                url: '/category/AllMainCategory',
                                type: 'POST',
                                dataType: 'json',
                                delay: 250,
                                data: function (params) {
                                    return {
                                        term: params.term || '',
                                        page: params.page || 1
                                    };
                                },
                                processResults: function (data, params) {
                                    params.page = params.page || 1;

                                    return {
                                        results: Array.isArray(data.results) ? data.results : [],
                                        pagination: { more: !!(data.pagination && data.pagination.more) }
                                    };
                                },
                                cache: true
                            },
                            templateResult: item => item.text || '',
                            templateSelection: item => item.text || '',
                            escapeMarkup: m => m
                        });


                    } else { }
                });
            }
        });

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

        $(document).on('click', '.delete-record', function () {
            var categoryId = $(this).data('id');

            Swal.fire({
                title: 'Silme Türünü Seçin',
                html: `
                <div style="text-align: left">
                  <label><input type="radio" name="delete-option" value="0" checked> Sadece bağlantıyı kaldır</label><br>
                  <label><input type="radio" name="delete-option" value="1"> Bağlantı ve alt kategorileri sil</label>
                </div>`,
                showCancelButton: true,
                confirmButtonText: 'Devam Et',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false,
                preConfirm: () => {
                    const selected = document.querySelector('input[name="delete-option"]:checked');
                    if (!selected) {
                        Swal.showValidationMessage('Lütfen bir seçenek seçin.');
                        return false;
                    }
                    return selected.value;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const chooseType = parseInt(result.value);
                    $.ajax({
                        url: '/category/delete',
                        type: 'POST',
                        data: {
                            id: categoryId,
                            chooseType: chooseType
                        },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Kategori başarıyla silindi.',
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
        addFilterText: addFilterText,
        addFilterDropdown: addFilterDropdown,
        init: init

    };
})(jQuery);
