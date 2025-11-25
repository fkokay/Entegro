var Entegro = Entegro || {};
Entegro.cargo = Entegro.cargo || {};

Entegro.cargo.list = (function ($) {

    function initList() {

        var table = $('#CargoTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json'
            },
            serverSide: true,
            ajax: {
                url: '/Cargo/GetShipments',
                type: 'POST',
                contentType: 'application/json',
                data: d => JSON.stringify(d)
            },

            columns: [
                { data: 'Id' }, 
                {
                    data: 'Id',
                    orderable: false,
                    render: DataTable.render.select() 
                },
                { data: 'OrderId' },           
                { data: 'Carrier' },           
                { data: 'PackageNo' },         
                {                              
                    data: 'IsPaymentDoor',
                    render: d => d ? "Evet" : "Hayır"
                },
                {                              
                    data: 'PaymentType',
                    render: d => d ? "Alıcı" : "Gönderici"
                },
                {
                    data: null,
                    title: "İşlemler",
                    orderable: false,
                    searchable: false,
                    render: (data, type, row) =>
                        `<div class="d-inline-block text-nowrap">
                             <button class="btn btn-text-danger rounded-pill btn-icon"
                                 onclick="Entegro.cargo.list.cancelCargo('${row.OrderNumber}', ${row.ShippingIntegrationId}, ${row.Id})">
                                 <i class="ti ti-package-off icon-22px"></i>
                             </button>
                         </div>`

                }
            ],

            columnDefs: [
                {
                    targets: 0,
                    className: "control",
                    searchable: false,
                    orderable: false,
                    render: () => ""
                },
                {
                    targets: 1,
                    orderable: false,
                    searchable: false,
                    checkboxes: { selectAllRender: '<input type="checkbox" class="form-check-input">' },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],

            select: { style: "multi", selector: "td:nth-child(2)" },
            order: [[2, "desc"]],
            displayLength: 10,

            layout: {
                topStart: {
                    rowClass: "card-header d-flex flex-wrap",
                    features: [{
                        search: {
                            className: "me-5 ms-n4",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 justify-content-between",
                    features: [{
                        buttons: [
                            {
                                text: `<i class="icon-base ti ti-plus me-1"></i><span>Yeni Kargo</span>`,
                                className: "btn btn-primary me-3",
                                action: () => window.location.href = "#"
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

            adjustments.forEach(a => {
                document.querySelectorAll(a.selector).forEach(el => {
                    if (a.classToRemove) a.classToRemove.split(" ").forEach(c => el.classList.remove(c));
                    if (a.classToAdd) a.classToAdd.split(" ").forEach(c => el.classList.add(c));
                });
            });
        }, 100);

    }

   
    function cancelCargo(integrationCode, shippingIntegrationId, shipmentId) {

        Swal.fire({
            title: 'Kargoyu İptal Et',
            text: `${integrationCode} numaralı sipariş için kargo iptal edilecek. Emin misiniz?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, iptal et',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {

            if (result.isConfirmed) {

                $.ajax({
                    url: '/Cargo/CancelCargo',
                    type: 'POST',
                    data: {
                        integrationCode: integrationCode,
                        shippingIntegrationId: shippingIntegrationId,
                        shipmentId: shipmentId
                    },
                    success: function (res) {

                        Swal.fire({
                            title: 'İşlem Tamamlandı',
                            text: res.message,
                            icon: 'success',
                            confirmButtonText: 'Tamam'
                        });

                        $('#CargoTable').DataTable().ajax.reload(null, false);
                    },
                    error: function (xhr) {

                        let msg = "Kargo iptali sırasında bir sorun oluştu.";

                        if (xhr.responseJSON && xhr.responseJSON.message) {
                            msg = xhr.responseJSON.message;
                        }

                        Swal.fire({
                            title: 'Hata!',
                            text: msg,
                            icon: 'error'
                        });
                    }
                });

            }
        });
    }


    return {
        initList: initList,
        cancelCargo: cancelCargo
    };

})(jQuery);
