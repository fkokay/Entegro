var Entegro = Entegro || {};
Entegro.specificationattribute = (function ($) {
    
    function showMessage(title, message, type = "info", redirectUrl = null, reload = null) {
        Swal.fire({
            title: title,
            text: message,
            icon: type, // success | error | warning | info | question
            confirmButtonText: 'Tamam',
            customClass: { confirmButton: 'btn btn-primary' },
            buttonsStyling: false
        }).then(() => {
            if (redirectUrl) {
                window.location.href = redirectUrl;
            }

            if (reload) {
                location.reload();
            }
        });
    }

    function SpecificationAttributeCreatePopup() {
        var popup = $('#SpecificationAttributePopup');
        var popupContent = $("#SpecificationAttributePopupContent");
        $.ajax({
            url: '/SpecificationAttribute/SpecificationAttributeCreatePopup',
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                $(popupContent).html(html);
                SpecificationAttributeCreatePopupInit(popup, popupContent);

                $(popup).modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    }
    function SpecificationAttributeCreatePopupInit(popup, popupContent) {

        popup.off('click', '#btnSaveSpecificationAttribute'); 
        popup.on('click', '#btnSaveSpecificationAttribute', function () {

            const payload = {
                name: $('#Name').val() || '',
            };

            $.ajax({
                url: '/SpecificationAttribute/SpecificationAttributeCreate',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(payload),
                success: function (json) {
                    if (json?.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı!',
                            text: 'Özellik başarıyla kaydedildi.',
                            confirmButtonText: 'Tamam'
                        }).then(() => {
                            popup.modal('hide');
                            location.reload();
                        });
                    } else {
                        showMessage("Hata!", json?.errors?.join('\n') || 'Kayıt başarısız.', "error");
                    }
                },
                error: function () {
                    showMessage("Sunucu Hatası!", "İşlem sırasında bir hata oluştu.", "error");
                }
            });
        });
    }

    function Validation() {
        const formValidation = FormValidation.formValidation(
            document.getElementById('specificationAttributeForm'),
            {
                locale: 'tr_TR',
                localization: FormValidation.locales.tr_TR,
                fields: {
                    'Name': {
                        validators: {
                            notEmpty: { message: 'Ürün adı boş bırakılamaz.' },
                            stringLength: { min: 3, message: 'Ürün adı en az 3 karakter olmalıdır.' }
                        }
                    },
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
                    instance.on('plugins.message.placed', function (e) {
                        if (e.element.parentElement.classList.contains('input-group')) {
                            e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
                        }
                    });

                    instance.on('core.form.valid', function () {
                        const form = $('#specificationAttributeForm');
                        const action = form.attr("action");
                        const formData = form.serialize();

                        $.ajax({
                            url: action,
                            type: 'POST',
                            data: formData,
                            success: function (response) {
                                if (response.success) {
                                    showMessage("Başarılı!", 'Ürün başarıyla kaydedildi.', "success", "/Product/List");
                                } else {
                                    showMessage("Hata!", response.message || 'Bir hata oluştu.', "error");
                                }
                            },
                            error: function (xhr) {
                                showMessage("Hata!", xhr.responseText || 'İşlem sırasında bir hata oluştu.', "error");
                            }
                        });
                    });
                }
            }
        );
    }

    function SpecificationAttributeUpdateFormValidationInit() {
        const form = document.getElementById('specificationattribute-form');

        const fv = FormValidation.formValidation(
            form,
            {
                locale: 'tr_TR',
                localization: FormValidation.locales.tr_TR,
                fields: {
                    'Name': {
                        validators: {
                            notEmpty: { message: 'Ürün adı boş bırakılamaz.' },
                            stringLength: { min: 3, message: 'Ürün adı en az 3 karakter olmalıdır.' }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap5: new FormValidation.plugins.Bootstrap5({
                        eleValidClass: '',
                        rowSelector: '.col-md-12'
                    }),
                    submitButton: new FormValidation.plugins.SubmitButton(),
                    autoFocus: new FormValidation.plugins.AutoFocus()
                },
                init: (instance) => {
                    instance.on('core.form.valid', function () {

                        const $form = $(form);
                        const url = $form.attr('action');
                        const formData = $form.serialize();

                        $.ajax({
                            url: url,
                            method: 'POST',
                            data: formData,
                            success: function (response) {
                                if (response.success) {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Başarılı!',
                                        text: 'Ürün başarıyla güncellendi.',
                                        confirmButtonText: 'Tamam'
                                    }).then(() => {
                                        window.location.href = '/SpecificationAttribute/List'; // 
                                    });
                                } else {
                                    Swal.fire('Hata!', response.message || 'Güncelleme başarısız.', 'error');
                                }
                            },
                            error: function (xhr) {
                                Swal.fire('Hata!', xhr.responseText || 'Sunucu hatası oluştu.', 'error');
                            }
                        });
                    });
                }
            }
        );
    }
   
    function SpecificationAttributeOptionCreatePopup(id) {
        var popup = $('#SpecificationAttributeOptionPopup');
        var popupContent = $("#SpecificationAttributeOptionPopupContent");
        $.ajax({
            url: '/SpecificationAttribute/SpecificationAttributeOptionCreatePopup?id=' + id,
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                $(popupContent).html(html);

                SpecificationAttributeOptionCreatePopupInit(popup, popupContent);

                $(popup).modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    }

    function SpecificationAttributeOptionCreatePopupInit(popup, popupContent) {
       
        $(document).on('click', '#btnSaveSpecificationAttributeOption', function () {
            const payload = {
                SpecificationAttributeId: Number($('#SpecificationAttributeId').val()) || 0,
                Name: $('#OptionName').val() || '',
                DisplayOrder: Number($('#DisplayOrder').val()) || 0
            };

            $.ajax({
                url: '/SpecificationAttribute/SpecificationAttributeOptionCreate',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(payload),
                success: function (json) {
                    if (json?.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı!',
                            text: 'Özellik başarıyla kaydedildi.',
                            confirmButtonText: 'Tamam'
                        }).then(() => {
                            location.reload();
                            $(popup).modal('hide');
                        });
                    } else {
                        showMessage("Hata!", json?.errors?.join('\n') || 'Kayıt başarısız.', "error")
                    }
                },
                error: function () {
                    showMessage("Sunucu Hatası!", "İşlem sırasında bir hata oluştu.", "error")
                }
            });
        });
    }
    function init() {
        $(document).on('click', '.btn-deleteOption', function () {
            const id = $(this).data('mapping-id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu özellik silinecek!',
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
                        url: '/SpecificationAttribute/SpecificationAttributeOptionDelete',
                        type: 'POST',
                        data: { id },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Özellik başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    location.reload();
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
        init: init,
        SpecificationAttributeCreatePopup: SpecificationAttributeCreatePopup,
        SpecificationAttributeOptionCreatePopup: SpecificationAttributeOptionCreatePopup,
        Validation: Validation,
        SpecificationAttributeUpdateFormValidationInit: SpecificationAttributeUpdateFormValidationInit
    };
})(jQuery);
