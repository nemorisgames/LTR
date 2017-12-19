<?php
require ('funciones.php');
// API access key from Google API's Console
define( 'API_ACCESS_KEY', 'AIzaSyCYSAfwNlvcxFKkKOpWmRfRaK_0cWhFn-Y' );

if($_GET['mensaje'] != ""){
	enviarPush();
}

function enviarPush(){
	$registrationIds = obtenerRegistrationID();
//obtenerRegistrationID;
	//$registrationIds = array( $_GET['id'] );

	// prep the bundle
	$msg = array
	(
    'message' 		=> $_GET['mensaje'],
	'title'			=> $_GET['titulo'],
	'subtitle'		=> $_GET['mensaje'],
	'tickerText'	=> $_GET['titulo'],
	'vibrate'	=> 1,
	'sound'		=> 1
	);

	$fields = array
	(
	'registration_ids' 	=> $registrationIds,
	'data'				=> $msg
	);
 
	$headers = array
	(
	'Authorization: key=' . API_ACCESS_KEY,
	'Content-Type: application/json'
	);
 
	$ch = curl_init();
	curl_setopt( $ch,CURLOPT_URL, 'https://android.googleapis.com/gcm/send' );
	curl_setopt( $ch,CURLOPT_POST, true );
	curl_setopt( $ch,CURLOPT_HTTPHEADER, $headers );
	curl_setopt( $ch,CURLOPT_RETURNTRANSFER, true );
	curl_setopt( $ch,CURLOPT_SSL_VERIFYPEER, false );
	curl_setopt( $ch,CURLOPT_POSTFIELDS, json_encode( $fields ) );
	$result = curl_exec($ch );
	curl_close( $ch );

	echo $result;
}

?>