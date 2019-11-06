//$inputAllegatoSegnalazione = $('#AllegatoSegnalazione');
//$urlSegnalazione = '/Home/Segnalazione';

$(document).ready(function () {

    // SEGNALAZIONE
    /*
    $('#reporting').click(function (event) {
        $('#boxSegnalazione').dialog({
            title: 'Reporting',
            width: 550,
            modal: true,
            open: function (event, ui) {
                initUploadSegnalazione();
            },
            close: function (event, ui) {
                $('#boxSegnalazione select[name="Tipologia"]').val(0);
                //$('#boxSegnalazione input[name="EmailRisposta"]').val('');
                $('#boxSegnalazione input[name="Oggetto"]').val('');
                $('#boxSegnalazione textarea[name="Testo"]').val('');
                $inputAllegatoSegnalazione.uploadifive('destroy');
            }
        });

        $('#boxSegnalazione .button').click(function (event) {
            var numeroUpload = $('.uploadifive-queue-item').length;
            //alert(numeroUpload);
            // verificare la presenza o meno dell'allegato per mandare la segnalazione
            if (numeroUpload <= 0) {
                $.ajax({
                    type: "POST",
                    url: $urlSegnalazione,
                    data: $('#boxSegnalazione').serialize(),
                    dataType: "html",
                    success: function (msg) {
                        segnalazioneInviata(msg);
                    },
                    error: function (error, status, msg) {
                        segnalazioneErrata(msg);
                    }
                });
            } else {
                $inputAllegatoSegnalazione.data('uploadifive').settings.formData = {
                    '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
                    'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
                    'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
                    'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
                    'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
                    'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
                    'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
                };
                $inputAllegatoSegnalazione.uploadifive('upload');
            }
        });
    });
    $('#reportingMobile').click(function (event) {
        $('#boxSegnalazione').dialog({
            title: 'Reporting',
            width: 550,
            modal: true,
            open: function (event, ui) {
                initUploadSegnalazione();
            },
            close: function (event, ui) {
                $('#boxSegnalazione select[name="Tipologia"]').val(0);
                //$('#boxSegnalazione input[name="EmailRisposta"]').val('');
                $('#boxSegnalazione input[name="Oggetto"]').val('');
                $('#boxSegnalazione textarea[name="Testo"]').val('');
                $inputAllegatoSegnalazione.uploadifive('destroy');
            }
        });

        $('#boxSegnalazione .button').click(function (event) {
            var numeroUpload = $('.uploadifive-queue-item').length;
            //alert(numeroUpload);
            // verificare la presenza o meno dell'allegato per mandare la segnalazione
            if (numeroUpload <= 0) {
                $.ajax({
                    type: "POST",
                    url: $urlSegnalazione,
                    data: $('#boxSegnalazione').serialize(),
                    dataType: "html",
                    success: function (msg) {
                        segnalazioneInviata(msg);
                    },
                    error: function (error, status, msg) {
                        segnalazioneErrata(msg);
                    }
                });
            } else {
                $inputAllegatoSegnalazione.data('uploadifive').settings.formData = {
                    '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
                    'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
                    'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
                    'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
                    'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
                    'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
                    'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
                };
                $inputAllegatoSegnalazione.uploadifive('upload');
            }
        });
    });
    */
});

/*
function initUploadSegnalazione() {
    // crea oggetto uploadifive
    $inputAllegatoSegnalazione.uploadifive({
        'buttonText': 'UPLOAD FILE',
        'buttonClass': 'upload',
        'auto': false,
        'removeCompleted': true,
        'fileObjName': 'Allegato',
        'fileSizeLimit': 500,
        'multi': false,
        'queueSizeLimit': 1,
        'simUploadLimit': 1,
        'uploadLimit': 0,
        'formData': {
            '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
            'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
            'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
            'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
            'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
            'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
            'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
        },
        'uploadScript': $urlSegnalazione,
        'onUploadComplete': function (file, data) {
            segnalazioneInviata(data);
        },
        'onError': function (errorType, file) {
            switch (errorType) {
                case 'QUEUE_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error number file');
                    break;
                case 'UPLOAD_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error number file');
                    break;
                case 'FILE_SIZE_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error size file');
                    break;
                case 'FORBIDDEN_FILE_TYPE':
                    segnalazioneErrata('Error format file');
                    break;
                case '404_FILE_NOT_FOUND':
                    segnalazioneErrata('File not found');
                    break;
                default:
                    segnalazioneErrata(errorType);
                    break;
            }
        }
    });
}

function segnalazioneInviata(data) {
    alert(data);
    $('#boxSegnalazione').dialog('close');
    $('html').loader('hide');
}

function segnalazioneErrata(msg) {
    $inputAllegatoSegnalazione.uploadifive('destroy');
    initUploadSegnalazione();
    $('html').loader('hide');
    alert(msg);
}
*/