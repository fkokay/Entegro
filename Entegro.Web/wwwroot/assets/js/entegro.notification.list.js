var Entegro = Entegro || {};
Entegro.notification = Entegro.notification || {};

Entegro.notification.list = (function ($) {

    function initList() {
        const table = $('#NotificationTable').DataTable({
            language: {
                search: "Bildirim Ara:",
                lengthMenu: "Göster _MENU_ kayıt",
                info: "_TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
                infoEmpty: "Kayıt yok",
                infoFiltered: "(_MAX_ kayıt içerisinden filtrelendi)",
                zeroRecords: "Eşleşen kayıt bulunamadı",
                paginate: {
                    next: '<i class="icon-base ti ti-chevron-right"></i>',
                    previous: '<i class="icon-base ti ti-chevron-left"></i>',
                    first: '<i class="icon-base ti ti-chevrons-left"></i>',
                    last: '<i class="icon-base ti ti-chevrons-right"></i>'
                }
            },
            serverSide: true,
            order: [[4, 'desc']],
            ajax: {
                url: '/Notification/NotificationList',
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            },
            columns: [
                { data: 'Id', orderable: false },
                { data: 'Id', visible: false },
                { data: 'Title' },
                { data: 'Message' },
                {
                    data: 'NotificationDate',
                    render: function (data, type) {
                        if (type === "sort" || type === "type") return data;
                        return moment(data).format("DD.MM.yyyy HH:mm");
                    }
                },
                {
                    data: 'Type',
                    render: function (data) {
                        const types = {
                            1: { text: 'Bilgi', class: 'badge bg-info' },
                            2: { text: 'Başarılı', class: 'badge bg-success' },
                            3: { text: 'Uyarı', class: 'badge bg-warning' },
                            4: { text: 'Hata', class: 'badge bg-danger' }
                        };
                        const type = types[data] || { text: 'Bilinmiyor', class: 'badge bg-secondary' };
                        return `<span class="${type.class}">${type.text}</span>`;
                    }
                },
                {
                    data: 'IsRead',
                    className: "text-center",
                    render: function (data) {
                        const checked = data ? "checked" : "";
                        const title = data ? "Okundu" : "Okunmadı";
                        return `
                            <div class="form-check d-inline-flex justify-content-center">
                                <input class="form-check-input" type="checkbox" ${checked} disabled title="${title}">
                            </div>`;
                    }
                },
            ],
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false,
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
            displayLength: 10
        });

        // Silme işlemi
        $(document).on('click', '.delete-record', function () {
            const notificationId = $(this).data('id');
            Swal.fire({
                title: 'Emin misiniz?',
                text: 'Bu bildirim silinecek!',
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
                        url: '/Notification/Delete',
                        type: 'POST',
                        data: { id: notificationId },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Silindi!',
                                    text: 'Bildirim başarıyla silindi.',
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

    return {
        init: initList
    };

})(jQuery);
