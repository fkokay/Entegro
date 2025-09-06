var Entegro = Entegro || {};
Entegro.erp = Entegro.erp || {};

Entegro.erp = (function ($) {

    function SelectErp(erpType) {
        $('#ErpType').val(erpType);
        const modalEl = document.getElementById('addErp');
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }

    function initFormSubmit() {
        const $form = $('#addErpForm');
        if ($form.length === 0) return;

        $form.off('submit.entegroErp').on('submit.entegroErp', function (e) {
            e.preventDefault();

            $.ajax({
                url: '/Settings/Erp',
                type: 'POST',
                data: $(this).serialize(),
                success: function (res) {
                    if (res && res.success) {
                        location.reload();
                    } else {
                        Swal.fire('Hata!', (res && res.message) || 'Bir hata oluştu.', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Hata!', 'Sunucuya bağlanırken hata oluştu.', 'error');
                }
            });
        });
    }

    function iptalEt(integrationSystemId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: 'Bu mağazayı silmek üzeresiniz!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Evet',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (!result.isConfirmed) return;

            fetch('/settings/ErpDelete', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(integrationSystemId)
            })
                .then(r => r.json())
                .then(data => {
                    if (data && data.success) {
                        Swal.fire({
                            title: 'İptal Edildi!',
                            text: 'Bu mağaza başarıyla silindi.',
                            icon: 'success',
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => {
                            window.location.href = '/settings/erp';
                        });
                    } else {
                        Swal.fire('Hata!', (data && data.message) || 'Bir hata oluştu.', 'error');
                    }
                })
                .catch(() => Swal.fire('Hata!', 'Sunucuya bağlanırken hata oluştu.', 'error'));
        });
    }

    return {
        SelectErp,
        initFormSubmit,
        iptalEt
    };

})(jQuery);


