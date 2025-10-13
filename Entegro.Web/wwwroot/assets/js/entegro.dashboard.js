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

    // Public API
    return {
        init: init
    };

})(jQuery);
