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
    function initList() {
        const statusDisplayMap = {
            0: "Beklemede",
            10: "Alındı",
            20: "İade Onaylandı",
            30: "Ürün(ler) Tamir Edildi",
            40: "Ürün(ler) İade Edildi",
            50: "Talep Reddedildi",
            60: "İptal Edildi"
        };

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
            order: [[9, 'asc']], // Tarihe göre sıralama
            ajax: {
                url: '/ReturnRequest/ReturnRequestList',
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
                { data: 'ReturnRequestStatusId', visible: false },
                {
                    data: 'ReturnRequestStatus',
                    render: function (data) {
                        return statusDisplayMap[data] || "Bilinmeyen";
                    }
                },
                {
                    name: 'CreatedOn',
                    data: 'CreatedOnUtc',
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
                    targets: 7, // ReturnRequestStatusId'nin indexi
                    visible: false,
                    searchable: false
                },
                {
                    targets: -1,
                    title: 'İşlemler',
                    searchable: false,
                    orderable: false,
                    render: (data, type, row) => `
                    <div class="d-inline-block text-nowrap">
                        <a href="#" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Detaylar">
                            <i class="icon-base ti ti-eye icon-22px"></i>
                        </a>
                        <a href="Edit?id=${row.Id}" class="btn btn-text-secondary rounded-pill waves-effect btn-icon" title="Düzenle">
                            <i class="icon-base ti ti-pencil icon-22px"></i>
                        </a>
                         <button class="btn btn-text-danger rounded-pill waves-effect btn-icon btn-delete" data-id="${row.Id}" title="Sil">
                            <i class="icon-base ti ti-trash icon-22px"></i>
                        </button>
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
                        buttons: []
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
                    if (this.dataSrc() === "ReturnRequestStatusId") {
                        Entegro.returnrequest.list.addFilterDropdown(this, ".returnRequestStatusFilter", "Durum", [
                            { title: "Beklemede", id: 0 },
                            { title: "Alındı", id: 10 },
                            { title: "İade Onaylandı", id: 20 },
                            { title: "Ürün(ler) Tamir Edildi", id: 30 },
                            { title: "Ürün(ler) İade Edildi", id: 40 },
                            { title: "Talep Reddedildi", id: 50 },
                            { title: "İptal Edildi", id: 60 }
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


    return {
        addFilterText: addFilterText,
        addFilterDropdown: addFilterDropdown,
        init: initList,
        deleteReturnRequest: deleteReturnRequest,
       
    };

})(jQuery);
