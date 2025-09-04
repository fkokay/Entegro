var Entegro = Entegro || {};
Entegro.emailaccount = (function ($) {
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
                'Email': {
                    validators: {
                        notEmpty: { message: 'Email adresi boş bırakılamaz.' },
                        emailAddress: { message: 'Geçerli bir email adresi giriniz.' }
                    }
                },
                'DisplayName': {
                    validators: {
                        notEmpty: { message: 'Görünen ad boş bırakılamaz.' },
                        stringLength: {
                            min: 3,
                            max: 100,
                            message: 'Görünen ad 3-100 karakter arasında olmalıdır.'
                        }
                    }
                },
                'Host': { validators: { notEmpty: { message: 'Sunucu (Host) boş bırakılamaz.' } } },
                'Port': {
                    validators: {
                        notEmpty: { message: 'Port numarası boş bırakılamaz.' },
                        integer: { message: 'Port geçerli bir tam sayı olmalıdır.' },
                        between: {
                            min: 1,
                            max: 65535,
                            message: 'Port 1 ile 65535 arasında olmalıdır.'
                        }
                    }
                },
                'Username': { validators: { notEmpty: { message: 'Kullanıcı adı boş bırakılamaz.' } } },
                'Password': {
                    validators: {
                        notEmpty: { message: 'Şifre boş bırakılamaz.' },
                        stringLength: { min: 6, message: 'Şifre en az 6 karakter olmalıdır.' }
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
            options.formSelector || '#emailaccount-form',
            options.postUrl || '/EmailAccount/Create',
            'Email hesabı başarıyla oluşturuldu.',
            options.redirectUrl || '/EmailAccount/List'
        );
    }


    function initUpdateForm(options) {
        initValidation(
            options.formSelector || '#emailaccount-form',
            options.postUrl || '/EmailAccount/Edit',
            'Email hesabı başarıyla güncellendi.',
            options.redirectUrl || '/EmailAccount/List'
        );
    }

    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm
    };

})(jQuery);
