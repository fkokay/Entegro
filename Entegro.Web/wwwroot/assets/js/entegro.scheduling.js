var Entegro = Entegro || {};
Entegro.scheduling = Entegro.scheduling || {};

Entegro.scheduling = (function ($) {

    function initHistoryTable(taskDescriptorId) {
        if (!taskDescriptorId) return;

        const table = $('#HistoryTable').DataTable({
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
            order: [[3, 'desc']], 
            ajax: {
                url: '/Scheduling/TaskExecutionInfoList?taskDescriptorId=' + taskDescriptorId,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', orderable: false }, // checkbox
                { data: 'Id', visible: false },
                { data: 'MachineName', title: 'Makine Adı' },
                {
                    data: 'StartedOn',
                    name: 'StartedOnUtc',
                    title: 'Uygulama Başlama Tarihi',
                    render: function (data) {
                        return data ? new Date(data).toLocaleString("tr-TR") : "-";
                    }
                },
                {
                    data: 'FinishedOn',
                    name: 'FinishedOnUtc',
                    title: 'Tamamlanma Tarihi',
                    render: function (data) {
                        return data ? new Date(data).toLocaleString("tr-TR") : "-";
                    }
                },
                {
                    data: 'Error',
                    title: 'Hata Mesajı',
                    orderable: false,
                    render: function (data, type, row) {
                        if (!data) {
                            return '<span class="text-muted">-</span>';
                        }
                        return `<pre class="bg-light border p-3 text-danger" style="white-space:pre-wrap">${data}</pre>`;
                    }
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
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
        return table;
    }

    return {
        initHistoryTable: initHistoryTable
    };

})(jQuery);
