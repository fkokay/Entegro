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
            const commissionPercent = parseFloat(document.getElementById("percentCommissionValue").value) || 0;
            const shippingFee = parseFloat(document.getElementById("shippingFee").value) || 0;

            const applyCommission = document.getElementById("applyCommission")?.checked;
            const applyShipping = document.getElementById("applyShipping")?.checked;

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
                if (cost <= 0) return;

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

                     
                        let result = oldVal + (cost * percent / 100);
                        alert("maliyet: "+ result);
                      
                        if (applyCommission) {
                            result = result + (result * commissionPercent / 100);
                            alert("komisyon: "+result);
                        }

                        if (applyShipping) {
                            result = result + shippingFee;
                            alert("kargo ücreti: "+ result);
                        }

                        input.value = result.toFixed(2);

                        const flag = row.querySelector(".is-changed-flag");
                        if (flag) flag.value = "true";

                        input.dispatchEvent(new Event("change"));
                    }
                });
            });

            bootstrap.Modal.getInstance(document.getElementById("priceUpdateModal")).hide();
        });
    }

    function initToggleOptions() {
        const applyCommission = document.getElementById("applyCommission");
        const commissionBox = document.getElementById("commissionBox");
        const applyShipping = document.getElementById("applyShipping");
        const shippingBox = document.getElementById("shippingBox");

        if (applyCommission && commissionBox) {
            applyCommission.checked = false; // ilk başta false
            commissionBox.style.display = "none";

            applyCommission.addEventListener("change", function () {
                commissionBox.style.display = this.checked ? "block" : "none";
            });
        }

        if (applyShipping && shippingBox) {
            applyShipping.checked = false; // ilk başta false
            shippingBox.style.display = "none";

            applyShipping.addEventListener("change", function () {
                shippingBox.style.display = this.checked ? "block" : "none";
            });
        }
    }

    function initBrandSelect2(selectedBrand) {
        const $brandDropdown = $('#brandFilter');
        if (!$brandDropdown.length) return;

        $brandDropdown.select2({
            width: '100%',
            placeholder: 'Marka seçiniz',
            allowClear: true,
            language: {
                noResults: () => 'Sonuç bulunamadı'
            }
        });

        
        if (selectedBrand) {
           
            if (selectedBrand === "0" || selectedBrand === "") {
                selectedBrand = "-1";
            }
            $brandDropdown.val(selectedBrand).trigger('change');
        } else {
            // hiç gönderilmediyse default -1 olsun
            $brandDropdown.val("-1").trigger('change');
        }

        $brandDropdown.on('change', function () {
            let selectedBrandId = $(this).val();

            if (!selectedBrandId) {
                // allowClear seçildiyse → 0
                selectedBrandId = 0;
            }

            const newUrl = new URL(window.location.href);
            newUrl.searchParams.set('brandId', selectedBrandId);
            newUrl.searchParams.set('page', 1);

            window.location.href = newUrl.toString();
        });
    }



    return {
        init: function (selectedBrand) {
           
            initFormHandler();
            initLeaveWarning();
            initToggleOptions();
            initPriceUpdatePopup();
            initBrandSelect2(selectedBrand);
        }
    };
})(jQuery);






