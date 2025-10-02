var Entegro = Entegro || {};
Entegro.productvariantattributevalue = Entegro.productvariantattributevalue || {};

Entegro.productvariantattributevalue.list = (function ($) {

    function initList(productVariantAttributeId) {
        const table = $('#AttributeValuesTable').DataTable({
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
                url: `/Product/ProductVariantAttributeValueList?productVariantAttributeId=${productVariantAttributeId}`,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', visible: false },   // id kolonunu gizle
                { data: 'Name' },
                { data: 'Id' }
            ],
            columnDefs: [
                {
                    targets: -1,
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                        <div class="d-inline-block text-nowrap">
                            <a 
                               class="btn btn-text-secondary rounded-pill waves-effect btn-icon btn-create-attribute-value" 
                                data-attribute-id="${row.productVariantAttributeId}"
                                data-table-id="${row.Id}"
                                data-bs-toggle="modal"
                                data-bs-target="#createOrUpdateProductVariantAttributeValueModal"
                               title="Düzenle">
                                <i class="icon-base ti ti-pencil icon-22px"></i>
                            </a>
                            <a
                                class="btn btn-text-secondary rounded-pill waves-effect btn-icon delete-record"
                                data-id="${row.Id}"
                                title="Sil">
                                <i class="icon-base ti ti-eraser icon-22px text-danger"></i>
                            </a>

                            
                        </div>`;
                    }
                }
            ],
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
                                text: `<button type="button"
                                        class="btn btn-primary btn-create-attribute-value"
                                        data-attribute-id="${productVariantAttributeId}"
                                        data-table-id="0"
                                        data-bs-toggle="modal"
                                        data-bs-target="#createOrUpdateProductVariantAttributeValueModal">
                                    <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                    <span class="d-none d-sm-inline-block">Yeni Bir Değer Ekle</span>
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

        // Class düzenlemeleri
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
            const valueId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu değer silinecek!',
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
                        url: '/Product/DeleteProductVariantAttributeValue',
                        type: 'POST',
                        data: { id: valueId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Değer başarıyla silindi.',
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

        return table;
    }

    function initCreateOrUpdateProductVariantAttributeValueModal() {
        $('#createOrUpdateProductVariantAttributeValueModal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            var attributeId = button.data('attribute-id');
            var tableId = button.data('table-id'); //tablonun id'si

            var modalBody = $('#createOrUpdateProductVariantAttributeValueModalBody');
            modalBody.html('<div class="text-center">Yükleniyor...</div>');
            if (attributeId) {
                $.ajax({
                    url: '/Product/CreateOrUpdateProductVariantAttributeValue',
                    type: 'GET',
                    data: { productVariantAttributeId: attributeId, id: tableId },
                    success: function (result) {
                        modalBody.html(result);
                    },
                    error: function () {
                        modalBody.html('<div class="text-danger text-center">Form yüklenemedi.</div>');
                    }
                });
            } else {
                modalBody.html('<div class="text-danger text-center">Geçersiz attribute ID.</div>');
            }
        });
    }
    return {
        init: initList,
        initCreateOrUpdateProductVariantAttributeValueModal: initCreateOrUpdateProductVariantAttributeValueModal
    };

})(jQuery);
