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
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Name' },
                { data: 'Id' }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                {
                    targets: -1,
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <div class="d-inline-block text-nowrap">
                                <a href="Edit?id=${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                                    <i class="icon-base ti ti-pencil icon-22px"></i>
                                </a>
                                <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                    <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                                </button>
                                <div class="dropdown-menu dropdown-menu-end m-0">
                                    <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Deðer Sil</a>
                                </div>
                            </div>`;
                    }
                }
            ],
            select: {
                style: "multi",
                selector: "td:nth-child(1)"
            },
            displayLength: 10
        });

        // Silme iþlemi
        $(document).on('click', '.delete-record', function () {
            const valueId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu deðer silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'Ýptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/ProductVariantAttributeValue/Delete',
                        type: 'POST',
                        data: { id: valueId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Deðer baþarýyla silindi.',
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
                                    text: response.message || 'Silme iþlemi baþarýsýz oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Sunucu Hatasý!',
                                text: 'Ýstek gönderilirken bir hata oluþtu.',
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
        init: initList
    };

})(jQuery);
