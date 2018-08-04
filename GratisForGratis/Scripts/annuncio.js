$(function () {
    function suggerimentoAttivazioneAnnuncio(id) {
        $.ajax({
            type: 'POST',
            url: '/Home/SuggerimentoAttivazioneAnnuncio',
            dataType: "json",
            data: {
                id: decodeURI(id)
            },
            success: function (data) {
                alert(data);
            }
        });
    }
    /*document.getElementById('albumAnnuncio').onclick = function (event) {
        event = event || window.event;
        var target = event.target || event.srcElement,
            link = target.src ? target.parentNode : target,
            options = { index: link, event: event, startSlideshow: true, carousel: true },
            links = this.getElementsByTagName('a');
        blueimp.Gallery(links, options);
    };*/
    blueimp.Gallery($('#albumAnnuncio a'), {
        container: '#blueimp-image-carousel',
        carousel: true
    });
    /*
    $('#acquisto .album').jGallery({
        height: '500px',
        width: '500px',
        thumbnailsFullScreen: true,
        zoomSize: 'original',
        hideThumbnailsOnInit: true,
        maxMobileWidth: 300,
        mode: 'standard',
        tooltips: true,
        tooltipZoom: 'Zoom',
        canChangeMode: false,
        canMinimalizeThumbnails: false,
        title: $(this).find('img').attr('alt')
    });
    */
    attivazioneInfoSpedizione($('.changeTipoScambio').val());
    $('.changeTipoScambio').change(function () {
        //alert($(this).val());
        attivazioneInfoSpedizione($(this).val());
    });

    $('.formAnnuncio .exchange .form-control').each(function () {
        $($(this).data('class')).val($(this).val());
    });

    $('.formAnnuncio .exchange .form-control').change(function () {
        $($(this).data('class')).val($(this).val());
    });
});

function attivazioneInfoSpedizione(valore) {
    //alert(valore);
    if (valore == 0) {
        $('.formAnnuncio .shipment').addClass('hide');
    } else {
        $('.formAnnuncio .shipment').removeClass('hide');
    }
}

function compraAnnuncio(link) {
    $form = $(link).parents('.formAnnuncio');
    /*alert($form.serialize());
    $.ajax({
        type: 'POST',
        url: '/Annuncio/Compra',
        dataType: "json",
        data: $form.serialize(),
        success: function (data) {
            alert(data);
        },
        error: function (error, status) {
            alert(error.responseJSON);
        }
    });*/
    $form.submit();
}