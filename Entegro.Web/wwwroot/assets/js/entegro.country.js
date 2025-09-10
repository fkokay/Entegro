var Entegro = Entegro || {};

Entegro.country = (function ($) {
    'use strict';

    function initValidation(formSelector, options) {
        const form = document.querySelector(formSelector);
        if (!form) return;

       
        const defaults = {
            url: '',            
            successRedirect: '', 
            successMessage: 'İşlem başarılı.', 
            errorMessage: 'Bir hata oluştu.'  
        };

        const settings = Object.assign({}, defaults, options);

        FormValidation.formValidation(form, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                'Name': {
                    validators: {
                        notEmpty: {
                            message: 'Ülke adı boş bırakılamaz.'
                        },
                        stringLength: {
                            min: 3,
                            message: 'Ülke adı en az 3 karakter olmalıdır.'
                        }
                    }
                },
                'DisplayOrder': {
                    validators: {
                        notEmpty: {
                            message: 'Sıra boş bırakılamaz.'
                        },
                        numeric: {
                            message: 'Sıra sadece sayı olmalıdır.'
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
            init: function (instance) {
                instance.on('plugins.message.placed', function (e) {
                    if (e.element.parentElement.classList.contains('input-group')) {
                        e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
                    }
                });

                instance.on('core.form.valid', function () {
                    const $form = $(form);
                    const serializedData = $form.serialize();

                    $.ajax({
                        url: settings.url,
                        type: 'POST',
                        data: serializedData,
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    title: 'Başarılı!',
                                    text: settings.successMessage,
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    if (settings.successRedirect) {
                                        window.location.href = settings.successRedirect;
                                    }
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: response.message || settings.errorMessage,
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
                                text: xhr.responseText || 'Sunucu ile bağlantı sırasında bir hata oluştu.',
                                icon: 'error',
                                confirmButtonText: 'Tamam',
                                customClass: {
                                    confirmButton: 'btn btn-danger'
                                },
                                buttonsStyling: false
                            });
                        }
                    });
                });
            }
        });
    }

    function initDelete(deleteButtonSelector, deleteUrl, redirectUrl, countryId) {
        $(deleteButtonSelector).on('click', function () {
            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu ülke silinecek!",
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
                        url: deleteUrl,
                        type: 'POST',
                        data: { id: countryId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Ülke başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    if (redirectUrl) {
                                        window.location.href = redirectUrl;
                                    }
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-danger'
                                    },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hata!',
                                text: 'Sunucu ile bağlantı kurulamadı.',
                                confirmButtonText: 'Tamam',
                                customClass: {
                                    confirmButton: 'btn btn-danger'
                                },
                                buttonsStyling: false
                            });
                        }
                    });
                }
            });
        });
    }

    function initIsPublishedCheckbox() {
        document.querySelector('form').addEventListener('submit', function (e) {
            var isChecked = document.querySelector('input[name="IsPublished"]').checked;
            // İsteğe bağlı olarak burada isChecked değerine göre işlem yapabilirsiniz
            console.log("Yayın Durumu:", isChecked);
        });
    }

    function initTownModal(modalTriggerSelector, hiddenInputSelector) {
        $(modalTriggerSelector).on('click', function () {
            var cityId = $(this).data('city-id');
            $(hiddenInputSelector).val(cityId);
        });
    }

    return {
        initValidation: initValidation,
        initDelete: initDelete,
        initIsPublishedCheckbox: initIsPublishedCheckbox,
        initTownModal: initTownModal
    };
})(jQuery);
