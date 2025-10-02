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

                if (event.target.dataset.bsTarget == "#form-tabs-attributes") {
                    if (event.target.dataset.url.length > 0) {
                        $("#form-tabs-attributes").load(event.target.dataset.url, function () {
                           
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
            show: function () {
                $(this).slideDown();

                var $idInput = $(this).find('[name$="[Id]"]');
                $idInput.val(0);
                var repeterItem = $(this).find("[data-repeater-item]");
                var productVariantAttributeId = $(repeterItem).attr("data-product-variant-attribute-id");
                var $idProductVariantAttributeInput = find("input[name*='ProductVariantAttributeId']");
                $idProductVariantAttributeInput.val(productVariantAttributeId);

               
            },
            hide: function (deleteElement) {
                var $idInput = $(deleteElement).find('[name$="[Id]"]');
                var id = $idInput.val();
             

                // Eğer id boşsa (yeni eklenmiş, henüz kaydedilmemiş varyant)
                if (!id || parseInt(id) === 0) {
                    Swal.fire({
                        title: 'Emin misiniz?',
                        text: "Bu varyant formdan silinecek.",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Evet, sil!',
                        cancelButtonText: 'İptal'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $(deleteElement).slideUp('fast', function () {
                                $(this).remove();
                            });

                            Swal.fire(
                                'Silindi!',
                                'Varyant formdan kaldırıldı.',
                                'success'
                            );
                        }
                    });
                    return;
                }

                // id varsa (veritabanında kayıtlı varyant)
                Swal.fire({
                    title: 'Emin misiniz?',
                    text: "Bu varyant silinecek. Bu işlemi geri alamazsınız!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Evet, sil!',
                    cancelButtonText: 'İptal'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $.ajax({
                            url: '/Product/ProductVariantAttributeDelete',
                            type: 'POST',
                            data: { id: id },
                            success: function (response) {
                                if (response.success) {
                                    $(deleteElement).slideUp('fast', function () {
                                        $(this).remove();
                                    });

                                    Swal.fire(
                                        'Silindi!',
                                        'Varyant başarıyla silindi.',
                                        'success'
                                    );
                                } else {
                                    Swal.fire(
                                        'Hata!',
                                        response.message || 'Bir hata oluştu.',
                                        'error'
                                    );
                                }
                            },
                            error: function () {
                                Swal.fire(
                                    'Hata!',
                                    'Sunucuya ulaşılamadı.',
                                    'error'
                                );
                            }
                        });
                    }
                });
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
    function initAttributesTable(productId) {
        if (!productId) return;

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
                            0: 'Açılır Liste',
                            1: 'Radyo Düğmesi Listesi',
                            2: 'Onay Kutusu',
                            3: 'Metin Kutusu',
                            4: 'Çok Satırlı Metin Kutusu',
                            5: 'Takvim',
                            6: 'Dosya Yükleme',
                            7: 'Kutular (Renk ve Görüntü)'
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
                    title: 'Özellikler',
                    render: function (data) {
                        const count = data?.length || 0;
                        return `${count} ürün özelliği var`;
                    }
                },
                {
                    data: 'ProductVariantAttributeValues',
                    title: 'Görüntülenme Sayısı',
                    render: function (data) {
                        return data?.length || 0;
                    }
                },
                {
                    data: 'Id',
                    title: 'Opsiyonlar',
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
                  <button class="btn btn-text-secondary rounded-pill waves-effect btn-icon dropdown-toggle hide-arrow"
                          data-bs-toggle="dropdown">
                      <i class="icon-base ti ti-dots-vertical icon-22px"></i>
                  </button>
                  <div class="dropdown-menu dropdown-menu-end m-0">
                      <button type="button"
                              class="dropdown-item text-danger btn-delete-attribute"
                              data-id="${row.Id}">
                          Sil
                      </button>
                      <button type="button"
                              class="dropdown-item text-success btn-view-values"
                              data-id="${row.Id}">
                          Özellik Değerlerini Gör
                      </button>
                  </div>
              </div>`;
                    }
                }
            ],

            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
                    responsivePriority: 3,
                    checkboxes: {
                        selectAllRender: '<input type="checkbox" class="form-check-input">'
                    },
                    render: () => '<input type="checkbox" class="dt-checkboxes form-check-input">'
                }
            ],
            select: {
                style: "multi",
                selector: "td:nth-child(1)"
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
                                    { extend: "print", className: "dropdown-item", text: "Print", exportOptions: { columns: [2] } },
                                    { extend: "csv", className: "dropdown-item", text: "CSV", exportOptions: { columns: [2] } },
                                    { extend: "excel", className: "dropdown-item", text: "Excel", exportOptions: { columns: [2] } },
                                    { extend: "pdf", className: "dropdown-item", text: "PDF", exportOptions: { columns: [2] } },
                                    { extend: "copy", className: "dropdown-item", text: "Copy", exportOptions: { columns: [2] } }
                                ]
                            },
                            {
                                text: `<button type="button"
                                        class="btn btn-primary btn-createorupdate-productvariant"
                                        data-product-id="${productId}"
                                        data-attribute-id="0"
                                        data-bs-toggle="modal"
                                        data-bs-target="#createOrUpdateProductVariantAttributeModal">
                                        <i class="icon-base ti ti-plus me-0 me-sm-1 icon-16px"></i>
                                        <span class="d-none d-sm-inline-block">Yeni Ürün Varyantı Oluştur</span>
                                    </button>`,
                                className: "p-0 border-0 bg-transparent"
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

    
        setTimeout(() => {
            const adjustments = [
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
    };
})(jQuery);
