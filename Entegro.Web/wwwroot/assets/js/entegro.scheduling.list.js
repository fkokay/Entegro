var Entegro = Entegro || {};
Entegro.scheduling = Entegro.scheduling || {};

Entegro.scheduling.list = (function ($) {
    function initList() {
        const table = $('#SchedulingTable').DataTable({
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
            order: [[1, 'asc']],
            ajax: {
                url: '/Scheduling/SchedulingList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', visible: false },
                { data: 'Name' },
                { data: 'CronExpression' },
                { data: 'Enabled' },
                { data: 'Id' }
            ],
            columnDefs: [
                {
                    targets: 2,
                    className: "text-center",
                    render: (data, type, row) => {
                        return `<div class="d-flex flex-column">
                                          <h6 class="mb-0">${row.CronExpression}</h6>
                                          <small class="text-truncate">${row.CronDescription || ""}</small>
                                      </div>`;
                    }
                },
                {
                    targets: -2,
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
                                <a href="Details?id=${row.Id}" class="dropdown-item">Detaylar</a>
                                <a href="Archive?id=${row.Id}" class="dropdown-item">Arşiv</a>
                                <div class="dropdown-divider"></div>
                                <a href="javascript:void(0);" class="dropdown-item text-danger delete-record" data-id="${row.Id}">Marka Sil</a>
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
                    rowClass: "row mt-0 justify-content-between",
                    features:[]
                },
                topEnd: {
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
    }

    return {
        init: initList
    };

})(jQuery);