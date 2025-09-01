var Entegro = Entegro || {};
Entegro.product = Entegro.product || {};
Entegro.product.list = (function ($) {
    function getIntegrationLogo(value) {
        switch (value) {
            case "Smartstore": return "https://smartstore.com/Themes/HP/images/smartstore-icon.svg";
            case "Trendyol": return "/assets/img/icons/brands/trendyol.png";
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
    function addFilterDropdown(column, containerSelector, placeholder, map) {
        let select = document.createElement("select");
        select.className = "form-select text-capitalize";
        select.innerHTML = `<option value="">${placeholder}</option>`;
        document.querySelector(containerSelector).appendChild(select);

        select.addEventListener("change", function () {
            const val = select.value ? `^${select.value}$` : "";
            column.search(val, true, false).draw();
        });

        column.data().unique().sort().each(function (value) {
            const item = map.find(x => x.id === value);
            if (item) {
                let option = document.createElement("option");
                option.value = item.id;
                option.textContent = item.title;
                select.appendChild(option);
            }
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
                    });
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            if (result.isConfirmed && result.value?.success) {
                Swal.fire(
                    'Başarılı!',
                    result.value.message,
                    'success'
                );
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
                url: '/Product/integrationDialog',
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
                url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
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
                { data: 'BrandId', visible: false },
                { data: 'Name' },
                { data: 'Code' },
                { data: 'Barcode' },
                {
                    data: 'Price',
                    render: $.fn.dataTable.render.number(".", ",", 2)
                },
                {
                    data: 'UpdatedOn',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.YYYY HH:mm");
                    }
                },
                { data: 'Published' },
                { data: 'Id' },
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
                {
                    targets: 3,
                    responsivePriority: 1,
                    render: (data, type, row) => {
                        const image = row.MainPicture ? `<img src="${row.MainPicture.Url}" class="rounded">` : "";
                        return `
                                  <div class="d-flex align-items-center product-name">
                                      <div class="avatar-wrapper">
                                          <div class="avatar me-2 me-sm-4 rounded-2 bg-label-secondary">
                                              ${image}
                                          </div>
                                      </div>
                                      <div class="d-flex flex-column">
                                          <h6 class="mb-0">${row.Name}</h6>
                                          <small class="text-truncate">${row.Brand?.Name || ""}</small>
                                      </div>
                                  </div>`;
                    }
                },
                {
                    targets: 8,
                    className: "text-center",
                    render: data => {
                        const checked = data ? "checked" : "";
                        const titleText = data ? "Yayında" : "Yayında Değil";
                        return `
                                <div class="form-check d-inline-flex justify-content-center">
                                    <input class="form-check-input" type="checkbox" ${checked} disabled title="${titleText}">
                                </div>`;
                    }
                },
                {
                    targets: -1,
                    title: "İşlemler",
                    searchable: false,
                    orderable: false,
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
                                        <li><a href="Edit?id=${row.Id}" class="dropdown-item">Güncelle</a></li>
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
                bottomStart: { features: ["info"] },
                bottomEnd: "paging"
            },
            initComplete: function () {
                this.api().columns(2).every(function () {
                    Entegro.product.list.addFilterDropdown(this, ".productFilterBrand", "Marka", [{ title: "mevababy", id: 83 }]);
                });
            }
        });

        table.on('draw.dt', function () {
            table.rows({ page: 'current' }).every(function () {
                var row = this;
                let integrationHtml = "";

                if (row.data().ProductIntegrations?.length) {
                    row.data().ProductIntegrations.forEach(pi => {
                        let paramValue = pi.IntegrationSystem.IntegrationSystemParameters?.[0]?.Value;
                        let logoSrc = Entegro.product.list.getIntegrationLogo(paramValue);

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

    return {
        getIntegrationLogo: getIntegrationLogo,
        createDropdownLinks: createDropdownLinks,
        addFilterDropdown: addFilterDropdown,
        productIntegration: productIntegration,
        editIntegration: editIntegration,
        initList: initList,
        deleteProduct: deleteProduct,
    };
})(jQuery);