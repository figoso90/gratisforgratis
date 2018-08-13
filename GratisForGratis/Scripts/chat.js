$(document).ready(function ()
{
    $('.carousel').carousel();
    enableFunzioni();
    $('#chatUtente .boxMessaggi .content').scrollTop($('#chatUtente .boxMessaggi .content')[0].scrollHeight);
});

function enableFunzioni() {
    // abilita funzioni
    $('#lnkAnnullaModifica').one("click", function () {
        reset();
    });
    $('#lnkInvia').one("click", function () {
        if ($("#ChatNewId").val() > 0) {
            modifica();
        } else {
            invia();
        }
    });
}

function alertErrore(data) {
    alert(data.responseText);
}

function refreshForm(data) {
    $("#boxNuovoMessaggio").html(data);
}

function refreshMessaggi(data) {
    $('#chatUtente .boxMessaggi .content').html(data);
    $('#chatUtente .boxMessaggi .content').scrollTop($('#chatUtente .boxMessaggi .content')[0].scrollHeight);
}

function enableModifica(id) {
    $.ajax({
        type: 'GET',
        url: '/Chat/FormModifica',
        dataType: "html",
        async: false,
        data: {
            id: decodeURI(id)
        },
        success: function (data) {
            // nel box nuovo messaggio inserisco form modifica...
            refreshForm(data);
            enableFunzioni();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertErrore(xhr);
        }
    });
}

function reset() {
    $.ajax({
        type: 'GET',
        url: '/Chat/FormInserimento',
        data: {
            destinatarioId: $("#ChatDestinatarioId").val()
        },
        dataType: "html",
        success: function (data) {
            // nel box nuovo messaggio inserisco form inserimento...
            refreshForm(data);
            enableFunzioni();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertErrore(xhr);
        }
    });
}

function invia() {
    $form = $('#FormChat');
    $.ajax({
        type: 'POST',
        url: '/Chat/Invia',
        dataType: "html",
        data: $form.serialize(),
        success: function (data) {
            if (data) {
                refreshMessaggi(data);
                reset();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertErrore(xhr);
        },
        complete: function () {
            $('#lnkInvia').one("click", function () {
                invia();
            });
        }
    });
}

function modifica() {
    $form = $('#FormChat');
    $.ajax({
        type: 'POST',
        url: '/Chat/Modifica',
        dataType: "html",
        data: $form.serialize(),
        success: function (data) {
            if (data) {
                refreshMessaggi(data);
                reset();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertErrore(xhr);
        },
        complete: function () {
            $('#lnkAnnullaModifica').one("click", function () {
                modifica();
            });
        }
    });
}

function elimina(id) {
    $.ajax({
        type: 'DELETE',
        url: '/Chat/Elimina',
        dataType: "html",
        data: {
            id: decodeURI(id)
        },
        success: function (data) {
            if (data) {
                refreshMessaggi(data);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertErrore(xhr);
        },
        complete: function () {
            $('#messaggio'+id + " .remove").one("click", function () {
                elimina(id);
            });
        }
    });
}