var Entegro = Entegro || {};
Entegro.mail = Entegro.mail || {};

Entegro.mail = (function ($) {

    function SelectMail(integrationSystemType) {
        $('#IntegrationSystemType').val(integrationSystemType);
        const modalEl = document.getElementById('addMail');
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }

    function initFormSubmit() {
        const $form = $('#addMailForm');
        if ($form.length === 0) return;

        $form.off('submit.entegroMail').on('submit.entegroMail', function (e) {
            e.preventDefault();

            $.ajax({
                url: '/settings/Mail', // JS dosyasında @Url.Action yerine direkt path
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

            fetch('/settings/MailDelete', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(integrationSystemId)
            })
                .then(r => r.json())
                .then(data => {
                    if (data && data.success) {
                        Swal.fire({
                            title: 'İptal Edildi!',
                            text: 'Entegrasyon başarıyla silindi.',
                            icon: 'success',
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => {
                            window.location.href = '/settings/mail';
                        });
                    } else {
                        Swal.fire('Hata!', (data && data.message) || 'Bir hata oluştu.', 'error');
                    }
                })
                .catch(() => Swal.fire('Hata!', 'Sunucuya bağlanırken hata oluştu.', 'error'));
        });
    }

    return {
        SelectMail,
        initFormSubmit,
        iptalEt
    };

})(jQuery);


