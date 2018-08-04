$(document).ready(function () {
    $tipoOfferta = $('#Offerta_TipoOfferta');
    sceltaOfferta($tipoOfferta);
    $tipoOfferta.change(function () {
        sceltaOfferta(this);
    });

    $('#CercaAnnuncio').autocomplete({
        position: { my: "left top", at: "left bottom+15px", collision: "flip" },
        delay: 0,
        minLength: 0,
        //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
        source: function (request, response) {
            $.ajax({
                url: "/Cerca/AnnunciBarattabili",
                dataType: "json",
                data: {
                    term: request.term,
                    tipoScambio: $('#formOfferta select[name="TipoScambio"]').val()
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $baratto = $('<input class="' + ui.item.Value + '" type="hidden" name="BarattiToken" value="' + ui.item.Value + '" />');
            $('#previewBarter').append($baratto);
            var prezzo = '';
            if (ui.item.Valuta !== null) {
                prezzo = '<span class="price">' + ui.item.Prezzo + '</span>' + ui.item.Valuta;
            }
            $annuncio = $('<p class="barter ' + ui.item.Value + '">' +
                ui.item.Label + prezzo +
                '<a class="remove" href="javascript:void(0);" onclick="rimuoviBaratto(\'' + ui.item.Value + '\',\'#' + $(this).attr('id') + '\');"> X</a></p>');
            $('#previewBarter').append($annuncio);
            

            calcoloCostoSpedizioneBaratto();
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.Label);
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        if ($('#previewBarter .' + item.Value).length <= 0)
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        else
            return ul;
    };
    /*
    $('#CercaOggetto').autocomplete({
        position: { my: "left top", at: "left bottom+15px", collision: "flip" },
        delay: 0,
        minLength: 0,
        //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
        source: function (request, response) {
            $.ajax({
                url: "/Cerca/OggettiBarattabili",
                dataType: "json",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $baratto = $('<input class="' + ui.item.Value + '" type="hidden" name="Offerta.OggettiBarattati" value="' + ui.item.Value + '" />');
            $oggetto = $('<p class="barter ' + ui.item.Value + '">' + ui.item.Label + '<a class="remove" href="javascript:void(0);" onclick="rimuoviBaratto(\'' + ui.item.Value + '\',\'#' + $(this).attr('id') + '\');"> X</a></p>')
            $('#previewBarter').append($baratto);
            $('#previewBarter').append($oggetto);
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.Label);
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        if ($('#previewBarter .' + item.Value).length <= 0)
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        else
            return ul;
    };

    $('#CercaServizio').autocomplete({
        position: { my: "left top", at: "left bottom+15px", collision: "flip" },
        delay: 0,
        minLength: 0,
        //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
        source: function (request, response) {
            $.ajax({
                url: "/Cerca/ServiziBarattabili",
                dataType: "json",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $baratto = $('<input class="' + ui.item.Value + '" type="hidden" name="Offerta.ServiziBarattati" value="' + ui.item.Value + '" />');
            $servizio = $('<span class="barter ' + ui.item.Value + '">' + ui.item.Label + '<a class="remove" href="javascript:void(0);" onclick="rimuoviBaratto(\'' + ui.item.Value + '\',\'#' + $(this).attr('id') + '\');"> X</a></span>')
            $('#previewBarter').append($baratto);
            $('#previewBarter').append($servizio);
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.Label);
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        if ($('#previewBarter .' + item.Value).length <= 0)
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        else
            return ul;
    };
    */

    $('.section-3 .photo img').each(function (key, value) {
        if ($(this).data('src') !== undefined) {
            $(this).attr('src', $(this).data('src'));
        }
    });
    //$("img").lazyload();
    // override Highslide settings here
    // instead of editing the highslide.js file
    //hs.graphicsDir = '/highslide/graphics/';
});

function calcoloCostoSpedizioneBaratto() {
    var costoSpedizione = 0;
    $("#previewBarter .price").each(function (index) {
        costoSpedizione += $(this).text();
    });

    if (costoSpedizione > 0) {
        $('#priceShipmentBarter').show();
        $('#priceShipmentBarter').html(costoSpedizione);
    } else {
        $('#priceShipmentBarter').hide();
        $('#priceShipmentBarter').html('');
    }
}

function sceltaOfferta(elemento)
{
    if ($(elemento).val() === 1) {
        $('#boxBarter').show();
        $("#CercaAnnuncio").autocomplete("search", "");
    } else {
        $('#boxBarter').hide();
    }
}

function rimuoviBaratto(link,cerca) {
    $('.' + link).remove();
    $(cerca).val('');
}

function toggleOfferta(tag, form)
{
    //alert("toggle: " + tag);
    $('.linkOfferta hide').removeClass('hide');
    $(tag).addClass('hide');
    $(form).toggle();
    $('html').loader('hide');
}

function inviaOffertaAnnuncio(link) {
    $form = $(link).parents('.formAnnuncio');
    $.ajax({
        type: 'POST',
        url: '/Annuncio/InviaOfferta',
        dataType: "json",
        data: $form.serialize(),
        success: function (data) {
            alert(data.Messaggio);
            $form.remove();
            window.location = "/Acquisti";
        },
        error: function (error, status) {
            alert(error.responseJSON);
        }
    });
}