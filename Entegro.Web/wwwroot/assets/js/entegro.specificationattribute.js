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

    function initUpdateFormValidation() {
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
    function bindEvents() {
        $(document).on('click', '.btn-edit-specificationAttribute', function () {
            const id = $(this).data('id');
            SpecificationAttributeCreatePopup(id);
        });
    }

    function init() {
        bindEvents();
    }

    return {
        init,
        SpecificationAttributeCreatePopup: SpecificationAttributeCreatePopup,
        Validation: Validation,
        initUpdateFormValidation: initUpdateFormValidation
    };
})(jQuery);
