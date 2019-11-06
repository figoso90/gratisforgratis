var parametro = new UploadImmagine();
parametro.nameInputOriginale = 'ImmagineProfilo';
parametro.inputNuovo = '#uploadifiveImmagineProfilo';
parametro.idListaFile = 'listaUpload';
parametro.actionSalvataggio = '/Utente/UploadImmagineProfilo';
parametro.nomeParametro = 'file';
parametro.callbackComplete = "";
parametro.actionEliminazione = '/Utente/DeleteImmagineProfilo';
parametro.galleriaFoto = '#boxFotoProfilo';

$(document).ready(function () {
    enableUploadFoto(parametro);
    $('form .required').each(function () {
        $(this).text($(this).text() + ' *');
    });
});

function deleteImmagineProfilo(nome, linkAnnullo) {
    annullaUploadFoto('/Utente/DeleteImmagineProfilo', '#Foto' + nome.replace('.jpg', '').replace('.jpeg', ''), 'Foto', '#carousel-example-generic', nome, linkAnnullo, parametro);
}