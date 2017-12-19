using UnityEngine;
using System.Collections;

public class LuzIntensidad : MonoBehaviour {
	float intensidadInicial;
	float intensidadActual;
	float velocidad = 2f;
	// Use this for initialization
	void Start () {
		intensidadInicial = GetComponent<Light>().intensity;
		intensidadActual = intensidadInicial;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		intensidadActual -= velocidad * Time.fixedDeltaTime;
		if (intensidadActual < intensidadInicial * 0.25f) {
			velocidad = -2f;
		}
		if (intensidadActual > intensidadInicial) {
			velocidad = 2f;
		}
		GetComponent<Light>().intensity = intensidadActual;
	}
}
