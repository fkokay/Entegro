var Entegro = Entegro || {};
Entegro.product = Entegro.product || {};
Entegro.product.list = (function ($) {
    function showMessage(title, message, type = "info", redirectUrl = null, reload = null) {
        Swal.fire({
            title: title,
            text: message,
            icon: type, // success | error | warning | info | question
            confirmButtonText: 'Tamam',
            customClass: { confirmButton: 'btn btn-primary' },
            buttonsStyling: false
        }).then(() => {
            if (redirectUrl) {
                window.location.href = redirectUrl;
            }

            if (reload) {
                location.reload();
            }
        });
    }
    function getIntegrationLogo(value) {
        switch (value) {
            case "Smartstore": return "/assets/img/brandicons/smartstore.png";
            case "Trendyol": return "/assets/img/brandicons/trendyol.webp";
            case "N11": return "/assets/img/brandicons/n11.jpeg";
            case "Pazarama": return "/assets/img/brandicons/pazarama.png";
            case "Idefix": return "/assets/img/brandicons/idefix.png";
            case "CicekSepeti": return "/assets/img/brandicons/ciceksepeti.jpeg";
            case "Hepsiburada": return "/assets/img/brandicons/hepsiburada.png";
            default: return "/assets/img/icons/brands/default.png";
        }
    }
    function createDropdownLinks(items, row, type) {
        return items.map(item => {
            const match = row.ProductIntegrations?.find(pi => pi.IntegrationSystem.Id === item.Id);
            let integrationValue = "";

            if (match?.IntegrationSystem?.IntegrationSystemParameters?.length) {
                integrationValue = match.IntegrationSystem.IntegrationSystemParameters[0]?.Value || "";
            } else if (item.Value) {
                integrationValue = item.Value;
            }

            const logoSrc = getIntegrationLogo(integrationValue);

            return `
                        <li>
                            <a href="javascript:void(0);" class="dropdown-item open-integration"
                               data-bs-toggle="modal" data-bs-target="#addIntegration"
                               data-product-id="${row.Id}"
                               data-integration-system-id="${item.Id}"
                               data-integration-value="${integrationValue}"
                               data-product-integration-id="${match?.Id || ''}">
                               ${type === "marketplace" ? `<img src="${logoSrc}" alt="${integrationValue}" style="width:16px;height:16px;margin-right:5px;">` : ""}
                               <b>${item.Name}</b> ata ${integrationValue ? `(${integrationValue})` : ""}
                            </a>
                        </li>
                    `;
        }).join("");
    }
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
    function productIntegration(integrationSystemId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Tüm ürünlere entegrasyon uygulanacak!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, uygula!',
            cancelButtonText: 'İptal',
            preConfirm: () => {
                
                Swal.close();

              
                window.showLoading("Lütfen bekleyiniz..");

                return fetch('/Product/CreateOrUpdateIntegrationAll', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: "integrationSystemId=" + encodeURIComponent(integrationSystemId)
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(response.statusText);
                        }
                        return response.json();
                    })
                    .catch(error => {
                        Swal.showValidationMessage(`Hata: ${error}`);
                        // Loading ekranını gizle
                        window.hideLoading();
                    });
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            
            window.hideLoading();

            if (result.isConfirmed && result.value?.success) {
                Swal.fire(
                    'Başarılı!',
                    result.value.message,
                    'success'
                ).then(() => {
                   
                    window.location.reload();
                });
            } else if (result.value && !result.value.success) {
                Swal.fire(
                    'Hata!',
                    result.value.message,
                    'error'
                );
            }
        });
    }
    function editIntegration() {
        $(document).on('click', '.product-integration, .open-integration', function (e) {
            e.preventDefault();

            const $el = $(this).find('.integration-name');
            const ProductId = $(this).data('product-id') || $el.data('product-id');
            const IntegrationSystemId = $(this).data('integration-system-id') || $el.data('integration-system-id');
            const IntegrationValue = $(this).data('integration-value') || $el.data('integration-value') || '';


            const ProductIntegrationId = $(this).hasClass('open-integration') ?
                0 :
                ($(this).data('product-integration-id') || $el.data('product-integration-id'));


            $.ajax({
                url: '/Product/ProductIntegrationDialog',
                type: 'GET',
                dataType: 'html',
                data: {
                    ProductIntegrationId,
                    IntegrationSystemId,
                    ProductId,
                    IntegrationValue
                },
                success: function (html) {
                    $('#integrationDialogContainer').html(html);

                    if ($.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse('#integrationDialog');
                    }

                    window.initDataToggler();

                    $("#Custom_ManageInventoryMethod").change(function () {
                        var manageInventoryMethod = $(this).val();
                        if (manageInventoryMethod == 0) {
                            $("#pnlStockQuantity").hide();
                            $("#pnlDisplayStockAvailability").hide();
                            $("#pnlDisplayStockQuantity").hide();
                            $("#pnlMinStockQuantity").hide();
                            $("#pnlLowStockActivity").hide();
                        } else if (manageInventoryMethod == 1) {
                            $("#pnlStockQuantity").show();
                            $("#pnlDisplayStockAvailability").show();
                            $("#pnlDisplayStockQuantity").show();
                            $("#pnlMinStockQuantity").show();
                            $("#pnlLowStockActivity").show();
                        } else if (manageInventoryMethod == 2) {
                            $("#pnlStockQuantity").hide();
                            $("#pnlDisplayStockAvailability").show();
                            $("#pnlDisplayStockQuantity").show();
                            $("#pnlMinStockQuantity").hide();
                            $("#pnlLowStockActivity").hide();
                        }
                    });

                    $("#Custom_ManageInventoryMethod").change();

                    $('#addIntegration').modal('show');
                },
                error: function (xhr) {
                    console.error(xhr.responseText);
                    alert('Form yüklenemedi.');
                }
            });
        });
    }
    function initList() {
        $(document).off('submit.integration')
            .on('submit.integration', '#integrationDialog', function (e) {
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
                            $('#addIntegration').modal('hide');
                            window.location.reload();
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
                            text: 'Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.'
                        });
                    },
                    complete: function () {
                        $form.data('submitting', false);
                        $buttons.prop('disabled', false);
                    }
                });
            });
        var table = $('#ProductTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            ajax: {
                url: '/Product/ProductList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id' },
                {
                    data: 'Id',
                    orderable: false,
                    render: DataTable.render.select()
                },
                { data: 'Code', visible: false },
                { data: 'BrandId', visible: false },
                { data: 'Barcode', visible: false },
                {
                    data: 'Name',
                    render: (data, type, row) => {
                        const image = row.MainPicture ? `<img src="${row.MainPicture.Url}" class="rounded">` : "";
                        return `
                        <div class="d-flex align-items-center product-name">
                          <div class="avatar-wrapper">
                            <div class="avatar me-2 me-sm-4 rounded-2 bg-label-secondary">${image}</div>
                          </div>
                          <div class="d-flex flex-column">
                            <h6 class="mb-0">${row.Name}</h6>
                            <small class="text-truncate">${row.Code || ""}</small>
                            <small class="text-truncate">${row.Brand?.Name || ""}</small>
                          </div>
                        </div>`;
                    }
                },
                {
                    data: 'Price',
                    render: (data, type, row) => {
                        if (type === "sort" || type === "type") {
                            return data;
                        }
                        return $.fn.dataTable.render.number('.', ',', 2).display(data) + ' ' + row.Currency;
                    }
                },
                { data: 'StockQuantity'},
                {
                    data: 'UpdatedOn',
                    name: 'UpdatedOnUtc',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.YYYY HH:mm");
                    }
                },
                {
                    data: 'Published',
                    render: data => {
                        const checked = data ? "checked" : "";
                        const titleText = data ? "Yayında" : "Yayında Değil";
                        return `
                        <div class="form-check d-inline-flex justify-content-center">
                          <input class="form-check-input" type="checkbox" ${checked} onclick="return false;" title="${titleText}">
                        </div>`;
                    }
                },
                {
                    data: null,
                    title: "İşlemler",
                    orderable: false,
                    searchable: false,
                    render: (data, type, row) => {
                        let eTicaretLinks = Entegro.product.list.createDropdownLinks(window.commerces, row, "commerce");
                        let pazarYeriLinks = Entegro.product.list.createDropdownLinks(window.marketPlaces, row, "marketplace");

                        return `
                        <div class="d-inline-block text-nowrap">
                          <a href="Edit?id=${row.Id}" class="btn btn-text-secondary rounded-pill btn-icon">
                            <i class="icon-base ti ti-pencil icon-22px"></i>
                          </a>
                          <button class="btn btn-text-secondary rounded-pill btn-icon dropdown-toggle hide-arrow"
                                  data-bs-toggle="dropdown" data-bs-auto-close="outside">
                            <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                          </button>
                          <ul class="dropdown-menu dropdown-menu-end m-0">
                            <li><a href="Edit?id=${row.Id}" class="dropdown-item">Düzenle</a></li>
                            <li><a href="javascript:void(0);" class="dropdown-item text-danger" onclick="Entegro.product.list.deleteProduct(${row.Id})">Sil</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li class="dropend">
                              <a href="javascript:void(0);" class="dropdown-item dropdown-toggle" data-bs-toggle="dropdown">E-Ticaret Bağlantıları</a>
                              <ul class="dropdown-menu">${eTicaretLinks || '<li><span class="dropdown-item text-muted">Bağlantı yok</span></li>'}</ul>
                            </li>
                            <li class="dropend">
                              <a href="javascript:void(0);" class="dropdown-item dropdown-toggle" data-bs-toggle="dropdown">Pazaryeri Bağlantıları</a>
                              <ul class="dropdown-menu">${pazarYeriLinks || '<li><span class="dropdown-item text-muted">Bağlantı yok</span></li>'}</ul>
                            </li>
                          </ul>
                        </div>`;
                    }
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
                },
            ],
            select: { style: "multi", selector: "td:nth-child(2)" },
            order: [3, "asc"],
            displayLength: 7,
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
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1"><i class="icon-base ti ti-import icon-xs"></i><span>E-Ticaret Entegrasyonu</span></span>`,
                                buttons: window.commerces.map(c => ({
                                    className: "dropdown-item text-center",
                                    text: `Tüm Ürünleri<br>${c.Name}<br> Mağazasına Bağla`,
                                    action: () => {
                                        Entegro.product.list.productIntegration(c.Id)
                                    }
                                }))
                            },
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1"><i class="icon-base ti ti-upload icon-xs"></i><span>Dışarı Aktar</span></span>`,
                                buttons: ["print", "csv", "excel", "pdf", "copy"]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-1"></i><span>Yeni Kayıt</span>`,
                                className: "btn btn-primary",
                                action: () => window.location.href = "Create"
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
            initComplete: function () {
                this.api().columns().every(function () {
                    if (this.dataSrc() === "Code") {
                        Entegro.product.list.addFilterText(this,".productFilterCode", "Ürün Kodu");
                    } else if (this.dataSrc() === "Name") {
                        Entegro.product.list.addFilterText(this, ".productFilterName", "Ürün Adı");
                    } else if (this.dataSrc() === "Barcode") {
                        Entegro.product.list.addFilterText(this, ".productFilterBarcode", "Ürün Barkodu");
                    } else if (this.dataSrc() === "Published") {
                        Entegro.product.list.addFilterDropdown(this, ".productFilterPublished", "Durum", [{ title: "Yayınlandı", id: true }, { title: "Yayınlanmadı", id: false }]);
                    } else if (this.dataSrc() === "BrandId") {
                        var column = this;

                        $('<select id="brandFilter" style="width:200px"></select>')
                            .appendTo(".productFilterBrand")
                            .on("change", function () {
                                const val = $(this).val();
                                column.search(val ? "^" + val + "$" : "", true, false).draw();
                            });

                        $('#brandFilter').select2({
                            placeholder: "Marka seçin",
                            allowClear: true,
                            ajax: {
                                url: '/Brand/AllBrand',
                                type: 'POST',
                                dataType: 'json',
                                delay: 250,
                                data: function (params) {
                                    return {
                                        term: params.term || "",
                                        page: params.page || 1
                                    };
                                },
                                processResults: function (data, params) {
                                    return {
                                        results: data.results,
                                        pagination: { more: data.pagination.more }
                                    };
                                }
                            }
                        });
                    }
                });

            }
        });

        table.on('draw.dt', function () {
            table.rows({ page: 'current' }).every(function () {
                var row = this;
                let integrationHtml = "";

                if (row.data().ProductIntegrations?.length) {
                    row.data().ProductIntegrations.forEach(pi => {

                        var typeValue = "";
                        switch (pi.IntegrationSystem.IntegrationSystemType) {
                            //Commerce
                            case 2:
                                typeValue = pi.IntegrationSystem.IntegrationSystemParameters?.find(x => x.Key == "CommerceType").Value;
                                break;
                            //Marketplace
                            case 3:
                                typeValue = pi.IntegrationSystem.IntegrationSystemParameters?.find(x => x.Key == "MarketplaceType").Value;
                                break;
                            default:
                        }

                        let logoSrc = Entegro.product.list.getIntegrationLogo(typeValue);

                        integrationHtml += `
                                <div class="col-2 mb-2">
                                    <div class="d-flex align-items-center product-integration">
                                        <div class="product-integration-image">
                                            <img src="${logoSrc}" title="${pi.IntegrationSystem.Name}" class="rounded">
                                        </div>
                                        <div class="product-integration-info">
                                            <span class="integration-name"
                                                data-product-id="${row.data().Id}"
                                                data-product-integration-id="${pi.Id}"
                                                data-integration-system-id="${pi.IntegrationSystem.Id}">
                                                ${pi.IntegrationSystem.Name}
                                            </span>
                                            <span class="price">${pi.Price.toLocaleString("tr-TR", { minimumFractionDigits: 2 })} TL</span>
                                        </div>
                                        <span class="w-px-30 h-px-30 d-flex justify-content-center align-items-center me-4 product-status">
                                            Satışta
                                        </span>
                                    </div>
                                </div>`;
                    });
                }

                if (!row.child.isShown()) {
                    row.child('<div class="row">' + integrationHtml + '</div>').show();
                }
            });
        });

        setTimeout(() => {
            const adjustments = [{
                selector: ".dt-buttons .btn",
                classToRemove: "btn-secondary"
            },
            {
                selector: ".dt-buttons.btn-group",
                classToAdd: "mb-md-0 mb-6"
            },
            {
                selector: ".dt-search .form-control",
                classToRemove: "form-control-sm",
                classToAdd: "ms-0"
            },
            {
                selector: ".dt-search",
                classToAdd: "mb-0 mb-md-6"
            },
            {
                selector: ".dt-length .form-select",
                classToRemove: "form-select-sm"
            },
            {
                selector: ".dt-layout-end",
                classToAdd: "gap-md-2 gap-0 mt-0"
            },
            {
                selector: ".dt-layout-start",
                classToAdd: "mt-0"
            },
            {
                selector: ".dt-layout-table",
                classToRemove: "row mt-2"
            },
            {
                selector: ".dt-layout-full",
                classToRemove: "col-md col-12",
                classToAdd: "table-responsive"
            }
            ];

            adjustments.forEach(({
                selector,
                classToRemove,
                classToAdd
            }) => {
                document.querySelectorAll(selector).forEach(element => {
                    if (classToRemove) {
                        classToRemove.split(" ").forEach(cls => element.classList.remove(cls));
                    }

                    if (classToAdd) {
                        classToAdd.split(" ").forEach(cls => element.classList.add(cls));
                    }
                });
            })
        }, 100);
    }
    function deleteProduct(productId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Bu ürün silinecek. Bu işlem geri alınamaz!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Evet, sil!',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Product/Delete?productId=${productId}`, // <- URL'yi backendine göre düzenle
                    type: 'POST',
                    success: function (response) {
                        Swal.fire({
                            title: 'Silindi!',
                            text: 'Ürün başarıyla silindi.',
                            icon: 'success',
                            timer: 1500,
                            showConfirmButton: false
                        });
                        location.reload();
                        // // Tabloyu yenile (örneğin DataTable kullanıyorsan)
                        // $('#yourTableId').DataTable().ajax.reload(null, false); // 2. parametre: sayfayı değiştirme
                    },
                    error: function (xhr) {
                        Swal.fire({
                            title: 'Hata!',
                            text: 'Ürün silinirken bir hata oluştu.',
                            icon: 'error'
                        });
                    }
                });
            }
        });
    }
    function deleteIntegration(integrationId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: 'Bu entegrasyon silinecek!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Evet, sil!',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Product/DeleteProductIntegration',
                    type: 'POST',
                    data: { id: integrationId },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                title: 'Silindi!',
                                text: 'Entegrasyon başarıyla silindi.',
                                icon: 'success',
                                timer: 1500,
                                showConfirmButton: false
                            }).then(() => {
                                location.reload(); // Sayfayı yenile
                            });
                        } else {
                            Swal.fire({
                                title: 'Hata!',
                                text: response.message || 'Bir hata oluştu.',
                                icon: 'error'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            title: 'Hata!',
                            text: 'Silme işlemi sırasında bir hata oluştu.',
                            icon: 'error'
                        });
                    }
                });
            }
        });
    }


    return {
        getIntegrationLogo: getIntegrationLogo,
        createDropdownLinks: createDropdownLinks,
        addFilterDropdown: addFilterDropdown,
        addFilterText: addFilterText,
        productIntegration: productIntegration,
        editIntegration: editIntegration,
        initList: initList,
        deleteProduct: deleteProduct,
        deleteIntegration: deleteIntegration 
    };
})(jQuery);