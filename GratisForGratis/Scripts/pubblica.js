﻿var parametro = new UploadImmagine();

$(document).ready(function () {
    //attiva menu
    $('menu ul .publication').addClass('active');

    attivaMenuCategoriaPubblica();
    // attiva upload custom delle foto
    parametro.nameInputOriginale = 'Foto';
    parametro.inputNuovo = '#file';
    parametro.idListaFile = 'listaFileAggiunti';
    parametro.actionSalvataggio = '/Pubblica/UploadImmagine';
    parametro.nomeParametro = 'file';
    parametro.callbackComplete = "validazioneAggiuntiva('#formPubblica', '#Foto')";
    parametro.actionEliminazione = '/Pubblica/AnnullaUploadImmagine';
    parametro.galleriaFoto = '#listaFileAggiunti';
    enableUploadFoto(parametro);

    $('#CategoriaId').change(function () {
        $('html').loader('show');
        validazioneAggiuntiva('#formPubblica', '#CategoriaId');
        // aggiornare form dati aggiuntivi
        loadInfoAggiuntive();
    });

    //alert("Categoria: " + $('#CategoriaId').val());
    if ($('#CategoriaId').val().trim() !== '' && $('#CategoriaId').val() > 0) {
        showInfoAggiuntive();
        //$('html').loader('show');
        validazioneAggiuntiva('#formPubblica', '#CategoriaId');
        // aggiornare form dati aggiuntivi
        loadInfoAggiuntive();
    }

    // blocco il tipo di spedizione a mano
    //$('#TipoSpedizione option')[0].prop('disabled',true);
});

function attivaMenuCategoriaPubblica() {
    $menuCategoriaPubblica = $('#menu').clone();
    $menuCategoriaPubblica.attr('id', 'menu2');
    $menuCategoriaPubblica.insertAfter('#menu');
    $menuCategoriaPubblica.slicknav({
        label: '',
        duration: 1000,
        easingOpen: "easeOutBounce",
        prependTo: '#menuCategoriaPubblica',
        closeOnClick: true,
        init: function () {
            var categoria = $('#categoriaAttuale').val();
            if ($('#CategoriaNome').val() !== '' && $('#CategoriaNome').val() !== undefined) {
                categoria = $('#CategoriaNome').val();
            }
            $('#menuCategoriaPubblica').find('.slicknav_menutxt').text(categoria);
            impostaCategoria('#menuCategoriaPubblica', '#menu2', '#CategoriaId', '#CategoriaNome', 'pubblica');
            $categoriaPreSelezionata = $("#menuCategoriaPubblica li a.trigger[data-value='" + $('#categoriaAttuale').data('id') + "']");
            if ($categoriaPreSelezionata !== '' && $categoriaPreSelezionata !== undefined && $categoriaPreSelezionata.data('pubblica') == true) {
                $categoriaPreSelezionata.click();
            }
        },
        beforeOpen: function (trigger) {
            //alert('Test');
            $('.slicknav_item').slicknav('close');
        }
    });
}

function loadInfoAggiuntive() {
    //$('#boxInfoAggiuntive').css('display','flex');
    $('#advanced').html('');
    $.ajax({
        type: "POST",
        url: "/Pubblica/LoadInfoAggiuntive",
        data: { categoria: decodeURI($('#CategoriaId').val()) },
        dataType: "html",
        success: function (data) {
            //$(data).insertAfter('#formPubblica .lastInfoBase');
            $('#advanced').append(data);
            showInfoAggiuntive();
            //showDatiExtra();
        },
        error: function (response, status, xhr) {
            if (status == "error") {
                alert("Ci scusiamo per il disagio, ma si è verificato un errore imprevisto. Vi preghiamo di segnalarlo ai pantofolai dell'assistenza. Grazie");
            }
        },
        complete: function () {
            $('html').loader('hide');
        }
    });
}

function showInfoAggiuntive() {
    //alert("Action: " + $('#ActionCategoria:last').val());
    $('#formPubblica').attr('action', $('#ActionCategoria:last').val());
    //$('#infoAggiuntive').show();
    refreshValidatoreForm();
    initAutocomplete();
    $('#advanced').css('display', 'flex');
    
    // ATTIVA FUNZIONI SPECIFICHE PER TIPOLOGIA ANNUNCIO
    $('[data-toggle="tooltip"]').tooltip();
    // inizializza scelta colore oggetto
    $('#Colore').ColorPicker();

    setAllCheckbox('input[name="Tutti"]', '#pubblicazione .day', true);
    $('input[name="Tutti"]').dblclick();
}
/*
function showDatiExtra() {
    if ($('#advanced:visible').length > 0) {
        $('#advanced').css('display', 'none');
    } else {
        $('#advanced').css('display', 'flex');
    }
}*/

function refreshValidatoreForm() {
    $('form').removeData('validator');
    $('form').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('form');
}

function addMateriale(tag) {
    var indiceTagDaClonare = $('.materials .form-control').length - 1;
    $nuovoTag = $('.materials .form-control:last').clone();
    $nuovoTag.attr('id', $nuovoTag.attr('id').replace(indiceTagDaClonare, indiceTagDaClonare + 1));
    $nuovoTag.attr('name', $nuovoTag.attr('name').replace(indiceTagDaClonare, indiceTagDaClonare + 1));
    $nuovoTag.val('');

    $remove = '<a class="remove" href="javascript:void(0);" onclick="removeMateriale(this);"><i class="fas fa-minus-circle icona"></i></a>';
    $row = $('<div class="row"></div>');
    $row.append($remove);
    $row.append($nuovoTag);
    $('.materials').append($row);
}

function removeMateriale(tag) {
    $(tag).parent('.row').remove();
    riordinaInput('Materiali');
}

function addComponente(tag) {
    var indiceTagDaClonare = $('.components .form-control').length - 1;
    $nuovoTag = $('.components .form-control:last').clone();
    $nuovoTag.attr('id', $nuovoTag.attr('id').replace(indiceTagDaClonare, indiceTagDaClonare + 1));
    $nuovoTag.attr('name', $nuovoTag.attr('name').replace(indiceTagDaClonare, indiceTagDaClonare + 1));
    $nuovoTag.val('');

    $remove = '<a class="remove" href="javascript:void(0);" onclick="removeComponente(this);"><i class="fas fa-minus-circle icona"></i></a>';
    $row = $('<div class="row"></div>');
    $row.append($remove);
    $row.append($nuovoTag);
    $('.components').append($row);
}

function removeComponente(tag) {
    $(tag).parent('.row').remove();
    riordinaInput('Componenti');
}

function sceltaScambio(elemento, isSpedizione) {
    $('.spedizione').addClass('hide');
    $('.spedizioneOnline').addClass('hide');
    isSpedizione = isSpedizione || false;
    if (isSpedizione && $(elemento).is(':checked')) {
        $('.spedizione').removeClass('hide');
    }
    sceltaSpedizione($('#TipoSpedizione'));
}

function sceltaSpedizione(elemento)
{
    $('.spedizioneOnline').addClass('hide');
    //disabilitaPrezzoSpedizione();
    if ($('#ScambioConSpedizione').is(':checked')) {
        if ($(elemento).val() != 1) {
            $('.spedizione.corrieri [label=Online]').removeAttr('disabled');
            $('.spedizione.corrieri [label=Online] option:first-child').prop('selected', true);
            $('.spedizione.corrieri [label=Privata]').attr('disabled', true);
            $('.spedizioneOnline').removeClass('hide');
        } else {
            $('.spedizione.corrieri [label=Privata]').removeAttr('disabled');
            $('.spedizione.corrieri [label=Privata] option:first-child').prop('selected', true);
            $('.spedizione.corrieri [label=Online]').attr('disabled', true);
        }
    }

    /*
    if ($('#ScambioConSpedizione').is(':checked') && $(elemento).val() != 2)
        abilitaPrezzoSpedizione();
    */
}

function abilitaPrezzoSpedizione() {
    $('.prezzoSpedizione .form-control').removeProp('disabled');
    $('.prezzoSpedizione').show();
}

function disabilitaPrezzoSpedizione()
{
    $('.prezzoSpedizione .form-control').val(1);
    $('.prezzoSpedizione .form-control').prop('disabled', true);
    $('.prezzoSpedizione').hide();
}

function getPrezzoSpedizione(servizioCorriere) {
    // verifica se spedizione privata o meno
    $corriere = $(servizioCorriere).val();
    $gruppo = $(servizioCorriere).find('option[value="' + $corriere + '"]').parent('optgroup');
    if ($gruppo.attr('label').toLowerCase() == 'privata')
    {
        return false;
    }

    $.ajax({
        type: 'POST',
        url: '/Pubblica/GetPrezzoSpedizione',
        dataType: "json",
        data: {
            servizioSpedizione: decodeURI($corriere),
            altezza: $('#Altezza').val(),
            larghezza: $('#Larghezza').val(),
            lunghezza: $('#Lunghezza').val()
        },
        success: function (data) {
            //alert(data.Prezzo);
            $('.prezzoSpedizione .form-control').val(data.Prezzo);
        },
        error: function (error, status, msg) {
            //alert("Errore: " + msg);
            $('.prezzoSpedizione .form-control').val(30);
        }
    });
}