var Entegro = Entegro || {};
Entegro.import = Entegro.import || {};

Entegro.import = (function ($) {

    function initImportPage() {
        $(document).ready(function () {

           
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

            $('#includeVariants').on('change', async function () {
                if ($(this).is(':checked')) {
                    $('#variantPreview').show();

                    const xmlUrl = $('#xmlUrlHidden').val();
                    const res = await fetch('/Import/GetFirstVariantPreview?xmlUrl=' + encodeURIComponent(xmlUrl));
                    const html = await res.text();

                    $('#variantData').html(html);
                } else {
                    $('#variantPreview').hide();
                }
            });

            
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

                Swal.fire({
                    icon: 'success',
                    title: 'Eşleştirme tamamlandı',
                    text: 'Şimdi ürünleri seçin.',
                    confirmButtonText: 'Tamam'
                }).then(async (result) => {
                    if (!result.isConfirmed) return;

                    const productRes = await fetch(`/Import/GetProductList?xmlUrl=${encodeURIComponent(xmlUrl)}`);
                    const productData = await productRes.json();

                    if (!productData.success || !Array.isArray(productData.data)) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: productData.message || "Ürünler yüklenemedi."
                        });
                        return;
                    }

                    const allProducts = productData.data;
                    const selectedProducts = new Set();
                    renderProductList(allProducts, selectedProducts);


                    const modal = new bootstrap.Modal(document.getElementById('productSelectModal'));
                    modal.show();

                    $('#searchProductInput').off('input').on('input', function () {
                        const searchTerm = $(this).val().toLowerCase();
                        const filtered = allProducts.filter(p =>
                            (p.Code || '').toLowerCase().includes(searchTerm) ||
                            (p.Name || '').toLowerCase().includes(searchTerm)
                        );
                        renderProductList(filtered, selectedProducts);
                    });


                    $('#btnSelectAll').off('click').on('click', function () {
                        $('.productCheckbox').each(function () {
                            $(this).prop('checked', true);
                            selectedProducts.add($(this).val());
                        });
                    });

                    $('#btnDeselectAll').off('click').on('click', function () {
                        $('.productCheckbox').each(function () {
                            $(this).prop('checked', false);
                        });
                        selectedProducts.clear();
                    });

                    $(document).off('change', '.productCheckbox').on('change', '.productCheckbox', function () {
                        const val = $(this).val();
                        if ($(this).is(':checked')) selectedProducts.add(val);
                        else selectedProducts.delete(val);
                    });

                    $('#btnContinueExport').off('click').on('click', async function () {
                        if (selectedProducts.size === 0) {
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
                            selectedProducts: Array.from(selectedProducts),
                            includeVariants
                        };

                        const res = await fetch('/Import/SaveMapping', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify(payload)
                        });

                        const data = await res.json();

                        if (data.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Kaydedildi',
                                text: 'Eşleştirme ve ürün seçimleri başarıyla kaydedildi. Aktarılması için onay bekleniyor.',
                                confirmButtonText: 'Tamam'
                            }).then(() => {
                                window.location.href = 'list';
                            });
                            modal.hide();
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', text: data.message });
                        }
                    });
                });
            });


          
            function renderProductList(products, selectedProducts) {
                let html = "";
                if (products.length === 0) {
                    html = '<div class="text-center text-muted py-3">Eşleşen ürün bulunamadı</div>';
                } else {
                    html += '<div class="row">';
                    products.forEach((p, i) => {
                        const checked = selectedProducts.has(p.Code) ? 'checked' : '';
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
