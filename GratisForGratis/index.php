<?php

/* @var $this yii\web\View */

$this->title = 'Genera Codice - Lista articoli';
use yii\widgets\LinkPager;
use yii\helpers\Url;

$this->registerCssFile(Url::base(true).'/css/articolo.css');

foreach ($models as $model) {
?>
	<div class="articolo">
		<h3 class="titolo">
			<?php echo $model->TITOLO; ?>
		</h3>
		<div id="avanzate">
			<div>
				<a id="linkArgomento" href="<?php echo Url::base().'/articolo/details/'.$model->ID; ?>"><?php echo Url::base(true).'/articolo/details/'.$model->ID; ?></a>
			</div>
			<div id="boxAutore">
				<span class="sezione">Autore: </span>
				<span class="nome"><?php echo $model->uTENTE->FIRMA; ?></span>
			</div>
			<div id="listaTag">
				<span class="sezione">Tag: </span>
				<?php foreach ($model->argomentoTags as $tag) { ?>
				<span class="nome"><?php echo $tag->tAG->NOME; ?> </span>
				<?php } ?>
			</div>
		</div>
	</div>
<?php
}
// display pagination
echo LinkPager::widget(['pagination' => $pages]);
?>