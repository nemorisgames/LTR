using UnityEngine;
using System.Collections;

public class ComportamientoSwitch : MonoBehaviour {
	public bool activado = false;
	bool activadoInicial = false;
	public GameObject[] objetivos;
	public string tagObjetivos = "Espina";
	public float tiempoActivo = -1f;
	public float tiempoActivoActual = 0f;

	bool switchHabilitado = true;
	// Use this for initialization
	void Start () {
		activadoInicial = activado;
		buscarObjetivos ();
		activarSwitch (activado);
	}

	void buscarObjetivos(){
		if(objetivos == null || objetivos.Length <= 0)
			objetivos = GameObject.FindGameObjectsWithTag (tagObjetivos);
	}

	void OnTriggerStay(Collider other){
		//if (!switchHabilitado)
		//	return;
		if (activadoInicial == activado && (other.CompareTag ("Player") || other.CompareTag ("Bloque"))) {
			if(tiempoActivo == -1f){
				activarSwitch(!activadoInicial);
			}
			else{
				tiempoActivoActual = Time.time;
				activarSwitch(!activadoInicial);
			}
			switchHabilitado = false;
		}
	}

	void activarSwitch(bool a){
		activado = a;
		GetComponent<Renderer>().material.color = (activado ?  Color.grey : Color.white);
		if (tiempoActivo != -1f) { 
			transform.parent.Find ("cubo_001_arreglado").GetComponent<Renderer>().material.color = (activado ? Color.grey : Color.white );

		}
		for(int i = 0; i < objetivos.Length; i++){
			if(objetivos[i] != null) objetivos[i].SendMessage("activar", activado);
		}
		if(GetComponent<AudioSource>() != null && Time.timeSinceLevelLoad > 1f){
			GetComponent<AudioSource>().pitch = 1f;
			if(activado != activadoInicial) GetComponent<AudioSource>().pitch = 1f;
			else GetComponent<AudioSource>().pitch = 2f;
			GetComponent<AudioSource>().Play();
		}
		transform.parent.Find ("Cube").transform.localPosition = new Vector3(0f, activado?0.75f:1f, 0f);
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player") || other.CompareTag ("Bloque")) {
			switchHabilitado = true;
			//nuevo
			activarSwitch(activadoInicial);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(tiempoActivo != -1f && activado != activadoInicial){
			if(tiempoActivoActual + tiempoActivo < Time.time){
				activarSwitch(activadoInicial);
			}
			else{
				if(tiempoActivoActual + tiempoActivo * 0.6f < Time.time){
					if(GetComponent<AudioSource>() != null) GetComponent<AudioSource>().pitch = 2f;
				}
			}
		}
	}
}
