using UnityEngine;
using System.Collections;

public class gotoScene : MonoBehaviour {
	public string escena = "Titulo";
	public bool cambiarAutomatico = false;
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
		if (cambiarAutomatico) irEscena ();
	}

	public void irEscena(){
		Application.LoadLevel (escena);
	}

}
