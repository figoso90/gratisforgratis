$(document).ready(function () {

});

function deleteNews(id) {
    $.ajax({
        type: 'DELETE',
        url: '/Notifica/DeleteNews',
        dataType: "json",
        data: {
            id: decodeURI(id)
        },
        success: function (data) {
            location.reload(true);
        },
        error: function () {
            alert("Attenzione!!!");
        }
    });
}