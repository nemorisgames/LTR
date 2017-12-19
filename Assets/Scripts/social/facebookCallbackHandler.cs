using UnityEngine;
using System.Collections;

public class facebookCallbackHandler : MonoBehaviour {
	IDictionary me;
	public UILabel texto;
	// Use this for initialization
	void Start () {
	
	}
	
	public void logged(IDictionary informacion){
		me = informacion;
		if(texto != null) texto.text = "logueado: " + me["name"];
	}
	
	public void logueando(){
		if(texto != null) texto.text = "logueando..";
	}
	
	public void logout(){
		if(texto != null) texto.text = "logout";
	}
	
	public void errorLogin(string error){
		if(texto != null) texto.text = "error en login: " + error;
	}
	
	public void actionSended(bool canceled){
		if(texto != null){
			if(canceled) texto.text = "accion cancelada";
			else texto.text = "accion enviada";
		}
	}
	
	public void posting(){
		if(texto != null) texto.text = "posteando...";
	}
	
	public void postError(string error){
		if(texto != null) texto.text = "error en envio: " + error;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
