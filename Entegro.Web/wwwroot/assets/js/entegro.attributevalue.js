var Entegro = Entegro || {};
Entegro.AttributeValue = Entegro.AttributeValue || {};

Entegro.AttributeValue.Form = (function ($) {
    'use strict';

    let attributeValueFV = null;
    let _paCache = null;

    function fetchAllPA() {
        if (_paCache) return $.Deferred().resolve(_paCache).promise();
        return $.getJSON('/ProductAttributeValue/GetAllProductAttribute')
            .then(data => _paCache = data?.results || []);
    }

    function initPASelectOnce() {
        const $el = $('#ProductAttributeId');
        if (!$el.data('select2')) {
            $el.select2({
                dropdownParent: $('#attributeValueModal'),
                placeholder: 'Varyant adı seçin...',
                allowClear: true,
                ajax: {
                    url: '/ProductAttributeValue/GetAllProductAttribute',
                    type: 'GET',
                    dataType: 'json',
                    delay: 200,
                    processResults: data => ({ results: data?.results || [] }),
                    cache: true
                },
                width: '100%'
            });
        }
    }

    function setSelect2Selected(selector, id, text) {
        const $el = $(selector);
        if (!id || !$el.length) return;

        const setOption = (t) => {
            if (!$el.find("option[value='" + id + "']").length) {
                const opt = new Option(t || ('#' + id), id, true, true);
                $el.append(opt).trigger('change');
            } else {
                $el.val(id).trigger('change');
            }
        };

        if (text && text.length) {
            setOption(text);
        } else {
            fetchAllPA().done(list => {
                const hit = list.find(x => x.id === id);
                setOption(hit ? hit.text : null);
            });
        }
    }

    function resetForm() {
        const $form = $('#attributeValueForm')[0];
        $form.reset();
        $('#AttributeValueId').val('');
        $('#ProductAttributeId').val(null).trigger('change');
        $('#DisplayOrder').val('0');
    }

    function openModal(mode = 'create', id = null) {
        initPASelectOnce();
        resetForm();

        const $modal = $('#attributeValueModal');
        const $title = $('#attributeValueModalTitle');

        if (mode === 'create') {
            $title.text('Yeni Varyant Değeri');
            $modal.modal('show');
            return;
        }

        // edit mode
        $title.text('Varyant Değeri Güncelle');
        $.getJSON('/ProductAttributeValue/Edit', { id: id })
            .done((m) => {
                $('#AttributeValueId').val(m.Id);
                $('#Name').val(m.Name ?? '');
                $('#DisplayOrder').val(m.DisplayOrder ?? 0);
                setSelect2Selected('#ProductAttributeId', m.ProductAttributeId);
                $modal.modal('show');
            })
            .fail(xhr => {
                Swal.fire({
                    title: 'Hata!',
                    text: xhr.responseText || 'Kayıt bilgisi alınamadı.',
                    icon: 'error',
                    confirmButtonText: 'Tamam',
                    customClass: { confirmButton: 'btn btn-danger' },
                    buttonsStyling: false
                });
            });
    }

    function ensureValidation() {
        if (attributeValueFV) return attributeValueFV;

        attributeValueFV = FormValidation.formValidation(document.getElementById('attributeValueForm'), {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                ProductAttributeId: {
                    validators: {
                        notEmpty: { message: 'Varyant adı seçilmelidir.' }
                    }
                },
                Name: {
                    validators: {
                        notEmpty: { message: 'Değer adı boş bırakılamaz.' },
                        stringLength: {
                            min: 1,
                            max: 100,
                            message: 'Değer adı 1–100 karakter olmalıdır.'
                        }
                    }
                },
                DisplayOrder: {
                    validators: {
                        notEmpty: { message: 'Gösterim sırası boş bırakılamaz.' },
                        integer: { message: 'Tam sayı olmalıdır.' },
                        greaterThan: {
                            inclusive: true,
                            min: 0,
                            message: '0 veya daha büyük olmalıdır.'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: '', rowSelector: '.mb-3' }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                autoFocus: new FormValidation.plugins.AutoFocus()
            },
            init: (instance) => {
                instance.on('core.form.valid', function () {
                    const $form = $('#attributeValueForm');
                    const isEdit = !!$('#AttributeValueId').val();
                    const url = isEdit ? '/ProductAttributeValue/Edit' : '/ProductAttributeValue/Create';
                    const $submitBtn = $form.find('button[type="submit"]');

                    $submitBtn.prop('disabled', true);

                    $.post(url, $form.serialize())
                        .done((res) => {
                            if (res?.success) {
                                Swal.fire({
                                    title: isEdit ? 'Güncellendi!' : 'Başarılı!',
                                    text: isEdit ? 'Varyant değeri güncellendi.' : 'Varyant değeri eklendi.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: { confirmButton: 'btn btn-success' },
                                    buttonsStyling: false
                                }).then(() => {
                                    $('#attributeValueModal').modal('hide');
                                    if (Entegro.AttributeValue.List?.reload) {
                                        Entegro.AttributeValue.List.reload();
                                    } else {
                                        window.location.reload();
                                    }
                                });
                            } else {
                                Swal.fire({ icon: 'error', title: 'Hata!', text: res.message || 'İşlem başarısız.' });
                            }
                        })
                        .fail((xhr) => {
                            Swal.fire({ icon: 'error', title: 'Hata!', text: xhr.responseText || 'İşlem sırasında hata oluştu.' });
                        })
                        .always(() => $submitBtn.prop('disabled', false));
                });
            }
        });

        return attributeValueFV;
    }

    function bindEvents() {
        $(document).on('click', '[data-action="create-attribute-value"]', () => openModal('create'));

        $(document).on('click', '.edit-attributeValue', function () {
            const id = $(this).data('id');
            if (id) openModal('edit', id);
        });

        $(document).on('click', '.delete-attributeValue', function () {
            const id = $(this).data('id');
            if (!id) return;

            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu işlem geri alınamaz!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Evet, sil!',
                cancelButtonText: 'Vazgeç',
                customClass: {
                    confirmButton: 'btn btn-danger me-2',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then((result) => {
                if (!result.isConfirmed) return;

                $.post('/ProductAttributeValue/Delete', { id })
                    .done((res) => {
                        if (res?.success) {
                            Swal.fire({ title: 'Silindi!', text: 'Varyant değeri başarıyla silindi.', icon: 'success' });
                            Entegro.AttributeValue.List.reload?.();
                        } else {
                            Swal.fire({ title: 'Hata!', text: res.message || 'Silme işlemi başarısız.', icon: 'error' });
                        }
                    })
                    .fail((xhr) => {
                        Swal.fire({ title: 'Hata!', text: xhr.responseText || 'Silme sırasında hata oluştu.', icon: 'error' });
                    });
            });
        });

        $('#attributeValueForm').on('reset', () => {
            if (attributeValueFV) attributeValueFV.resetForm(true);
            setTimeout(() => $('#ProductAttributeId').val(null).trigger('change'), 0);
        });

        $('#attributeValueModal').on('shown.bs.modal', initPASelectOnce);
    }

    function init() {
        $(function () {
            ensureValidation();
            bindEvents();
        });
    }

    return {
        init
    };
})(jQuery);
