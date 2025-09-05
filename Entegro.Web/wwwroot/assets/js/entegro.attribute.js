var Entegro = Entegro || {};
Entegro.Attribute = Entegro.Attribute || {};

Entegro.Attribute.Form = (function ($) {
    'use strict';

    const init = function () {
        const formEl = document.getElementById('productAttributeForm');
        if (!formEl) return;

        const fv = FormValidation.formValidation(formEl, {
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
                    validators: {
                        stringLength: { max: 1000, message: 'Açıklama en fazla 1000 karakter olabilir.' }
                    }
                },
                DisplayOrder: {
                    validators: {
                        notEmpty: { message: 'Gösterim sırası boş bırakılamaz.' },
                        integer: { message: 'Tam sayı olmalıdır.' },
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
                    const $form = $('#productAttributeForm');
                    const id = $('#Attribute_Id').val();
                    const isEdit = !!id;
                    const url = isEdit ? '/ProductAttribute/Edit' : '/ProductAttribute/Create';

                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (res) {
                            if (res && res.success) {
                                Swal.fire({
                                    title: isEdit ? 'Güncellendi!' : 'Başarılı!',
                                    text: isEdit ? 'Kayıt başarıyla güncellendi.' : 'Kayıt başarıyla eklendi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    $('#productAttributeModal').modal('hide');
                                    $(document).trigger('attributeTable.reload');
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: res?.message || 'İşlem sırasında hata oluştu.',
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
            }
        });

        // Yeni kayıt formu
        $(document).on('attributeForm.openCreate', function () {
            const $form = $('#productAttributeForm');
            if ($form.length) $form[0].reset();
            $('#Attribute_Id').val('');
            $('#Attribute_DisplayOrder').val(0);
            fv.resetForm(true);
            $('#productAttributeModal .modal-title-text').text('Yeni Varyant Kaydı');
            $('#productAttributeModal').modal('show');
        });

        // Güncelleme formu
        $(document).on('click', '.edit-attribute', function () {
            const id = $(this).data('id');
            if (!id) return;

            $.getJSON('/ProductAttribute/Edit', { id: id })
                .done(function (m) {
                    $('#Attribute_Id').val(m.Id);
                    $('#Attribute_Name').val(m.Name ?? '');
                    $('#Attribute_Description').val(m.Description ?? '');
                    $('#Attribute_DisplayOrder').val(m.DisplayOrder ?? 0);
                    fv.resetForm(true);
                    $('#productAttributeModal .modal-title-text').text('Varyant Güncelle');
                    $('#productAttributeModal').modal('show');
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

        // Silme
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
                            $(document).trigger('attributeTable.reload');
                        } else {
                            Swal.fire({
                                title: 'Hata!',
                                text: res?.message || 'Silme sırasında hata oluştu.',
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
    };

    return { init };
})(jQuery);
