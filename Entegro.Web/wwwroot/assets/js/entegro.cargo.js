var Entegro = Entegro || {};
Entegro.cargo = Entegro.cargo || {};

Entegro.cargo = (function ($) {

    function SelectCargo(cargoType) {
        $('#CargoType').val(cargoType);
        bootstrap.Modal.getOrCreateInstance(document.getElementById('addCargo')).show();
    }

    function initFormSubmit() {
        $('#addCargoForm').on('submit', function (e) {
            e.preventDefault();

            $.ajax({
                url: '/settings/Cargo',   
                type: 'POST',
                data: $(this).serialize(),
                success: function (res) {
                    if (res.success) {
                        location.reload();
                    }
                },
                error: function () {
                    alert("Hata oluştu");
                }
            });
        });
    }

    function iptalEt(integrationSystemId) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Bu entegrasyonu silmek üzeresiniz!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Evet',
            cancelButtonText: 'Vazgeç'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch('/settings/CargoDelete', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(integrationSystemId)
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            Swal.fire({
                                title: 'İptal Edildi!',
                                text: 'Entegrasyon başarıyla silindi.',
                                icon: 'success',
                                timer: 1500,
                                showConfirmButton: false
                            }).then(() => {
                                window.location.href = '/settings/cargo';
                            });
                        } else {
                            Swal.fire('Hata!', data.message || 'Bir hata oluştu.', 'error');
                        }
                    })
                    .catch(() => Swal.fire('Hata!', 'Sunucuya bağlanırken hata oluştu.', 'error'));
            }
        });
    }

    return {
        SelectCargo: SelectCargo,
        initFormSubmit: initFormSubmit,
        iptalEt: iptalEt
    };

})(jQuery);
