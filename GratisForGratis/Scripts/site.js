$inputAllegatoSegnalazione = $('#AllegatoSegnalazione');
$inputFotoPossiedo = $('#file');
$urlSegnalazione = '/Home/Segnalazione';
/* VARIABILI GLOBALI */
var CLICK_PULSANTE = 0;

function UploadImmagine() { 
    this.nameInputOriginale = "";
    this.inputNuovo = "";
    this.idListaFile = "";
    this.actionSalvataggio = "";
    this.nomeParametro = "";
    this.actionEliminazione = "";
    this.callbackComplete = "";
    this.galleriaFoto = "";
}

$(document).ready(function () {
    initAutocomplete();
    attivaMenuCategoria();

    //$("img.lazy").show().lazyload();
    $('#menuMobile .icona').click(function (event) {
        /*if ($('body').css('overflow') == 'hidden')
            $('body').css('overflow', '');
        else
            $('body').css('overflow', 'hidden');*/
        $('#menuMobileOverlay').slideToggle('slow');
        $('#menuMobile .menu').slideToggle('slow');
    });

    //$('form').submit(pauseSubmit(this));
    
    $('.btn:not(.dropdown-toggle)').click(function (event) {
        //attendiInvio(this);
        $('html').loader('show');
    });
    
    var tryNumber = 0;
    $('input[type=submit]').click(function (event) {
        var self = $(this);
        if (self.closest('form').valid()) {
            $('html').loader('show');
            if (tryNumber > 0) {
                tryNumber++;
                //alert('Your form has been already submited. wait please');
                return false;
            }
            else {
                tryNumber++;
            }
        };
    });
    /*
    $('a.link').click(function (event) {
        $('html').loader('show');
        if (tryNumber > 0) {
            tryNumber++;
            //alert('Your form has been already submited. wait please');
            return false;
        }
        else {
            tryNumber++;
        }
    });*/

    $("form").bind("invalid-form.validate", function () {
        $('html').loader('hide');
    });
    $(document).ajaxComplete(function () {
        $('html').loader('hide');
    });
    
    // SEGNALAZIONE
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
    $('[data-toggle="tooltip"]').tooltip();

    /* CATEGORIA 21-09
    $('#dropdownMenu1').click(function () {
        $('#menu').toggle();
    });
    */

    // nuova funzione per il replace
    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.replace(new RegExp(search, 'g'), replacement);
    };

    $('.infoTooltip').tooltip({
        track: true,
        disabled: true,
        //show: { effect: "blind", duration: 800 },
        hide: { effect: "explode", duration: 1000 },
        close: function (event, ui)
        {
            $(this).tooltip("disable");
        }
    });

    $('.infoTooltip').click(function () {
        $(this).tooltip("enable");
        $(this).tooltip("open");
    });

    $.urlParam = function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (results != undefined)
            return results[1] || 0;
        else
            return results;
    }
});

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

// Inizializza i campi di ricerca rapida (autocomplete)
// tolto proprietà delay a 0 perchè rallentava troppo le chiamate remote
function initAutocomplete() {
    $('*[data-autocomplete-url]').each(function () {
        if ($(this).autocomplete("instance")!=undefined)
            $(this).autocomplete("destroy");
        var url = $(this).data("autocomplete-url");
        var filtro = $(this).data("autocomplete-filtro-extra");
        $(this).autocomplete({
            position: { my: "left top", at: "left bottom+15px", collision: "flip" },
            delay: 500,
            minLength: 2,
            //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
            source: function (request, response) {
                $.ajax({
                    url: url,
                    dataType: "json",
                    data: {
                        term: request.term,
                        filtroExtra: $(filtro).val()
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            select: function (event, ui) {
                //$("#" + $(this).data("autocomplete-id")).val(ui.item.Value);
                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                $(this).val(ui.item.Label);
            },
            change: function (event, ui) {
                if (ui.item != null) {
                    $("#" + $(this).data("autocomplete-id")).val(ui.item.Value);
                } else {
                    $("#" + $(this).data("autocomplete-id")).val('');
                }
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        };
    });
}

/**
** slider = selettore del div da trasformare in slider
** element = selettore dell'input dove mostrare il valore
**/
function setSlider(slider, label, min, max, unitaMisura, inputMin, inputMax) {
    min = (min) ? min : 0;
    max = (max) ? max : 100000;
    unitaMisura = (unitaMisura) ? unitaMisura : "";

    //alert("Slider: " + slider + " => Min: " + min + " Max: " + max);

    $(slider).slider({
        range: true,
        min: min,
        max: max,
        values: [$(inputMin).val(), $(inputMax).val()],
        create: function (event, ui) {
            //$(label).text(unitaMisura + " " + $(inputMin).val() + " - " + unitaMisura + " " + $(inputMax).val());
            $(label).text(unitaMisura);
        },
        slide: function (event, ui) {
            //$(label).text(unitaMisura + " " + ui.values[0] + " - " + unitaMisura + " " + ui.values[1]);
            $(inputMin).val(ui.values[0]);
            $(inputMax).val(ui.values[1]);
        }
    });
}

/*
    Inizializza toolbar ricerca minima
*/
function attivaMenuCategoria() {
    $('#menu').slicknav({
        label: '',
        duration: 1000,
        easingOpen: "easeOutBounce",
        prependTo: '#menuCategoriaRicerca',
        closeOnClick: true,
        init: function () {
            $('#menuCategoriaRicerca').find('.slicknav_menutxt').text($('#categoriaAttuale').val());
            inizializzaMenu();
            impostaCategoria('#menuCategoriaRicerca', '#menu', '#Cerca_IDCategoria', '#Cerca_Categoria', 'all');
        },
        beforeOpen: function (trigger) {
            //alert('Test');
            $('.slicknav_item').slicknav('close');
        }
    });
}

function inizializzaMenu() {
    $(".dropdown-menu > li .trigger").hover(function (e) {
        var current = $(this).next();
        var grandparent = $(this).parent().parent();
        grandparent.find(".sub-menu:visible").not(current).hide();
        current.show();
        e.stopPropagation();
    });
    $(".dropdown-menu > li a:not(.trigger)").hover(function () {
        var root = $(this).closest('.dropdown');
        root.find('.left-caret').toggleClass('right-caret left-caret');
        root.find('.sub-menu:visible').hide();
    });
}

// usata in : site.js, pubblica.js
// serve a settare i valori id e nome della categoria scelta
function impostaCategoria(idBoxCategoria, menu, inputIdCategoria, inputLabelCategoria, checkChange, callBack) {
    checkChange = checkChange || 'all';
    //alert(idBoxCategoria + ' li a.trigger. Numero elementi: ' + $(idBoxCategoria + ' li a.trigger').length);
    $(idBoxCategoria + ' li a.trigger').on('click', function (event) {
        //alert("enter: " + checkChange);
        // se voglio poter selezionare tutte le categorie o se la categoria selezionata è selezionabile (ricerca categoria o pubblicazione annuncio)
        if (checkChange == 'all' || $(this).data(checkChange)) {
            //alert("Id: " + inputIdCategoria + " value: " + $(this).data('value'));
            $(inputIdCategoria).val($(this).data('value'));
            $(inputIdCategoria).change();
            $(inputLabelCategoria).val($(this).data('text'));
            $(inputLabelCategoria).change();
            // nuovo metodo
            $(idBoxCategoria).find('.slicknav_menutxt').text($(this).data('text'));
            $(menu).toggle();
            // chiude tutti i menu quando li nasconde
            //$(this).parents('.dropdown-menu').toggle();
            if (callBack != undefined)
                window[callBack].apply();
        }
    });
}

// inizializza controllo per le checkbox multiple
function setAllCheckbox(checkboxAll, subCheckbox, force) {
    if (force) {
        if ($(checkboxAll).is(':checked')) {
            $(subCheckbox + ':not(:checked)').prop('checked', true);
        }
    }

    // sul click al checkbox tutti
    $(checkboxAll).change(function () {
        if ($(this).is(':checked')) {
            $(subCheckbox + ':not(:checked)').prop('checked', true);
        } else {
            $(subCheckbox + ':checked').prop('checked', false);
        }
    });

    // sul click degli altri checkbox
    $(subCheckbox).change(function () {
        if (!$(this).is(':checked')) {
            $(checkboxAll).prop('checked', false);
        } else if ($(subCheckbox + ':not(:checked)').length <= 0) {
            $(checkboxAll).prop('checked', true);
        }
    });
}

function pauseSubmit(form) {
    var button = $(form).find('input[type="submit"]');
    setTimeout(function () {
        button.removeAttr('disabled');
    }, 1);
}

// blocca il pulsante fino alla fine dell'esecuzione lato server
function attendiInvio(pulsante) {
    var self = $(pulsante);

    //if (self.closest('form').valid()) {
    if (CLICK_PULSANTE > 0) {
        CLICK_PULSANTE++;
        //alert('Your form has been already submited. wait please');
        return false;
    }
    else {
        CLICK_PULSANTE++;
        $.loader({
            className: "blue-with-image-2",
            content: ''
        });
    }
    //};
}

function reinvioEmailRegistrazione()
{
    $.ajax({
        type: "GET",
        url: "/Utente/ReinvioEmailRegistrazione",
        dataType: "json",
        success: function (data) {
            alert("Invio effettuato. Controlla anche nella tua casella di spam.");
        }
    });
}

function validazioneAggiuntiva(form, tagForm) {
    var validator = $(form).validate();
    validator.element(tagForm);
}

function enableUploadFoto(parametro)
{
    // attiva upload custom delle foto
    $(parametro.inputNuovo).uploadifive({
        'uploadScript': parametro.actionSalvataggio,
        // Put your options here
        'fileObjName': parametro.nomeParametro,
        'auto': true,
        'multi': true,
        'fileSizeLimit': '20MB',
        'queueID': parametro.idListaFile,
        'buttonText': 'Carica foto',
        'buttonClass': 'btnUpload',
        'fileTypeExts': '*.gif; *.jpg; *.png; *.jpeg; *.tiff; *.bmp;',
        //'itemTemplate': '<div class="uploadifive-queue-item"><span class="filename"></span> | <span class="fileinfo" style></span><span class="chiudi" style="display:none;"></span></div>',
        'itemTemplate': '<div class="uploadifive-queue-item"><span class="chiudi" style="display:none;"></span></div>',
        'formData': {
            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val(),
            '__TokenUploadFoto': $('input[name="TokenUploadFoto"]').val()
        },
        'onUploadComplete': function (file, data) {
            
            $fileCreato = JSON.parse(data);
            var idCodificato = file.name;
            $(parametro.galleriaFoto).html($fileCreato.responseText);
            riordinaInput(parametro.nameInputOriginale);
            file.queueItem.find('.chiudi').click(function (event) {
                event.preventDefault();
                annullaUploadFoto(parametro.actionEliminazione, '#Foto' + idCodificato.replace('.jpg', '').replace('.jpeg', ''), parametro.nameInputOriginale, parametro.galleriaFoto, idCodificato, this, parametro);
            });
            enableUploadFoto(parametro);
            
            $('.carousel').carousel();
            if (parametro.callbackComplete != 'undefined' && parametro.callbackComplete != undefined)
                eval(parametro.callbackComplete);
        },
        'onFallback': function () {
            alert('Oops!  You have to use the non-HTML5 file uploader.');
        },
        'onError': function (errorType) {
            alert('Errore: ' + errorType);
        }
    });

    $('.uploadifive-queue-item:not(.copia) .chiudi').each(function () {
        var idCodificato = $(this).data("nome");
        $(this).click(function (event) {
            event.preventDefault();
            annullaUploadFoto(parametro.actionEliminazione, '#Foto' + idCodificato.replace('.jpg', '').replace('.jpeg', ''), parametro.nameInputOriginale, parametro.galleriaFoto, idCodificato, this, parametro);
        });
    });

    $('.uploadifive-queue-item2:not(.copia) .chiudi').each(function () {
        var idCodificato = $(this).data("nome");
        $(this).click(function (event) {
            event.preventDefault();
            annullaUploadFoto(parametro.actionEliminazione, '#Foto' + idCodificato.replace('.jpg', '').replace('.jpeg', ''), parametro.nameInputOriginale, parametro.galleriaFoto, idCodificato, this, parametro);
        });
    });
}

function createInputFotoPerUploadifive(nameInputOriginale, id)
{
    //alert($('input[name^="Foto"]').length);
    var idPulito = id.replace('.jpg', '').replace('.jpeg', '');
    var indice = $('input[name^="Foto"]').length;
    $input = $('<input id="' + nameInputOriginale + idPulito + '" name="' + nameInputOriginale + '[' + indice + ']" class="form-control" type="hidden" />');
    $input.val(id);
    return $input;
}

function annullaUploadFoto(actionAnnullo, inputFoto, nameInputOriginale, idListaFile, nome, linkAnnullo, parametro) {
    var idPulito = nome.replace('.jpg', '').replace('.jpeg', '');

    $.ajax({
        type: 'POST',
        url: actionAnnullo,
        //dataType: "json",
        dataType: "html",
        data: {
            nome: decodeURI(nome),
            '__TokenUploadFoto': $('input[name="TokenUploadFoto"]').val()
        },
        success: function (data) {
            $(linkAnnullo).parents('.uploadifive-queue-item').remove();
            $(linkAnnullo).parents('.uploadifive-queue-item2').remove();
            //alert("Input da eliminare: " + inputFoto);
            $(inputFoto).remove();
            riordinaInput(nameInputOriginale);
            /*
            if ($('#' + idListaFile).find('input').length <= 0) {
                $('#' + idListaFile).addClass('hide');
                $('#' + idListaFile).html('');
                //alert("Finito");
                $('#' + nameInputOriginale + idPulito).val('');
            }
            */
            $(parametro.galleriaFoto).html(data);
            enableUploadFoto(parametro);
            $('.carousel').carousel();
        },
        error: function (error, status, msg) {
            alert("Errore: " + msg);
        }
    });
}
/*
function annullaUploadImmagine(nome, linkAnnullo, parametro) {
    annullaUploadFoto('/Pubblica/AnnullaUploadImmagine', '#Foto' + nome.replace('.jpg', '').replace('.jpeg', ''), 'Foto', 'listaFileAggiunti', nome, linkAnnullo, parametro);
}
*/

function rimuoviFotoDaCopia(nome, linkAnnullo)
{
    var idPulito = nome.replace('.jpg', '').replace('.jpeg', '');
    $(linkAnnullo).parents('.uploadifive-queue-item2').remove();    
    $('#Foto' + idPulito).remove();
    riordinaInput('Foto');
}

function riordinaInput(name)
{
    $('input[name^="' + name + '"]').each(function (index, value) {
        var nameAttuale = $(this).attr('name');
        var nameNuovo = name + "[" + index + "]";
        //alert("Nuovo nome: " + nameNuovo);
        $(this).attr('name', nameNuovo);
    });
}

function desidero(token, elemento) {
    $.ajax({
        type: "POST",
        url: "/Annuncio/Desidero",
        data: { 'token': token},
        dataType: "html",
        success: function (data) {
            $(elemento).after(data);
            $(elemento).remove();
        }
    });
}

function nonDesidero(token, elemento) {
    $.ajax({
        type: "POST",
        url: "/Annuncio/NonDesidero",
        data: { 'token': token },
        dataType: "html",
        success: function (data) {
            $(elemento).after(data);
            $(elemento).remove();
        }
    });
}

// DEPRECATED E DISATTIVATA
function suggestAdActivation(elemento, chiave1) {
    /*
    $.ajax({
        type: "POST",
        url: '/Cerca/SuggestAdActivation',
        data: "{ 'chiave1': '" + chiave1 + "', 'idAttivita': '" + $('#BusinessSuggestAd').val() + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //$(elemento).remove();
            $(elemento).find('.suggestAdActivation').addClass('sended');
            //alert(msg);
        },
        error: function (error, status, msg) {
            alert("Errore: " + msg);
        }
    });
    */

    /*
    $.post("/Utente/SuggestAdActivation", { 'id': idAd, 'idAttivita': $('#BusinessSuggestAd').val() }, function (data) {
        alert(data);
    });
    */

    var newForm = jQuery('<form>', {
        'action': '/Utente/SuggestAdActivation',
        'method': 'post',
        'style': "display:none;"
    });

    newForm.append(jQuery('<input>', {
        'name': 'chiave1',
        'value': chiave1,
        'type': 'hidden'
        }))
        .append(jQuery('<input>', {
            'name': 'idAttivita',
            'value': $('#BusinessSuggestAd').val(),
            'type': 'hidden'
        }));
    $(document.body).append(newForm);
    
    newForm.submit();
}

function possiedo(token, testo)
{
    initUploadPossiedo(token, testo);
}

function initUploadPossiedo(token, testo)
{
    $.ajax({
        type: "POST",
        url: "/Pubblica/GetFormCopiaAnnuncio",
        data: { token: decodeURI(token) },
        async: false,
        dataType: "html",
        success: function (msg) {
            $('#boxPossiedo').append(msg);
            var parametriUploadPossiedo = new UploadImmagine();
            parametriUploadPossiedo.nameInputOriginale = 'Foto';
            parametriUploadPossiedo.inputNuovo = '#file';
            parametriUploadPossiedo.idListaFile = 'listaFileDaCopiare';
            parametriUploadPossiedo.actionSalvataggio = '/Pubblica/UploadImmagine';
            parametriUploadPossiedo.nomeParametro = 'file';
            parametriUploadPossiedo.callbackComplete = "validazioneAggiuntiva('#formCopiaOggetto', '#Foto')";
            parametriUploadPossiedo.actionEliminazione = '/Pubblica/AnnullaUploadImmagine';
            parametriUploadPossiedo.galleriaFoto = '#listaFileDaCopiare';
            enableUploadFoto(parametriUploadPossiedo);
            $('#boxPossiedo').dialog({
                title: testo,
                width: 550,
                modal: true,
                open: function (event, ui) {

                },
                close: function (event, ui) {
                    $('#file').uploadifive('destroy');
                    $('#boxPossiedo').dialog('destroy');
                    $('#boxPossiedo').html('');
                }
            });
        },
        error: function (error, status, msg) {
            alert(msg);
        },
        complete: function () {
            $('html').loader('hide');
        }
    });
}

function copiaAnnuncio(metodo, link) {
    $vendita = $(link).parents('form');
    $token = $vendita.find('#TokenOK').val();
    $.ajax({
        type: "POST",
        url: '/Pubblica/' + metodo,
        data: $vendita.serialize(),
        dataType: "html",
        success: function (msg) {
            //alert($token);
            $linkPossiedo = $('#Token' + $token).find('.possession');
            $linkPossiedo.after(msg);
            $linkPossiedo.remove();
            $('#boxPossiedo').dialog('close');
            // SE PAGINA 1 DI RICERCA ALLORA REFRESH PAGINA
            $paginaRicerca = $.urlParam('Pagina');
            if ($paginaRicerca == null || $paginaRicerca.length <= 0 || $paginaRicerca == "1")
                location.reload();
        },
        error: function (errore, stato, messaggio) {
            alert("Errore annullo vendita: " + decodeURIComponent(errore.responseText));
        }
    });
}

function nonPossiedo(tokenAnnuncio, tokenAnnuncioUtente, link) {
    $vendita = $(link).parents('.object');
    var metodo = 'AnnullaVendita';
    if ($vendita.attr('id') != tokenAnnuncioUtente)
        metodo = 'NonPossiedo';
    $.ajax({
        type: "DELETE",
        url: '/Annuncio/' + metodo + '?' + $.param({ "tokenAnnuncio": tokenAnnuncio, "tokenAnnuncioUtente" : tokenAnnuncioUtente }),
        dataType: "html",
        success: function (msg) {
            if ($vendita.attr('id') != undefined && $vendita.attr('id').toLowerCase() == ('Token' + tokenAnnuncioUtente).toLowerCase()) {
                $vendita.remove();
            } else if ($('.object[id*="' + tokenAnnuncioUtente.substring(3, tokenAnnuncioUtente.length - 5) + '"]').length > 0){
                $('.object[id*="' + tokenAnnuncioUtente.substring(3, tokenAnnuncioUtente.length - 5) + '"]').remove();
                //alert("Link " + $(link).attr('class') + " Message: " + msg);
                $(link).after(msg);
                $(link).remove();
            }else {
                $(link).after(msg);
                $(link).remove();
            }
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                nonPossiedo(token, link);
            });
            alert("Errore annullo vendita: " + decodeURIComponent(errore.responseText));
        }
    });
}