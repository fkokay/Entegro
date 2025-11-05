var Entegro = Entegro || {};
Entegro.mapcolumns = Entegro.mapcolumns || {};

Entegro.mapcolumns = (function ($) {

    function initFormValidation() {
        const importForm = document.getElementById("importForm");
        if (!importForm) return;

        importForm.addEventListener("submit", function (e) {
            let isValid = true;
            let hasAnyChecked = false;
            const rows = document.querySelectorAll("tbody tr");

            rows.forEach((row, index) => {
                const checkbox = row.querySelector(`input[name="ColumnMappings[${index}].IsImport"]`);
                const select = row.querySelector(`select[name="ColumnMappings[${index}].DbColumn"]`);
                const $container = $(select).next('.select2');

                if (checkbox && checkbox.checked) {
                    hasAnyChecked = true;
                    if (select && select.value === "") {
                        isValid = false;
                        $container.find('.select2-selection').addClass('is-invalid');
                    } else {
                        $container.find('.select2-selection').removeClass('is-invalid');
                    }
                }
                else if (select && select.value !== "") {
                    isValid = false;
                    $container.find('.select2-selection').addClass('is-invalid');
                }
                else {
                    $container.find('.select2-selection').removeClass('is-invalid');
                }
            });

            const profileNameInput = document.querySelector('input[name="ProfileName"]');
            if (!profileNameInput || !profileNameInput.value.trim()) {
                e.preventDefault();

                Swal.fire({
                    icon: 'warning',
                    title: 'Eksik Bilgi',
                    text: 'Lütfen profil adı giriniz.',
                    confirmButtonText: 'Tamam',
                    confirmButtonColor: '#dc3545'
                });

                if (profileNameInput) {
                    profileNameInput.classList.add("is-invalid");
                    profileNameInput.focus();
                }
                return;
            } else {
                profileNameInput.classList.remove("is-invalid");
            }

            if (!hasAnyChecked) {
                e.preventDefault();
                Swal.fire({
                    icon: 'info',
                    title: 'Seçim Gerekli',
                    text: "Lütfen en az bir satır için 'İçe Aktarılsın mı?' kutucuğunu işaretleyin.",
                    confirmButtonText: 'Tamam',
                    confirmButtonColor: '#0d6efd'
                });
                return;
            }

            if (!isValid) {
                e.preventDefault();
                Swal.fire({
                    icon: 'error',
                    title: 'Hatalı Eşleştirme',
                    text: "İşaretlenen satırlarda eksik veya hatalı eşleştirme var. Lütfen kontrol edin.",
                    confirmButtonText: 'Tamam',
                    confirmButtonColor: '#dc3545'
                });
                return;
            }

            // Form geçerliyse yükleniyor ekranını göster
            window.showLoading("Lütfen bekleyiniz..");
        });
    }

    function initSelect2() {
        $('.select2').select2({
            placeholder: "-- Seçiniz --",
            allowClear: true
        });

        $('.select2').on('change', function () {
            const $container = $(this).next('.select2');
            $container.find('.select2-selection').removeClass('is-invalid');
        });

        $('input[type="checkbox"][name^="ColumnMappings"]').on('change', function () {
            const row = $(this).closest('tr');
            const select = row.find('select');
            const $container = $(select).next('.select2');

            if ($(this).is(':checked')) {
                $container.find('.select2-selection').removeClass('is-invalid');
            }
        });
    }

    return {
        init: function () {
            initFormValidation();
            initSelect2();
        }
    };

})(jQuery);
