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
                        url: '/Customer/DeleteCustomerAddress',
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
    function initCreateOrUpdateCustomerAddressModal() {
        $('#createAddressModal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            var customerId = button.data('customer-id');
            var addressId = button.data('address-id');

            var modalBody = $('#createAddressModalBody');
            modalBody.html('<div class="text-center">Yükleniyor...</div>');

            if (customerId) {
                $.ajax({
                    url: '/Customer/CreateOrUpdateCustomerAddressMapping',
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

    function initCustomerAddressTable(customerId) {
        if (!customerId) return;

        if ($.fn.DataTable.isDataTable('#CustomerAddressTable')) {
            $('#CustomerAddressTable').DataTable().ajax.url('/Customer/CustomerAddressList?customerId=' + customerId).load();
            return;
        }

        const table = $('#CustomerAddressTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            order: [[2, 'asc']],
            ajax: {
                url: '/Customer/CustomerAddressList?customerId=' + customerId,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Title' }, // Adres Başlığı
                {
                    data: 'Id',
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                    <div class="d-inline-block text-nowrap">
                        <button type="button"
                                class="btn btn-text-secondary rounded-pill waves-effect btn-icon btn-create-address"
                                data-customer-id="${customerId}"
                                data-address-id="${row.Id}"
                                data-bs-toggle="modal"
                                data-bs-target="#createAddressModal"
                                title="Güncelle">
                            <i class="icon-base ti ti-pencil icon-22px"></i>
                        </button>
                        <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                            <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end m-0">
                            <button type="button"
                                    class="dropdown-item text-danger btn-delete-address"
                                    data-id="${row.Id}"
                                    data-customer-id="${customerId}">
                                Sil
                            </button>
                        </div>
                    </div>`
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: {
                style: "multi",
                selector: "td:nth-child(1)"
            },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                        buttons: [
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1">
                                    <i class="icon-base ti ti-upload icon-xs"></i>
                                    <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                                  </span>`,
                                buttons: [
                                    { extend: "print", className: "dropdown-item", text: "Print", exportOptions: { columns: [2] } },
                                    { extend: "csv", className: "dropdown-item", text: "CSV", exportOptions: { columns: [2] } },
                                    { extend: "excel", className: "dropdown-item", text: "Excel", exportOptions: { columns: [2] } },
                                    { extend: "pdf", className: "dropdown-item", text: "PDF", exportOptions: { columns: [2] } },
                                    { extend: "copy", className: "dropdown-item", text: "Copy", exportOptions: { columns: [2] } }
                                ]
                            },
                            {
                                text: `<button type="button"
                                        class="btn btn-primary btn-create-address"
                                        data-customer-id="${customerId}"
                                        data-address-id="0"
                                        data-bs-toggle="modal"
                                        data-bs-target="#createAddressModal">
                                        <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                        <span class="d-none d-sm-inline-block">Yeni Adres Oluştur</span>
                                    </button>`,
                                className: "p-0 border-0 bg-transparent"
                            }
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            }
        });

        // Görsel sınıf düzeltmeleri
        setTimeout(() => {
            const adjustments = [
                { selector: ".dt-buttons .btn", classToRemove: "btn-secondary" },
                { selector: ".dt-buttons.btn-group", classToAdd: "mb-md-0 mb-6" },
                { selector: ".dt-search .form-control", classToRemove: "form-control-sm", classToAdd: "ms-0" },
                { selector: ".dt-search", classToAdd: "mb-0 mb-md-6" },
                { selector: ".dt-length .form-select", classToRemove: "form-select-sm" },
                { selector: ".dt-layout-end", classToAdd: "gap-md-2 gap-0 mt-0" },
                { selector: ".dt-layout-start", classToAdd: "mt-0" },
                { selector: ".dt-layout-table", classToRemove: "row mt-2" },
                { selector: ".dt-layout-full", classToRemove: "col-md col-12", classToAdd: "table-responsive" }
            ];
            adjustments.forEach(({ selector, classToRemove, classToAdd }) => {
                document.querySelectorAll(selector).forEach(el => {
                    if (classToRemove) classToRemove.split(" ").forEach(cls => el.classList.remove(cls));
                    if (classToAdd) classToAdd.split(" ").forEach(cls => el.classList.add(cls));
                });
            });
        }, 100);

        return table;
    }

    function initCreateOrUpdateCustomerAddressForm() {
        $(document).off('submit.createAddressForm')
            .on('submit.createAddressForm', '#createAddressForm', function (e) {
                e.preventDefault();

                const $form = $(this);

                if ($form.valid && !$form.valid()) return;

                if ($form.data('submitting')) return;

                $form.data('submitting', true);
                const $buttons = $form.find('button[type="submit"], input[type="submit"]');
                $buttons.prop('disabled', true);

                $.ajax({
                    url: $form.attr('action'),
                    type: 'POST',
                    data: $form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            $('#createAddressModal').modal('hide');
                            $form[0].reset();
                            $('#CustomerAddressTable').DataTable().ajax.reload(null, false);
                            Swal.fire({
                                icon: 'success',
                                title: 'Başarılı',
                                text: 'Adres kaydedildi.',
                                timer: 2000,
                                showConfirmButton: false
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hata',
                                text: response.message || 'Bilinmeyen bir hata oluştu.',
                                confirmButtonText: 'Tamam'
                            });
                        }
                    },
                    error: function (xhr) {
                        console.error(xhr.responseText);
                        Swal.fire({
                            icon: 'error',
                            title: 'Sunucu Hatası',
                            text: 'Adres kaydedilirken bir hata oluştu.'
                        });
                    },
                    complete: function () {
                        $form.data('submitting', false);
                        $buttons.prop('disabled', false);
                    }
                });
            });
    }


    return {
        initCreateForm: initCreateForm,
        initUpdateForm: initUpdateForm,
        initDeleteCustomer: initDeleteCustomer,
        initDeleteAddress: initDeleteAddress,
        initCreateOrUpdateCustomerAddressModal: initCreateOrUpdateCustomerAddressModal,
        initCustomerAddressTable: initCustomerAddressTable,
        initCreateOrUpdateCustomerAddressForm: initCreateOrUpdateCustomerAddressForm

    };

})(jQuery);
