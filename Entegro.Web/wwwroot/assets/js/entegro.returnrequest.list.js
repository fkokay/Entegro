var Entegro = Entegro || {};
Entegro.returnrequest = Entegro.returnrequest || {};

Entegro.returnrequest.list = (function ($) {
    function initList() {
        const table = $('#ReturnRequestTable').DataTable({
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
            order: [[2, 'asc']], // ID'ye göre sıralama
            ajax: {
                url: '/ReturnRequest/ReturnRequestList', // Kendi endpoint'ine göre değiştir
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Id' }, // Id
                { data: 'ProductName' }, 
                { data: 'Quantity' },
                { data: 'Customer.Name' }, 
                { data: 'OrderItemId' },
                { data: 'ReturnRequestStatus' },
                {
                    data: 'CreatedOn',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.yyyy HH:mm");
                    }
                },
                { data: 'Id' } 
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
                },
                {
                    targets: -1,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                    <div class="d-inline-block text-nowrap">
                        <a href="Details?id=${row.OrderItemId}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Detaylar">
                            <i class="icon-base ti ti-eye icon-22px"></i>
                        </a>
                        <a href="Edit?id=${row.OrderItemId}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                            <i class="icon-base ti ti-pencil icon-22px"></i>
                        </a>
                    </div>`
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
                           
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            },
            initComplete: function () {
                this.api().columns().every(function () {
                    if (this.dataSrc() === "ReturnRequestStatus") {
                        Entegro.returnrequest.list.addFilterDropdown(this, ".returnRequestStatusFilter", "Durum", [
                            { title: "Beklemede", id: 0 },
                            { title: "Onaylandı", id: 1 },
                            { title: "Reddedildi", id: 2 }
                        ]);
                    }
                });
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
    }


    return {
        init: initList
    };

})(jQuery);
