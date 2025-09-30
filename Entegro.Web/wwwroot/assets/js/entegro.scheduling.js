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
                        if (!data) return '-';
                        const date = new Date(data);
                        const timeAgo = moment(date).fromNow();
                        return `
                <div class="d-flex flex-column">
                    <span>${date.toLocaleString("tr-TR")}</span>
                    <small class="text-muted">${timeAgo}</small>
                </div>`;
                    }
                },
                {
                    data: 'FinishedOn',
                    name: 'FinishedOnUtc',
                    title: 'Tamamlanma Tarihi',
                    render: function (data) {
                        if (!data) return '-';
                        const date = new Date(data);
                        const timeAgo = moment(date).fromNow();
                        return `
                <div class="d-flex flex-column">
                    <span>${date.toLocaleString("tr-TR")}</span>
                    <small class="text-muted">${timeAgo}</small>
                </div>`;
                    }
                },
                {
                    data: null,
                    title: 'Süre',
                    orderable: false,
                    render: function (data, type, row) {
                        if (!row.StartedOn || !row.FinishedOn) return '-';

                        const start = moment(row.StartedOn);
                        const end = moment(row.FinishedOn);
                        const duration = moment.duration(end.diff(start));

                        const hours = duration.hours();
                        const minutes = duration.minutes();
                        const seconds = duration.seconds();

                        const parts = [];
                        if (hours) parts.push(`${hours} saat`);
                        if (minutes) parts.push(`${minutes} dakika`);
                        if (seconds || parts.length === 0) parts.push(`${seconds} saniye`);

                        return `<span>${parts.join(' ')}</span>`;
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
                topEnd: null,
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
    function timeAgo(date) {
        const now = new Date();
        const ts = (now - new Date(date)) / 1000; // saniye farkı

        if (ts < 60) {
            return `${Math.floor(ts)} saniye önce`;
        }
        if (ts < 3600) {
            return `${Math.floor(ts / 60)} dakika önce`;
        }
        if (ts < 86400) {
            return `${Math.floor(ts / 3600)} saat önce`;
        }
        if (ts < 2592000) {
            return `${Math.floor(ts / 86400)} gün önce`;
        }
        if (ts < 31536000) {
            return `${Math.floor(ts / 2592000)} ay önce`;
        }
        return `${Math.floor(ts / 31536000)} yıl önce`;
    }

    function timeAgoOrAfter(date) {
        const now = new Date();
        const target = new Date(date);
        const diffSec = Math.floor((target - now) / 1000);

        const absSec = Math.abs(diffSec);

        if (absSec < 60) {
            return diffSec >= 0 ? `${absSec} saniye sonra` : `${absSec} saniye önce`;
        }
        if (absSec < 3600) {
            const min = Math.floor(absSec / 60);
            return diffSec >= 0 ? `${min} dakika sonra` : `${min} dakika önce`;
        }
        if (absSec < 86400) {
            const hrs = Math.floor(absSec / 3600);
            return diffSec >= 0 ? `${hrs} saat sonra` : `${hrs} saat önce`;
        }
        if (absSec < 2592000) {
            const days = Math.floor(absSec / 86400);
            return diffSec >= 0 ? `${days} gün sonra` : `${days} gün önce`;
        }
        if (absSec < 31536000) {
            const months = Math.floor(absSec / 2592000);
            return diffSec >= 0 ? `${months} ay sonra` : `${months} ay önce`;
        }
        const years = Math.floor(absSec / 31536000);
        return diffSec >= 0 ? `${years} yıl sonra` : `${years} yıl önce`;
    }

    function formatDuration(start, end) {
        const diffMs = new Date(end) - new Date(start);

        let ms = diffMs % 1000;
        let sec = Math.floor((diffMs / 1000) % 60);
        let min = Math.floor((diffMs / (1000 * 60)) % 60);
        let hrs = Math.floor(diffMs / (1000 * 60 * 60));

        return `${hrs}:${min.toString().padStart(2, "0")}:${sec.toString().padStart(2, "0")},${ms}`;
    }
    return {
        initHistoryTable: initHistoryTable
    };

})(jQuery);
