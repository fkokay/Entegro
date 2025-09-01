var Entegro = Entegro || {};
Entegro.brand = Entegro.brand || {};

Entegro.brand = (function ($) {

    function initUploader() {
        if (!$('#MediaUpload').length) return;

        $("#MediaUpload").dropzoneWrapper({
            onUploading: null,
            onUploadCompleted: null,
            onAborted: null,
            onError: null,
            onFileRemove: null,
            onCompleted: null,
            onMediaSelected: null,
            maxFiles: 1,
            maxFilesSize: 102400,
            timeout: 300000,
            clickableElement: null,
            previewContainerId: "",
            showRemoveButton: true,
            showRemoveButtonAfterUpload: true,
            downloadEnabled: false
        });
    }

    function initDeleteButton() {
        const $btn = $('#btnDeleteBrand');
        if (!$btn.length) return;

        $btn.on('click', function () {
            const brandId = $(this).data('id');

            Swal.fire({
                title: 'Silme Türünü Seçin',
                html: `
                    <div style="text-align: left">
                        <label><input type="radio" name="delete-option" value="0" checked> Sadece bağlantıyı kaldır</label><br>
                        <label><input type="radio" name="delete-option" value="1"> Bağlantı ve alt markaları sil</label>
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: 'Devam Et',
                cancelButtonText: 'İptal',
                customClass: {
                    confirmButton: 'btn btn-danger me-3',
                    cancelButton: 'btn btn-secondary'
                },
                buttonsStyling: false,
                preConfirm: () => {
                    const selected = document.querySelector('input[name="delete-option"]:checked');
                    if (!selected) {
                        Swal.showValidationMessage('Lütfen bir seçenek seçin.');
                        return false;
                    }
                    return selected.value;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const chooseType = parseInt(result.value);

                    $.ajax({
                        url: '/brand/delete',
                        type: 'POST',
                        data: {
                            id: brandId,
                            chooseType: chooseType
                        },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Marka başarıyla silindi.',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    window.location.href = '/brand/list';
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Hata!',
                                    text: response.message || 'Silme işlemi başarısız oldu.',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-danger'
                                    },
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
                                customClass: {
                                    confirmButton: 'btn btn-danger'
                                },
                                buttonsStyling: false
                            });
                        }
                    });
                }
            });
        });
    }

    function initFormValidation(formActionUrl) {
        const form = document.getElementById('brand-form');
        if (!form) return;

        FormValidation.formValidation(form, {
            locale: 'tr_TR',
            localization: FormValidation.locales.tr_TR,
            fields: {
                'Name': {
                    validators: {
                        notEmpty: {
                            message: 'Marka adı boş bırakılamaz.'
                        },
                        stringLength: {
                            min: 3,
                            message: 'Marka adı en az 3 karakter olmalıdır.'
                        }
                    }
                },
                'DisplayOrder': {
                    validators: {
                        notEmpty: {
                            message: 'Sıra boş bırakılamaz.'
                        },
                        numeric: {
                            message: 'Sıra sadece sayı olmalıdır.'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap5: new FormValidation.plugins.Bootstrap5({
                    eleValidClass: '',
                    rowSelector: '.row, .mb-3'
                }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                autoFocus: new FormValidation.plugins.AutoFocus()
            },
            init: instance => {
                instance.on('plugins.message.placed', function (e) {
                    if (e.element.parentElement.classList.contains('input-group')) {
                        e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
                    }
                });

                instance.on('core.form.valid', function () {
                    const $form = $(form);
                    const serializedData = $form.serialize();

                    $.ajax({
                        url: formActionUrl,
                        type: 'POST',
                        data: serializedData,
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    title: 'Başarılı!',
                                    text: 'İşlem başarıyla tamamlandı.',
                                    icon: 'success',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-success'
                                    },
                                    buttonsStyling: false
                                }).then(() => {
                                    window.location.href = '/brand/list';
                                });
                            } else {
                                Swal.fire({
                                    title: 'Hata!',
                                    text: response.message || 'Bir hata oluştu.',
                                    icon: 'error',
                                    confirmButtonText: 'Tamam',
                                    customClass: {
                                        confirmButton: 'btn btn-danger'
                                    },
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
                                customClass: {
                                    confirmButton: 'btn btn-danger'
                                },
                                buttonsStyling: false
                            });
                        }
                    });
                });
            }
        });
    }

    function initSelect2() {
        const $dropdown = $('#ParentBrandId');
        if (!$dropdown.length) return;

        if ($dropdown.parent().css('position') !== 'relative') {
            $dropdown.wrap('<div class="position-relative"></div>');
        }

        $dropdown.select2({
            width: '100%',
            placeholder: 'Üst Marka Seçiniz',
            allowClear: true,
            dropdownParent: $dropdown.parent(),
            minimumInputLength: 0,
            language: {
                inputTooShort: () => 'Daha fazla karakter yazın',
                searching: () => 'Aranıyor...',
                noResults: () => 'Sonuç bulunamadı'
            },
            ajax: {
                url: '/brand/AllBrands',
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
                        results: Array.isArray(data.results) ? data.results : [],
                        pagination: {
                            more: !!(data.pagination && data.pagination.more)
                        }
                    };
                },
                cache: true
            },
            templateResult: item => item.text || '',
            templateSelection: item => item.text || '',
            escapeMarkup: m => m
        });
    }

    return {
        init: function (formActionUrl) {
            initUploader();
            initFormValidation(formActionUrl);
            initDeleteButton();
            initSelect2();
        }
    };

})(jQuery);
