var Entegro = Entegro || {};
Entegro.User = (function ($) {
    'use strict';


    function initValidation(formSelector, postUrl, successMessage, redirectUrl) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        const fv = FormValidation.formValidation(form, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                'FirstName': {
                    validators: {
                        notEmpty: { message: 'Ad boş bırakılamaz.' },
                        stringLength: {
                            min: 2,
                            max: 50,
                            message: 'Ad 2-50 karakter arasında olmalıdır.'
                        }
                    }
                },
                'LastName': {
                    validators: {
                        notEmpty: { message: 'Soyadı boş bırakılamaz.' },
                        stringLength: {
                            min: 2,
                            max: 50,
                            message: 'Soyadı 2-50 karakter arasında olmalıdır.'
                        }
                    }
                },
                'Email': {
                    validators: {
                        notEmpty: { message: 'Email adresi boş bırakılamaz.' },
                        emailAddress: { message: 'Geçerli bir email adresi giriniz.' }
                    }
                },
                'Password': {
                    validators: {
                        notEmpty: { message: 'Şifre boş bırakılamaz.' },
                        stringLength: {
                            min: 6,
                            message: 'Şifre en az 6 karakter olmalıdır.'
                        }
                    }
                },
                'PhoneNumber': {
                    validators: {
                        notEmpty: { message: 'Telefon numarası boş bırakılamaz.' },
                        phone: {
                            country: 'TR',
                            message: 'Geçerli bir telefon numarası giriniz.'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap5: new FormValidation.plugins.Bootstrap5({
                    eleValidClass: '',
                    rowSelector: '.mb-3'
                }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                autoFocus: new FormValidation.plugins.AutoFocus()
            },
            init: (instance) => {
                instance.on('core.form.valid', function () {
                    const $form = $(formSelector);
                    const serializedData = $form.serialize();

                    $.ajax({
                        url: postUrl,
                        type: 'POST',
                        data: serializedData,
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    title: 'Başarılı!',
                                    text: successMessage,
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    if (redirectUrl) window.location.href = redirectUrl;
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: response.message || 'Bir hata oluştu.',
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

                // Hatalı alan odaklanma
                instance.on('core.field.invalid', function (e) {
                    const fieldEl = e.elements && e.elements.length ? e.elements[0] : null;
                    if (fieldEl) focusFieldAndShowTab(fieldEl);
                });
            }
        });
    }
   
    function initCreateForm(options) {
        initValidation(
            options.formSelector || '#user-form',
            options.postUrl || '/User/Create',
            'Kullanıcı başarıyla oluşturuldu.',
            options.redirectUrl || '/User/List'
        );
    }

    function initUpdateForm(options) {
        initValidation(
            options.formSelector || '#user-form',
            options.postUrl || '/User/Edit',
            'Kullanıcı başarıyla güncellendi.',
            options.redirectUrl || '/User/List'
        );
    }

    // Kullanıcıyı silme işlemi
    function initDeleteUser(options) {
        const deleteButton = document.querySelector('#btnDeleteUser');
        if (!deleteButton) return;

        deleteButton.addEventListener('click', function () {
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu kullanıcı silinecek.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'Hayır, iptal et',
                customClass: {
                    confirmButton: 'btn btn-danger me-2',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: options.deleteUrl || '/User/Delete',
                        type: 'POST',
                        data: { id: options.id || '@Model.Id' },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    title: 'Silindi!',
                                    text: 'Kullanıcı başarıyla silindi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    window.location.href = options.redirectUrl || '/User/List';
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    icon: 'error',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-danger'
                                    },
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
                }
            });
        });
    }

    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm,
        initDeleteUser: initDeleteUser
    };

})(jQuery);
