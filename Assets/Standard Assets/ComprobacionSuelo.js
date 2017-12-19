#pragma strict

public var ultimoBloque:GameObject = null;

function Start () {

}

function OnTriggerStay(c:Collider){
	if(c.CompareTag("Bloque"))
		ultimoBloque = c.gameObject; 
}

function Update () {

}