var Entegro = Entegro || {};
Entegro.product = (function ($) {
    var fullEditor;

    // -------------------- IMAGE TAB --------------------
    function initImageTab(productId) {
        $('button[data-bs-target="#form-tabs-image"]').on('shown.bs.tab', function () {
            if (productId > 0) {
                $("#imageTabContent").load("/Product/ProductImagesPartial?id=" + productId, function () {
                    initProductImageUploader();
                });
            } else {
                $("#imageTabContent").html('<div class="alert alert-danger">Bu ürüne resim yüklemek için önce ürünü kaydetmeniz gerekmektedir.</div>');
            }
        });
    }

    function initProductImageUploader() {
        $("#upload-1388498530").dropzoneWrapper({
            maxFiles: 500,
            maxFilesSize: 102400,
            timeout: 300000,
            previewContainerId: "preview-106864758",
            showRemoveButton: false,
            showRemoveButtonAfterUpload: true,
            downloadEnabled: false
        });

        $(document).on("click", ".set-main-picture", function (e) {
            var el = $(this).closest('.dz-image-preview');
            var previewContainer = $(this).closest(".preview-container");
            el.insertBefore(previewContainer.find('.dz-image-preview').first());
            previewContainer.trigger("sort", { item: el });
            return false;
        });
    }

    // -------------------- QUILL EDITOR --------------------
    function initFullEditor(selector, initialValueSelector) {
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

    function getFullEditorHtml() {
        return fullEditor ? fullEditor.root.innerHTML : '';
    }

    // -------------------- FORM SUBMIT --------------------
    function initFormSubmit(formSelector, urlSubmit, redirectUrl) {
        const $form = $(formSelector);
        if (!$form.length) return;

        $form.on('submit', function (e) {
            e.preventDefault();
            if (!fullEditor) return;

            // Quill içeriğini gizli inputa aktar
            $('#Description').val(getFullEditorHtml());

            const data = $form.serialize();

            $.ajax({
                url: urlSubmit,
                type: 'POST',
                data: data,
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: 'Başarılı!',
                            text: 'Ürün başarıyla kaydedildi.',
                            icon: 'success',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-success' },
                            buttonsStyling: false
                        }).then(() => {
                            if (redirectUrl) window.location.href = redirectUrl;
                        });
                    } else {
                        Swal.fire({
                            title: 'Hata!',
                            text: response.message || 'Bir hata oluştu.',
                            icon: 'error',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-danger' },
                            buttonsStyling: false
                        });
                    }
                },
                error: function (xhr) {
                    Swal.fire({
                        title: 'Hata!',
                        text: xhr.responseText || 'İşlem sırasında bir hata oluştu.',
                        icon: 'error',
                        confirmButtonText: 'Tamam',
                        customClass: { confirmButton: 'btn btn-danger' },
                        buttonsStyling: false
                    });
                }
            });
        });
    }

    // -------------------- PRODUCT CATEGORIES --------------------
    function initCategoryModal(modalSelector, tableSelector) {
        const $modal = $(modalSelector);
        if (!$modal.length) return;

        $modal.appendTo('body');

        $modal.on('shown.bs.modal', function () {
            const $cat = $('#CategoryId');
            if (!$cat.data('select2')) {
                $cat.select2({
                    placeholder: 'Kategori seçiniz',
                    allowClear: true,
                    dropdownParent: $modal,
                    width: '100%',
                    ajax: {
                        url: '/Category/AllCategory',
                        type: 'POST',
                        dataType: 'json',
                        delay: 250,
                        data: function (params) { return { term: params.term || '', page: params.page || 1 }; },
                        processResults: function (data, params) {
                            params.page = params.page || 1;
                            return { results: data.results, pagination: { more: data.pagination?.more === true } };
                        },
                        cache: true
                    }
                });
            }
            /* clearCategoryValidationUI();*/
        });

        $(document).on('click', '#btnSaveProductCategory', function () {
            //if (!validateProductCategoryModal().valid) return;

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
                            location.reload();
                        });
                        //$modal.modal('hide');
                        //loadProductCategories($('#productId').val(), tableSelector);

                    } else {
                        Swal.fire({ icon: 'error', title: 'Hata!', text: json?.errors?.join('\n') || 'Kayıt başarısız.' });
                    }
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Sunucu Hatası!', text: 'İşlem sırasında bir hata oluştu.' });
                }
            });
        });

        // Silme işlemi
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

    function initCategoryTabLoader(tabBtnSelector, tabPaneSelector) {
        const tabBtn = document.querySelector(tabBtnSelector);
        const tabPane = document.querySelector(tabPaneSelector);

        if (!tabBtn || !tabPane) return;

        tabBtn.addEventListener('shown.bs.tab', async function () {
            const loaded = tabPane.getAttribute('data-loaded') === 'true';
            if (loaded) return;

            const url = tabBtn.getAttribute('data-url');
            if (!url) return;

            // İsteğe bağlı: Yükleniyor mesajı
            tabPane.innerHTML = `<div class="m-3">Yükleniyor...</div>`;

            try {
                const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
                if (!res.ok) throw new Error('Yükleme başarısız: ' + res.status);

                const html = await res.text();
                tabPane.innerHTML = html;
                tabPane.setAttribute('data-loaded', 'true');
            } catch (err) {
                tabPane.innerHTML = `<div class="alert alert-danger m-3">Kategori içerikleri yüklenemedi. ${err.message}</div>`;
            }
        });
    }


    // -------------------- REPEATER / VARIANTS --------------------
    function initVariantsTabLoader(tabBtnSelector, tabPaneSelector) {
        const tabBtn = document.querySelector(tabBtnSelector);
        const tabPane = document.querySelector(tabPaneSelector);

        if (!tabBtn || !tabPane) return;

        tabBtn.addEventListener('shown.bs.tab', async function (e) {
            const loaded = tabPane.getAttribute('data-loaded') === 'true';
            if (loaded) return;

            const url = tabBtn.getAttribute('data-url');
            if (!url) return;

            const loadingEl = document.getElementById('variants-loading');
            if (loadingEl) loadingEl.style.display = 'block';

            try {
                const res = await fetch(url, { method: 'GET', headers: { 'X-Requested-With': 'XMLHttpRequest' } });
                if (!res.ok) throw new Error('Sunucu hatası: ' + res.status);
                const html = await res.text();

                tabPane.innerHTML = html;
                tabPane.setAttribute('data-loaded', 'true');

                // repeater ve select2 initialize
                Entegro.product.initVariantsRepeater(window.variantData || []);

                if ($('#SelectedProductAttributeIds').length && !$('#SelectedProductAttributeIds').data('select2')) {
                    $('#SelectedProductAttributeIds').wrap('<div class="position-relative"></div>');
                    $('#SelectedProductAttributeIds').select2({
                        width: '100%',
                        placeholder: 'Varyant seçiniz',
                        allowClear: true,
                        dropdownParent: $('#SelectedProductAttributeIds').parent()
                    });
                };

            } catch (err) {
                tabPane.innerHTML = `
                <div class="alert alert-danger m-3">
                    Varyant içeriği yüklenemedi. Detay: ${err.message}
                </div>`;
            } finally {
                if (loadingEl) loadingEl.style.display = 'none';
            }
        });
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

    // -------------------- ACTIVE TAB --------------------
    function activateTab(paneSelector) {
        const triggerEl =
            document.querySelector(`[data-bs-target="${paneSelector}"]`) ||
            document.querySelector(`a[href="${paneSelector}"]`);
        if (!triggerEl || !window.bootstrap?.Tab) return;

        const tab = new bootstrap.Tab(triggerEl);
        tab.show();

        const pane = document.querySelector(paneSelector);
        if (pane) pane.scrollIntoView({ behavior: 'smooth', block: 'start' });

        // localStorage ile aktif tab kaydet
        localStorage.setItem('productActiveTab', paneSelector);
    }

    function restoreActiveTab() {
        const activeTab = localStorage.getItem('productActiveTab');
        if (activeTab) {
            activateTab(activeTab);
            localStorage.removeItem('productActiveTab');
        }
    }

    function saveActiveTab(tabSelector) {
        if (tabSelector) localStorage.setItem('productActiveTab', tabSelector);
    }

    function initTabPersistence() {
        const tabButtons = document.querySelectorAll('[data-bs-toggle="tab"]');
        tabButtons.forEach(btn => {
            btn.addEventListener('shown.bs.tab', function (e) {
                const target = e.target.getAttribute('data-bs-target') || e.target.getAttribute('href');
                saveActiveTab(target);
            });
        });
    }

    // -------------------- PUBLIC API --------------------
    return {
        initImageTab: initImageTab,
        initFullEditor: initFullEditor,
        getFullEditorHtml: getFullEditorHtml,
        initFormSubmit: initFormSubmit,
        initCategoryModal: initCategoryModal,
        initVariantsTabLoader: initVariantsTabLoader,
        initVariantsRepeater: initVariantsRepeater,
        fillRepeater: fillRepeater,
        updateRepeaterIndexes: updateRepeaterIndexes,
        initCategoryTabLoader: initCategoryTabLoader,
        activateTab: activateTab,
        saveActiveTab: saveActiveTab,
        restoreActiveTab: restoreActiveTab,
        initTabPersistence: initTabPersistence
    };
})(jQuery);
