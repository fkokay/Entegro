$(function () {
    // ================== DataTable ==================
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
                                <!-- Kalem: popup ile güncelle -->
                                <a href="javascript:void(0);"
                                   class="btn btn-text-secondary rounded-pill waves-effect btn-icon edit-attribute"
                                   data-id="${row.Id}">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                </a>

                                <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                </button>
                                <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="javascript:void(0);" class="dropdown-item">View</a>
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
                                { extend: "print", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Print</span>`, exportOptions: { columns: [3, 4, 5, 6, 7] } },
                                { extend: "csv", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`, exportOptions: { columns: [3, 4, 5, 6, 7] } },
                                { extend: "excel", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`, exportOptions: { columns: [3, 4, 5, 6, 7] } },
                                { extend: "pdf", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`, exportOptions: { columns: [3, 4, 5, 6, 7] } },
                                { extend: "copy", className: "dropdown-item", text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`, exportOptions: { columns: [3, 4, 5, 6, 7] } }
                            ]
                        },
                        // Yeni Kayıt: create modal aç
                        {
                            text: `
                                        <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                        <span class="d-none d-sm-inline-block">Yeni Kayıt</span>`,
                            className: "add-new btn btn-primary",
                            action: function () {
                                const $form = $('#createProductAttributeForm');
                                if ($form.length) $form[0].reset();
                                $('#createProductAttribute').find('h3.mb-2').text('Yeni Varyant Kaydı');
                                if (window.createFormValidation) window.createFormValidation.resetForm(true);
                                $('#createProductAttribute').one('shown.bs.modal', function () {
                                    $('#DisplayOrder').val(0);
                                });
                                $('#createProductAttribute').modal('show');
                            }
                        }
                    ]
                }]
            },
            bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
            bottomEnd: "paging"
        }
    });

    // ================== CREATE: FormValidation + POST ==================
    (function () {
        const formEl = document.getElementById('createProductAttributeForm');
        if (!formEl) return;

        window.createFormValidation = FormValidation.formValidation(formEl, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                Name: {
                    validators: {
                        notEmpty: { message: 'Ad alanı boş bırakılamaz.' },
                        stringLength: { min: 3, max: 100, message: 'Ad 3–100 karakter olmalıdır.' }
                    }
                },
                Description: {
                    validators: { stringLength: { max: 1000, message: 'Açıklama en fazla 1000 karakter olabilir.' } }
                },
                DisplayOrder: {
                    validators: {
                        notEmpty: { message: 'Gösterim sırası boş bırakılamaz.' },
                        integer: { message: 'Gösterim sırası tam sayı olmalıdır.' },
                        greaterThan: { inclusive: true, min: 0, message: '0 veya daha büyük olmalıdır.' }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '', rowSelector: '.mb-3' }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                autoFocus: new FormValidation.plugins.AutoFocus()
            },
            init: (instance) => {
                instance.on('core.form.valid', function () {
                    const $form = $('#createProductAttributeForm');
                    $.ajax({
                        url: '/ProductAttribute/Create',
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (res) {
                            if (res && res.success) {
                                Swal.fire({
                                    title: 'Başarılı!',
                                    text: 'Kayıt başarıyla eklendi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    $('#createProductAttribute').modal('hide');
                                    dt.ajax.reload(null, false);
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: (res && res.message) || 'Bir hata oluştu.',
                                    icon: 'error',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function (xhr) {
                            Swal.fire({
                                title: 'Hata!',
                                text: xhr.responseText || 'İşlem sırasında bir hata oluştu.',
                                icon: 'error',
                                confirmButtonText: 'Tamam',
                                customClass: { confirmButton: 'btn btn-danger' },
                                buttonsStyling: false
                            });
                        }
                    });
                });
            }
        });
    })();

    // ================== EDIT: Aç (GET), doldur, POST ==================
    // Hem kalem ikonunda hem menüde "Güncelle" öğesinde aynı sınıfı kullandık: .edit-attribute
    $(document).on('click', '.edit-attribute', function () {
        const id = $(this).data('id');
        if (!id) return;

        const $editForm = $('#editProductAttributeForm');
        if ($editForm.length) $editForm[0].reset();
        if (window.editFormValidation) window.editFormValidation.resetForm(true);

        $.getJSON('/ProductAttribute/Edit', { id: id })
            .done(function (m) {
                $('#Edit_Id').val(m.Id);
                $('#Edit_Name').val(m.Name ?? '');
                $('#Edit_Description').val(m.Description ?? '');
                $('#Edit_DisplayOrder').val(m.DisplayOrder ?? 0);

                $('#editProductAttribute').find('h3.mb-2').text('Varyant Güncelle');
                $('#editProductAttribute').modal('show');
            })
            .fail(function (xhr) {
                Swal.fire({
                    title: 'Hata!',
                    text: xhr.responseText || 'Kayıt bilgisi alınamadı.',
                    icon: 'error',
                    confirmButtonText: 'Tamam',
                    customClass: { confirmButton: 'btn btn-danger' },
                    buttonsStyling: false
                });
            });
    });

    // Edit form doğrulama + POST (/ProductAttribute/Edit -> UpdateProductAttributeViewModel)
    (function () {
        const editFormEl = document.getElementById('editProductAttributeForm');
        if (!editFormEl) return;

        window.editFormValidation = FormValidation.formValidation(editFormEl, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                Name: {
                    validators: {
                        notEmpty: { message: 'Ad alanı boş bırakılamaz.' },
                        stringLength: { min: 3, max: 100, message: 'Ad 3–100 karakter olmalıdır.' }
                    }
                },
                Description: {
                    validators: { stringLength: { max: 1000, message: 'Açıklama en fazla 1000 karakter olabilir.' } }
                },
                DisplayOrder: {
                    validators: {
                        notEmpty: { message: 'Gösterim sırası boş bırakılamaz.' },
                        integer: { message: 'Gösterim sırası tam sayı olmalıdır.' },
                        greaterThan: { inclusive: true, min: 0, message: '0 veya daha büyük olmalıdır.' }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '', rowSelector: '.mb-3' }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                autoFocus: new FormValidation.plugins.AutoFocus()
            },
            init: (instance) => {
                instance.on('core.form.valid', function () {
                    const $form = $('#editProductAttributeForm');
                    $.ajax({
                        url: '/ProductAttribute/Edit',
                        type: 'POST',
                        data: $form.serialize(), // Id, Name, Description, DisplayOrder
                        success: function (res) {
                            if (res && res.success) {
                                Swal.fire({
                                    title: 'Güncellendi!',
                                    text: 'Kayıt başarıyla güncellendi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    $('#editProductAttribute').modal('hide');
                                    dt.ajax.reload(null, false);
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: (res && res.message) || 'Güncelleme sırasında bir hata oluştu.',
                                    icon: 'error',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function (xhr) {
                            Swal.fire({
                                title: 'Hata!',
                                text: xhr.responseText || 'İşlem sırasında bir hata oluştu.',
                                icon: 'error',
                                confirmButtonText: 'Tamam',
                                customClass: { confirmButton: 'btn btn-danger' },
                                buttonsStyling: false
                            });
                        }
                    });
                });
            }
        });
    })();

    // ================== DELETE: SweetAlert2 onay + POST ==================
    $(document).on('click', '.delete-attribute', function () {
        const id = $(this).data('id');
        if (!id) return;

        Swal.fire({
            title: 'Emin misiniz?',
            text: 'Bu işlem geri alınamaz!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, sil!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-danger me-2',
                cancelButton: 'btn btn-label-secondary'
            },
            buttonsStyling: false
        }).then((result) => {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/ProductAttribute/Delete',
                type: 'POST',
                data: { id: id },
                success: function (res) {
                    if (res && res.success) {
                        Swal.fire({
                            title: 'Silindi!',
                            text: 'Kayıt başarıyla silindi.',
                            icon: 'success',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-success' },
                            buttonsStyling: false
                        });
                        dt.ajax.reload(null, false);
                    } else {
                        Swal.fire({
                            title: 'Hata!',
                            text: (res && res.message) || 'Silme sırasında hata oluştu.',
                            icon: 'error',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-danger' },
                            buttonsStyling: false
                        });
                    }
                },
                error: function (xhr) {
                    Swal.fire({
                        title: 'Hata!',
                        text: xhr.responseText || 'İşlem sırasında hata oluştu.',
                        icon: 'error',
                        confirmButtonText: 'Tamam',
                        customClass: { confirmButton: 'btn btn-danger' },
                        buttonsStyling: false
                    });
                }
            });
        });
    });
});