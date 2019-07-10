window.fbAsyncInit = function () {
    FB.init({
        appId: '294838660866806',
        status: true,
        cookie: true,
        xfbml: true
    });
};

(function (doc) {
    var js;
    var id = 'facebook-jssdk';
    var ref = doc.getElementsByTagName('script')[0];
    if (doc.getElementById(id)) {
        return;
    }
    js = doc.createElement('script');
    js.id = id;
    js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));

$(document).ready(function () {
    document.getElementById('shareBtn').onclick = function () {
        //alert($(this).data('href'));
        condividiSuFB(this);
    };
});

function condividiSuFB(tag) {
    var token = $(tag).data('token');
    $pulsanteCondivisione = $(tag);
    FB.ui({
        method: 'share',
        display: 'popup',
        href: $(tag).data('href'),
        quote: $(tag).data('quote') + '! Gratis is new business!'
    }, function (response) {
        alert(JSON.stringify(response));
        //alert(token);
        if (response != undefined && response != "undefined") {
            $.ajax({
                type: "POST",
                url: "/pubblica/CondividiSuFacebook",
                data: { token: decodeURI(token) },
                //dataType: "html",
                success: function (msg) {
                    alert("Condivisione riuscita");
                    $pulsanteCondivisione.remove();
                },
                error: function (error, status, msg) {

                }
            });
        }
    });
}

function condividiSuFB2(tag) {
    FB.ui(
        {
            method: 'feed',
            name: $(tag).data('quote') + '! Gratis is new business!',
            link: $(tag).data('href'),
            //picture: 'http://fbrell.com/f8.jpg',
            //caption: 'Reference Documentation',
            description: $(tag).data('quote') + '! Gratis is new business!'
        },
        function (response) {
            alert(JSON.stringify(response));
            if (response && response.post_id) {
                alert('Post was published.');
            } else {
                alert('Post was not published.');
            }
        }
    );
}