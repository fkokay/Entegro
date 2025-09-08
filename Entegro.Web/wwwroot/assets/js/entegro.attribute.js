var Entegro = Entegro || {};
Entegro.Attribute = Entegro.Attribute || {};

Entegro.Attribute = (function ($) {
    'use strict';

    const init = function () {
        $(function () {
           

            // CREATE form validation
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
                        Description: { validators: { stringLength: { max: 1000, message: 'Açıklama en fazla 1000 karakter olabilir.' } } },
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

            // EDIT aç ve doldur
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

            // EDIT form validation
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
                        Description: { validators: { stringLength: { max: 1000, message: 'Açıklama en fazla 1000 karakter olabilir.' } } },
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
                                data: $form.serialize(),
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

            // DELETE
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
    };

    return {
        init: init
    };

})(jQuery);



