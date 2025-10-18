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
                { data: 'Customer.Name', width: '200px' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'Id' },
                { data: 'PaymentStatus' },
                { data: 'Id', width: '150px', } // İşlemler
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

                        const orderDate = moment.utc(row.OrderDate);
                        const dueDate = moment.utc(row.DueDate);
                        const now = moment.utc();

                        let html = `
                           <div>
                               <div><i class="menu-icon tf-icons ti ti-package"></i><b>#${row.OrderNumber}</b></div>
                               <div>Sipariş Tarihi : <b>${orderDate.format("DD.MM.YYYY HH:mm")}</b></div>
                               <div class="mb-3">Paket No : <b>${row.PackageNo}</b></div>
        `               ;

                        if (row.ShippingStatusId == 30) {
                            // Kargoda
                            if (row.ShippedDateUtc) {
                                const shippedDate = moment.utc(row.ShippedDateUtc);
                                const diffDays = now.diff(shippedDate, 'days');
                                const diffHours = now.diff(shippedDate, 'hours') % 24;
                                const diffMinutes = now.diff(shippedDate, 'minutes') % 60;

                                html += `
                                <div class="text-info">
                                    <div>Taşıma Durumunda: <b>${shippedDate.format("DD.MM.YYYY HH:mm")}</b></div>
                                    <div>${diffDays} gün ${diffHours} saat ${diffMinutes} dakikadır kargoda</div>
                                </div>
                            `;
                            } else {
                                html += `<div class="text-warning">Kargo bilgisi bekleniyor</div>`;
                            }
                        }
                        else if (row.ShippingStatusId == 40) {
                            // Teslim Edildi
                            if (row.DeliveryDateUtc) {
                                const deliveryDate = moment.utc(row.DeliveryDateUtc);
                                const diffDays = now.diff(deliveryDate, 'days');
                                const diffHours = now.diff(deliveryDate, 'hours') % 24;
                                const diffMinutes = now.diff(deliveryDate, 'minutes') % 60;

                                html += `
                                <div class="text-success">
                                    <div>Teslim Edildi: <b>${deliveryDate.format("DD.MM.YYYY HH:mm")}</b></div>
                                </div>
                            `;
                            } else {
                                html += `<div class="text-warning">Teslim tarihi bilgisi yok</div>`;
                            }
                        }
                        else if (row.ShippingStatusId == 20) {
                            html += `<div class="text-danger">#İptal Edildi</div>`;
                        }
                        else {
                            // Diğer durumlar (kalan süre / süre aşıldı)
                            let diffMs = dueDate.diff(now);
                            const overdue = diffMs < 0;
                            if (overdue) diffMs = Math.abs(diffMs);

                            const duration = moment.duration(diffMs);
                            const days = Math.floor(duration.asDays());
                            const hours = duration.hours();
                            const minutes = duration.minutes();

                            const kalanMetin = overdue
                                ? `Süre aşıldı: ${days} gün ${hours} saat ${minutes} dakika`
                                : `${days} gün ${hours} saat ${minutes} dakika`;

                            html += `
                                <div class="${overdue ? 'text-danger' : 'text-primary'}">
                                    <div>Kalan Süre:</div>
                                    <div>${kalanMetin}</div>
                                </div>
                            `;
                        }

                        html += `</div>`;
                        return html;
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
                                    `<div onclick="Entegro.order.OrderList.ProductIntegration(${row.IntegrationSystemId}, '${row.OrderItems[i].IntegrationProductName}', '${row.OrderItems[i].IntegrationSku}')" class="order-item-no-integration">
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
                                        <img src="${row.OrderItems[i].ProductMainPicture}" width="60"/>
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
                        if (!row.ShippingTrackingNumber) {
                            return ``;
                        }

                        let logoUrl = "";

                        // Kargo firması kontrolü
                        if (row.ShipmentCarrier === "Trendyol Express Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/17.png";
                        } else if (row.ShipmentCarrier === "Aras Kargo Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/7.png";
                        } else if (row.ShipmentCarrier === "Yurtiçi Kargo Marketplace") {
                            logoUrl = "https://www.yurticikargo.com/web_files/yurtici-kargo/assets/img/logo.svg";
                        } else {
                            // Varsayılan logo (istersen boş bırakabilirsin)
                            logoUrl = "https://via.placeholder.com/100x40?text=Kargo";
                        }

                        return `
                          <div>
                              <div><img src="${logoUrl}" width="100px" style="margin:0px auto;display:block;"/></div>
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
                            <div>İndirim:${row.OrderDiscount} TL</div>
                            <div>Faturalanacak Tutar</div>
                            <div class="mb-4"><b>${row.OrderTotal} TL</b></div>
                            <div class="text-warning mb-1">Fatura Bekleniyor</div>
                            <div class="btn-group">
                              <button type="button" class="btn btn-outline-secondary dropdown-toggle waves-effect" data-bs-toggle="dropdown" aria-expanded="false">Fatura İşlemleri</button>
                              <ul class="dropdown-menu">
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Oluştur</a></li>
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Bilgileri</a></li>
                                <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Yükle</a></li>
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
                                 <button class="btn btn-warning mb-4" style="width:200px;" onclick="Entegro.order.OrderList.OrderPrint(${row.Id}, '${row.PackageNo}')">Etiketi Yazdır</button>
                                 <div class="btn-group" style="width:200px;">
                                   <button type="button" class="btn btn-info waves-effect waves-light">Diğer İşlemler</button>
                                   <button type="button" class="btn btn-info dropdown-toggle dropdown-toggle-split waves-effect waves-light" data-bs-toggle="dropdown" aria-expanded="false">
                                     <span class="visually-hidden">Toggle Dropdown</span>
                                   </button>
                                   <ul class="dropdown-menu">
                                     <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Oluştur</a></li>
                                     <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Bilgileri</a></li>
                                     <li><a class="dropdown-item waves-effect" href="javascript:void(0);">Fatura Yükle</a></li>
                                   </ul>
                                 </div>
                             </div>`;
                        } else if (orderStatus == 3 || orderStatus == 4) {
                             if (row.TrackingUrl && row.TrackingUrl.trim() !== "") {
                                 return `
                             <div>
                                 <button class="btn btn-warning btn-sm" onclick="window.open('${row.TrackingUrl}', '_blank')">Kargo Takibi</button>
                             </div>`;
                             }else {
                                return `
                                    <div>
                                        <button class="btn btn-secondary" disabled>Kargo Takibi (Yok)</button>
                                    </div>`;
                             }
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
            },


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

       
        checkAndInit();
    };

    const initTab = function (defaultOrderStatus) {

        $(".order-actions .btn").click(function () {
            var orderStatus = $(this).data("order-status");
            $(".order-actions .btn").removeClass("active");
            $(this).addClass("active");

         
            initTable(orderStatus);
        });


        if (defaultOrderStatus !== undefined && defaultOrderStatus !== null) {
            var $defaultBtn = $('.order-actions .btn[data-order-status="' + defaultOrderStatus + '"]');
            if ($defaultBtn.length > 0) {
                $(".order-actions .btn").removeClass("active");
                $defaultBtn.addClass("active");
                
                initTable(defaultOrderStatus);
            }
        }
    };

    const OrderPackage = function OrderPackage(id) {
        $.ajax({
            url: '/Order/Packaging?id=' + id,
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

    const ProductIntegration = function ProductIntegration(integrationSystemId, productIntegrationName, productIntegrationSku) {
        $("#IntegrationSystemId").val(integrationSystemId);
        $("#ProductIntegrationName").val(productIntegrationName);
        $("#ProductIntegrationSku").val(productIntegrationSku);
        $("#IntegrationCode").val(productIntegrationSku);
        $("#ProductId").select2({
            language: {
                noResults: function () {
                    return $(`
                        <div style="padding: 6px; text-align: center;">
                            <button type="button" id="createIfNotExistBtn" class="btn btn-outline-danger btn-sm">
                                Aradığınız Ürün Yok Kaydet Ve Eşleştir
                            </button>
                        </div>
                    `);
                }
            },
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
            },
            templateResult: function (state) {
                if (!state.id) {
                    return state.text;
                }

                var $state = $('<span>' + state.text + '</span><br><span>' + state.code + '</span>');
                return $state;
            }
        });
        $("#ProductId").change(function () {
            var productId = $(this).val();
            const select = document.getElementById("ProductVariantAttributeCombinationId");

            if (productId == undefined) {
                select.innerHTML = "";
                $("#ProductVariantAttributeCombination").hide();
            } else {
                window.showLoading("Lütfen bekleyiniz varyantlar kontrol ediliyor..");
                $.ajax({
                    url: '/Product/GetProductVariantAttributeCombination?productId=' + productId,
                    type: 'POST',
                    success: function (response) {
                        if (response.length > 0) {

                            select.innerHTML = "";

                            response.forEach(item => {
                                const option = document.createElement("option");
                                option.value = item.Id;            // backend'den gelen Id
                                option.text = item.Name;           // backend'den gelen isim
                                select.appendChild(option);
                            });

                            $("#ProductVariantAttributeCombinationId").select2({
                                placeholder: 'Varyant seçiniz',
                                allowClear: false,
                                dropdownParent: $('#ProudctIntegrationModal'),
                                width: '100%'
                            });

                            $("#ProductVariantAttributeCombination").show();
                        } else {
                            $("#ProductVariantAttributeCombination").hide();
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Sunucu Hatası!',
                            text: 'İstek gönderilirken bir hata oluştu.',
                            confirmButtonText: 'Tamam',
                            customClass: {
                                confirmButton: 'btn btn-danger'
                            },
                            buttonsStyling: false
                        });
                    },
                    complete: function () {
                        window.hideLoading();
                    }
                });
            }
        });
        $('#ProudctIntegrationModal').modal('show');

        $("#CreateProductIntegrationForm").submit(function (e) {
            e.preventDefault();
            const $form = $(this);
            const serializedData = $form.serialize();

            if ($form.find("#ProductId").val() == undefined) {
                Swal.fire({
                    title: 'Hata!',
                    text: 'Ürün seçiniz',
                    icon: 'error',
                    confirmButtonText: 'Tamam',
                    customClass: {
                        confirmButton: 'btn btn-danger'
                    },
                    buttonsStyling: false
                });
                return;
            }

            const $submitBtn = $form.find('button[type="submit"]');
            $submitBtn.prop('disabled', true);
            window.showLoading("Lütfen bekleyiniz..");

            $.ajax({
                url: "/Order/CreateProductIntegration",
                type: 'POST',
                data: serializedData,
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: 'Başarılı!',
                            text: 'İşlem başarıyla tamamlandı.',
                            icon: 'success',
                            confirmButtonText: 'Tamam',
                            customClass: {
                                confirmButton: 'btn btn-success'
                            },
                            buttonsStyling: false
                        }).then(() => {
                            location.reload();
                        });
                    }
                    else {
                        Swal.fire({
                            title: 'Hata!',
                            text: response.message || 'Bir hata oluştu.',
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
                        customClass: {
                            confirmButton: 'btn btn-danger'
                        },
                        buttonsStyling: false
                    });
                },
                complete: function () {
                    $("#ProductVariantAttributeCombinationId").html("");
                    $("#ProductVariantAttributeCombination").hide();
                    $submitBtn.prop('disabled', false);

                    window.hideLoading();
                }
            });
        });
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
    function initCustomScriptForOrderStatus2() {
       
        $(document).off("click", "#createIfNotExistBtn").on("click", "#createIfNotExistBtn", function (e) {
            e.preventDefault();
            e.stopPropagation();

            const integrationSystemId = parseInt(document.getElementById("IntegrationSystemId")?.value || 0);
            const productIntegrationSku = document.getElementById("ProductIntegrationSku")?.value || "";

            if (!integrationSystemId || !productIntegrationSku) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Eksik Bilgi',
                    text: 'Lütfen tüm alanları doldurun.'
                });
                return;
            }

            window.showLoading("Lütfen bekleyiniz..");

            fetch('/Product/CreateIfNotExistProductTrendyol', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    integrationSystemId: integrationSystemId,
                    productIntegrationSku: productIntegrationSku
                })
            })
                .then(response => {
                    console.log("Fetch cevabı geldi:", response);
                    return response.json();
                })
                .then(result => {
                    window.hideLoading();
                    console.log("Fetch result:", result);

                    if (result.success === false) {
                        Swal.fire({
                            icon: 'error',
                            title: 'İşlem Başarısız',
                            text: result.message || "Bir hata oluştu. Lütfen tekrar deneyin.",
                            footer: result.errorCode ? `Hata Kodu: ${result.errorCode}` : null
                        });
                        return;
                    }

                    Swal.fire({
                        icon: 'success',
                        title: 'Başarılı',
                        text: result.message || "İşlem başarıyla tamamlandı."
                    }).then(() => {
                        const modalEl = document.getElementById('ProudctIntegrationModal');
                        const modal = bootstrap.Modal.getInstance(modalEl);
                        if (modal) modal.hide();
                        location.reload();
                    });
                })
                .catch(error => {
                    window.hideLoading();
                    console.error("Fetch hatası:", error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Sunucu Hatası',
                        text: "Sunucuya bağlanırken bir sorun oluştu. Lütfen internet bağlantınızı kontrol edin."
                    });
                });
        });
    }

    function checkAndInit() {
        const activeOrderStatus = parseInt($('.order-actions .btn.active').data('order-status')) || 0;
        if (activeOrderStatus > 0) {
            initCustomScriptForOrderStatus2();
        }
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

