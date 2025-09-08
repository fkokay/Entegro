var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue = (function ($) {
    'use strict';

    const init = function () {
        $(function () {
           
            // ================== Görsel küçük ayarlar (opsiyonel) ==================
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

            // ================== Select2 kaynakları ==================
            initPASelect('#ProductAttributeId', '#createAttributeValue');     // Create modal
            initPASelect('#Edit_ProductAttributeId', '#editAttributeValue');  // Edit modal

            // Tek seferlik cache: tüm attribute listesi (Edit text set etmek için)
            let _paCache = null;
            function fetchAllPA() {
                if (_paCache) return $.Deferred().resolve(_paCache).promise();
                return $.getJSON('/ProductAttributeValue/GetAllProductAttribute')
                    .then((data) => { _paCache = data?.results || []; return _paCache; });
            }

            function initPASelect(selector, modalSelector) {
                const $el = $(selector);
                if (!$el.length) return;

                $el.select2({
                    dropdownParent: $(modalSelector),
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

            // Seçili ProductAttributeId’yi Select2’ye yerleştir (ismi mümkünse doldur)
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

            // ================== CREATE: FormValidation + POST ==================
            (function () {
                const formEl = document.getElementById('createAttributeValueForm');
                if (!formEl) return;

                window.createPAVValidation = FormValidation.formValidation(formEl, {
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
                            const $form = $('#createAttributeValueForm');
                            $.ajax({
                                url: '/ProductAttributeValue/Create',
                                type: 'POST',
                                data: $form.serialize(), // ProductAttributeId, Name, DisplayOrder
                                success: function (res) {
                                    if (res && res.success) {
                                        Swal.fire({
                                            title: 'Başarılı!',
                                            text: 'Varyant değeri eklendi.',
                                            icon: 'success',
                                            confirmButtonText: 'Tamam',
                                            customClass: { confirmButton: 'btn btn-success' },
                                            buttonsStyling: false
                                        }).then(() => {
                                            $('#createAttributeValue').modal('hide');
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
            $(document).on('click', '.edit-attributeValue', function () {
                const id = $(this).data('id');
                if (!id) return;

                const $editForm = $('#editAttributeValueForm');
                if ($editForm.length) $editForm[0].reset();
                if (window.editPAVValidation) window.editPAVValidation.resetForm(true);

                $.getJSON('/ProductAttributeValue/Edit', { id: id })
                    .done(function (m) {
                        // Beklenen JSON: { Id, Name, ProductAttributeId, DisplayOrder }
                        $('#Edit_Id').val(m.Id);
                        $('#Edit_Name').val(m.Name ?? '');
                        $('#Edit_DisplayOrder').val(m.DisplayOrder ?? 0);

                        // ProductAttribute select2'yi seçili getir (text yoksa cache'ten bul)
                        setSelect2Selected('#Edit_ProductAttributeId', m.ProductAttributeId /*, m.ProductAttributeName*/);

                        $('#editAttributeValue').find('h3.mb-2').text('Varyant Değeri Güncelle');
                        $('#editAttributeValue').modal('show');
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

            (function () {
                const editFormEl = document.getElementById('editAttributeValueForm');
                if (!editFormEl) return;

                window.editPAVValidation = FormValidation.formValidation(editFormEl, {
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
                            const $form = $('#editAttributeValueForm');
                            $.ajax({
                                url: '/ProductAttributeValue/Edit',
                                type: 'POST',
                                data: $form.serialize(), // Id, ProductAttributeId, Name, DisplayOrder
                                success: function (res) {
                                    if (res && res.success) {
                                        Swal.fire({
                                            title: 'Güncellendi!',
                                            text: 'Varyant değeri başarıyla güncellendi.',
                                            icon: 'success',
                                            confirmButtonText: 'Tamam',
                                            customClass: { confirmButton: 'btn btn-success' },
                                            buttonsStyling: false
                                        }).then(() => {
                                            $('#editAttributeValue').modal('hide');
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


