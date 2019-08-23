$(document).ready(function () {
    /*
    $('.selectpicker').selectpicker({
        width: '100px',
        iconBase: 'fab'
    });
    $('#ddlsearch').change(function (e) {
        Cookies.set("genera_ricerca_tag", $('#ddlsearch').val(), { expires: 7 });
    });
    var cookieTag = Cookies.get("genera_ricerca_tag");
    if (cookieTag != undefined && cookieTag != "") {
        $("#ddlsearch option[value='" + cookieTag + "']").prop('selected', true);
        $('#ddlsearch').selectpicker('refresh')
    }
    getConteggioArticoli();
    */
    $('#menuHome .icona').click(function (event) {
        $('#menuHomeOverlay').slideToggle('slow');
        $('#menuHome .menu').slideToggle('slow');
    });
});

function getConteggioArticoli() {
    $.ajax({
        url: lingua + '/articolo/conteggio',
        type: 'get',
        dataType: 'json'
    }).done(function (data) {
        //alert(data.numero);
        $('.numeroArticoli').text(data.numero);
    }).fail(function (errore) {
        alert("errore: " + errore.responseText);
    });
}

function anchorPage(tag) {
    $('html,body').animate({ scrollTop: $(tag).offset().top }, 'slow');
}