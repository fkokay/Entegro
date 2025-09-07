var Entegro = Entegro || {};
Entegro.AttributeForm = (function ($) {
    'use strict';

    function focusFieldAndShowTab(el) {
        if (!el) return;
        const tabPane = el.closest('.tab-pane');
        if (tabPane) {
            const tabId = tabPane.id;
            const trigger = document.querySelector(`*[data-bs-target="#${tabId}"]`);
            if (trigger) new bootstrap.Tab(trigger).show();
        }
        setTimeout(() => {
            try { el.focus({ preventScroll: true }); } catch (_) { }
            el.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }, 50);
    }

    function initValidation(formSelector, postUrl, successMessage, redirectUrl) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        const fv = FormValidation.formValidation(form, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                'Name': {
                    validators: {
                        notEmpty: { message: 'Ad alanı boş bırakılamaz.' },
                        stringLength: {
                            min: 3,
                            max: 100,
                            message: 'Ad 3-100 karakter arasında olmalıdır.'
                        }
                    }
                },
                'DisplayOrder': {
                    validators: {
                        notEmpty: { message: 'Sıralama boş bırakılamaz.' },
                        integer: { message: 'Geçerli bir sayı giriniz.' },
                        between: {
                            min: 0,
                            max: 1000,
                            message: 'Sıralama 0 ile 1000 arasında olmalıdır.'
                        }
                    }
                },
                'Description': {
                    validators: {
                        stringLength: {
                            max: 500,
                            message: 'Açıklama en fazla 500 karakter olabilir.'
                        }
                    }
                }
                // Gerekirse "Values" alanı buraya eklenebilir.
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

                instance.on('core.field.invalid', function (e) {
                    const fieldEl = e.elements && e.elements.length ? e.elements[0] : null;
                    if (fieldEl) focusFieldAndShowTab(fieldEl);
                });

                instance.on('core.form.invalid', function () {
                    const invalidEl = document.querySelector('[data-field].is-invalid, .is-invalid');
                    if (invalidEl) focusFieldAndShowTab(invalidEl);
                });

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
            }
        });
    }

    function initCreateForm(options) {
        initValidation(
            options.formSelector || '#attribute-form',
            options.postUrl || '/ProductAttribute/Create',
            'Özellik başarıyla oluşturuldu.',
            options.redirectUrl || '/ProductAttribute/List'
        );
    }

    function initUpdateForm(options) {
        initValidation(
            options.formSelector || '#attribute-form',
            options.postUrl || '/ProductAttribute/Edit',
            'Özellik başarıyla güncellendi.',
            options.redirectUrl || '/ProductAttribute/List'
        );
    }

    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm
    };

})(jQuery);
