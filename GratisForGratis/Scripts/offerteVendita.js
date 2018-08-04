$(document).ready(function () {

    // inizializzo pulsanti di accettazione dell' offerta
    $('#grid .ok').one('click', function (event) {
        //accettaOfferta(this, $(this).parents('.purchase').attr('id'));
        $form = $(this).parents('form');
        $form.submit();
    });
    $('#grid .ko').one('click', function (event) {
        $form = $(this).parents('form');
        $form.attr('action', '/Annuncio/RifiutaOfferta');
        $form.submit();
        //rifiutaOfferta(this, $(this).parents('.purchase').attr('id'));
    });
});

/*
function accettaOfferta(link, token) {
    $.ajax({
        type: "POST",
        url: '/Annuncio/AccettaOfferta',
        data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (msg) {
            if (msg) {
                var risposta = JSON.parse(msg);
                $(link).parent('.stateText').html(risposta.Messaggio);
            }
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                accettaOfferta(link, token);
            });
            alert("Errore accettazione offerta: " + errore.responseText);
        }
    });
}

function rifiutaOfferta(link, token) {
    $.ajax({
        type: "POST",
        url: '/Annuncio/RifiutaOfferta',
        data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (msg) {
            if (msg) {
                var risposta = JSON.parse(msg);
                $(link).parent('.stateText').html(risposta.Messaggio);
            }
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                rifiutaOfferta(link, token);
            });
            alert("Errore rifiuto offerta: " + errore.responseText);
        }
    });
}
*/