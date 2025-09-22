var Entegro = Entegro || {};
Entegro.Attribute = Entegro.Attribute || {};

Entegro.Attribute = (function ($) {
    'use strict';

    const create = function () {
        $($("#CreateOrUpdateModal").find(".title")).text("Varyant Ekle");

        $.ajax({
            url: '/ProductAttribute/CreateOrUpdate?id=0',
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
        $($("#CreateOrUpdateModal").find(".title")).text("Varyant Düzenle");

        $.ajax({
            url: '/ProductAttribute/CreateOrUpdate?id=' + id,
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
        const productAttributeForm = document.getElementById('ProductAttributeForm');
        if (!productAttributeForm) {
            console.error("ProductAttributeForm isimli form bulunmadı.");
            return;
        }

        window.createFormValidation = FormValidation.formValidation(productAttributeForm, {
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
                    const $form = $('#ProductAttributeForm');
                    $.ajax({
                        url: '/ProductAttribute/CreateOrUpdate',
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
            $(document).on('click', '.edit-attribute', function () {
                const id = $(this).data('id');
                if (!id) return;

                const $editForm = $('#ProductAttributeForm');
                if ($editForm.length)
                    $editForm[0].reset();
                if (window.editFormValidation)
                    window.editFormValidation.resetForm(true);


                edit(id);
            });

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
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Özellik başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    window.location.href = '/ProductAttribute/list';
                                });
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
        init: init
    };

})(jQuery);



