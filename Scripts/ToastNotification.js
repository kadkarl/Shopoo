$(document).ready(function ($) {

    toastr.options.closeMethod = 'fadeOut';
    toastr.options.closeDuration = 1000;
    toastr.options.closeEasing = 'swing';
    toastr.options.fadeOut = 2500;


    $(".btnAjoutProduitPanier").click((e) => {
        e.preventDefault();

        var url = e.target.href;

        $.ajax({
            url: url,
            async: true,
            type: 'GET',
            success: (res) => {
                if (res.success === true) {
                    toastr.success(res.msg, "Shopoo");
                    var delay = 1000;
                    var url = '/home/index'
                    setTimeout(function () { window.location = url; }, delay);
                }
            }
        });
    })
});