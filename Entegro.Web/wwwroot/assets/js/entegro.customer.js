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
   
    function initDeleteCustomer(options) {
        const deleteButton = document.querySelector('#btnDeleteCustomer');
        if (!deleteButton) return;

        deleteButton.addEventListener('click', function () {
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu müşteri silinecek.',
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
                        url: options.deleteUrl || '/Customer/Delete',
                        type: 'POST',
                        data: { id: options.id || '@Model.Id' },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    title: 'Silindi!',
                                    text: 'Müşteri başarıyla silindi.',
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

    function loadCustomerAddresses(customerId, targetSelector) {
        if (!customerId || customerId <= 0) return;

        const $target = $(targetSelector);
        if ($target.length === 0) return;

        $target.html('<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Yükleniyor...</span></div>');

        $.ajax({
            url: '/Customer/GetCustomerAddress', 
            type: 'GET',
            data: { customerId: customerId },
            success: function (html) {
                $target.html(html);
            },
            error: function () {
                $target.html('<div class="alert alert-danger">Adresler yüklenirken bir hata oluştu.</div>');
            }
        });
    }
    function initDeleteAddress() {
        $(document).on('click', '.btn-delete-address', function () {
            var button = $(this);
            var addressId = button.data('id');
            var customerId = button.data('customer-id');

            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu adres kalıcı olarak silinecek!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Customer/DeleteAddress',
                        type: 'POST',
                        data: {
                            customerId: customerId,
                            addressId: addressId
                        },
                        success: function (response) {
                            if (response.success) {
                                button.closest('tr').remove();
                                Swal.fire(
                                    'Silindi!',
                                    'Adres başarıyla silindi.',
                                    'success'
                                );
                            } else {
                                Swal.fire(
                                    'Hata!',
                                    response.message || 'Adres silinirken bir hata oluştu.',
                                    'error'
                                );
                            }
                        },
                        error: function () {
                            Swal.fire(
                                'Hata!',
                                'Sunucuya bağlanırken hata oluştu.',
                                'error'
                            );
                        }
                    });
                }
            });
        });
    }
    function initCreateAddressModal() {
        $('#createAddressModal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            var customerId = button.data('customer-id');
            var addressId = button.data('address-id');

            var modalBody = $('#createAddressModalBody');
            modalBody.html('<div class="text-center">Yükleniyor...</div>');

            if (customerId) {
                $.ajax({
                    url: '/Customer/CreateCustomerAddressMapping',
                    type: 'GET',
                    data: { customerId: customerId,addressId:addressId },
                    success: function (result) {
                        modalBody.html(result);
                    },
                    error: function () {
                        modalBody.html('<div class="text-danger text-center">Adres formu yüklenemedi.</div>');
                    }
                });
            } else {
                modalBody.html('<div class="text-danger text-center">Geçersiz müşteri ID.</div>');
            }
        });
    }

    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm,
        initDeleteCustomer: initDeleteCustomer,
        loadCustomerAddresses: loadCustomerAddresses,
        initDeleteAddress: initDeleteAddress,
        initCreateAddressModal: initCreateAddressModal
    };

})(jQuery);
