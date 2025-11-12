var Entegro = Entegro || {};
Entegro.import = Entegro.import || {};

Entegro.import = (function ($) {

    async function initImportPage() {
        $(document).ready(async function () {

            // ✅ Global state başlat
            window._enteGroSelectedProducts = window._enteGroSelectedProducts || new Set();

            // ✅ ViewBag verilerini yükle
            const savedMappings = window._enteGroSavedMappings || [];
            const includeVariants = window._enteGroIncludeVariants || false;
            const xmlUrl = window._enteGroXmlUrl || "";
            const profileId = window._enteGroProfileId || 0;

            // ✅ Inputlara ata
            $('#xmlUrlHidden').val(xmlUrl);
            $('#profileId').val(profileId);
            $('#includeVariants').prop('checked', includeVariants);

            // ✅ Kayıtlı mappingleri doldur
            if (savedMappings.length > 0) {
                savedMappings.forEach(m => {
                    const $select = $(`select[data-col="${m.ColumnName}"]`);
                    if ($select.length) {
                        if (Array.isArray(m.XmlTags)) {
                            $select.val(m.XmlTags).trigger("change");
                        } else {
                            $select.val([m.XmlTags]).trigger("change");
                        }
                    }
                });
            }

            // 🔹 Select2 başlat
            $('.select2').select2({
                language: "tr",
                placeholder: "XML tag seçiniz",
                allowClear: true,
                width: '100%'
            });

            $('.select2-multi').select2({
                language: "tr",
                placeholder: "Birden fazla image tag seçin...",
                width: '100%'
            });

            // 🔹 Varyant kutusu toggle
            $('#includeVariants').on('change', async function () {
                if ($(this).is(':checked')) {
                    $('#variantPreview').show();
                    $('#variantData').html('<div class="text-muted small">Yükleniyor...</div>');

                    const xmlUrl = $('#xmlUrlHidden').val();
                    try {
                        const res = await fetch('/Import/GetFirstVariantPreview?xmlUrl=' + encodeURIComponent(xmlUrl));
                        const html = await res.text();
                        $('#variantData').html(html);
                    } catch (err) {
                        $('#variantData').html('<div class="text-danger small">Varyant verisi yüklenemedi.</div>');
                    }
                } else {
                    $('#variantPreview').hide();
                }
            });

            // 🔹 Sayfa ilk yüklendiğinde varyant işaretliyse otomatik aç
            if ($('#includeVariants').is(':checked')) {
                $('#variantPreview').show();
                $('#variantData').html('<div class="text-muted small">Varyantlar yükleniyor...</div>');
                const xmlUrl = $('#xmlUrlHidden').val();

                try {
                    const res = await fetch('/Import/GetFirstVariantPreview?xmlUrl=' + encodeURIComponent(xmlUrl));
                    const html = await res.text();
                    $('#variantData').html(html);
                } catch (err) {
                    $('#variantData').html('<div class="text-danger small">Varyant önizleme yüklenemedi.</div>');
                }
            }

            // 🔹 Eşleştirmeleri kaydet
            $('#saveMappings').on('click', async function () {
                const mappings = [];

                // Eşleştirmeleri topla
                $('#mappingForm tbody tr').each(function () {
                    const col = $(this).find('.dbColumnInput').data('col');
                    if (!col) return;

                    const isImage = $(this).find('.isImage').is(':checked');
                    let xmlTags = [];

                    if ($(this).find('.select2-multi').length > 0) {
                        xmlTags = $(this).find('.select2-multi').val() || [];
                    } else if ($(this).find('.select2').length > 0) {
                        const singleTag = $(this).find('.select2').val();
                        if (singleTag) xmlTags.push(singleTag);
                    }

                    if (xmlTags.length > 0) {
                        mappings.push({
                            columnName: col,
                            xmlTags: xmlTags,
                            isImage: isImage
                        });
                    }
                });

                if (mappings.length === 0) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Uyarı',
                        text: 'Devam etmek için önce eşleştirme yapın!',
                        confirmButtonText: 'Tamam'
                    });
                    return;
                }

                const profileId = $('#profileId').val();
                const xmlUrl = $('#xmlUrlHidden').val();

                // 🔹 Popup öncesi loading göster
                if (window.showLoading) {
                    window.showLoading("Lütfen bekleyiniz..");
                }

                // 🔹 Ürünleri getir
                const productRes = await fetch(`/Import/GetProductList?xmlUrl=${encodeURIComponent(xmlUrl)}`);
                const productData = await productRes.json();

                // 🔹 Loading kapat
                if (window.hideLoading) {
                    window.hideLoading();
                }

                if (!productData.success || !Array.isArray(productData.data)) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: productData.message || "Ürünler yüklenemedi."
                    });
                    return;
                }

                Swal.fire({
                    icon: 'success',
                    title: 'Eşleştirme tamamlandı',
                    text: 'Şimdi ürünleri seçin.',
                    confirmButtonText: 'Tamam'
                }).then(async (result) => {
                    if (!result.isConfirmed) return;

                    const allProducts = productData.data;

                    // 🔹 Ürünleri render et (önceden seçilenler işaretli gelsin)
                    renderProductList(allProducts);

                    // 🔹 Modalı göster
                    const modal = new bootstrap.Modal(document.getElementById('productSelectModal'));
                    modal.show();

                    // 🔹 Arama
                    $('#searchProductInput').off('input').on('input', function () {
                        const searchTerm = $(this).val().toLowerCase();
                        const filtered = allProducts.filter(p =>
                            (p.Code || '').toLowerCase().includes(searchTerm) ||
                            (p.Name || '').toLowerCase().includes(searchTerm)
                        );
                        renderProductList(filtered);
                    });

                    // 🔹 Tümünü Seç
                    $('#btnSelectAll').off('click').on('click', function () {
                        $('.productCheckbox').each(function () {
                            $(this).prop('checked', true);
                            window._enteGroSelectedProducts.add($(this).val());
                        });
                    });

                    // 🔹 Seçimleri Kaldır
                    $('#btnDeselectAll').off('click').on('click', function () {
                        $('.productCheckbox').each(function () {
                            $(this).prop('checked', false);
                        });
                        window._enteGroSelectedProducts.clear();
                    });

                    // 🔹 Checkbox değişimi
                    $(document).off('change', '.productCheckbox').on('change', '.productCheckbox', function () {
                        const val = $(this).val();
                        if ($(this).is(':checked')) window._enteGroSelectedProducts.add(val);
                        else window._enteGroSelectedProducts.delete(val);
                    });

                    // 🔹 Devam Et ve Kaydet
                    $('#btnContinueExport').off('click').on('click', async function () {
                        if (window._enteGroSelectedProducts.size === 0) {
                            Swal.fire({
                                icon: 'warning',
                                title: 'Ürün Seçilmedi',
                                text: 'Lütfen en az bir ürün seçiniz!',
                                confirmButtonText: 'Tamam'
                            });
                            return;
                        }

                        const includeVariants = $('#includeVariants').is(':checked');
                        const payload = {
                            profileId,
                            xmlUrl,
                            mappings,
                            selectedProducts: Array.from(window._enteGroSelectedProducts),
                            includeVariants
                        };

                        if (window.showLoading) {
                            window.showLoading("Kaydediliyor...");
                        }

                        const res = await fetch('/Import/SaveMapping', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify(payload)
                        });

                        if (window.hideLoading) {
                            window.hideLoading();
                        }

                        const data = await res.json();

                        if (data.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Kaydedildi',
                                text: 'Eşleştirme ve ürün seçimleri başarıyla kaydedildi.',
                                confirmButtonText: 'Tamam'
                            }).then(() => {
                                modal.hide();
                                window.location.href = '/Import/List';
                            });
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', text: data.message });
                        }
                    });
                });
            });

            // 🔹 Ürün listesi oluşturucu
            function renderProductList(products) {
                let html = "";
                if (products.length === 0) {
                    html = '<div class="text-center text-muted py-3">Eşleşen ürün bulunamadı</div>';
                } else {
                    html += '<div class="row">';
                    products.forEach((p) => {
                        const checked = window._enteGroSelectedProducts.has(p.Code) ? 'checked' : '';
                        html += `
                            <div class="col-md-4 mb-2">
                                <label class="form-check-label d-block border rounded bg-white px-2 py-1">
                                    <input type="checkbox" class="form-check-input me-1 productCheckbox" value="${p.Code}" ${checked}>
                                    <span class="fw-semibold">${p.Code}</span><br>
                                    <small class="text-muted">${p.Name}</small>
                                </label>
                            </div>`;
                    });
                    html += '</div>';
                }

                $('#productListContainer').html(html);
                $('#productCountLabel').text(`${products.length} ürün listelendi`);
            }
        });
    }

    return {
        initImportPage: initImportPage
    };

})(jQuery);
