using UnityEngine;
using System.Collections;

public class facebookMuroPublicarFoto : MonoBehaviour {
	GameObject redesSociales;
	// Use this for initialization
	void Start () {
		redesSociales = GameObject.Find("RedesSociales");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void facebookPublicarFoto(){
		if(redesSociales != null) redesSociales.SendMessage("publicarFoto");
	}
}
