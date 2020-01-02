$(document).ready(function () {
    
    $.ajax({
        type: 'GET',
        url: "/Notifica/HomeProfilo",
        dataType: "html",
        success: function (data) {
            $('#notificheHome').html(data);
            $('#notificheHome').show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("ERRORE: " + jqXHR.responseText);
        }
    });
});

function setNotificaLetta(tag) {
    $item = $(tag).parents('.carousel-item');
    $.ajax({
        type: 'POST',
        url: "/Notifica/Letto/" + $item.data('id'),
        success: function (data) {
            $item.remove();
            /*$('#carouselNotificheHome').carousel('dispose');
            $('#carouselNotificheHome').carousel();*/
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("ERRORE: " + jqXHR.responseText);
        }
    });
}