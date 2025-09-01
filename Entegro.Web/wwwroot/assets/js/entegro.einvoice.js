var Entegro = Entegro || {};
Entegro.einvoice = Entegro.einvoice || {};

Entegro.einvoice = (function ($) {

    function SelectEInvoice(integrationSystemType) {
        $('#IntegrationSystemType').val(integrationSystemType);
        const modalEl = document.getElementById('addEinvoice');
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }

    function initFormSubmit() {
        const $form = $('#addEinvoiceForm');
        if ($form.length === 0) return;

        $form.off('submit.entegroEinvoice').on('submit.entegroEinvoice', function (e) {
            e.preventDefault();

            $.ajax({
                url: '/settings/Einvoice', 
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
            text: 'Bu entegrasyonu silmek üzeresiniz!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Evet',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (!result.isConfirmed) return;

            fetch('/settings/EinvoiceDelete', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(integrationSystemId)
            })
                .then(r => r.json())
                .then(data => {
                    if (data && data.success) {
                        Swal.fire({
                            title: 'İptal Edildi!',
                            text: 'Bu entegrasyon başarıyla silindi.',
                            icon: 'success',
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => {
                            window.location.href = '/settings/Einvoice';
                        });
                    } else {
                        Swal.fire('Hata!', (data && data.message) || 'Bir hata oluştu.', 'error');
                    }
                })
                .catch(() => Swal.fire('Hata!', 'Sunucuya bağlanırken hata oluştu.', 'error'));
        });
    }

    return {
        SelectEInvoice,
        initFormSubmit,
        iptalEt
    };

})(jQuery);

