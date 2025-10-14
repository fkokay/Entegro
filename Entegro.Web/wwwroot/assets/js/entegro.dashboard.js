var Entegro = Entegro || {};
Entegro.dashboard = Entegro.dashboard || {};

Entegro.dashboard = (function ($) {
    let chart;
    const totalRevenueChartEl = document.querySelector('#totalRevenueChart');

    function init() {
        const currentYear = new Date().getFullYear();
        buildYearDropdown(currentYear);
        loadChart(currentYear);
        bindEvents();

    
        initMarketplaceSalesList();
    }

    function buildYearDropdown(currentYear) {
        document.getElementById("currentYear").innerText = currentYear;

        const dropdown = document.getElementById("yearDropdown");
        dropdown.innerHTML = "";

        for (let i = 0; i <= 3; i++) {
            const year = currentYear - i;
            const a = document.createElement("a");
            a.className = "dropdown-item year-option";
            a.setAttribute("href", "javascript:void(0);");
            a.setAttribute("data-year", year);
            a.innerText = year;
            dropdown.appendChild(a);
        }
    }

    function loadChart(year) {
        $.ajax({
            url: '/Order/GetMonthlySales?year=' + year,
            type: 'GET',
            success: function (response) {
                let categories = response.map(item => getMonthName(item.Month));
                let seriesData = response.map(item => item.TotalAmount);

                let totalYearly = seriesData.reduce((a, b) => a + b, 0);

                document.getElementById("yearlyTotal").innerText =
                    totalYearly.toLocaleString("tr-TR") + " ₺";
                document.getElementById("yearlyBudget").innerText =
                    totalYearly.toLocaleString("tr-TR");

                const chartOptions = {
                    series: [{ name: 'Satış Tutarı', data: seriesData }],
                    chart: { height: 365, type: 'bar', toolbar: { show: false } },
                    plotOptions: { bar: { columnWidth: '40%', borderRadius: 10 } },
                    dataLabels: { enabled: false },
                    xaxis: { categories: categories },
                    yaxis: {
                        labels: {
                            style: { fontSize: '13px' },
                            formatter: function (value) {
                                return value.toLocaleString("tr-TR") + " ₺";
                            }
                        }
                    }
                };

                if (chart) {
                    chart.updateOptions(chartOptions);
                } else {
                    chart = new ApexCharts(totalRevenueChartEl, chartOptions);
                    chart.render();
                }
            }
        });
    }

    function bindEvents() {
        $(document).on("click", ".year-option", function () {
            const selectedYear = $(this).data("year");
            loadChart(selectedYear);
            $("#budgetId").text(selectedYear);
        });
    }

    function getMonthName(monthNumber) {
        const months = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];
        return months[monthNumber - 1];
    }


    function initMarketplaceSalesList() {
        const fmtTRY = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        function formatTRY(v) { return fmtTRY.format(v) + " ₺"; }

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

        const $table = $('.datatable-marketplacesales');
        const $groupSelect = $('#groupBySelect');

        $groupSelect.val("1"); 

        const table = $table.DataTable({
            language: { url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json' },
            serverSide: true,
            processing: true,
            order: [[null, 'desc']], 
            ajax: {
                url: '/Order/GetMarketplaceSalesList?groupByType=1',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: null, orderable: false },
                { data: 'IntegrationValue' },
                { data: 'IntegrationSystemName' },
                { data: 'TotalQuantitySold' },
                { data: 'TotalOrderAmount' },
                { data: 'Period', orderable: false }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                {
                    targets: 1,
                    render: function (data, type, full) {
                        if (type === 'sort' || type === 'type') return data || '';
                        const val = full?.IntegrationValue ?? '';
                        const logo = getIntegrationLogo(val);
                        return `<div class="d-flex align-items-center">
                                    <img src="${logo}" alt="${val}" class="me-2" style="width:24px;height:24px;border-radius:4px;">
                                    <span>${val}</span>
                                </div>`;
                    }
                },
                {
                    targets: 3,
                    className: 'text-end',
                    render: (data, type) => {
                        const v = Number(data || 0);
                        return (type === 'sort' || type === 'type') ? v : v.toLocaleString('tr-TR');
                    }
                },
                {
                    targets: 4,
                    className: 'text-end',
                    render: (data, type) => {
                        const v = Number(data || 0);
                        return (type === 'sort' || type === 'type') ? v : formatTRY(v);
                    }
                }
            ],
            displayLength: 10
        });

        // Select değişince tabloyu güncelle
        $groupSelect.on('change', function () {
            const newUrl = '/Order/GetMarketplaceSalesList?groupByType=' + $(this).val();
            table.ajax.url(newUrl).load();
        });

        return table;
    }


    return {
        init: init
    };

})(jQuery);
