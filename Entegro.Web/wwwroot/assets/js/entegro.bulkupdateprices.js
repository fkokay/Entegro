var Entegro = Entegro || {};
Entegro.bulkupdateprices = Entegro.bulkupdateprices || {};

Entegro.bulkupdateprices = (function ($) {
    let isFormChanged = false;

    function initFormHandler() {
        const form = document.getElementById('integration-form');
        if (!form) return;
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

                    // satırı renklendir
                    row.classList.add("row-changed");
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
                return; 
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

                        // Kaydedildikten sonra renkleri temizle
                        form.querySelectorAll("tr.row-changed").forEach(r => {
                            r.classList.remove("row-changed");
                        });
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
                // 4. sütun maliyet fiyatı
                const costText = row.querySelector("td:nth-child(4)")?.innerText || "0";
                const cost = parseFloat(costText.replace(",", ".")) || 0;
                if (cost <= 0) return;

                columns.forEach(col => {
                    let input;

                    if (col === "Price") {
                        input = row.querySelector("input[name$='Price']");
                    }
                    else if (col === "SalePrice") {
                        input = row.querySelector("input[name*='SalePrice']");
                    }
                    else {
                        input = row.querySelector(`input[name*='IntegrationPrices[${col}]'][name$='.Price']`);
                    }

                    if (input) {
                        const oldVal = parseFloat(input.value.replace(",", ".")) || 0;

                        let result = oldVal + (cost * percent / 100);

                        if (applyShipping) {
                            result = result + shippingFee;
                        }

                        if (applyCommission) {
                            result = result + (result * commissionPercent / 100);
                        }

                        input.value = result.toFixed(2);

                        // değişiklik flag
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
            applyCommission.checked = false;
            commissionBox.style.display = "none";

            applyCommission.addEventListener("change", function () {
                commissionBox.style.display = this.checked ? "block" : "none";
            });
        }

        if (applyShipping && shippingBox) {
            applyShipping.checked = false;
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
    function initExcelImport(selectedBrandId) {
        const fileInput = document.getElementById("excelFile");
        if (!fileInput) return;

        fileInput.addEventListener("change", async function () {
            const file = this.files[0];
            const brandId = selectedBrandId || 0;

            if (!file) {
                Swal.fire({ icon: "warning", title: "Dosya Seçilmedi", text: "Lütfen bir Excel dosyası seçiniz." });
                return;
            }

            const allowedExtensions = [".xls", ".xlsx"];
            const fileExt = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
            if (!allowedExtensions.includes(fileExt)) {
                Swal.fire({ icon: "error", title: "Geçersiz Dosya Tipi", text: "Sadece Excel (.xls, .xlsx) dosyaları yüklenebilir." });
                return;
            }

            const data = await file.arrayBuffer();
            const workbook = XLSX.read(data, { type: "array" });
            const firstSheetName = workbook.SheetNames[0];
            const worksheet = workbook.Sheets[firstSheetName];
            const headers = XLSX.utils.sheet_to_json(worksheet, { header: 1 })[0];


            let htmlContent = "<div class='text-start'>";
            headers.forEach((h, i) => {
                if (!h) return;

                // Eğer başlık "ProductId" veya "Id" ise → seçili ve disabled
                if (h.toLowerCase() === "productid" || h.toLowerCase() === "id") {
                    htmlContent += `
                    <div>
                        <input type="checkbox" id="chk_${i}" value="${h}" checked disabled>
                        <label for="chk_${i}">${h} (zorunlu)</label>
                    </div>`;
                } else {
                    htmlContent += `
                    <div>
                        <input type="checkbox" id="chk_${i}" value="${h}">
                        <label for="chk_${i}">${h}</label>
                    </div>`;
                }
            });
            htmlContent += "</div>";


            Swal.fire({
                title: "Başlıkları Seç",
                html: htmlContent,
                showCancelButton: true,
                confirmButtonText: "Yükle",
                cancelButtonText: "İptal",
                preConfirm: () => {
                    const selected = [];
                    headers.forEach((h, i) => {
                        const chk = document.getElementById("chk_" + i);
                        if (chk && (chk.checked || chk.disabled)) {
                            selected.push(h);
                        }
                    });

                    if (selected.length <= 1) {
                        Swal.showValidationMessage("Id dışında en az bir başlık seçmelisiniz!");
                    }

                    return selected;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const selectedHeaders = result.value;

                    const formData = new FormData();
                    formData.append("File", file);
                    formData.append("BrandId", brandId);

                    selectedHeaders.forEach((h, i) => {
                        formData.append(`SelectedHeaders[${i}]`, h);
                    });

                    window.showLoading("Lütfen bekleyiniz..");

                    fetch(window.excelImportUrl, {
                        method: "POST",
                        body: formData
                    })
                        .then(response => {
                            if (!response.ok) throw new Error("Yükleme başarısız");
                            return response.json();
                        })
                        .then(data => {
                            window.hideLoading();

                            let message = `Excel başarıyla yüklendi. ${data.count} kayıt işlendi.`;
                            if (data.notUpdatedProductCount && data.notUpdatedProductCount > 0) {
                                message += ` ${data.notUpdatedProductCount} ürün fiyatı güncellenemedi.`;
                            }

                            Swal.fire({
                                icon: "success",
                                title: "Başarılı",
                                text: message
                            }).then(() => location.reload());
                        })
                        .catch(error => {
                            window.hideLoading();
                            Swal.fire({ icon: "error", title: "Hata", text: error.message });
                        });

                } else {
                    Swal.fire({
                        icon: "info",
                        title: "İptal Edildi",
                        text: "Yükleme işlemi iptal edildi."
                    }).then(() => location.reload());
                }
            });
        });
    }


    return {
        init: function (selectedBrand) {
           
            initFormHandler();
            initLeaveWarning();
            initToggleOptions();
            initPriceUpdatePopup();
            initBrandSelect2(selectedBrand);
            initExcelImport(selectedBrand);
        }
    };
})(jQuery);






