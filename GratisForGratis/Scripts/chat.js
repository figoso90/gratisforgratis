$(document).ready(function ()
{
    // abilita funzioni
});

function enableModifica() {
    $.ajax({
        type: 'GET',
        url: '/Chat/FormModifica',
        dataType: "html",
        data: {
            id: decodeURI(id)
        },
        success: function (data) {
            // nel box nuovo messaggio inserisco form modifica...
            disableModifica();
        }
    });
}

function disableModifica() {
    $.ajax({
        type: 'GET',
        url: '/Chat/FormInserimento',
        dataType: "html",
        success: function (data) {
            // nel box nuovo messaggio inserisco form inserimento...
        }
    });
}

function invia() {
    $form = $('#boxNuovoMessaggio > form');
    $.ajax({
        type: 'POST',
        url: '/Chat/Invia',
        dataType: "json",
        data: $form.serialize(),
        success: function (data) {
            if (data) {
                // ricarica pagina
            }
        }
    });
}

function inviaChatFallito(data) {
    alert(data);
}

function modifica() {
    $form = $('#boxNuovoMessaggio > form');
    $.ajax({
        type: 'POST',
        url: '/Chat/Modifica',
        dataType: "json",
        data: $form.serialize(),
        success: function (data) {
            if (data) {
                // ricarica pagina
            }
        }
    });
}

function elimina() {
    $.ajax({
        type: 'DELETE',
        url: '/Chat/Elimina',
        dataType: "json",
        data: {
            id: decodeURI(id)
        },
        success: function (data) {
            if (data) {
                // ricarica pagina
            }
        }
    });
}