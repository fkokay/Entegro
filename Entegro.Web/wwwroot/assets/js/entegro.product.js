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

    function Init() {
        if ($('#SelectedProductAttributeIds').length && !$('#SelectedProductAttributeIds').data('select2')) {
            $('#SelectedProductAttributeIds').wrap('<div class="position-relative"></div>');
            $('#SelectedProductAttributeIds').select2({
                width: '100%',
                placeholder: 'Varyant seçiniz',
                allowClear: true,
                dropdownParent: $('#SelectedProductAttributeIds').parent()
            });
        };
    }

    function TabsInit() {
        const tabButtons = document.querySelectorAll('#product-tabs button[data-bs-toggle="tab"]');

        tabButtons.forEach(function (button) {
            button.addEventListener('shown.bs.tab', function (event) {
                if (event.target.dataset.bsTarget == "#form-tabs-images") {
                    if (event.target.dataset.url.length > 0) {
                        $("#form-tabs-images").load(event.target.dataset.url, function () {
                            ProductImageDropzone();
                            ProductImageMianPicture();
                        });
                    }

                }

                if (event.target.dataset.bsTarget == "#form-tabs-categories") {
                    if (event.target.dataset.url.length > 0) {
                        $("#form-tabs-categories").load(event.target.dataset.url, function () {
                            ProductCategoryDelete();
                        });
                    }
                }
            });
        });
    }

    function ProductImageDropzone() {
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

    function ProductImageMianPicture() {
        $(document).on("click", ".set-main-picture", function (e) {
            var el = $(this).closest('.dz-image-preview');
            var previewContainer = $(this).closest(".preview-container");
            el.insertBefore(previewContainer.find('.dz-image-preview').first());
            previewContainer.trigger("sort", { item: el });
            return false;
        });
    }

    function DescriptionEditor() {
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

        fullEditor = new Quill("#full-editor", {
            bounds: "#full-editor",
            placeholder: 'Açıklama Giriniz...',
            modules: { formula: true, toolbar: fullToolbar },
            theme: 'snow'
        });

        const description = document.querySelector("#Description");
        if (description && description.value) {
            fullEditor.root.innerHTML = description.value;
        }
    }

    function ProductCategoryCreatePopup(productId) {
        var popup = $('#ProductCategoryPopup');
        var popupContent = $("#ProductCategoryPopupContent");
        $.ajax({
            url: '/Product/ProductCategoryCreatePopup?productId=' + productId,
            type: 'GET',
            dataType: 'html',
            success: function (html) {
                $(popupContent).html(html);

                ProductCategoryCreatePopupInit(popup, popupContent);

                $(popup).modal('show');
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('Form yüklenemedi.');
            }
        });
    }

    function ProductCategoryCreatePopupInit(popup, popupContent) {
        var categorySelect = $(popupContent).find("#CategoryId");
        $(categorySelect).select2({
            placeholder: 'Kategori seçiniz',
            allowClear: true,
            dropdownParent: popup,
            width: '100%',
            ajax: {
                url: "/Category/AllCategory",
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
                            RefreshTab("#form-tabs-categories");
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

    function ProductCategoryDelete() {
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

    function ProductVariantAttributeCombinationRepeaterInit(data) {
        const $repeater = $('#ProductVariantAttributeCombinationRepeater');
        if (!$repeater.length) return;

        $repeater.repeater({
            initEmpty: false,
            show: function () { $(this).slideDown(); },
            hide: function (deleteElement) {
                if (confirm('Varyant silinecek emin misiniz?')) {
                    $(this).slideUp(deleteElement);
                };
            },
            repeaters: [{
                selector: '.ProductVariantAttributeSelectionRepeater',
                initEmpty: false,
                show: function () { $(this).slideDown(); },
                hide: function (deleteElement) {
                    if (confirm('Attribute silinecek emin misiniz?')) {
                        $(this).slideUp(deleteElement); 
                    }
                }
            }]
        });

        ProductVariantAttributeCombinationRepeaterLoad(data || []);
        ProductVariantAttributeCombinationRepeaterIndexes();
    }

    function ProductVariantAttributeCombinationRepeaterLoad(data) {
        var $list = $('#ProductVariantAttributeCombinationRepeater [data-repeater-list="ProductVariantAttributeCombinations"]');
        var $template = $list.find('[data-repeater-item]:first').clone(true, true);
        $list.empty();

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

    function ProductVariantAttributeCombinationRepeaterIndexes() {
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

    function Validation() {
        const formValidation = FormValidation.formValidation(
            document.getElementById('product-form'),
            {
                locale: 'tr_TR',
                localization: FormValidation.locales.tr_TR,
                fields: {
                    'Name': {
                        validators: {
                            notEmpty: { message: 'Ürün adı boş bırakılamaz.' },
                            stringLength: { min: 3, message: 'Ürün adı en az 3 karakter olmalıdır.' }
                        }
                    },
                    'Code': {
                        validators: {
                            notEmpty: { message: 'Ürün kodu boş bırakılamaz.' }
                        }
                    },
                    'Price': {
                        validators: {
                            notEmpty: { message: 'Fiyat boş bırakılamaz.' },
                            numeric: { message: 'Fiyat yalnızca sayı olabilir.' }
                        }
                    },
                    'Currency': {
                        validators: { notEmpty: { message: 'Para birimi seçilmelidir.' } }
                    },
                    'StockQuantity': {
                        validators: {
                            notEmpty: { message: 'Stok miktarı boş bırakılamaz.' },
                            integer: { message: 'Stok miktarı tam sayı olmalıdır.' }
                        }
                    },
                    'Weight': {
                        validators: {
                            notEmpty: { message: 'Ağırlık boş bırakılamaz.' },
                            numeric: { message: 'Ağırlık geçerli bir sayı olmalıdır.' }
                        }
                    },
                    'Length': {
                        validators: {
                            notEmpty: { message: 'Uzunluk boş bırakılamaz.' },
                            numeric: { message: 'Uzunluk geçerli bir sayı olmalıdır.' }
                        }
                    },
                    'Width': {
                        validators: {
                            notEmpty: { message: 'Genişlik boş bırakılamaz.' },
                            numeric: { message: 'Genişlik geçerli bir sayı olmalıdır.' }
                        }
                    },
                    'Height': {
                        validators: {
                            notEmpty: { message: 'Yükseklik boş bırakılamaz.' },
                            numeric: { message: 'Yükseklik geçerli bir sayı olmalıdır.' }
                        }
                    },
                    'VatRate': {
                        validators: {
                            notEmpty: { message: 'Kdv oranı boş bırakılamaz.' },
                            numeric: { message: 'Kdv oranı geçerli bir sayı olmalıdır.' }
                        }
                    },
                    'Barcode': {
                        validators: {
                            stringLength: { max: 100, message: 'Barkod en fazla 100 karakter olabilir.' }
                        }
                    },
                    'MetaTitle': {
                        validators: {
                            stringLength: { max: 60, message: 'Meta başlık en fazla 60 karakter olabilir.' }
                        }
                    },
                    'MetaKeywords': {
                        validators: {
                            stringLength: { max: 150, message: 'Anahtar kelimeler en fazla 150 karakter olabilir.' }
                        }
                    },
                    'MetaDescription': {
                        validators: {
                            stringLength: { max: 160, message: 'Meta açıklama en fazla 160 karakter olabilir.' }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap5: new FormValidation.plugins.Bootstrap5({
                        eleValidClass: '',
                        rowSelector: '.mb-3'
                    }),
                    submitButton: new FormValidation.plugins.SubmitButton(),
                    autoFocus: new FormValidation.plugins.AutoFocus()
                },
                init: (instance) => {
                    instance.on('plugins.message.placed', function (e) {
                        if (e.element.parentElement.classList.contains('input-group')) {
                            e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
                        }
                    });

                    instance.on('core.field.invalid', function (e) {
                        const fieldEl = e.elements && e.elements.length ? e.elements[0] : null;
                        if (fieldEl) FocusFieldAndShowTab(fieldEl);
                    });

                    instance.on('core.form.invalid', function () {
                        const invalidEl = document.querySelector('[data-field].is-invalid, .is-invalid');
                        if (invalidEl) FocusFieldAndShowTab(invalidEl);
                    });

                    instance.on('core.form.valid', function () {
                        if (typeof fullEditor !== "undefined" && fullEditor.root) {
                            document.getElementById('Description').value = fullEditor.root.innerHTML;
                        }

                        const form = $('#product-form');
                        const action = $('#product-form').attr("action");
                        const formData = $(form).serialize();

                        $.ajax({
                            url: action,
                            type: 'POST',
                            data: formData,
                            success: function (response) {
                                if (response.success) {
                                    showMessage("Başarılı!", 'Ürün başarıyla kaydedildi.', "success", "/Product/List");
                                } else {
                                    showMessage("Hata!", response.message || 'Bir hata oluştu.', "error");
                                }
                            },
                            error: function (xhr) {
                                showMessage("Hata!", xhr.responseText || 'İşlem sırasında bir hata oluştu.', "error");
                            }
                        });
                    });
                }
            }
        );

        document.querySelectorAll('[name^="ProductVariantAttributeCombinations"][name$="[StokCode]"]').forEach(function (el) {
            const name = el.getAttribute('name');

            formValidation.addField(name, {
                validators: {
                    notEmpty: { message: 'Varyant Stok kodu boş bırakılamaz.' },
                    stringLength: {
                        max: 50,
                        message: 'Varyant Stok kodu en fazla 50 karakter olabilir.'
                    }
                }
            });

            el.addEventListener('input', function () {
                formValidation.revalidateField(name);
            });
        });
        document.querySelectorAll('[name^="ProductVariantAttributeCombinations"][name$="[Price]"]').forEach(function (el) {
            const name = el.getAttribute('name');

            formValidation.addField(name, {
                validators: {
                    notEmpty: { message: 'Varyant fiyatı boş bırakılamaz.' },
                }
            });

            el.addEventListener('input', function () {
                formValidation.revalidateField(name);
            });
        });
        document.querySelectorAll('[name^="ProductVariantAttributeCombinations"][name$="[StockQuantity]"]').forEach(function (el) {
            const name = el.getAttribute('name');

            formValidation.addField(name, {
                validators: {
                    notEmpty: { message: 'Varyant stok miktarı boş bırakılamaz.' },
                }
            });

            el.addEventListener('input', function () {
                formValidation.revalidateField(name);
            });
        });
    }

    function FocusFieldAndShowTab(fieldEl) {
        const tabPane = fieldEl.closest('.tab-pane');
        if (tabPane) {
            const tabId = tabPane.getAttribute('id');
            const tabTrigger = document.querySelector(`[data-bs-target="#${tabId}"]`);
            if (tabTrigger) new bootstrap.Tab(tabTrigger).show();
        }
        fieldEl.focus();
    }

    function RefreshTab(tabSelector) {
        const tabButton = document.querySelector(`#product-tabs button[data-bs-target="${tabSelector}"]`);
        if (!tabButton) return;

        const url = tabButton.dataset.url;
        const paneEl = document.querySelector(tabSelector);

        if (url && paneEl) {
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

    return {
        Init: Init,
        TabsInit: TabsInit,
        DescriptionEditor: DescriptionEditor,
        ProductCategoryCreatePopup: ProductCategoryCreatePopup,
        ProductVariantAttributeCombinationRepeaterInit: ProductVariantAttributeCombinationRepeaterInit,
        Validation: Validation,
    };
})(jQuery);
