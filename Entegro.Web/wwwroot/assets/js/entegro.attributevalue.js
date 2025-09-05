var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue.List = (function ($) {
    'use strict';

    const init = function () {
        $(function () {
            // ================== DataTable ==================
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
                    data: (d) => JSON.stringify(d),
                },
                columns: [
                    { data: 'Id' },
                    { data: 'Id', orderable: false, render: DataTable.render.select() },
                    { data: 'ProductAttributeName' },
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
                    }
                ],
                select: { style: "multi", selector: "td:nth-child(2)" },
                order: [4, "asc"],       // DisplayOrder'a göre
                displayLength: 7,
                layout: {
                    topStart: {
                        rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                        features: [
                            { search: { className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0", placeholder: "Ara..", text: "_INPUT_" } }
                        ]
                    },
                    topEnd: {
                        rowClass: "row m-3 my-0 justify-content-between",
                        features: [
                            {
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
                                            { extend: "print", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Print</span>`, exportOptions: { columns: [2, 3, 4] } },
                                            { extend: "csv", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`, exportOptions: { columns: [2, 3, 4] } },
                                            { extend: "excel", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`, exportOptions: { columns: [2, 3, 4] } },
                                            { extend: "pdf", className: "dropdown-item", text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`, exportOptions: { columns: [2, 3, 4] } },
                                            { extend: "copy", className: "dropdown-item", text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`, exportOptions: { columns: [2, 3, 4] } }
                                        ]
                                    },
                                    {
                                        text: `
                                            <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                            <span class="d-none d-sm-inline-block">Yeni Kayıt</span>`,
                                        className: "add-new btn btn-primary",
                                        attr: { "data-action": "create-attribute-value" },
                                        action: function () {
                                            const $form = $('#createAttributeValueForm');
                                            if ($form.length) $form[0].reset();
                                            if (window.createPAVValidation) window.createPAVValidation.resetForm(true);

                                            const $pa = $('#ProductAttributeId');
                                            if ($pa.data('select2')) $pa.val(null).trigger('change');

                                            $('#createAttributeValue').find('h3.mb-2').text('Yeni Varyant Değeri');

                                            $('#createAttributeValue').one('shown.bs.modal', function () {
                                                $('#DisplayOrder').val(0);
                                            });
                                            $('#createAttributeValue').modal('show');
                                        }
                                    }
                                ]
                            }
                        ]
                    },
                    bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
                    bottomEnd: "paging"
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

            let _paCache = null;

            function fetchAllPA() {
                if (_paCache) return $.Deferred().resolve(_paCache).promise();
                return $.getJSON('/ProductAttributeValue/GetAllProductAttribute')
                    .then((data) => { _paCache = data?.results || []; return _paCache; });
            }

            function initPASelectOnce() {
                const $el = $('#ProductAttributeId');
                if (!$el.data('select2')) {
                    $el.select2({
                        dropdownParent: $('#attributeValueModal'),
                        placeholder: 'Varyant adı seçin...',
                        allowClear: true,
                        ajax: {
                            url: '/ProductAttributeValue/GetAllProductAttribute',
                            type: 'GET',
                            dataType: 'json',
                            delay: 200,
                            processResults: function (data) {
                                return { results: data?.results || [] };
                            },
                            cache: true
                        },
                        width: '100%'
                    });
                }
            }

            function setSelect2Selected(selector, id, text) {
                const $el = $(selector);
                if (!id || !$el.length) return;

                const setOption = (t) => {
                    if (!$el.find("option[value='" + id + "']").length) {
                        const opt = new Option(t || ('#' + id), id, true, true);
                        $el.append(opt).trigger('change');
                    } else {
                        $el.val(id).trigger('change');
                    }
                };

                if (text && text.length) {
                    setOption(text);
                } else {
                    fetchAllPA().done(list => {
                        const hit = list.find(x => x.id === id) || null;
                        setOption(hit ? hit.text : null);
                    }).fail(() => setOption(null));
                }
            }

            /* ============ Modal aç/kapat ve formu doldur ============ */
            function resetAttributeValueForm() {
                const $form = $('#attributeValueForm')[0];
                $form.reset();
                $('#AttributeValueId').val('');
                $('#ProductAttributeId').val(null).trigger('change'); // select2 temizle
                $('#DisplayOrder').val('0');
            }

            function openAttributeValueModal(mode = 'create', id = null) {
                initPASelectOnce();
                resetAttributeValueForm();

                const $title = $('#attributeValueModalTitle');
                const $modal = $('#attributeValueModal');

                if (mode === 'create') {
                    $title.text('Yeni Varyant Değeri');
                    $modal.modal('show');
                    return;
                }

                // edit
                $title.text('Varyant Değeri Güncelle');
                if (!id) return;

                // Veriyi çek ve doldur
                $.getJSON('/ProductAttributeValue/Edit', { id: id })
                    .done(function (m) {
                        // Beklenen JSON: { Id, Name, ProductAttributeId, DisplayOrder, [ProductAttributeName] }
                        $('#AttributeValueId').val(m.Id);
                        $('#Name').val(m.Name ?? '');
                        $('#DisplayOrder').val(m.DisplayOrder ?? 0);
                        setSelect2Selected('#ProductAttributeId', m.ProductAttributeId /*, m.ProductAttributeName*/);

                        $modal.modal('show');
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
            }

            /* ============ FormValidation (tek örnek) ============ */
            let attributeValueFV = null;

            function ensureValidation() {
                if (attributeValueFV) return attributeValueFV;

                const formEl = document.getElementById('attributeValueForm');
                if (!formEl) return null;

                attributeValueFV = FormValidation.formValidation(formEl, {
                    locale: 'tr_TR',
                    localization: FormValidation.locales.tr_TR,
                    fields: {
                        ProductAttributeId: { validators: { notEmpty: { message: 'Varyant adı seçilmelidir.' } } },
                        Name: {
                            validators: {
                                notEmpty: { message: 'Değer adı boş bırakılamaz.' },
                                stringLength: { min: 1, max: 100, message: 'Değer adı 1–100 karakter olmalıdır.' }
                            }
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
                            const $form = $('#attributeValueForm');
                            const id = $('#AttributeValueId').val();
                            const isEdit = !!id;

                            // Endpoint ve payload
                            const url = isEdit ? '/ProductAttributeValue/Edit' : '/ProductAttributeValue/Create';

                            // Çift tıklamayı engelle
                            const $submitBtn = $form.find('button[type="submit"]');
                            $submitBtn.prop('disabled', true);

                            $.ajax({
                                url: url,
                                type: 'POST',
                                data: $form.serialize(), // Id (varsa), ProductAttributeId, Name, DisplayOrder
                                success: function (res) {
                                    if (res && res.success) {
                                        Swal.fire({
                                            title: isEdit ? 'Güncellendi!' : 'Başarılı!',
                                            text: isEdit ? 'Varyant değeri başarıyla güncellendi.' : 'Varyant değeri eklendi.',
                                            icon: 'success',
                                            confirmButtonText: 'Tamam',
                                            customClass: { confirmButton: 'btn btn-success' },
                                            buttonsStyling: false
                                        }).then(() => {
                                            $('#attributeValueModal').modal('hide');
                                            // DataTable varsa güncelle
                                            window.location.reload();
                                        });
                                    } else {
                                        Swal.fire({
                                            title: 'Hata!',
                                            text: (res && res.message) || 'İşlem sırasında bir hata oluştu.',
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
                                },
                                complete: function () {
                                    $submitBtn.prop('disabled', false);
                                }
                            });
                        });
                    }
                });

                return attributeValueFV;
            }

            /* ============ Başlatma & Olay bağlama ============ */
            $(function () {
                // FormValidation’ı hazırla
                ensureValidation();

                // Modal her açıldığında select2 parent doğru olsun
                $('#attributeValueModal').on('shown.bs.modal', function () {
                    initPASelectOnce();
                });

                // "Yeni" butonu örneği: data-action="create-attribute-value"
                $(document).on('click', '[data-action="create-attribute-value"]', function () {
                    openAttributeValueModal('create');
                });

                // "Düzenle" butonu örneği: .edit-attributeValue (data-id ile)
                $(document).on('click', '.edit-attributeValue', function () {
                    const id = $(this).data('id');
                    if (!id) return;
                    openAttributeValueModal('edit', id);
                });

                // Reset’e basınca validasyon temizlensin
                $('#attributeValueForm').on('reset', function () {
                    if (attributeValueFV) attributeValueFV.resetForm(true);
                    setTimeout(() => { $('#ProductAttributeId').val(null).trigger('change'); }, 0);
                });
            });

            $(document).on('click', '.delete-attributeValue', function () {
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
                        url: '/ProductAttributeValue/Delete',
                        type: 'POST',
                        data: { id: id },
                        success: function (res) {
                            if (res && res.success) {
                                Swal.fire({
                                    title: 'Silindi!',
                                    text: 'Varyant değeri başarıyla silindi.',
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
    };

    return { init };
})(jQuery);


