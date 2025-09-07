var Entegro = Entegro || {};
Entegro.AttributeValue = (function ($) {
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

    function initPASelect() {
        const $el = $('#ProductAttributeId');
        if (!$el.data('select2')) {
            $el.select2({
                placeholder: 'Varyant adı seçin...',
                allowClear: true,
                width: '100%'
            });
        }
    }
    
    function initValidation(formSelector, postUrl, successMessage, redirectUrl) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        initPASelect();

        const fv = FormValidation.formValidation(form, {
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
                        notEmpty: { message: 'Değer adı boş bırakılamaz.' },
                        stringLength: {
                            min: 1,
                            max: 100,
                            message: 'Değer adı 1–100 karakter olmalıdır.'
                        }
                    }
                },
                DisplayOrder: {
                    validators: {
                        notEmpty: { message: 'Gösterim sırası boş bırakılamaz.' },
                        integer: { message: 'Tam sayı olmalıdır.' },
                        between: {
                            min: 0,
                            max: 1000,
                            message: 'Gösterim sırası 0 ile 1000 arasında olmalıdır.'
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
            options.formSelector || '#attributeValueForm',
            options.postUrl || '/ProductAttributeValue/Create',
            'Varyant değeri başarıyla eklendi.',
            options.redirectUrl || '/ProductAttributeValue/List'
        );
    }

    function initUpdateForm(options) {
        initValidation(
            options.formSelector || '#attributeValueForm',
            options.postUrl || '/ProductAttributeValue/Edit',
            'Varyant değeri başarıyla güncellendi.',
            options.redirectUrl || '/ProductAttributeValue/List'
        );
    }

    return {
        initCreateForm,
        initUpdateForm,
    };
})(jQuery);
