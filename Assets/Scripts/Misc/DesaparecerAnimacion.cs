using UnityEngine;
using System.Collections;

public class DesaparecerAnimacion : MonoBehaviour {
	public float tiempoVida;
	float tiempoVidaAux;
	public float delay = 2f;
	float delayAux;
	public float intervalo = 0.3f;
	float intervaloAux;
	// Use this for initialization
	void OnEnable () {
		GetComponent<Renderer>().enabled = true;
		tiempoVidaAux = tiempoVida + Time.timeSinceLevelLoad;
		delayAux = delay + Time.timeSinceLevelLoad;
		intervaloAux = intervalo;
	}

	/*void OnEnable(){
		print ("habilitado");
	}*/
	
	// Update is called once per frame
	void Update () {
		if (delayAux > Time.timeSinceLevelLoad)
			return;
		intervaloAux -= Time.deltaTime;
		if (intervaloAux <= 0f) {
			GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
			intervaloAux = (tiempoVidaAux - Time.timeSinceLevelLoad) * 0.03f;
		}
	}
}