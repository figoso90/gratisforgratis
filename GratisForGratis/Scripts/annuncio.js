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

    blueimp.Gallery($('#albumAnnuncio a'), {
        container: '#blueimp-image-carousel',
        carousel: true,
        //titleProperty: 'title',
        onslidecomplete: function (index, slide) {
            // Callback function executed when the Gallery is initialized.
            //alert("Index: " + index);
            $item = $('#blueimp-image-carousel').find('.slide').eq(index);
            //$item.attr('data-title', $('#albumAnnuncio a:eq(' + index + ')').attr('title'));
            $item.attr('data-image', $item.children('img').attr('src'));
            $item.attr('data-scale', 2.4);
            zoomImmagine($item);
        },
    });
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
    //alert($('#CercaAnnuncio').length);
    if ($('#CercaAnnuncio').length > 0) {
        $('#previewBarter .barter').each(function () {
            rimuoviBaratto($(this).data('id'), '#CercaAnnuncio');
        });
    }

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