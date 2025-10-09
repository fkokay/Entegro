var Entegro = Entegro || {};
Entegro.bulkupdateprices = Entegro.bulkupdateprices || {};

Entegro.bulkupdateprices = (function ($) {
    let isFormChanged = false;

    function initFormHandler() {
        const form = document.getElementById('integration-form');
        if (!form) return;

        // tüm inputları izle
        form.querySelectorAll('input').forEach(input => {
            input.addEventListener('change', () => {
                isFormChanged = true;

                // ilgili satırdaki IsChanged hidden inputunu bul ve true yap
                const row = input.closest('tr');
                if (row) {
                    const isChangedInput = row.querySelector('input[name*="IsChanged"]');
                    if (isChangedInput) {
                        isChangedInput.value = "true";
                    }
                }
            });
        });

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            if (!isFormChanged) {
                Swal.fire({
                    title: 'Uyarı',
                    text: 'Formda herhangi bir değişiklik yapılmadı!',
                    icon: 'info',
                    confirmButtonText: 'Tamam',
                    customClass: { confirmButton: 'btn btn-primary' },
                    buttonsStyling: false
                });
                return; // post etme
            }

            const formData = new FormData(form);
            window.showLoading("Lütfen bekleyiniz..");

            fetch(form.action, {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    window.hideLoading();

                    if (data.success) {
                        const toastElement = document.getElementById('successToast');
                        const toast = new bootstrap.Toast(toastElement);
                        toast.show();
                        isFormChanged = false;
                    } else {
                        Swal.fire({
                            title: 'Hata!',
                            text: data.message || 'Kayıt başarısız!',
                            icon: 'error',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-danger' },
                            buttonsStyling: false
                        });
                    }
                })
                .catch(error => {
                    window.hideLoading();
                    console.error('Hata oluştu:', error);
                    Swal.fire({
                        title: 'Sunucu Hatası!',
                        text: 'Sunucu hatası oluştu.',
                        icon: 'error',
                        confirmButtonText: 'Tamam',
                        customClass: { confirmButton: 'btn btn-danger' },
                        buttonsStyling: false
                    });
                });
        });
    }

    function initBrandFilter(selectedBrand) {
        const brandFilter = document.getElementById('brandFilter');

        if (brandFilter && selectedBrand) {
            brandFilter.value = selectedBrand;
        }

        brandFilter?.addEventListener('change', function () {
            const selectedBrandId = this.value;
            const newUrl = new URL(window.location.href);

            newUrl.searchParams.set('brandId', selectedBrandId);
            newUrl.searchParams.set('page', 1);

            window.location.href = newUrl.toString();
        });
    }

    function initLeaveWarning() {
        document.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', function (e) {
                const href = this.getAttribute('href');

                if (!href || href.startsWith('#')) return;

                if (isFormChanged) {
                    e.preventDefault();
                    Swal.fire({
                        title: 'Uyarı',
                        text: 'Kaydedilmemiş değişiklikler var. Sayfadan ayrılmak istiyor musunuz?',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'Sayfadan Ayrıl',
                        cancelButtonText: 'Sayfada Kal'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = href;
                        }
                    });
                }
            });
        });
    }
  
    function initPriceUpdatePopup() {
        const applyBtn = document.getElementById("applyPercentBtn");
        if (!applyBtn) return;

        applyBtn.addEventListener("click", function () {
            const percent = parseFloat(document.getElementById("percentValue").value) || 0;
            const checkedCols = Array.from(document.querySelectorAll(".apply-column:checked"));

            if (checkedCols.length === 0) {
                Swal.fire({
                    title: 'Uyarı',
                    text: 'En az bir sütun seçmelisiniz!',
                    icon: 'warning',
                    confirmButtonText: 'Tamam'
                });
                return;
            }

            const columns = checkedCols.map(c => c.value);

            document.querySelectorAll("tbody tr").forEach(row => {
                
                const cost = parseFloat(row.querySelector("td:nth-child(2)").innerText) || 0;

                if (cost <= 0) {
                    return;
                }

                columns.forEach(col => {
                    let input;

                    if (col === "Price") {
                        input = row.querySelector("input[name*='Price'][name$='.Price']");
                    }
                    else if (col === "SalePrice") {
                        input = row.querySelector("input[name*='SalePrice']");
                    }
                    else {
                        input = row.querySelector(`input[name*='IntegrationPrices[${col}]'][name$='.Price']`);
                    }

                    if (input) {
                        const oldVal = parseFloat(input.value) || 0;
                        
                        const newVal = (oldVal + (cost * percent / 100)).toFixed(2);
                        input.value = newVal;

                        const flag = row.querySelector(".is-changed-flag");
                        if (flag) flag.value = "true";


                        input.dispatchEvent(new Event("change"));
                    }
                });
            });

            bootstrap.Modal.getInstance(document.getElementById("priceUpdateModal")).hide();
        });
    }




    return {
        init: function (selectedBrand) {
            initBrandFilter(selectedBrand);
            initFormHandler();
            initLeaveWarning();
            initPriceUpdatePopup();
        }
    };
})(jQuery);






