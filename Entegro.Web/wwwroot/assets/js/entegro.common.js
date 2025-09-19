(function ($, window, document) {
    window.getQueryStrings = function (search) {
        var assoc = {};
        var decode = function (s) { return decodeURIComponent(s.replace(/\+/g, " ")); };
        var queryString = (search || location.search).substring(1);
        var keyValues = queryString.split('&');

        for (var i in keyValues) {
            var item = keyValues[i].split('=');
            if (item.length > 1) {
                var key = decode(item[0]).toLowerCase();
                var val = decode(item[1]);
                if (assoc[key] === undefined) {
                    assoc[key] = val;
                } else {
                    var v = assoc[key];
                    if (v.constructor != Array) {
                        assoc[key] = [];
                        assoc[key].push(v);
                    }
                    assoc[key].push(val);
                }
            }
        }

        return assoc;
    };

    window.displayNotification = function (message, type, sticky, delay) {
        if (window.EventBroker === undefined || window._ === undefined)
            return;

        var notify = function (msg) {
            if (!msg)
                return;

            EventBroker.publish("message", {
                text: msg,
                type: type,
                delay: delay || (type === "success" ? 2500 : 5000),
                hide: !sticky
            });
        };

        if (_.isArray(message)) {
            $.each(message, function (i, val) {
                notify(val);
            });
        }
        else {
            notify(message);
        }
    };

    window.showLoading = function (message) {
        $.blockUI({
            message: '<div class="custom-loader"><div class="spinner"></div><p>' + message +'</p></div>',
            css: {
                border: 'none',
                padding: '20px',
                backgroundColor: 'transparent',
                color: '#fff',
                zIndex:999999
            },
            overlayCSS: {
                backgroundColor: 'rgba(0,0,0,0.6)',
                opacity: 1,
                cursor: 'wait',
                zIndex:99999
            }
        });
    };

    window.hideLoading = function () {
        $.unblockUI();
    };

    window.connectionTest = function () {
        const integrationSystemId = $('#IntegrationSystemId').val();
        const marketplaceType = $('#MarketplaceType').val();

       
        const data = {
            IntegrationSystemId: integrationSystemId,
            MarketplaceType: marketplaceType.trim()
        };

        $.ajax({
            url: '/settings/MarketplaceTest',
            type: 'POST',
            data: data,
            success: function (res) {
                if (res && res.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Bağlantı Başarılı',
                        text: 'Pazaryeri bağlantısı başarıyla sağlandı.',
                        confirmButtonText: 'Tamam'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Bağlantı Başarısız',
                        text: res?.message || 'Bağlantı sırasında bir hata oluştu.'
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Hatası:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Sunucu Hatası',
                    text: 'Sunucuya bağlanırken bir hata oluştu.'
                });
            }
        });
    };

})(jQuery, this, document);