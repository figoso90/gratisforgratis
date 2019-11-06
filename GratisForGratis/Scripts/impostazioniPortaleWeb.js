var parametro = new UploadImmagine();
parametro.nameInputOriginale = 'ImmagineProfilo';
parametro.inputNuovo = '#uploadifiveImmagineProfilo';
parametro.idListaFile = 'listaUpload';
parametro.actionSalvataggio = '/PortaleWeb/UploadImmagineProfilo';
parametro.nomeParametro = 'file';
parametro.callbackComplete = "";
parametro.actionEliminazione = '/PortaleWeb/DeleteImmagineProfilo';
parametro.galleriaFoto = '#boxFotoProfilo';
parametro.token = $('#Token').val();

$(document).ready(function () {
    enableUploadFoto(parametro);
});

function deleteImmagineProfilo(nome, linkAnnullo) {
    annullaUploadFoto('/PortaleWeb/DeleteImmagineProfilo', '#Foto' + nome.replace('.jpg', '').replace('.jpeg', ''), 'Foto', '#carousel-example-generic', nome, linkAnnullo, parametro);
}