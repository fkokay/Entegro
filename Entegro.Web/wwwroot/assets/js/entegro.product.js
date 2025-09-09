var Entegro = Entegro || {};
Entegro.product = (function ($) {
    var fullEditor;
    function showMessage(title, message, type = "info", redirectUrl = null, reload = null) {
        Swal.fire({
            title: title,
            text: message,
            icon: type, // success | error | warning | info | question
            confirmButtonText: 'Tamam',
            customClass: { confirmButton: 'btn btn-primary' },
            buttonsStyling: false
        }).then(() => {
            if (redirectUrl) {
                window.location.href = redirectUrl;
            }

            if (reload) {
                location.reload();
            }
        });
    }

    function tabInit(productId) {
        const tabButtons = document.querySelectorAll('#product-tabs button[data-bs-toggle="tab"]');

        tabButtons.forEach(function (button) {
            button.addEventListener('shown.bs.tab', function (event) {
                if (event.target.dataset.bsTarget == "#form-tabs-image") {
                    $("#imageTabContent").load("/Product/LoadTabImages?productId=" + productId, function () {
                        productUploaderInit();
                        productMainPictureClick();
                    });
                }

                if (event.target.dataset.bsTarget == "#form-tabs-categories") {
                    $("#form-tabs-categories").load("/Product/LoadTabCategories?productId=" + productId, function () {
                        categoryDelete();
                    });
                }

                if (event.target.dataset.bsTarget == "#form-tabs-variants") {
                    $("#form-tabs-variants").load("/Product/LoadTabVariants?productId=" + productId, function () {
                        if ($('#SelectedProductAttributeIds').length && !$('#SelectedProductAttributeIds').data('select2')) {
                            $('#SelectedProductAttributeIds').wrap('<div class="position-relative"></div>');
                            $('#SelectedProductAttributeIds').select2({
                                width: '100%',
                                placeholder: 'Varyant seçiniz',
                                allowClear: true,
                                dropdownParent: $('#SelectedProductAttributeIds').parent()
                            });
                        };

                        initVariantsRepeater(window.variantData || []);
                    });
                }
            });
        });
    }

    function productUploaderInit() {
        $("#upload").dropzoneWrapper({
            maxFiles: 500,
            maxFilesSize: 102400,
            timeout: 300000,
            previewContainerId: "preview",
            showRemoveButton: false,
            showRemoveButtonAfterUpload: true,
            downloadEnabled: false
        });
    }

    function productMainPictureClick() {
        $(document).on("click", ".set-main-picture", function (e) {
            var el = $(this).closest('.dz-image-preview');
            var previewContainer = $(this).closest(".preview-container");
            el.insertBefore(previewContainer.find('.dz-image-preview').first());
            previewContainer.trigger("sort", { item: el });
            return false;
        });
    }

    function textEditorInit(selector, initialValueSelector) {
        const fullToolbar = [
            [{ font: [] }, { size: [] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ color: [] }, { background: [] }],
            [{ script: 'super' }, { script: 'sub' }],
            [{ header: '1' }, { header: '2' }, 'blockquote', 'code-block'],
            [{ list: 'ordered' }, { list: 'bullet' }, { indent: '-1' }, { indent: '+1' }],
            [{ direction: 'rtl' }],
            ['link', 'image', 'video', 'formula'],
            ['clean']
        ];

        fullEditor = new Quill(selector, {
            bounds: selector,
            placeholder: 'Açıklama Giriniz...',
            modules: { formula: true, toolbar: fullToolbar },
            theme: 'snow'
        });

        const initialDesc = document.querySelector(initialValueSelector);
        if (initialDesc && initialDesc.value) {
            fullEditor.root.innerHTML = initialDesc.value;
        }
    }

    function getTextEditorValue() {
        return fullEditor ? fullEditor.root.innerHTML : '';
    }

    function formInit(form, url, rediect) {
        const $form = $(form);
        if (!$form.length) return;

        $form.on('submit', function (e) {
            e.preventDefault();

            if (!fullEditor) {
                console.log("Text Editör Düzgün Yüklenemedi");
                return;
            }

            $('#Description').val(getTextEditorValue());

            const data = $form.serialize();

            $.ajax({
                url: url,
                type: 'POST',
                data: data,
                success: function (response) {
                    if (response.success) {
                        showMessage("Başarılı", "Ürün başarıyla kaydedildi.", "success", rediect);
                    } else {
                        showMessage("Hata!", response.message || 'Bir hata oluştu.', "error");
                    }
                },
                error: function (xhr) {
                    howMessage("Hata!", xhr.responseText || 'İşlem sırasında bir hata oluştu.', "error");
                }
            });
        });
    }

    function ProductCategoryCreatePopup(productId) {
        $.ajax({
            url: '/Product/ProductCategoryCreatePopup?productId=' + productId,
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                var popup = $('#ProductCategoryPopup');
                $('#ProductCategoryPopupContent').html(html);
                const categorySelect = $('#CategoryId');
                selectLoad(categorySelect, popup, "/Category/AllCategory");
                categorySave();
                $(popup).modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    }

    function categorySave() {
        $(document).on('click', '#btnSaveProductCategory', function () {
            const payload = {
                productId: Number($('#ProductId').val()) || 0,
                categoryId: Number($('#CategoryId').val()) || 0,
                displayOrder: Number($('#DisplayOrder').val()) || 0
            };

            $.ajax({
                url: '/Product/ProductCategoryInsert',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(payload),
                success: function (json) {
                    if (json?.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı!',
                            text: 'Kategori başarıyla kaydedildi.',
                            confirmButtonText: 'Tamam'
                        }).then(() => {
                            refreshTab("#form-tabs-categories");
                            var popup = $('#ProductCategoryPopup');
                            $(popup).modal('hide');
                        });
                    } else {
                        showMessage("Hata!", json?.errors?.join('\n') || 'Kayıt başarısız.', "error")
                    }
                },
                error: function () {
                    showMessage("Sunucu Hatası!", "İşlem sırasında bir hata oluştu.", "error")
                }
            });
        });
    }
    function categoryDelete() {
        $(document).on('click', '#productCategoryTable .btn-delete', function () {
            const $tr = $(this).closest('tr');
            const mappingId = $tr.data('mapping-id');

            if (mappingId === undefined || mappingId === null) return;

            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu ürün-kategori eşleştirmesi silinecek.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil',
                cancelButtonText: 'Vazgeç'
            }).then((result) => {
                if (!result.isConfirmed) return;

                $.ajax({
                    url: '/Product/ProductCategoryDelete',
                    type: 'POST',
                    data: { id: mappingId },
                    success: function () {
                        $tr.remove();
                        Swal.fire({ icon: 'success', title: 'Silindi', timer: 1200, showConfirmButton: false });
                    },
                    error: function () {
                        Swal.fire({ icon: 'error', title: 'Silme başarısız', text: 'Bir hata oluştu.' });
                    }
                });
            });
        });
    }
    function selectLoad(select,parent, url) {
        $(select).select2({
            placeholder: 'Kategori seçiniz',
            allowClear: true,
            dropdownParent: parent,
            width: '100%',
            ajax: {
                url: url,
                type: 'POST',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term || '', page: params.page || 1
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.results,
                        pagination: {
                            more: data.pagination?.more === true
                        }
                    };
                },
                cache: true
            }
        });
    }
    function refreshTab(tabSelector) {
        const tabButton = document.querySelector(`#product-tabs button[data-bs-target="${tabSelector}"]`);
        if (!tabButton) return;

        const url = tabButton.dataset.url;
        const paneEl = document.querySelector(tabSelector);

        if (url && paneEl) {
            // loading animasyonu istersen
            paneEl.innerHTML = '<div class="p-3 text-center text-muted">Yükleniyor...</div>';

            fetch(url)
                .then(response => response.text())
                .then(html => {
                    paneEl.innerHTML = html;
                    console.log(tabSelector + ' tabı yenilendi.');
                })
                .catch(err => {
                    paneEl.innerHTML = '<div class="p-3 text-danger">Yükleme hatası!</div>';
                    console.error('Tab refresh hatası:', err);
                });
        }
    }
    function initVariantsRepeater(data) {
        const $repeater = $('#ProductVariantAttributeCombinationRepeater');
        if (!$repeater.length) return;

        $repeater.repeater({
            initEmpty: false,
            show: function () { $(this).slideDown(); },
            hide: function (deleteElement) {
                if (confirm('Varyant silinecek emin misiniz?')) {
                    console.log($(this)); // Bu, silinecek olan elemanı temsil eder
                    $(this).slideUp(deleteElement);
                };
            },
            repeaters: [{
                selector: '.ProductVariantAttributeSelectionRepeater',
                initEmpty: false,
                show: function () { $(this).slideDown(); },
                hide: function (deleteElement) { if (confirm('Attribute silinecek emin misiniz?')) $(this).slideUp(deleteElement); }
            }]
        });

        fillRepeater(data || []);
        updateRepeaterIndexes();
    }
    function fillRepeater(data) {
        var $list = $('#ProductVariantAttributeCombinationRepeater [data-repeater-list="ProductVariantAttributeCombinations"]');
        var $template = $list.find('[data-repeater-item]:first').clone(true, true);
        $list.empty();

        if (!data.length) {
            if ($("#SelectedProductAttributeIds").val().length > 0) $list.append($template);
            return;
        }

        $.each(data, function (i, item) {
            var $row = $template.clone(); i
            $row.find('[name$="[Id]"]').val(item.Id);
            $row.find('[name$="[StokCode]"]').val(item.StokCode);
            $row.find('[name$="[ManufacturerPartNumber]"]').val(item.ManufacturerPartNumber);
            $row.find('[name$="[Gtin]"]').val(item.Gtin);
            $row.find('[name$="[Price]"]').val(item.Price);
            $row.find('[name$="[StockQuantity]"]').val(item.StockQuantity);

            $.each(item.ProductVariantAttributeSelections, function (j, attr) {
                var $attrRow = $row.find('[data-repeater-list="ProductVariantAttributeSelections"]');
                if ($attrRow.length) {
                    $attrRow.find('[name$="[ProductVariantAttributeSelections][' + j + '][ProductVariantAttributeId]"]').val(attr.ProductVariantAttributeId);
                    $attrRow.find('[name$="[ProductVariantAttributeSelections][' + j + '][ProductVariantAttributeValueId]"]').val(attr.ProductVariantAttributeValueId);
                }
            });

            $list.append($row);
        });
    }
    function updateRepeaterIndexes() {
        var $list = $('#ProductVariantAttributeCombinationRepeater [data-repeater-list="ProductVariantAttributeCombinations"]');

        $list.children('[data-repeater-item]').each(function (i) {
            var $row = $(this);

            $row.find('input[name*="ProductVariantAttributeCombinations"]').each(function () {
                var name = $(this).attr('name');
                if (!name) return;
                $(this).attr('name', name.replace(/ProductVariantAttributeCombinations\[\d+\]/, 'ProductVariantAttributeCombinations[' + i + ']'));
            });

            $row.find('[data-repeater-list="ProductVariantAttributeSelections"]').children('[data-repeater-item]').each(function (j) {
                var $attr = $(this);
                $attr.find('input, select').each(function () {
                    var name = $(this).attr('name');
                    if (!name) return;

                    var newName = 'ProductVariantAttributeCombinations[' + i + '][ProductVariantAttributeSelections][' + j + ']';
                    if (name.includes('ProductVariantAttributeId')) newName += '[ProductVariantAttributeId]';
                    else if (name.includes('ProductVariantAttributeValueId')) newName += '[ProductVariantAttributeValueId]';

                    $(this).attr('name', newName);
                });
            });
        });
    }

    return {
        tabInit: tabInit,
        textEditorInit: textEditorInit,
        formInit: formInit,
        ProductCategoryCreatePopup: ProductCategoryCreatePopup,

    };
})(jQuery);
