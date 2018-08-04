$(document).ready(function () {
    $('#grid .purchase .cancelBarter').one('click', function (event) {
        anullaBaratto(this);
    });
    $('#grid .purchase .deleteSell').one('click', function (event) {
        anullaVendita(this);
    });
    $('#grid .purchase .enableSell').one('click', function (event) {
        attivaVendita(this);
    });
    $('#grid .purchase .unblock').one('click', function (event) {
        sbloccaAnnuncio(this);
    });
});
function anullaBaratto(link) {
    $vendita = $(link).parents('.purchase');
    var token = $vendita.attr('id');
    $.ajax({
        type: "DELETE",
        url: '/Annuncio/AnnullaBaratto?' + $.param({ "token": token }),
        //data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $(link).parent('.cella').remove();
            $vendita.find('.stateText').text(msg.Messaggio);
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                anullaBaratto(link);
            });
            alert("Errore annullo baratto: " + decodeURIComponent(errore.responseText));
        }
    });
}
function anullaVendita(link) {
    $vendita = $(link).parents('.purchase');
    var token = $vendita.attr('id');
    $.ajax({
        type: "DELETE",
        url: '/Annuncio/AnnullaVendita?' + $.param({ "token": token }),
        //data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //$(link).parent('.cella').remove();
            // elimina tutti i bottoni presenti se elimino la vendita (tasto offerta - attiva annuncio)
            $vendita.find('.btn').each(function (index, value) {
                $(this).parent('.cella').remove();
            });
            $vendita.find('.stateText').html(msg.Messaggio);
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                anullaVendita(link);
            });
            alert("Errore annullo vendita: " + decodeURIComponent(errore.responseText));
        }
    });
}
function attivaVendita(link) {
    $vendita = $(link).parents('.purchase');
    var token = $vendita.attr('id');
    $.ajax({
        type: "POST",
        url: '/Annuncio/AttivaVendita',
        data: {
            token: decodeURI(token)
        },
        dataType: "json",
        success: function (msg) {
            // refresh pagina
            location.reload(true);
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                attivaVendita(link);
            });
            alert(decodeURIComponent(errore.responseText));
        }
    });
}

function sbloccaAnnuncio(link) {
    $vendita = $(link).parents('.purchase');
    var token = $vendita.attr('id');
    $.ajax({
        type: "POST",
        url: '/Annuncio/Sblocca',
        data: {
            token: decodeURI(token)
        },
        dataType: "json",
        success: function (msg) {
            // refresh pagina
            location.reload(true);
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                sbloccaAnnuncio(link);
            });
            alert(decodeURIComponent(errore.responseText));
        }
    });
}