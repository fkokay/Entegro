var Entegro = Entegro || {};
Entegro.customer = (function ($) {
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
                        notEmpty: { message: 'Ad / Şirket adı boş bırakılamaz.' },
                        stringLength: {
                            min: 3,
                            max: 100,
                            message: 'Ad / Şirket adı 3-100 karakter arasında olmalıdır.'
                        }
                    }
                },
                'Email': {
                    validators: {
                        notEmpty: { message: 'Email adresi boş bırakılamaz.' },
                        emailAddress: { message: 'Geçerli bir email adresi giriniz.' }
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
                },
                'City': { validators: { notEmpty: { message: 'Şehir boş bırakılamaz.' } } },
                'District': { validators: { notEmpty: { message: 'İlçe boş bırakılamaz.' } } },
                'Address': {
                    validators: {
                        notEmpty: { message: 'Adres bilgisi boş bırakılamaz.' },
                        stringLength: { max: 500, message: 'Adres en fazla 500 karakter olabilir.' }
                    }
                },
                'CustomerType': {
                    validators: {
                        notEmpty: { message: 'Müşteri tipi seçilmelidir.' },
                        integer: { message: 'Müşteri tipi geçersiz.' }
                    }
                },
                'TaxOffice': {
                    validators: {
                        callback: {
                            message: 'Kurumsal müşteriler için vergi dairesi zorunludur.',
                            callback: function (input) {
                                const type = parseInt(form.querySelector('[name="CustomerType"]').value);
                                return type === 1 ? input.value.trim().length > 0 : true;
                            }
                        }
                    }
                },
                'TaxNumber': {
                    validators: {
                        callback: {
                            message: 'Kurumsal müşteriler için vergi numarası zorunludur.',
                            callback: function (input) {
                                const type = parseInt(form.querySelector('[name="CustomerType"]').value);
                                return type === 1 ? input.value.trim().length > 0 : true;
                            }
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
            options.formSelector || '#customer-form',
            options.postUrl || '/Customer/Create',
            'Müşteri başarıyla oluşturuldu.',
            options.redirectUrl || '/Customer/List'
        );
    }

    function initUpdateForm(options) {
        initValidation(
            options.formSelector || '#customer-form',
            options.postUrl || '/Customer/Edit',
            'Müşteri başarıyla güncellendi.',
            options.redirectUrl || '/Customer/List'
        );
    }
   


    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm
    };

})(jQuery);
