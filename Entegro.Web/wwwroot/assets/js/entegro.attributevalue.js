var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue = (function ($) {
    'use strict';
    
    const create = function () {
        $($("#CreateOrUpdateModal").find(".title")).text("Varyant Değeri Ekle");

        $.ajax({
            url: '/ProductAttributeValue/CreateOrUpdate?id=0',
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                $('#CreateOrUpdateModalContent').html(html);
                validation();
                
                $('#CreateOrUpdateModal').modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    };
    const edit = function (id) {
        $($("#CreateOrUpdateModal").find(".title")).text("Varyant Değeri Düzenle");

        $.ajax({
            url: '/ProductAttributeValue/CreateOrUpdate?id=' + id,
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                $('#CreateOrUpdateModalContent').html(html);
                validation();
                
                $('#CreateOrUpdateModal').modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    };
    const validation = function () {
        const productAttributeValueForm = document.getElementById('ProductAttributeValueForm');
        if (!productAttributeValueForm) {
            console.error("ProductAttributeValueForm isimli form bulunmadı.");
            return;
        }

        window.createFormValidation = FormValidation.formValidation(productAttributeValueForm, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                ProductAttributeId: {
                    validators: {
                        notEmpty: { message: 'Varyant adı seçilmelidir.' }
                    }
                },
                Name: {
                    validators: {
                        notEmpty: { message: 'Ad alanı boş bırakılamaz.' },
                        stringLength: { min: 3, max: 100, message: 'Ad 3–100 karakter olmalıdır.' }
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
                    const $form = $('#ProductAttributeValueForm');
                    $.ajax({
                        url: '/ProductAttributeValue/CreateOrUpdate',
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (res) {
                            if (res && res.success) {
                                $('#CreateOrUpdateModal').modal('hide');
                                Swal.fire({
                                    title: 'Başarılı!',
                                    text: 'Kayıt başarıyla eklendi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    location.reload();
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
    };
    const init = function () {
        $(function () {
            $(document).on('click', '.edit-attributeValue', function () {
                const id = $(this).data('id');
                if (!id) return;

                const $editForm = $('#ProductAttributeValueForm');
                if ($editForm.length)
                    $editForm[0].reset();
                if (window.editFormValidation)
                    window.editFormValidation.resetForm(true);


                edit(id);
            });

            // DELETE
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

    return {
        create: create,
        init: init };
})(jQuery);


