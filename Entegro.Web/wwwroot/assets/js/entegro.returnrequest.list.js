var Entegro = Entegro || {};
Entegro.returnrequest = Entegro.returnrequest || {};

Entegro.returnrequest.list = (function ($) {

    function addFilterDropdown(column, containerSelector, placeholder, map = null) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        let select = document.createElement("select");
        select.className = "form-select select2 text-capitalize";
        select.innerHTML = `<option value="">${placeholder}</option>`;
        container.appendChild(select);

        if (map && Array.isArray(map)) {
            map.forEach(item => {
                let option = document.createElement("option");
                option.value = item.id;
                option.textContent = item.title;
                select.appendChild(option);
            });
        } else {
            column.data().unique().sort().each(function (value) {
                if (value !== null && value !== undefined && value !== "") {
                    let option = document.createElement("option");
                    option.value = value;
                    option.textContent = value;
                    select.appendChild(option);
                }
            });
        }

        if (window.jQuery && $(select).select2) {
            $(select).select2({
                placeholder: placeholder,
                allowClear: true,
                width: "resolve"
            });


            $(select).on("change", function () {
                const val = this.value || "";
                column.search(val, false, false).draw();
            });
        } else {
            select.addEventListener("change", function () {
                const val = select.value ? `^${select.value}$` : "";
                column.search(val, true, false).draw();
            });
        }

    }
    function addFilterText(column, containerSelector, placeholder) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        // input elementini oluştur
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control";
        input.placeholder = placeholder;

        container.appendChild(input);

        // her yazımda filtre uygula (debounce ile optimize edebilirsin)
        input.addEventListener("keyup", function () {
            const val = input.value || "";
            column.search(val, false, true).draw();
        });
    }


    const initTable = function (returnRequestStatusId) {
        const table = $('#ReturnRequestTable').DataTable({
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
                url: '/ReturnRequest/ReturnRequestList?returnRequestStatusId=' + returnRequestStatusId,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {

                    const filters = {
                        customerName: $("#filterCustomerName").val() || "",
                        orderNo: $("#filterOrderNo").val() || "",
                        returnCode: $("#filterReturnCode").val() || "",
                        barcode: $("#filterBarcode").val() || "",
                        filterClaimReasonCode: $("#filterClaimReasonCode").val() || "",
                        startDate: $("#filterStartDate").val() || null,
                        endDate: $("#filterEndDate").val() || null
                    };


                    return JSON.stringify({
                        grid: d,        // GridCommand
                        filters: filters // OrderListFilter
                    });
                }
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
                        } else if (row.IntegrationSystem.IntegrationSystemTypeLabelHint == "ERP Entegrasyonu") {
                            type = row.IntegrationSystem.IntegrationSystemParameters.find(x => x.Key === "ErpType").Value;
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
                        const returnDate = moment.utc(row.ClaimDate);
                        const now = moment.utc();

                        let html = `
                           <div>
                               <div><i class="menu-icon tf-icons ti ti-package"></i><b>#${row.OrderNumber}</b></div>
                               <div>Sipariş Tarihi : <b>${orderDate.format("DD.MM.YYYY HH:mm")}</b></div>
                               <div class="mb-3">İade Talep Tarihi No : <b>${returnDate.format("DD.MM.YYYY HH:mm")} </b></div>
        `               ;

                        html += `</div>`;
                        return html;
                    }
                },

                {
                    targets: 4,
                    orderable: false,
                    searchable: true,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        return `
                        <div>
                            <div><i class="menu-icon tf-icons ti ti-star"></i><b>${row.CustomerFirstName + " " + row.CustomerLastName}</b></div>
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
                        for (var i = 0; i < row.Items.length; i++) {
                            if (row.Items[i].ProductId == null) {
                                console.log(row.Items);
                                items +=
                                    `<div  class="order-item-no-integration">
                                       <div class="p-5 d-flex align-items-start gap-3">
                                              <div class="me-5 position-relative">
                                                 <img src="${row.Items[i].ProductImageUrl}" width="60"/>
                                                 <span style="background: #ff6060;color: #fff;font-size: 15px;width: 30px;height: 30px;border-radius: 30px;text-align: center;line-height: 30px;display: block;right: -15px;top: -15px;position: absolute;">1</span>
                                             </div>
                                          <div>
                                            <div class="order-item-no-integration-title">
                                              Eşleştirilmemiş Ürün, Eşleştirme Yapmak İçin Tıklayın.
                                            </div>
                                            <div class="order-item-no-integration-info">
                                              ${row.Items[i].ProductName}
                                            </div>
                                            <div class="order-item-no-integration-info">
                                              ${row.Items[i].MerchantSku}
                                            </div>
                                          </div>
                                        </div>

                                    </div>`;
                            } else {
                                items +=
                                    `<div class="d-flex mb-5 mt-5">
                                    <div class="me-5 position-relative">
                                        <img src="${row.Items[i].ProductMainPicture}" width="60"/>
                                        <span style="background: #ff6060;color: #fff;font-size: 15px;width: 30px;height: 30px;border-radius: 30px;text-align: center;line-height: 30px;display: block;right: -15px;top: -15px;position: absolute;">${row.OrderItems[i].Quantity}</span>
                                    </div>
                                    <div>
                                        <div><b>${row.Items[i].ProductName}</b></div>
                                        <div>Barkod      : ${row.Items[i].IntegrationSku ?? ""}</div>
                                        <div>Birim Fiyat : ${row.Items[i].UnitPrice} TL</div>
                                        <div>
                                            <b class="text-muted">
                                                ${row.Items[i].AttributeDescription ? row.Items[i].AttributeDescription : 'Açıklama yok'}
                                            </b>
                                        </div>
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

                        let logoUrl = "";

                        // Kargo firması kontrolü
                        if (row.CargoProviderName === "Trendyol Express Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/17.png";
                        } else if (row.CargoProviderName === "Aras Kargo Marketplace" || row.CargoProviderName === "Aras Kargo") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/7.png";
                        } else if (row.CargoProviderName === "Yurtiçi Kargo Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/4.png";
                        } else if (row.CargoProviderName === "Horoz Kargo Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/6.png";
                        } else if (row.CargoProviderName === "Sürat Kargo Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/9.png";
                        } else if (row.CargoProviderName === "MNG Kargo Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/10.png";
                        } else if (row.CargoProviderName === "DHL eCommerce Marketplace" || row.CargoProviderName === "DHL eCommerce" || row.CargoProviderName === "DHL  Inc.") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/40.png";
                        } else if (row.CargoProviderName === "Kolay Gelsin Marketplace") {
                            logoUrl = "https://cdn.dsmcdn.com/seller-center/oms/nexus/cargo-provider/38.png";
                        } else {
                            logoUrl = "";
                        }

                        const hasTrackingNumber =
                            row.CargoTrackingNumber &&
                            row.CargoTrackingNumber !== "0" &&
                            row.CargoTrackingNumber !== 0;

                        const trackingButton = row.CargoTrackingLink
                            ? `<div class="text-center mt-2">
                                   <a href="${row.CargoTrackingLink}" target="_blank" class="btn btn-sm btn-outline-warning">
                                        Kargo Takibi
                                    </a>
                                </div>`
                            : "";
                        return `
                        <div>
                            <div>
                                <img src="${logoUrl}" width="100px" style="margin:0px auto;display:block;"/>
                            </div>
                            <div class="text-center">
                                ${hasTrackingNumber
                                ? row.CargoTrackingNumber
                                : 'Kargo bilgisi henüz oluşturulmamış veya takip numarası bulunmamaktadır.'}
                            </div>
                             ${trackingButton}
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
                            <div>Tutar  :${row.SubTotal} TL</div>
                            <div>İndirim:${row.OrderDiscount} TL</div>
                            <div>Faturalanacak Tutar</div>
                            <div class="mb-4"><b>${row.OrderTotal} TL</b></div>
                        </div>`;
                    }
                },
                {
                    targets: 8,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    render: function (data, type, row) {
                        let claimHtml = "";

                        if (row.Items && row.Items.length > 0) {
                            row.Items.forEach(item => {
                                if (item.CustomerClaimReasonName || item.CustomerNote) {
                                    claimHtml += `
                                    <div class="mb-2" style="width:125px;">
                                        <div><b>${item.CustomerClaimReasonName ?? "-"}</b></div>
                                        <div>Müşteri Notu :</div>
                                        <div><b>${item.CustomerNote ?? "-"}</b></div>
                                    </div>
                                `;
                                }
                            });
                        }

                        return `
                        <div>
                            ${claimHtml || '<div class="text-muted">İade / talep bilgisi yok</div>'}
                        </div>
                            `;
                    }
                },

                {
                    targets: -1,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row) {
                       
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
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
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
                                       <span class="d-none d-sm-inline-block">Yeni İade</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    window.location.href = "#";
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

    const initTab = function (defaultReturnRequestStatus) {

        $(".returnrequest-actions .btn").click(function () {
            var returnRequestStatusId = $(this).data("returnrequest-status");
            $(".returnrequest-actions .btn").removeClass("active");
            $(this).addClass("active");


            initTable(returnRequestStatusId);
        });


        if (defaultReturnRequestStatus !== undefined && defaultReturnRequestStatus !== null) {
            var $defaultBtn = $('.returnrequest-actions .btn[data-returnrequest-status="' + defaultReturnRequestStatus + '"]');
            if ($defaultBtn.length > 0) {
                $(".returnrequest-actions .btn").removeClass("active");
                $defaultBtn.addClass("active");

                initTable(defaultReturnRequestStatus);
            }
        }
    };
    function deleteReturnRequest() {
        $(document).on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu kaydı silmek üzeresiniz!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: `/ReturnRequest/Delete`,
                        type: 'POST',
                        data: { id: id },
                        success: function (response) {
                            Swal.fire(
                                'Silindi!',
                                'Kayıt başarıyla silindi.',
                                'success'
                            ).then(() => {
                                window.location.href = "/ReturnRequest/List";
                            });
                        },
                        error: function () {
                            Swal.fire(
                                'Hata!',
                                'Kayıt silinirken bir sorun oluştu.',
                                'error'
                            );
                        }
                    });
                }
            });
        });

    }

    function checkAndInit() {
        const activeReturnRequestStatus = parseInt($('.returnrequest-actions .btn.active').data('returnrequest-status')) || 0;
        if (activeReturnRequestStatus > 0) {
        }
    }


    return {
        addFilterText: addFilterText,
        addFilterDropdown: addFilterDropdown,
        initTable: initTable,
        initTab: initTab,
        deleteReturnRequest: deleteReturnRequest,

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
            case "Logo": return "/assets/img/logo/logo.webp";
            default: return "/assets/img/icons/logo/default.png";
        }
    }
})(jQuery);
