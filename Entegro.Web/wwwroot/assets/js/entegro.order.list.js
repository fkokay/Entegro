var Entegro = Entegro || {};
Entegro.order = Entegro.order || {};


Entegro.order.OrderList = (function ($) {

    const initTable = function (orderStatus) {
        const table = $('#OrderTable').DataTable({
            destroy: true,
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
                url: '/Order/OrderList?orderStatus=' + orderStatus,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false }, // checkbox
                { data: 'Id', visible: false }, // hidden ID
                { data: 'IntegrationSystemId' },
                { data: 'Id' },
                { data: 'Customer.Name' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'PaymentStatus' },
                { data: 'Id', width: '200px', } // İşlemler
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
                        var type = "";
                        if (row.IntegrationSystem.IntegrationSystemTypeLabelHint == "E-Ticareti Entegrasyonu") {
                            type = row.IntegrationSystem.IntegrationSystemParameters.find(x => x.Key === "CommerceType").Value;
                        } else if (row.IntegrationSystem.IntegrationSystemTypeLabelHint == "Pazaryeri Entegrasyonu") {
                            type = row.IntegrationSystem.IntegrationSystemParameters.find(x => x.Key === "MarketplaceType").Value;
                        }

                        return `<div>
                                <div><img src="${getIntegrationLogo(type)}" style="max-width:115px;"/></div>
                                <div class="text-center">Mağaza Adı</div>
                                <div class="text-center fw-bold">${row.IntegrationSystem.Name}</div>
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
                            <div class="mb-3">Paket No : <b>${row.PackageNo}</b></div>
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
                            <div><i class="menu-icon tf-icons ti ti-star"></i><b>${row.CustomerName}</b></div>
                            <div>${row.CustomerOrderCounts}. sipariş</div>
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
                            if (row.OrderItems[i].ProductId == null) {
                                items +=
                                    `<div onclick="Entegro.order.OrderList.ProductIntegration(${row.OrderItems[i].Id}, '${row.OrderItems[i].IntegrationProductName}', '${row.OrderItems[i].IntegrationSku}')" class="order-item-no-integration">
                                        <div class="p-5">
                                            <div class="order-item-no-integration-title">Eşleştirilmemiş Ürün, Eşleştirme Yapmak İçin Tıklayın.</div>
                                            <div class="order-item-no-integration-info">${row.OrderItems[i].IntegrationProductName}</div>
                                            <div class="order-item-no-integration-info">${row.OrderItems[i].IntegrationSku}</div>
                                        </div>
                                    </div>`;
                            } else {
                                items +=
                                    `<div class="d-flex mb-5 mt-5">
                                    <div class="me-5 position-relative">
                                        <img src="${row.OrderItems[i].ProductMainPicture}" width="40" height="60"/>
                                        <span style="background: #ff6060;color: #fff;font-size: 15px;width: 30px;height: 30px;border-radius: 30px;text-align: center;line-height: 30px;display: block;right: -15px;top: -15px;position: absolute;">${row.OrderItems[i].Quantity}</span>
                                    </div>
                                    <div>
                                        <div><b>${row.OrderItems[i].ProductName}</b></div>
                                        <div>Stok Kodu   : ${row.OrderItems[i].ProductCode}</div>
                                        <div>Barkod      : ${row.OrderItems[i].ProductBarcode ?? ""}</div>
                                        <div>Birim Fiyat : ${row.OrderItems[i].UnitPrice} TL</div>
                                    </div>
                                </div>
                                `;
                            }
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
                        if (row.ShippingTrackingNumber == null || row.ShippingTrackingNumber == "") {
                            return ``;
                        }
                        return `
                        <div>
                            <div><img src="https://www.yurticikargo.com/web_files/yurtici-kargo/assets/img/logo.svg" width="100px" style="margin:0px auto;display:block;"/></div>
                            <div class="text-center">${row.ShippingTrackingNumber}</div>
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
                            <div>Tutar  :${row.OrderSubTotal} TL</div>
                            <div>İndirim:${row.OrderDiscountTotal} TL</div>
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
                            <div><b>${row.PaymentMethod}</b></div>
                            <div>Ödeme Durumu   :</div>
                            <div><b>${row.PaymentStatus}</b></div>
                        </div>`;

                    }
                },
                {
                    targets: -1,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row) {
                        if (orderStatus == 1) {
                            return `<div><button class="btn btn-warning" onclick="Entegro.order.OrderList.OrderPackage(${row.Id})">Paketle</button></div>`;
                        } else if (orderStatus == 2) {
                            return `
                            <div>
                                <button class="btn btn-warning mb-4" style="width:200px;" onclick="Entegro.order.OrderList.OrderPrint(${row.Id},'${row.PackageNo}')">Etiketi Yazıdr</button>
                                <div class="btn-group" style="width:200px;">
                                  <button type="button" class="btn btn-info waves-effect waves-light">Diğer İşlemler</button>
                                  <button type="button" class="btn btn-info dropdown-toggle dropdown-toggle-split waves-effect waves-light" data-bs-toggle="dropdown" aria-expanded="false">
                                    <span class="visually-hidden">Toggle Dropdown</span>
                                  </button>
                                  <ul class="dropdown-menu" style="">
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


                        return ``;
                    }
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
    };

    const initTab = function () {
        $(".order-actions .btn").click(function () {
            var orderStatus = $(this).data("order-status");
            $(".order-actions .btn").removeClass("active");
            $(this).addClass("active");

            initTable(orderStatus);
        });
    };

    const OrderPackage = function OrderPackage(id) {
        $.ajax({
            url: '/Order/Packaging?id='+id,
            type: 'POST',
            dataType: 'html',
            success: function (html) {
                $('#OrderPackageModal .modal-body').html(html);
                OrderPackageInit();
                $('#OrderPackageModal').modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    }

    const ProductIntegration = function ProductIntegration(orderItemId, productIntegrationName, productIntegrationSku) {

        $("#ProductIntegrationName").val(productIntegrationName);
        $("#ProductIntegrationSku").val(productIntegrationSku);
        $("#ProductId").select2({
            placeholder: 'Ürün seçiniz',
            allowClear: true,
            dropdownParent: $('#ProudctIntegrationModal'),
            width: '100%',
            ajax: {
                url: "/Product/AllProduct",
                type: 'POST',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term || '', page: params.page || 1
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.results,
                        pagination: {
                            more: data.pagination?.more === true
                        }
                    };
                },
                cache: true
            }
        });
        $('#ProudctIntegrationModal').modal('show');
    }

    const OrderPrint = function OrderPrint(id, packageNo) {
        $('#OrderPrintModal').find(".modal-body").html(' <iframe src="/Order/Print?id=' + id + '&packageNo=' + packageNo + '" width="100%" height="600px"></iframe>')
        $('#OrderPrintModal').modal('show');
    };

    function OrderPackageInit() {
        $(".btn-minus").click(function () {
            var input = $(this).parent().find("input");
            var span = $(this).parent().find("span");
            var maxQuantity = parseInt($(span).data("max-quantity"));
            var quantity = parseInt($(span).html());
            if (quantity > 1) {
                quantity = quantity - 1;
            }

            if (quantity == 1) {
                $(this).prop("disabled", true);
            }

            if (quantity < maxQuantity) {
                $(".btn-plus").prop("disabled", false);
            }

            $(span).html(quantity);
            $(input).val(quantity);
        });

        $(".btn-plus").click(function () {
            var input = $(this).parent().find("input");
            var span = $(this).parent().find("span");
            var maxQuantity = parseInt($(span).data("max-quantity"));
            var quantity = parseInt($(span).html());
            if (quantity < maxQuantity) {
                quantity = quantity + 1;
            }

            if (quantity == maxQuantity) {
                $(this).prop("disabled", true);
            }

            $(".btn-minus").prop("disabled", false);

            $(span).html(quantity);
            $(input).val(quantity);
        });

        $(".package-remove").click(function () {

        });

        $(".orderpackage-save").click(function () {
            const action = $('#PackagingForm').attr("action");
            const formData = $('#PackagingForm').serialize();

            $.ajax({
                url: action,
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.success) {
                        showMessage("Başarılı!", 'Ürün başarıyla kaydedildi.', "success", "/Product/List");
                    } else {
                        showMessage("Hata!", response.message || 'Bir hata oluştu.', "error");
                    }
                },
                error: function (xhr) {
                    showMessage("Hata!", xhr.responseText || 'İşlem sırasında bir hata oluştu.', "error");
                }
            });
        });
    }

    return {
        initTable: initTable,
        initTab: initTab,
        OrderPackage: OrderPackage,
        ProductIntegration: ProductIntegration,
        OrderPrint: OrderPrint
    };

    function getIntegrationLogo(value) {
        switch (value) {
            case "Smartstore": return "/assets/img/logo/smartstore.png";
            case "Trendyol": return "/assets/img/logo/trendyol.png";
            case "N11": return "/assets/img/logo/n11.png";
            case "Pazarama": return "/assets/img/logo/pazarama.png";
            case "Idefix": return "/assets/img/logo/idefix.png";
            case "CicekSepeti": return "/assets/img/logo/ciceksepeti.png";
            case "Hepsiburada": return "/assets/img/logo/hepsiburada.png";
            default: return "/assets/img/icons/logo/default.png";
        }
    }



})(jQuery);

