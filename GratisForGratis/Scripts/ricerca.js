$(document).ready(function () {
    initRicerca();
});

function initRicerca() {
    enableFiltroAvanzate();
    // caricamento slider
    setSlider('#SliderPunti', '#FormRicerca .punti .lblRange', $('#Cerca_PuntiMin').data('val-range-min'), $('#Cerca_PuntiMax').data('val-range-max'), MONETA, '#Cerca_PuntiMin', '#Cerca_PuntiMax');
    changeSliderFromTextBox('#SliderPunti', '#Cerca_PuntiMin', '#Cerca_PuntiMax');
    setSlider('#SliderAnno', '#FormRicerca .anno .lblRange', 0, (new Date).getFullYear(), '', '#Cerca_AnnoMin', '#Cerca_AnnoMax');
    changeSliderFromTextBox('#SliderAnno', '#Cerca_AnnoMin', '#Cerca_AnnoMax');
    // da sistemare
    setAllCheckbox('#Cerca_Tutti', '#FormRicerca .day', true);
}

function ricercaAvanzata(metodo) {
    $.ajax({
        type: "GET",
        url: '/Cerca/' + metodo,
        data: $('#FormRicerca').serialize(),
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (msg) {
            $('#ricerca').remove();
            $('#page').remove();
            $('#window header').after(msg);
            $('html').loader('hide');
            initRicerca();
            initAutocomplete();
        },
        error: function (error, status, msg) {
            alert("Errore: " + msg);
        }
    });
}

function saveVendita(metodo) {
    //alert("save: " + metodo);
    window.location.href = '/Cerca/' + metodo + '?' + $('#FormRicerca').serialize();
}

function changeSliderFromTextBox(slider, valoreMinimo, valoreMassimo) {
    $(valoreMinimo).change(function () {
        $(slider).slider("values", 0, $(this).val());
    });
    $(valoreMassimo).change(function () {
        $(slider).slider("values", 1, $(this).val());
    });
}

function enableFiltroAvanzate() {
    var avanzata = Cookies.get('ricercaAvanzata');
    //alert("Aprire? " + avanzata);
    if (avanzata != 1) {
        Cookies.set('ricercaAvanzata', 0, { expires: 7 });
    } else {
        $('#FormRicerca').slideToggle('slow');
    }
    $('#ricerca .advancedSearch').click(function () {
        $('#FormRicerca').slideToggle('slow');
        //alert("Ripristino");
        var avanzata = Cookies.get('ricercaAvanzata');
        Cookies.remove('ricercaAvanzata');
        if (avanzata == 1) {
            Cookies.set('ricercaAvanzata', 0, { expires: 7 });
        } else {
            // setta attivazione ricerca
            //alert("Attiva");
            Cookies.set('ricercaAvanzata', 1, { expires: 7 });
        }
    });
}