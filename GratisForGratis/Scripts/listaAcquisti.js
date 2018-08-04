$(document).ready(function () {
    // inizializzo click per aprire il dettaglio
    /*$('#grid .preview .cella:first-child').on('click', function (event) {
        $(this).next('.details').toggle();
    });*/

    // inizializzo pulsanti di conferma acquisto
    /*
    $('#grid .ok').one('click', function (event) {
        //alert("Id acquisto: " + $(this).parents('.purchase').attr('id'));
        confermaAcquisto(this, $(this).parents('.purchase').attr('id'));
    });
    */
});

function confermaAcquisto(link,token) {
    $acquisto = $('#grid input:checked[name="acquisto"]').parents('.riga');
    $data = $acquisto.attr('id');
    if ($.trim(token).length > 0)
        $data = token;
    if ($.trim($data).length > 0) {
        $.ajax({
            type: "POST",
            url: '/Pagamento/Definitivo',
            data: "acquisti=" + $data,
            //contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                if (msg) {
                    // DA MODIFICARE PERCHè SONO STATE PERSE LE MODIFICHE PRECEDENTI
                    //alert(JSON.stringify(msg));
                    $(link).parents('.purchase').find('.state .stateText').html(msg.Messaggio);
                    $(link).parents('.purchase').find('.state .payment').remove();
                }
            },
            error: function (errore, stato, messaggio) {
                $(link).one('click', function (event) {
                    confermaAcquisto(link, token);
                });
                alert("Errore conferma acquisto: " + errore.responseText);
            }
        });
    } else {
        alert('Nessun acquisto selezionato!');
    }
}