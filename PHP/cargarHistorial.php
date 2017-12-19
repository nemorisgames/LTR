<?php
include("funciones.php");
   $dbaddress='localhost'; $dbuser='nemorisg_LHD'; $dbpass='Simulador1'; $dbname='nemorisg_simuladorLHD';
   
$dbcnx = mysql_connect($dbaddress,$dbuser,$dbpass)
or die("Could not connect: " . mysql_error());
mysql_select_db($dbname, $dbcnx) or die ('Unable to select the database: ' . mysql_error());

$query = mysql_query("select * from Historial where IdHistorial = \"" . $_POST["id"] . "\"")
or die("Error en la query" . mysql_error());
$salida="";
while($resultQuery = mysql_fetch_array($query)){
	//agregar los campos del historial
  $salida=$salida.$resultQuery['IdNivel'].'*'.$resultQuery['PorPreguntas'].'*'.$resultQuery['TiempoEmpleado'].'*'.$resultQuery['Check1'].'*'.$resultQuery['revFunc1'].'*'.$resultQuery['revCab1'].'*'.$resultQuery['revEst1'].'*'.$resultQuery['prevRies1'].'*'.$resultQuery['Check2'].'*'.$resultQuery['revFunc2'].'*'.$resultQuery['revCab2'].'*'.$resultQuery['revEst2'].'*'.$resultQuery['prevRies2'].'*'.$resultQuery['OrdenEj'].'*'.$resultQuery['MotorPunta'].'*'.$resultQuery['BaldePunta'].'*'.$resultQuery['VueltasCorrectas'].'*'.$resultQuery['EntregaNombrada'].'*'.$resultQuery['TonelajeTotal'].'*'.$resultQuery['CaidaMat'].'*'.$resultQuery['CorrectoCarguio'].'*'.$resultQuery['Patinaje'].'*'.$resultQuery['IntMaquina'].'*'.$resultQuery['IntPost'].'*'.$resultQuery['IntPostIzq'].'*'.$resultQuery['IntPostDer'].'*'.$resultQuery['IntMedioDer'].'*'.$resultQuery['IntCabina'].'*'.$resultQuery['IntBalde'].'*'.$resultQuery['Zipper'].'*'.$resultQuery['CantZipper'].'*'.$resultQuery['Tunel'].'*'.$resultQuery['CantTunel'].'*'.$resultQuery['Camion'].'*'.$resultQuery['CantCamion'].'*'.$resultQuery['Traslado'].'*'.$resultQuery['CantVueltas'].'*'.$resultQuery['EntregaNombradaSup'].'*'.$resultQuery['TerminoFaena'].'*'.$resultQuery['Fecha'];
};


echo $salida;
?>