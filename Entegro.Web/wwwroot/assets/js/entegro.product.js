var Entegro = Entegro || {};
Entegro.product = (function ($) {
    var fullEditor;
    var formValidation;
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
    function addFilterDropdown(column, containerSelector, placeholder, map = null) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        let select = document.createElement("select");
        select.className = "form-select select2 text-capitalize";
        select.innerHTML = `<option value="">${placeholder}</option>`;
        container.appendChild(select);

        if (map && Array.isArray(map)) {
            map.forEach(item => {
                let option = document.createElement("option");
                option.value = item.id;
                option.textContent = item.title;
                select.appendChild(option);
            });
        } else {
            column.data().unique().sort().each(function (value) {
                if (value !== null && value !== undefined && value !== "") {
                    let option = document.createElement("option");
                    option.value = value;
                    option.textContent = value;
                    select.appendChild(option);
                }
            });
        }

        if (window.jQuery && $(select).select2) {
            $(select).select2({
                placeholder: placeholder,
                allowClear: true,
                width: "resolve"
            });


            $(select).on("change", function () {
                const val = this.value || "";
                column.search(val, false, false).draw();
            });
        } else {
            select.addEventListener("change", function () {
                const val = select.value ? `^${select.value}$` : "";
                column.search(val, true, false).draw();
            });
        }

    }
    function addFilterText(column, containerSelector, placeholder) {
        const container = document.querySelector(containerSelector);
        if (!container) {
            console.warn(`Filter container bulunamadı: ${containerSelector}`);
            return;
        }

        // input elementini oluştur
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control";
        input.placeholder = placeholder;

        container.appendChild(input);

        // her yazımda filtre uygula (debounce ile optimize edebilirsin)
        input.addEventListener("keyup", function () {
            const val = input.value || "";
            column.search(val, false, true).draw();
        });
    }
    function Init() {
        document.querySelector('#SaveBtn').addEventListener('click', function (e) {
            e.preventDefault();

            formValidation.validate().then(function (status) {
                if (status === 'Valid') {
                    if (typeof fullEditor !== "undefined" && fullEditor.root) {
                        document.getElementById('Description').value = fullEditor.root.innerHTML;
                    }

                    const form = $('#product-form');
                    const action = form.attr("action");
                    const formData = form.serialize();

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
                } else {
                    console.log("Form hatalı, submit iptal.");
                }
            });
        });
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
                    var id = event.target.dataset.productId;
                    if (id && id > 0) {
                        Entegro.product.initProductCategoryTable(id);
                    }
                }
                if (event.target.dataset.bsTarget == "#form-tabs-crosssaleproduct") {

                    var id = event.target.dataset.productId;
                    if (id && id > 0) {
                        Entegro.product.initCrossSellList(id);
                    }
                }
                if (event.target.dataset.bsTarget == "#form-tabs-relatedProduct") {

                    var id = event.target.dataset.productId;
                    if (id && id > 0) {
                        Entegro.product.initRelatedProductList(id);
                    }
                }
                if (event.target.dataset.bsTarget == "#form-tabs-attributes") {

                    var id = event.target.dataset.productId;
                    if (id && id > 0) {
                        Entegro.product.initProductSpecificationAttributeTable(id);
                    }
                }


                if (event.target.dataset.bsTarget == "#form-tabs-variants") {
                    var id = event.target.dataset.productId;
                    if (id && id > 0) {
                        Entegro.product.initAttributesTable(id);
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
                        term: params.term || '',
                        page: params.page || 1
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


        $(document).off('click', '#btnSaveProductCategory').on('click', '#btnSaveProductCategory', function () {
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
                            const table = $('#ProductCategoryTable').DataTable();
                            if (table) {
                                table.ajax.reload(null, false);
                            }
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
            show: function () {
                $(this).slideDown();

                var $idInput = $(this).find('[name$="[Id]"]');
                $idInput.val(0);
                $(this).find("[data-repeater-item]").each(function () {
                    var productVariantAttributeId = $(this).attr("data-product-variant-attribute-id");
                    var $idProductVariantAttributeInput = $(this).find("input[name*='ProductVariantAttributeId']");
                    $idProductVariantAttributeInput.val(productVariantAttributeId);
                });

                Validation();
            },
            hide: function (deleteElement) {
                var $item = $(this);
                var id = $item.find("input[name$='[Id]']").val();

                Swal.fire({
                    title: 'Emin misiniz?',
                    text: "Bu varyant silinecek. Bu işlemi geri alamazsınız!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Vazgeç'
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (!id || parseInt(id) === 0) {
                            $(deleteElement).slideUp('fast', function () {
                                $(this).remove();
                            });

                            Swal.fire('Silindi!', 'Varyant formdan kaldırıldı.', 'success');
                        } else {
                            $.ajax({
                                url: '/Product/ProductVariantAttributeDelete',
                                type: 'POST',
                                data: { id: id },
                                success: function (response) {
                                    if (response.success) {
                                        $(deleteElement).slideUp('fast', function () {
                                            $(this).remove();
                                        });

                                        Swal.fire('Silindi!', 'Varyant formdan kaldırıldı.', 'success');
                                    }
                                    else {
                                        Swal.fire('Hata!', response.message || 'Bir hata oluştu.', 'error');
                                    }
                                },
                                error: function () {
                                    Swal.fire('Hata!', 'Sunucuya ulaşılamadı.', 'error');
                                }
                            });
                        }
                    }
                });

                Validation();
            },
            repeaters: [{
                selector: '.ProductVariantAttributeSelectionRepeater',
                initEmpty: false,
                show: function () {
                    $(this).slideDown();
                },
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
            $row.find('[name$="[AssignedPictureIds][]"]').each(function () {
                let val = parseInt($(this).val(), 10);
                if (item.AssignedPictureIds.includes(val)) {
                    $(this).prop("checked", true);
                } else {
                    $(this).prop("checked", false);
                }
            });
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
        formValidation = FormValidation.formValidation(
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
                        validators: {
                            notEmpty: { message: 'Para birimi seçilmelidir.' }
                        }
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
                    // Input-group sonrası mesaj konumlandırma
                    instance.on('plugins.message.placed', function (e) {
                        if (e.element.parentElement.classList.contains('input-group')) {
                            e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
                        }
                    });

                    // Alan invalid olursa ilgili sekmeyi aç
                    instance.on('core.field.invalid', function (e) {
                        const fieldEl = e.elements && e.elements.length ? e.elements[0] : null;
                        if (fieldEl) FocusFieldAndShowTab(fieldEl);
                    });

                    // Form invalid olursa ilk hatalı alana odaklan
                    instance.on('core.form.invalid', function () {
                        const invalidEl = document.querySelector('[data-field].is-invalid, .is-invalid');
                        if (invalidEl) FocusFieldAndShowTab(invalidEl);
                    });
                }
            }
        );

        // Dinamik olarak ilk yüklemede alanları ekle
        InitVariantValidation(formValidation);
        RemoveVariantValidation(formValidation);
    }
    function InitVariantValidation(formValidation, scope) {
        const container = scope ? $(scope) : $(document);

        container.find('[name^="ProductVariantAttributeCombinations"][name$="[StokCode]"]').each(function () {
            const name = $(this).attr('name');
            formValidation.addField(name, {
                validators: {
                    notEmpty: { message: 'Varyant Stok kodu boş bırakılamaz.' },
                    stringLength: { max: 50, message: 'Varyant Stok kodu en fazla 50 karakter olabilir.' }
                }
            });

            $(this).on('input', function () {
                formValidation.revalidateField(name);
            });
        });

        container.find('[name^="ProductVariantAttributeCombinations"][name$="[StockQuantity]"]').each(function () {
            const name = $(this).attr('name');
            formValidation.addField(name, {
                validators: {
                    notEmpty: { message: 'Varyant stok miktarı boş bırakılamaz.' },
                    integer: { message: 'Varyant stok miktarı tam sayı olmalıdır.' }
                }
            });

            $(this).on('input', function () {
                formValidation.revalidateField(name);
            });
        });
    }
    function RemoveVariantValidation(formValidation, scope) {
        const container = $(scope);

        container.find('[name^="ProductVariantAttributeCombinations"]').each(function () {
            formValidation.removeField($(this).attr('name'));
        });
    }

    function initAttributesTable(productId) {
        if (!productId) return;


        if ($.fn.DataTable.isDataTable('#AttributesTable')) {
            $('#AttributesTable').DataTable().ajax.url('/Product/ProductVariantAttributeList?productId=' + productId).load();
            return;
        }

        const table = $('#AttributesTable').DataTable({

            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            order: [[2, 'asc']],
            ajax: {
                url: '/Product/ProductVariantAttributeList?productId=' + productId,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', visible: false },
                {
                    data: 'ProductAttribute.Name',
                },
                {
                    data: 'AttributeControlTypeId',
                    title: 'Kontrol Türü',
                    render: function (data, type, row) {
                        const types = {
                            1: 'Açılır Liste',
                            2: 'Radyo Düğmesi Listesi',
                            3: 'Onay Kutusu',
                            4: 'Metin Kutusu',
                            10: 'Çok Satırlı Metin Kutusu',
                            20: 'Takvim',
                            30: 'Dosya Yükleme',
                            40: 'Kutular (Renk ve Görüntü)'
                        };
                        return types[data] || 'Bilinmiyor';
                    }
                },
                {
                    data: 'IsRequried',
                    render: function (data) {
                        return data ? 'Evet' : 'Hayır';
                    }
                },
                {
                    data: 'ProductVariantAttributeValues',
                    render: function (data, type, row) {
                        const count = Array.isArray(data) ? data.length : 0;

                        const productVariantAttributeId = row.Id;
                        const clickableText = `
                        <span class="text-primary btn-view-values cursor-pointer" 
                              data-attribute-id="${productVariantAttributeId}" 
                              data-product-id="${productId}">
                            ${count} seçenekleri düzenle
                        </span> `;
                        return clickableText;
                    }
                },
                {
                    data: 'DisplayOrder',
                },
                {
                    data: 'Id',
                    orderable: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return `
                        <div class="d-inline-block text-nowrap">
                            <button type="button"
                                    class="btn btn-text-secondary rounded-pill waves-effect btn-icon btn-createorupdate-productvariant"
                                    data-product-id="${productId}"
                                    data-attribute-id="${row.ProductAttributeId}"
                                    data-bs-toggle="modal"
                                    data-bs-target="#createOrUpdateProductVariantAttributeModal">
                                <i class="icon-base ti ti-pencil icon-22px"></i>
                            </button>
                            
                            <button type="button"
                                    class="btn btn-text-danger rounded-pill waves-effect btn-icon btn-delete-attribute"
                                    data-id="${row.Id}">
                                <i class="icon-base ti ti-trash icon-22px text-danger"></i>
                            </button>

                        </div>`;
                    }
                }
            ],
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: []
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                        buttons: [
                            {
                                text: `<button type="button"
                                        class="btn btn-primary btn-createorupdate-productvariant"
                                        data-product-id="${productId}"
                                        data-attribute-id="0"
                                        data-bs-toggle="modal"
                                        data-bs-target="#createOrUpdateProductVariantAttributeModal">
                                        <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                        <span class="d-none d-sm-inline-block">Yeni Özellik Ekle</span>
                                    </button>`,
                                className: "p-0 border-0 bg-transparent"
                            }
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 mb-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: ""
            }
        });


        setTimeout(() => {
            const adjustments = [
                { selector: ".dt-container", classToAdd: "border rounded" },
                { selector: ".dt-buttons .btn", classToRemove: "btn-secondary" },
                { selector: ".dt-buttons.btn-group", classToAdd: "mb-md-0 mb-6" },
                { selector: ".dt-search .form-control", classToRemove: "form-control-sm", classToAdd: "ms-0" },
                { selector: ".dt-search", classToAdd: "mb-0 mb-md-6" },
                { selector: ".dt-length .form-select", classToRemove: "form-select-sm" },
                { selector: ".dt-layout-end", classToAdd: "gap-md-2 gap-0 mt-0" },
                { selector: ".dt-layout-start", classToAdd: "mt-0" },
                { selector: ".dt-layout-table", classToRemove: "row mt-2" },
                { selector: ".dt-layout-full", classToRemove: "col-md col-12", classToAdd: "table-responsive" }
            ];
            adjustments.forEach(({ selector, classToRemove, classToAdd }) => {
                document.querySelectorAll(selector).forEach(el => {
                    if (classToRemove) classToRemove.split(" ").forEach(cls => el.classList.remove(cls));
                    if (classToAdd) classToAdd.split(" ").forEach(cls => el.classList.add(cls));
                });
            });
        }, 100);

        return table;
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
    function initCreateOrUpdateProductVariantAttributeModal() {
        $('#createOrUpdateProductVariantAttributeModal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            var productId = button.data('product-id');
            var attributeId = button.data('attribute-id');

            var modalBody = $('#createOrUpdateProductVariantAttributeModalBody');
            modalBody.html('<div class="text-center">Yükleniyor...</div>');

            if (productId) {
                $.ajax({
                    url: '/Product/CreateOrUpdateProductVariantAttribute',
                    type: 'GET',
                    data: { productId: productId, productVariantAttributeId: attributeId },
                    success: function (result) {
                        modalBody.html(result);
                    },
                    error: function () {
                        modalBody.html('<div class="text-danger text-center">Adres formu yüklenemedi.</div>');
                    }
                });
            } else {
                modalBody.html('<div class="text-danger text-center">Geçersiz ürün ID.</div>');
            }
        });
    }
    function initCreateOrUpdateProductVariantAttributeForm() {
        $(document).off('submit.createOrUpdateProductVariantAttributeForm')
            .on('submit.createOrUpdateProductVariantAttributeForm', '#createOrUpdateProductVariantAttributeForm', function (e) {
                e.preventDefault();

                const $form = $(this);

                if ($form.valid && !$form.valid()) return;

                if ($form.data('submitting')) return;

                $form.data('submitting', true);
                const $buttons = $form.find('button[type="submit"], input[type="submit"]');
                $buttons.prop('disabled', true);

                $.ajax({
                    url: $form.attr('action'),
                    type: 'POST',
                    data: $form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            // Eğer modal içindeyse kapat
                            $('#createOrUpdateProductVariantAttributeModal').modal('hide');

                            // Formu sıfırla
                            $form[0].reset();

                            // DataTable'ı yeniden yükle
                            $('#AttributesTable').DataTable().ajax.reload(null, false);

                            // Başarı mesajı
                            Swal.fire({
                                icon: 'success',
                                title: 'Başarılı',
                                text: 'Ürün varyant özelliği kaydedildi.',
                                timer: 2000,
                                showConfirmButton: false
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hata',
                                text: response.message || 'Bilinmeyen bir hata oluştu.',
                                confirmButtonText: 'Tamam'
                            });
                        }
                    },
                    error: function (xhr) {
                        console.error(xhr.responseText);
                        Swal.fire({
                            icon: 'error',
                            title: 'Sunucu Hatası',
                            text: 'Varyant kaydedilirken bir hata oluştu.'
                        });
                    },
                    complete: function () {
                        $form.data('submitting', false);
                        $buttons.prop('disabled', false);
                    }
                });
            });
    }
    function initDeleteAttributeHandler(tableSelector = '#AttributesTable') {
        $(document).on('click', '.btn-delete-attribute', function () {
            const attributeId = $(this).data('id');

            if (!attributeId) return;

            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu öğe silinecek ve geri alınamaz!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'Vazgeç',
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: `/Product/DeleteProductVariantAttribute?id=${attributeId}`,
                        type: 'POST',
                        success: function () {
                            Swal.fire({
                                title: 'Silindi!',
                                text: 'Öğe başarıyla silindi.',
                                icon: 'success',
                                timer: 1500,
                                showConfirmButton: false
                            });

                            $(tableSelector).DataTable().ajax.reload(null, false);
                        },
                        error: function (xhr) {
                            Swal.fire({
                                title: 'Hata!',
                                text: 'Silme işlemi sırasında bir hata oluştu.',
                                icon: 'error'
                            });
                        }
                    });
                }
            });
        });
    }
    function initViewVariantAttributeValues() {
        $(document).on('click', '.btn-view-values', function () {
            const attributeId = $(this).data('attribute-id');
            const productId = $(this).data('product-id');

            const url = `/Product/ProductVariantAttributeValues?productVariantAttributeId=${attributeId}&productId=${productId}`;
            window.location.href = url;
        });
    }
    function initProductCategoryTable(productId) {
        if (!productId) return;

        const tableId = '#ProductCategoryTable';
        const url = '/Product/ProductCategoryList?productId=' + productId;

        if ($.fn.DataTable.isDataTable(tableId)) {
            $(tableId).DataTable().ajax.url(url).load();
            return;
        }

        const table = $(tableId).DataTable({
            language: {
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                }
            },
            serverSide: true,
            processing: true,
            order: [[3, 'asc']], // DisplayOrder'a göre sırala
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: 'Id', orderable: false }, // checkbox
                { data: 'Id', visible: false },
                { data: 'CategoryBreadcrumb', title: 'Kategori Yolu' },
                { data: 'DisplayOrder', title: 'Sırası' },
                {
                    data: 'Id',
                    title: 'İşlemler',
                    orderable: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return `
                        <div class="d-inline-block text-nowrap">
                             <a href="javascript:void(0);"  class="btn btn-text-secondary rounded-pill waves-effect btn-icon text-danger delete-category-mapping" title="Sil" data-id="${data}">
                              <i class="icon-base ti ti-trash icon-22px"></i>
                             </a>
                        </div>
                    `;
                    }
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 1,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: {
                style: 'multi',
                selector: 'td:nth-child(1)'
            },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                        buttons: [
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1">
                                    <i class="icon-base ti ti-upload icon-xs"></i>
                                    <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                                  </span>`,
                                buttons: [
                                    {
                                        extend: "print",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-printer me-1"></i> Print</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "csv",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file me-1"></i> Csv</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "excel",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-upload me-1"></i> Excel</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "pdf",
                                        className: "dropdown-item",
                                        text: `<span class="d-flex align-items-center"><i class="icon-base ti tabler-file-text me-1"></i> Pdf</span>`,
                                        exportOptions: { columns: [2, 3] }
                                    },
                                    {
                                        extend: "copy",
                                        className: "dropdown-item",
                                        text: `<i class="icon-base ti tabler-copy me-1"></i> Copy`,
                                        exportOptions: { columns: [2, 3] }
                                    }
                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                <span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    const id = productId;

                                    $.ajax({
                                        url: `/Product/ProductCategoryCreatePopup`,
                                        type: 'GET',
                                        data: { id: id },
                                        success: function (result) {
                                            $('#ProductCategoryPopupContent').html(result);
                                            $('#ProductCategoryPopup').modal('show');
                                            ProductCategoryCreatePopup(id);
                                        },
                                        error: function () {
                                            alert("Modal yüklenemedi. Lütfen tekrar deneyin.");
                                        }
                                    });
                                }
                            }

                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            }
        });

        // Silme işlemi
        $(document).on('click', '.delete-category-mapping', function () {
            const id = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu kategori ilişkisi silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Product/ProductCategoryDelete',
                        type: 'POST',
                        data: { id: id },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Kategori ilişkisi başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    table.ajax.reload(null, false);
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Sunucu Hatası!',
                                text: 'İstek gönderilirken bir hata oluştu.',
                                confirmButtonText: 'Tamam',
                                customClass: { confirmButton: 'btn btn-danger' },
                                buttonsStyling: false
                            });
                        }
                    });
                }
            });
        });
    }

    function CreateCombination() {
        $("[data-repeater-create]").click();
    }

    function CreateAllCombinations(productId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: 'Tüm özellikleri birleştirmek istiyor musunuz? Mevcut kombinasyonlar silinecektir!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Product/ProductVariantAttributeCreateAll',
                type: 'POST',
                data: { productId: productId },
                success: function () {
                    location.reload();
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Hata', text: 'Bir hata oluştu.' });
                }
            });
        });
    }

    function DeleteAllCombinations(productId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: 'Tüm kombinasyonlar silinecek emin misin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Product/ProductVariantAttributeAllDelete',
                type: 'POST',
                data: { productId: productId },
                success: function () {
                    location.reload();
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Silme başarısız', text: 'Bir hata oluştu.' });
                }
            });
        });
    }

    function initCrossSellList(productId) {


        if (!productId) return;

        const tableId = '#CrossSaleProductTable';
        const url = '/Product/CrossSellProductList?productId=' + productId;

        if ($.fn.DataTable.isDataTable(tableId)) {
            $(tableId).DataTable().ajax.url(url).load();
            return;
        }


        const table = $('#CrossSaleProductTable').DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            processing: true,
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            order: [[0, 'asc']], // DisplayOrder sırasına göre
            columns: [
                { data: 'Id', orderable: false },
                {
                    data: 'Product2.name',
                    render: function (data, type, row) {
                        return row.Product2?.Name ?? '-';
                    }
                },
                {
                    data: 'Product2.Code',
                    render: function (data, type, row) {
                        return row.Product2?.Code ?? '-';
                    }
                },
                {
                    data: 'Product2.Published',
                    render: data => {
                        const checked = data ? "checked" : "";
                        const titleText = data ? "Yayında" : "Yayında Değil";
                        return `
                     <div class="form-check d-inline-flex justify-content-center">
                       <input class="form-check-input" type="checkbox" ${checked} onclick="return false;" title="${titleText}">
                     </div>`;
                    }
                },
                {
                    data: null,
                    orderable: false,
                    className: 'text-center',
                    render: function (data, type, row) {
                        return `
                     <div class="btn-group" role="group">
                         <button class="btn btn-sm btn-outline-danger delete-crosssell" data-id="${row.Id}" title="Sil">
                             <i class="ti ti-trash"></i>
                         </button>
                     </div>
                 `;
                    }
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: { selectRow: true },
                    className: "text-center",
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: {
                style: "multi",
                selector: "td:first-child"
            },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: { menu: [10, 25, 50, 100], text: "_MENU_" },
                        buttons: [
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1">
                                 <i class="icon-base ti ti-upload icon-xs"></i>
                                 <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                               </span>`,
                                buttons: [
                                    { extend: "print", className: "dropdown-item", text: `<i class="icon-base ti tabler-printer me-1"></i> Print`, exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "csv", className: "dropdown-item", text: `<i class="icon-base ti tabler-file me-1"></i> CSV`, exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "excel", className: "dropdown-item", text: `<i class="icon-base ti tabler-upload me-1"></i> Excel`, exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "pdf", className: "dropdown-item", text: `<i class="icon-base ti tabler-file-text me-1"></i> PDF`, exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "copy", className: "dropdown-item", text: `<i class="icon-base ti tabler-copy me-1"></i> Kopyala`, exportOptions: { columns: [1, 2, 3] } },

                                ]
                            },
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                         <span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {

                                    $('#CrossSaleProductModal').modal('show');

                                    $('#ProductId2').select2({
                                        language: "tr",
                                        placeholder: 'Ürün seçiniz',
                                        allowClear: true,
                                        dropdownParent: $('#CrossSaleProductModal'), // doğru modal ID
                                        width: '100%',
                                        ajax: {
                                            url: "/Product/AllProduct",
                                            type: 'POST',
                                            dataType: 'json',
                                            delay: 250,
                                            data: function (params) {
                                                return {
                                                    term: params.term || '',
                                                    page: params.page || 1
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
                                        },
                                        templateResult: function (state) {
                                            if (!state.id) {
                                                return state.text;
                                            }
                                            var $state = $('<span>' + state.text + '</span><br><span>' + state.code + '</span>');
                                            return $state;
                                        }
                                    });
                                }
                            },
                            {
                                text: `<span class="d-flex align-items-center gap-1">
                                    <i class="icon-base ti ti-trash icon-xs"></i>
                                    <span class="d-none d-sm-inline-block">Seçilenleri Sil</span>
                                </span>`,
                                className: 'btn btn-outline-danger',
                                attr: { id: 'deleteSelectedCrossSell', disabled: true },
                                action: function () {
                                    const selectedData = table.rows({ selected: true }).data().toArray();
                                    const ids = selectedData.map(row => row.Id);
                                    if (ids.length === 0) return;

                                    Swal.fire({
                                        title: 'Emin misiniz?',
                                        text: `${ids.length} çapraz satış silinecek!`,
                                        icon: 'warning',
                                        showCancelButton: true,
                                        confirmButtonText: 'Evet, sil!',
                                        cancelButtonText: 'İptal',
                                        customClass: {
                                            confirmButton: 'btn btn-danger me-3',
                                            cancelButton: 'btn btn-secondary'
                                        },
                                        buttonsStyling: false
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            $.ajax({
                                                url: '/Product/DeleteMultipleCrossSell',
                                                type: 'POST',
                                                contentType: 'application/json',
                                                data: JSON.stringify(ids),
                                                success: function (response) {
                                                    if (response.success) {
                                                        Swal.fire({
                                                            icon: 'success',
                                                            title: 'Silindi!',
                                                            text: 'Seçilen çapraz satışlar silindi.',
                                                            confirmButtonText: 'Tamam',
                                                            customClass: { confirmButton: 'btn btn-success' },
                                                            buttonsStyling: false
                                                        }).then(() => {
                                                            table.ajax.reload(null, false);
                                                        });
                                                    } else {
                                                        Swal.fire({
                                                            icon: 'error',
                                                            title: 'Hata!',
                                                            text: response.message || 'Silme işlemi başarısız oldu.',
                                                            confirmButtonText: 'Tamam',
                                                            customClass: { confirmButton: 'btn btn-danger' },
                                                            buttonsStyling: false
                                                        });
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            },
            initComplete: function () {
                $('#CrossSaleProductTable thead th:first-child')
                    .html('<input type="checkbox" id="selectAllCrossSell" class="form-check-input" />');

                $('#selectAllCrossSell').on('change', function () {
                    if ($(this).is(':checked')) {
                        table.rows().select();
                    } else {
                        table.rows().deselect();
                    }
                });
            }
        });

        table.on('select deselect', function () {
            const totalRows = table.rows().count();
            const selectedData = table.rows({ selected: true }).data().toArray();
            const selectedRows = selectedData.length;

            $('#selectAllCrossSell').prop('checked', selectedRows === totalRows && totalRows > 0);
            $('#CrossSaleProductTable tbody tr').each(function () {
                const isSelected = table.row(this).selected();
                $(this).find('input.dt-checkboxes').prop('checked', isSelected);
            });

            $('#deleteSelectedCrossSell').prop('disabled', selectedRows === 0);
        });

        table.on('draw', function () {
            $('#selectAllCrossSell').prop('checked', false);
            $('#deleteSelectedCrossSell').prop('disabled', true);
            $('#CrossSaleProductTable tbody input.dt-checkboxes').prop('checked', false);
        });

        // Tekli silme
        $(document).on('click', '.delete-crosssell', function () {
            const id = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu çapraz satış bağlantısı silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Product/DeleteCrossSell',
                        type: 'POST',
                        data: { id: id },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Çapraz satış silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    table.ajax.reload(null, false);
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-danger' },
                                    buttonsStyling: false
                                });
                            }
                        }
                    });
                }
            });
        });


        // Form Submit işlemi - AJAX POST
        $(document).on('submit', '#CrossSaleProductForm', function (e) {
            e.preventDefault();

            const productId1 = $('#ProductId1').val();
            const productId2 = $('#ProductId2').val();

            if (!productId2) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Uyarı!',
                    text: 'Lütfen eşlenecek ürünü seçiniz.',
                    confirmButtonText: 'Tamam',
                    customClass: { confirmButton: 'btn btn-warning' },
                    buttonsStyling: false
                });
                return;
            }

            $.ajax({
                url: '/Product/CreateCrossSaleProduct',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    ProductId1: parseInt(productId1),
                    ProductId2: parseInt(productId2)
                }),
                success: function (response) {
                    if (response.success) {
                        $('#CrossSaleProductModal').modal('hide');

                        // DataTable yenile
                        $('#CrossSaleProductTable').DataTable().ajax.reload(null, false);

                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı!',
                            text: 'Ürün eşleştirildi.',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-success' },
                            buttonsStyling: false
                        });

                        // Formu sıfırla
                        $('#CrossSaleProductForm')[0].reset();
                        $('#ProductId2').val(null).trigger('change');
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata!',
                            text: response.message || 'Eşleştirme işlemi başarısız oldu.',
                            confirmButtonText: 'Tamam',
                            customClass: { confirmButton: 'btn btn-danger' },
                            buttonsStyling: false
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Sunucu Hatası!',
                        text: 'İstek gönderilirken bir hata oluştu.',
                        confirmButtonText: 'Tamam',
                        customClass: { confirmButton: 'btn btn-danger' },
                        buttonsStyling: false
                    });
                }
            });
        });



    }
    function initRelatedProductList(productId) {
        if (!productId) return;

        const tableId = '#RelatedProductTable';
        const url = '/Product/RelatedProductList?productId=' + productId;

        if ($.fn.DataTable.isDataTable(tableId)) {
            $(tableId).DataTable().ajax.url(url).load();
            return;
        }

        const table = $(tableId).DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json',
            },
            serverSide: true,
            processing: true,
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            order: [[3, 'asc']],
            columns: [
                { data: 'Id', orderable: false },
                {
                    data: 'Product2.Name',
                    render: (data, type, row) => row.Product2?.Name ?? '-'
                },
                {
                    data: 'Product2.Code',
                    render: (data, type, row) => row.Product2?.Code ?? '-'
                },
                {
                    data: 'DisplayOrder'
                },
                {
                    data: 'Product2.Published',
                    render: data => {
                        const checked = data ? "checked" : "";
                        const titleText = data ? "Yayında" : "Yayında Değil";
                        return `
                        <div class="form-check d-inline-flex justify-content-center">
                            <input class="form-check-input" type="checkbox" ${checked} onclick="return false;" title="${titleText}">
                        </div>`;
                    }
                },
                {
                    data: null,
                    orderable: false,
                    className: 'text-center',
                    render: row => `
                    <div class="btn-group" role="group">
                        <button class="btn btn-sm btn-outline-danger delete-related" data-related-id="${row.Id}" title="Sil">
                            <i class="ti ti-trash"></i>
                        </button>
                    </div>
                `
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: { selectRow: true },
                    className: "text-center",
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: { style: "multi", selector: "td:first-child" },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: { menu: [10, 25, 50, 100], text: "_MENU_" },
                        buttons: [
                            {
                                extend: "collection",
                                className: "btn btn-label-secondary dropdown-toggle me-4",
                                text: `<span class="d-flex align-items-center gap-1">
                                <i class="icon-base ti ti-upload icon-xs"></i>
                                <span class="d-none d-sm-inline-block">Dışarı Aktar</span>
                            </span>`,
                                buttons: [
                                    { extend: "print", className: "dropdown-item", text: "Print", exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "csv", className: "dropdown-item", text: "CSV", exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "excel", className: "dropdown-item", text: "Excel", exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "pdf", className: "dropdown-item", text: "PDF", exportOptions: { columns: [1, 2, 3] } },
                                    { extend: "copy", className: "dropdown-item", text: "Kopyala", exportOptions: { columns: [1, 2, 3] } }
                                ]
                            },
                            {
                                text: `<i class="ti ti-plus icon-16px me-1"></i><span class="d-none d-sm-inline-block">Yeni Ekle</span>`,
                                className: "add-new btn btn-primary",
                                action: function () {
                                    $('#RelatedProductModal').modal('show');
                                    $('#ProductId2Related').select2({
                                        language: "tr",
                                        placeholder: 'Ürün seçiniz',
                                        allowClear: true,
                                        dropdownParent: $('#RelatedProductModal'),
                                        width: '100%',
                                        ajax: {
                                            url: "/Product/AllProduct",
                                            type: 'POST',
                                            dataType: 'json',
                                            delay: 250,
                                            data: params => ({
                                                term: params.term || '',
                                                page: params.page || 1
                                            }),
                                            processResults: (data, params) => {
                                                params.page = params.page || 1;
                                                return {
                                                    results: data.results,
                                                    pagination: { more: data.pagination?.more === true }
                                                };
                                            },
                                            cache: true
                                        },
                                        templateResult: state => {
                                            if (!state.id) return state.text;
                                            return $('<span>' + state.text + '</span><br><span>' + state.code + '</span>');
                                        }
                                    });
                                }
                            },
                            {
                                text: `<span class="d-flex align-items-center gap-1">
                                    <i class="icon-base ti ti-trash icon-xs"></i>
                                    <span class="d-none d-sm-inline-block">Seçilenleri Sil</span>
                                </span>`,
                                className: 'btn btn-outline-danger',
                                attr: { id: 'deleteSelectedRelated', disabled: true },
                                action: function () {
                                    const selectedData = table.rows({ selected: true }).data().toArray();
                                    const ids = selectedData.map(row => row.Id);

                                    if (ids.length === 0) return;

                                    Swal.fire({
                                        title: 'Emin misiniz?',
                                        text: `${ids.length} ilişki silinecek!`,
                                        icon: 'warning',
                                        showCancelButton: true,
                                        confirmButtonText: 'Evet, sil!',
                                        cancelButtonText: 'İptal',
                                        customClass: { confirmButton: 'btn btn-danger me-3', cancelButton: 'btn btn-secondary' },
                                        buttonsStyling: false
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            $.ajax({
                                                url: '/Product/DeleteMultipleRelated',
                                                type: 'POST',
                                                contentType: 'application/json',
                                                data: JSON.stringify(ids),
                                                success: function (response) {
                                                    if (response.success) {
                                                        Swal.fire({ icon: 'success', title: 'Silindi!', text: 'İlişkili ürün(ler) silindi.' })
                                                            .then(() => table.ajax.reload(null, false));
                                                    } else {
                                                        Swal.fire({ icon: 'error', title: 'Hata!', text: response.message || 'Silme işlemi başarısız oldu.' });
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        ]
                    }]
                },
                bottomStart: { rowClass: "row mx-3 justify-content-between", features: ["info"] },
                bottomEnd: "paging"
            },
            initComplete: function () {
                $('#RelatedProductTable thead th:first-child').html('<input type="checkbox" id="selectAllRelated" class="form-check-input" />');
                $('#selectAllRelated').on('change', function () {
                    $(this).is(':checked') ? table.rows().select() : table.rows().deselect();
                });
            }
        });

        table.on('select deselect', function () {
            const totalRows = table.rows().count();
            const selectedRows = table.rows({ selected: true }).count();
            $('#selectAllRelated').prop('checked', selectedRows === totalRows && totalRows > 0);
            $('#RelatedProductTable tbody tr').each(function () {
                const isSelected = table.row(this).selected();
                $(this).find('input.dt-checkboxes').prop('checked', isSelected);
            });
            $('#deleteSelectedRelated').prop('disabled', selectedRows === 0);
        });

        table.on('draw', function () {
            $('#selectAllRelated').prop('checked', false);
            $('#deleteSelectedRelated').prop('disabled', true);
            $('#RelatedProductTable tbody input.dt-checkboxes').prop('checked', false);
        });

       
        $('#RelatedProductTable').on('click', '.delete-related', function () {
            const relatedProductId = $(this).attr('data-related-id'); // attr ile al
            console.log("Silinecek tekli ID:", relatedProductId);

            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu ilişki silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                customClass: { confirmButton: 'btn btn-danger me-3', cancelButton: 'btn btn-secondary' },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Product/DeleteRelated',
                        type: 'POST',
                        data: { relatedProductId: relatedProductId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({ icon: 'success', title: 'Silindi!', text: 'İlişki başarıyla silindi.' })
                                    .then(() => table.ajax.reload(null, false));
                            } else {
                                Swal.fire({ icon: 'error', title: 'Hata!', text: response.message || 'Silme işlemi başarısız oldu.' });
                            }
                        },
                        error: function () {
                            Swal.fire({ icon: 'error', title: 'Sunucu Hatası!', text: 'İstek gönderilirken bir hata oluştu.' });
                        }
                    });
                }
            });
        });

      
        $('#RelatedProductForm').on('submit', function (e) {
            e.preventDefault();
            const productId1 = $('#ProductId1').val();
            const relatedProductId2 = $('#ProductId2Related').val();
            const displayOrder = $('#DisplayOrderRelated').val();

            if (!relatedProductId2) {
                Swal.fire({ icon: 'warning', title: 'Uyarı!', text: 'Lütfen eşlenecek ürünü seçiniz.' });
                return;
            }

            $.ajax({
                url: '/Product/CreateRelatedProduct',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    ProductId1: parseInt(productId1),
                    ProductId2: parseInt(relatedProductId2),
                    DisplayOrder: parseInt(displayOrder) || 0
                }),
                success: function (response) {
                    if (response.success) {
                        $('#RelatedProductModal').modal('hide');
                        table.ajax.reload(null, false);
                        Swal.fire({ icon: 'success', title: 'Başarılı!', text: 'Ürün eşleştirildi.' });
                        $('#RelatedProductForm')[0].reset();
                        $('#ProductId2Related').val(null).trigger('change');
                    } else {
                        Swal.fire({ icon: 'error', title: 'Hata!', text: response.message || 'Eşleştirme işlemi başarısız oldu.' });
                    }
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Sunucu Hatası!', text: 'İstek gönderilirken bir hata oluştu.' });
                }
            });
        });
    }
    function initProductSpecificationAttributeTable(productId) {
        if (!productId) return;

        const tableId = '#ProductSpecificationAttributeMappingTable';
        const url = '/Product/ProductSpecificationAttributeMappingList?productId=' + productId;

        if ($.fn.DataTable.isDataTable(tableId)) {
            $(tableId).DataTable().ajax.url(url).load();
            return;
        }

        const table = $(tableId).DataTable({
            language: {
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right scaleX-n1-rtl icon-18px"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left scaleX-n1-rtl icon-18px"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left scaleX-n1-rtl icon-18px"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right scaleX-n1-rtl icon-18px"></i>'
                },
                url: 'https://cdn.datatables.net/plug-ins/2.3.2/i18n/tr.json'
            },
            serverSide: true,
            processing: true,
            order: [[3, 'asc']],
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            columns: [
                { data: null, defaultContent: '', orderable: false }, // checkbox alanı
                { data: 'Id', visible: false },
                {
                    data: 'SpecificationAttributeOption.SpecificationAttributeName',
                },
                {
                    data: 'SpecificationAttributeOption.Name',
                },
                { data: 'SpecificationAttributeOption.SpecificationAttributeId', visible: false },
                {
                    data: 'Id',
                    orderable: false,
                    searchable: false,
                    render: function (data) {
                        return `
                        <div class="d-inline-block text-nowrap">
                            <button type="button"
                                class="btn btn-text-danger rounded-pill waves-effect btn-icon btn-delete-spec-attribute"
                                data-id="${data}">
                                <i class="icon-base ti ti-trash icon-22px text-danger"></i>
                            </button>
                        </div>`;
                    }
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    checkboxes: { selectRow: true },
                    className: "text-center",
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                },
                {
                    targets: 4,
                    visible: false,
                    searchable: false
                }
            ],
            select: { style: "multi", selector: "td:first-child" },
            displayLength: 10,
            layout: {
                topStart: {
                    rowClass: "card-header d-flex border-top rounded-0 flex-wrap py-0 flex-column flex-md-row align-items-start",
                    features: [{
                        search: {
                            className: "me-5 ms-n4 pe-5 mb-n6 mb-md-0",
                            placeholder: "Ara..",
                            text: "_INPUT_"
                        }
                    }]
                },
                topEnd: {
                    rowClass: "row m-3 my-0 justify-content-between",
                    features: [{
                        pageLength: {
                            menu: [10, 25, 50, 100],
                            text: "_MENU_"
                        },
                        buttons: [
                            {
                                text: `<i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                   <span class="d-none d-sm-inline-block">Yeni Özellik Ekle</span>`,
                                className: "btn btn-primary btn-add-spec-attribute",
                                action: function () {
                                    $('#ProductSpecificationAttributeForm')[0].reset();
                                    $('#SpecificationAttributeId').val(null).trigger('change');
                                    $('#SpecificationAttributeOptionId').val(null).trigger('change').empty();
                                    $('#ProductSpecificationAttributeForm #ProductId').val(productId);

                                    $('#ProductSpecificationAttributeModal').modal('show');
                                    initSpecificationAttributeDropdowns();
                                }
                            },
                            {
                                text: `<i class="icon-base ti ti-trash icon-16px me-1"></i>
                                   <span class="d-none d-sm-inline-block">Seçilenleri Sil</span>`,
                                className: "btn btn-outline-danger",
                                attr: { id: 'deleteSelectedSpecAttributes', disabled: true },
                                action: function () {
                                    const selectedData = table.rows({ selected: true }).data().toArray();
                                    const ids = selectedData.map(row => row.Id);

                                    if (ids.length === 0) return;

                                    Swal.fire({
                                        title: 'Emin misiniz?',
                                        text: `${ids.length} özellik eşleştirmesi silinecek!`,
                                        icon: 'warning',
                                        showCancelButton: true,
                                        confirmButtonText: 'Evet, sil!',
                                        cancelButtonText: 'İptal',
                                        customClass: {
                                            confirmButton: 'btn btn-danger me-3',
                                            cancelButton: 'btn btn-secondary'
                                        },
                                        buttonsStyling: false
                                    }).then(result => {
                                        if (result.isConfirmed) {
                                            $.ajax({
                                                url: '/Product/DeleteMultipleProductSpecificationAttributeMapping',
                                                type: 'POST',
                                                contentType: 'application/json',
                                                data: JSON.stringify(ids),
                                                success: function (response) {
                                                    if (response.success) {
                                                        Swal.fire({
                                                            icon: 'success',
                                                            title: 'Silindi!',
                                                            text: 'Seçilen özellikler silindi.'
                                                        }).then(() => table.ajax.reload(null, false));
                                                    } else {
                                                        Swal.fire({ icon: 'error', title: 'Hata!', text: response.message });
                                                    }
                                                },
                                                error: function () {
                                                    Swal.fire({
                                                        icon: 'error',
                                                        title: 'Sunucu Hatası!',
                                                        text: 'İstek gönderilirken hata oluştu.'
                                                    });
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        ]
                    }]
                },
                bottomStart: {
                    rowClass: "row mx-3 mb-3 justify-content-between",
                    features: ["info"]
                },
                bottomEnd: "paging"
            },
            initComplete: function () {
                this.api().columns().every(function () {
                    if (this.dataSrc() === "SpecificationAttributeOption.SpecificationAttributeId") {
                        var column = this;

                        $('<select id="optionFilter" style="width:200px"></select>')
                            .appendTo(".optionAttributeFilter")
                            .on("change", function () {
                                const val = $(this).val();
                                column.search(val ? "^" + val + "$" : "", true, false).draw();
                            });

                        $('#optionFilter').select2({
                            placeholder: "Özellik seçin",
                            allowClear: true,
                            ajax: {
                                url: '/SpecificationAttribute/AllSpecificationAttribute',
                                type: 'POST',
                                dataType: 'json',
                                delay: 250,
                                data: params => ({
                                    term: params.term || "",
                                    page: params.page || 1
                                }),
                                processResults: (data, params) => ({
                                    results: data.results,
                                    pagination: { more: data.pagination.more }
                                })
                            }
                        });
                    }
                });

                // SelectAll checkbox ekle
                $('#ProductSpecificationAttributeMappingTable thead th:first-child')
                    .html('<input type="checkbox" id="selectAllSpec" class="form-check-input" />');

                $('#selectAllSpec').on('change', function () {
                    $(this).is(':checked') ? table.rows().select() : table.rows().deselect();
                });
            }
        });

        // Satır seçiminde delete butonu yönet
        table.on('select deselect', function () {
            const totalRows = table.rows().count();
            const selectedRows = table.rows({ selected: true }).count();
            $('#selectAllSpec').prop('checked', selectedRows === totalRows && totalRows > 0);
            $('#ProductSpecificationAttributeMappingTable tbody tr').each(function () {
                const isSelected = table.row(this).selected();
                $(this).find('input.dt-checkboxes').prop('checked', isSelected);
            });
            $('#deleteSelectedSpecAttributes').prop('disabled', selectedRows === 0);
        });

        table.on('draw', function () {
            $('#selectAllSpec').prop('checked', false);
            $('#deleteSelectedSpecAttributes').prop('disabled', true);
            $('#ProductSpecificationAttributeMappingTable tbody input.dt-checkboxes').prop('checked', false);
        });

        // Form submit (tekli ekleme)
        $('#ProductSpecificationAttributeForm').on('submit', function (e) {
            e.preventDefault();

            const productId = $('#ProductId').val();
            const optionId = $('#SpecificationAttributeOptionId').val();
            const displayOrder = $('#DisplayOrderRelated').val();

            if (!optionId) {
                Swal.fire({ icon: 'warning', title: 'Uyarı!', text: 'Lütfen özellik değeri seçiniz.' });
                return;
            }

            $.ajax({
                url: '/Product/CreateProductSpecificationAttributeMapping',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    ProductId: parseInt(productId),
                    SpecificationAttributeOptionId: parseInt(optionId),
                    DisplayOrder: parseInt(displayOrder) || 0
                }),
                success: function (response) {
                    if (response.success) {
                        $('#ProductSpecificationAttributeModal').modal('hide');
                        table.ajax.reload(null, false);
                        Swal.fire({ icon: 'success', title: 'Başarılı!', text: 'Özellik başarıyla eşleştirildi.' });

                        $('#ProductSpecificationAttributeForm')[0].reset();
                        $('#SpecificationAttributeId').val(null).trigger('change');
                        $('#SpecificationAttributeOptionId').val(null).trigger('change').empty();
                    } else {
                        Swal.fire({ icon: 'error', title: 'Hata!', text: response.message || 'Eşleştirme işlemi başarısız oldu.' });
                    }
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Sunucu Hatası!', text: 'İstek gönderilirken bir hata oluştu.' });
                }
            });
        });

        // Tekli sil
        $('#ProductSpecificationAttributeMappingTable').on('click', '.btn-delete-spec-attribute', function () {
            const id = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu özellik eşleştirmesi silinecek!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Product/DeleteProductSpecificationAttributeMapping',
                        type: 'POST',
                        data: { id: id },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Özellik başarıyla silindi.' })
                                    .then(() => table.ajax.reload(null, false));
                            } else {
                                Swal.fire({ icon: 'error', title: 'Hata!', text: response.message || 'Silme işlemi başarısız oldu.' });
                            }
                        },
                        error: function () {
                            Swal.fire({ icon: 'error', title: 'Sunucu Hatası!', text: 'İstek gönderilirken hata oluştu.' });
                        }
                    });
                }
            });
        });

        return table;
    }

    function initSpecificationAttributeDropdowns() {
        $('#SpecificationAttributeId').select2({
            language: "tr",
            placeholder: 'Özellik seçiniz',
            allowClear: true,
            dropdownParent: $('#ProductSpecificationAttributeModal'),
            width: '100%',
            ajax: {
                url: "/SpecificationAttribute/AllSpecificationAttribute",
                type: 'POST',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term || '',
                        page: params.page || 1
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

      
        $('#SpecificationAttributeId').on('select2:select', function (e) {
            const selected = e.params.data;
            const options = selected.specificationAttributeOptions || [];

            $('#SpecificationAttributeOptionId').empty();

            options.forEach(opt => {
                const newOption = new Option(opt.text, opt.id, false, false);
                $('#SpecificationAttributeOptionId').append(newOption);
            });

            $('#SpecificationAttributeOptionId').val(null).trigger('change');
        });


        $('#SpecificationAttributeOptionId').select2({
            language: "tr",
            placeholder: 'Özellik değeri seçiniz',
            allowClear: true,
            dropdownParent: $('#ProductSpecificationAttributeModal'),
            width: '100%'
        });
    }



    return {
        Init: Init,
        TabsInit: TabsInit,
        DescriptionEditor: DescriptionEditor,
        initAttributesTable: initAttributesTable,
        ProductCategoryCreatePopup: ProductCategoryCreatePopup,
        ProductVariantAttributeCombinationRepeaterInit: ProductVariantAttributeCombinationRepeaterInit,
        Validation: Validation,
        initCreateOrUpdateProductVariantAttributeModal: initCreateOrUpdateProductVariantAttributeModal,
        initCreateOrUpdateProductVariantAttributeForm: initCreateOrUpdateProductVariantAttributeForm,
        initDeleteAttributeHandler: initDeleteAttributeHandler,
        initViewVariantAttributeValues: initViewVariantAttributeValues,
        CreateCombination: CreateCombination,
        CreateAllCombinations: CreateAllCombinations,
        DeleteAllCombinations: DeleteAllCombinations,
        initProductCategoryTable: initProductCategoryTable,
        initCrossSellList: initCrossSellList,
        initRelatedProductList: initRelatedProductList,
        initProductSpecificationAttributeTable: initProductSpecificationAttributeTable,
        initSpecificationAttributeDropdowns: initSpecificationAttributeDropdowns,
        addFilterDropdown: addFilterDropdown,
        addFilterText: addFilterText
    };
})(jQuery);
