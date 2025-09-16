var Entegro = Entegro || {};
Entegro.order = Entegro.order || {};


Entegro.order.OrderList = (function ($) {
    function getIntegrationLogo(value) {
        switch (value) {
            case "Smartstore": return "/assets/img/logo/smartstore.png";
            case "Trendyol": return "/assets/img/logo/trendyol.webp";
            case "N11": return "/assets/img/logo/n11.jpeg";
            case "Pazarama": return "/assets/img/logo/pazarama.png";
            case "Idefix": return "/assets/img/logo/idefix.png";
            case "CicekSepeti": return "/assets/img/logo/ciceksepeti.jpeg";
            case "Hepsiburada": return "/assets/img/logo/hepsiburada.png";
            default: return "/assets/img/icons/logo/default.png";
        }
    }

    const init = function () {
        const table = $('#OrderTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            order: [[3, 'asc']],
            ajax: {
                url: '/Order/OrderList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false }, // checkbox
                { data: 'Id', visible: false }, // hidden ID
                { data: 'OrderSourceLabelHint',name:'OrderSourceId' },
                { data: 'Id' },
                { data: 'Customer.Name' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'PaymentStatusLabelHint' },
                { data: 'Id' } // İşlemler
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
                    targets: 2,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <img src="${getIntegrationLogo(row.OrderSourceLabelHint)}" style="max-width:115px;"/>
                        </div>`;
                    }
                },
                {
                    targets: 3,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div><i class="menu-icon tf-icons ti ti-package"></i><b>#${row.OrderNumber}</b></div>
                            <div>Sipariş Tarihi : <b>${moment(row.OrderDate).format("DD.MM.yyyy HH:mm")}</b></div>
                            <div class="mb-3">Paket No : <b>3250544</b></div>
                            <div class="text-primary">
                                <div>Kalan Süre:</div>
                                <div>1 gün 07 saat 37 dakika</div>
                            </div>
                        </div>`;
                    }
                },
                {
                    targets: 4,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div><i class="menu-icon tf-icons ti ti-star"></i><b>${row.Customer.Name}</b></div>
                            <div>1. sipariş</div>
                        </div>`;
                    }
                },
                {
                    targets: 5,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        var items = "";
                        for (var i = 0; i < row.OrderItems.length; i++) {
                            items +=
                                `<div class="d-flex mb-5">
                                    <div class="me-5 position-relative">
                                        <img src="${row.OrderItems[i].Product.MainPicture.Url}" width="40" height="60"/>
                                        <span style="background: #ff6060;color: #fff;font-size: 15px;width: 30px;height: 30px;border-radius: 30px;text-align: center;line-height: 30px;display: block;right: -15px;top: -15px;position: absolute;">${row.OrderItems[i].Quantity}</span>
                                    </div>
                                    <div>
                                        <div><b>${row.OrderItems[i].Product.Name}</b></div>
                                        <div>Stok Kodu: ${row.OrderItems[i].Product.Code}</div>
                                        <div>Birim Fiyat: ${row.OrderItems[i].UnitPrice} TL</div>
                                    </div>
                                </div>`;
                        }


                        return `
                        <div>
                            ${items}
                        </div>`;
                    }
                },
                {
                    targets: 6,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div><img src="https://www.yurticikargo.com/web_files/yurtici-kargo/assets/img/logo.svg" width="100px" style="margin:0px auto;display:block;"/></div>
                            <div class="text-center">7330026152879643</div>
                        </div>`;
                       
                    }
                },
                {
                    targets: 7,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div>Tutar  :${row.OrderSubtotalInclTax} TL</div>
                            <div>İndirim:${row.OrderDiscount} TL</div>
                            <div>Faturalanacak Tutar</div>
                            <div class="mb-4"><b>${row.OrderTotal} TL</b></div>
                            <div class="text-warning mb-1">Fatura Bekleniyor</div>
                            <div class="btn-group">
                              <button type="button" class="btn btn-outline-secondary dropdown-toggle waves-effect" data-bs-toggle="dropdown" aria-expanded="false">Fatura İşlemleri</button>
                              <ul class="dropdown-menu">
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Action</a></li>
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Another action</a></li>
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Something else here</a></li>
                                <li>
                                  <hr class="dropdown-divider">
                                </li>
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Separated link</a></li>
                              </ul>
                            </div>
                        </div>`;

                    }
                },
                {
                    targets: 8,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div>Ödeme Yöntemi  :</div>
                            <div><b>${row.PaymentMethodSystemName}</b></div>
                            <div>Ödeme Durumu   :</div>
                            <div><b>${row.PaymentStatusLabelHint}</b></div>
                        </div>`;

                    }
                },
                {
                    targets: -1,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                        <div class="d-inline-block text-nowrap">
                            <a href="Edit?id=${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                                <i class="icon-base ti ti-pencil icon-22px"></i>
                            </a>
                            <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                            </button>
                            <div class="dropdown-menu dropdown-menu-end m-0">
                                <a href="/Order/Detail?id=${row.Id}" class="dropdown-item">Sipariş Detaylar</a>
                                <a href="Archive?id=${row.Id}" class="dropdown-item">Arşiv</a>
                                <div class="dropdown-divider"></div>
                                <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Sipariş Sil</a>
                            </div>
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
                                        text: `<i class="icon-base ti tabler-printer me-1"></i> Yazdır`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-file me-1"></i> Csv`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-upload me-1"></i> Excel`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-file-text me-1"></i> Pdf`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Kopyala`,
                                        exportOptions: { columns: [2, 3, 4, 5] }
                                    }
                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                       <span class="d-none d-sm-inline-block">Yeni Sipariş</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    window.location.href = "/Order/Create";
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

        // Delete Button
        $(document).on('click', '.delete-record', function () {
            const orderId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu sipariş silinecek!',
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
                        url: '/Order/Delete',
                        type: 'POST',
                        data: { id: orderId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Sipariş başarıyla silindi.',
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
    };

    return {
        init: init
    };

})(jQuery);

