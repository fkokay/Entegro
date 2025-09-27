var Entegro = Entegro || {};
Entegro.import = Entegro.import || {};

Entegro.import = (function ($) {

    function initXmlImportFormValidation() {
        const form = document.getElementById("importFormXml");
        if (!form) return;

        form.addEventListener("submit", function (e) {
            const profileNameInput = document.querySelector('input[name="ProfileName"]');
            const urlInput = document.querySelector('input[name="MediaFileUrl"]');

            if (!profileNameInput || !profileNameInput.value.trim()) {
                e.preventDefault();
                alert("Lütfen profil adı giriniz.");
                profileNameInput.classList.add("is-invalid");
                profileNameInput.focus();
                return;
            } else {
                profileNameInput.classList.remove("is-invalid");
            }

            const urlValue = urlInput.value.trim();
            const urlPattern = /^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$/;

            if (!urlValue) {
                e.preventDefault();
                alert("Lütfen bir URL giriniz.");
                urlInput.classList.add("is-invalid");
                urlInput.focus();
                return;
            } else if (!urlPattern.test(urlValue)) {
                e.preventDefault();
                alert("Lütfen geçerli bir URL giriniz (örn: https://example.com).");
                urlInput.classList.add("is-invalid");
                urlInput.focus();
                return;
            } else {
                urlInput.classList.remove("is-invalid");
            }
        });
    }

    function initSelect2() {
        $(document).ready(function () {
            $('.select2').select2({
                language: "tr",
                placeholder: "Bir seçenek seçin",
                allowClear: true,
                width: '100%'
            });
        });
    }
    function validateXmlProductForm() {
        const barcode = $('[name="ProductImport.Barcode"]').val();
        const productName = $('[name="ProductImport.Name"]').val();

        if (!barcode) {
            alert("Lütfen Genel Bilgiler bölümündeki Barkod alanını doldurun.");
            return false;
        }

        if (!productName) {
            alert("Lütfen Genel Bilgiler bölümündeki Ürün Adı alanını doldurun.");
            return false;
        }

        const stockCode = $('[name="ProductImport.Code"]').val();
        const stockQuantity = $('[name="ProductImport.StockQuantity"]').val();

        if (!stockCode || !stockQuantity) {
            alert("Lütfen Stok Bilgileri bölümündeki Stok Kodu ve Stok Adedi alanlarını doldurun.");
            return false;
        }

        const price = $('[name="ProductImport.Price"]').val();

        if (!price) {
            alert("Lütfen Fiyat Bilgileri bölümündeki Mağaza Satış Fiyatı alanını doldurun.");
            return false;
        }

        const applyPriceAdjustment = $('[name="ApplyPriceAdjustment"]:checked').val();
        if (applyPriceAdjustment === "uygula") {
            const priceAdjustmentType = $('[name="PriceAdjustmentType"]').val();
            const priceAdjustmentAmount = $('[name="PriceAdjustmentAmount"]').val();

            if (!priceAdjustmentType || !priceAdjustmentAmount) {
                alert("Lütfen Fiyat Artışı işlem türü ve işlem tutarı alanlarını doldurun.");
                return false;
            }
        }

        const isVariantProduct = $('input[name="IsVariantProduct"]:checked').val();
        if (isVariantProduct === "yes") {
            const variantPrice = $('[name="AttributePrice"]').val();
            const variantStockQty = $('[name="AttributeStockQuantity"]').val();

            if (!variantPrice || !variantStockQty) {
                alert("Lütfen Varyant Fiyat ve Varyant Stok Miktarı alanlarını doldurun.");
                return false;
            }
        }

        const selectedImages = $('[name="ProductImport.Images"]').val();
        $('#SelectedImagePaths').val(selectedImages && selectedImages.length > 0 ? selectedImages.join(',') : '');

        const selectedAttributeSpecifications = $('[name="AttributeSpecifications"]').val();
        $('#SelectedAttributeSpecifications').val(selectedAttributeSpecifications && selectedAttributeSpecifications.length > 0 ? selectedAttributeSpecifications.join(',') : '');
        // Dinamik alanlar init
        initPriceAdjustmentToggle();
        initVariantToggle();
        return true;
    }
   
    function initPriceAdjustmentToggle() {
        const islemTuruAlanlari = document.getElementById("islemTuruAlanlari");
        const islemTuru = document.querySelector('[name="PriceAdjustmentType"]');
        const islemTutari = document.querySelector('[name="PriceAdjustmentAmount"]');
        const fiyatArtisiRadioButtons = document.querySelectorAll('input[name="ApplyPriceAdjustment"]');

        function toggleIslemTuru() {
            const selectedValue = document.querySelector('input[name="ApplyPriceAdjustment"]:checked')?.value;

            if (selectedValue === "uygula") {
                islemTuruAlanlari.style.display = "flex";
                islemTuru?.setAttribute("required", "required");
                islemTutari?.setAttribute("required", "required");
            } else {
                islemTuruAlanlari.style.display = "none";
                islemTuru?.removeAttribute("required");
                islemTutari?.removeAttribute("required");
            }
        }

        toggleIslemTuru();

        fiyatArtisiRadioButtons.forEach(radio => {
            radio.addEventListener("change", toggleIslemTuru);
        });
    }
    function initVariantToggle() {
        const variantFieldsRow = document.getElementById("variantFieldsRow");
        const variantRadios = document.querySelectorAll('input[name="IsVariantProduct"]');
        const attrStockQuantity = document.getElementById("variantStockQuantity");
        const attrPrice = document.getElementById("variantPrice");

        function toggleVariantFields() {
            const selected = document.querySelector('input[name="IsVariantProduct"]:checked').value;

            if (selected === "yes") {
                variantFieldsRow.style.display = "flex";
                attrStockQuantity?.setAttribute("required", "required");
                attrPrice?.setAttribute("required", "required");
            } else {
                variantFieldsRow.style.display = "none";
                attrStockQuantity?.removeAttribute("required");
                attrPrice?.removeAttribute("required");
            }
        }

        toggleVariantFields();

        variantRadios.forEach(radio => {
            radio.addEventListener("change", toggleVariantFields);
        });
    }


    return {
        validateXmlProductForm: validateXmlProductForm,
        initXmlImportFormValidation: initXmlImportFormValidation,
        initSelect2: initSelect2
    };

})(jQuery);
