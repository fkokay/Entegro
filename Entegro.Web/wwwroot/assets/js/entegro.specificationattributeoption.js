var Entegro = Entegro || {};
Entegro.specificationattributeoption = Entegro.specificationattributeoption || {};

Entegro.specificationattributeoption = (function ($) {
    'use strict';

    function initSpecificationTable(attributeId) {
        if (!attributeId) return;



        const tableId = '#SpecificationAttributeOptionTable';
        const url = '/SpecificationAttribute/SpecificationAttributeValueList?attributeId=' + attributeId;


        if ($.fn.DataTable.isDataTable(tableId)) {
            $(tableId).DataTable().ajax.url(url).load();
            return
        }


        const table = $(tableId).DataTable({
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
            order: [[3, 'asc']],
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Name', title: 'Özellik Değer Adı' },
                { data: 'DisplayOrder', title: 'Sırası' },
                {
                    data: 'Id',
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                    <div class="d-inline-block text-nowrap">
                        <a href="Edit?id=#" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                            <i class="icon-base ti ti-pencil icon-22px"></i>
                        </a>
                        <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                            <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end m-0">
                            <a href="Details?id=${row.Id}" class="dropdown-item">Detaylar</a>
                            <div class="dropdown-divider"></div>
                            <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Değer Sil</a>
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
                                    {
                                        extend: "print",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Print</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`,
                                        exportOptions: { columns: [2, 3] }
                                    }
                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                <span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    const id = attributeId;

                                    $.ajax({
                                        url: `/SpecificationAttribute/SpecificationAttributeOptionCreatePopup`,
                                        type: 'GET',
                                        data: { id: id },
                                        success: function (result) {
                                            $('#SpecificationAttributeOptionPopupContent').html(result);
                                            $('#SpecificationAttributeOptionPopup').modal('show');


                                            SpecificationAttributeOptionCreatePopup(id);
                                        },
                                        error: function () {
                                            alert("Modal yüklenemedi. Lütfen tekrar deneyin.");
                                        }
                                    });
                                }
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

        // Silme işlemi
        $(document).on('click', '.delete-record', function () {
            const id = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu özellik değeri silinecek!',
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
                        data: { id: id },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Özellik değeri başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    table.ajax.reload(null, false);
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
        const fv = initializeFormValidation('specificationAttributeOptionForm', {
            'Name': {
                validators: {
                    notEmpty: { message: 'Adı alanı boş bırakılamaz.' },
                    stringLength: { min: 3, message: 'Adı alanı en az 3 karakter olmalıdır.' }
                }
            },
            'DisplayOrder': {
                validators: {
                    notEmpty: { message: 'Sıra alanı boş bırakılamaz.' },
                    integer: { message: 'Sıra alanı geçerli bir sayı olmalıdır.' }
                }
            }
        });
        $(document).on('click', '#btnSaveSpecificationAttributeOption', function () {
            fv.validate().then(function (status) {
                if (status === 'Valid') {
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
                }
            });
        });
    }

    function initializeFormValidation(formId, fieldValidators, onValidCallback) {
        const form = document.getElementById(formId);

        if (!form) return;

        const fv = FormValidation.formValidation(
            form,
            {
                locale: 'tr_TR',
                localization: FormValidation.locales.tr_TR,
                fields: fieldValidators,
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
                    if (onValidCallback) {
                        instance.on('core.form.valid', onValidCallback);
                    }
                }
            }
        );

        return fv;
    }
   
    return {
        initTable: initSpecificationTable,
        SpecificationAttributeOptionCreatePopup: SpecificationAttributeOptionCreatePopup,
        initializeFormValidation: initializeFormValidation
    };

})(jQuery);
