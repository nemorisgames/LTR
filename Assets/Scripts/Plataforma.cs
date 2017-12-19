using UnityEngine;
using System.Collections;

public class Plataforma : MonoBehaviour {
	public float velocidadMaxima = 1f;
	public Vector3 orientacion = Vector3.forward;
	public float recorrido = 10f;
	float velocidadActual = 0f;
	public Vector3 posicionInicial;
	public Vector3 posicionFinal;
	public bool activa = true;
	bool adelante = true;
	// Use this for initialization
	void Start () {
		posicionInicial = transform.parent.localPosition;
		posicionFinal = transform.parent.localPosition + orientacion * recorrido * transform.parent.localScale.y;
		velocidadActual = velocidadMaxima * transform.parent.localScale.y;
	}

	void activar(bool a){
		activa = a;
		if (activa) GetComponent<AudioSource>().Play ();
		else GetComponent<AudioSource>().Stop ();
	}

	// Update is called once per frame
	void LateUpdate () {

		if (!activa) { GetComponent<AudioSource>().Stop (); return; }
		if (gameObject.GetComponent<Renderer> ().enabled) {
			if(Time.timeScale <= 0) GetComponent<AudioSource>().Stop ();
			else
				if(!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play ();
			//para compensar el movimiento de la lava
			//posicionInicial -= Vector3.up * Time.deltaTime * 0.05f;
			//posicionFinal -= Vector3.up * Time.deltaTime * 0.05f;
				
			//if (Vector3.Distance (transform.parent.localPosition, posicionFinal) < 0.1f * transform.parent.localScale.y){
			if (Vector3.Distance (transform.parent.localPosition, posicionInicial) > Vector3.Distance (posicionFinal, posicionInicial)){
				//print (Vector3.Distance (transform.parent.localPosition, posicionInicial) +">"+ Vector3.Distance (posicionFinal, posicionInicial));
				velocidadActual = - velocidadMaxima * transform.parent.localScale.y;
				if(adelante){
					adelante = false;
					transform.parent.localPosition = posicionFinal;
				}
				//transform.parent.localPosition = posicionFinal;
			}
			else{
				if (Vector3.Distance (transform.parent.localPosition, posicionFinal) > Vector3.Distance (posicionFinal, posicionInicial)){
					//print (Vector3.Distance (transform.parent.localPosition, posicionFinal) +">"+ Vector3.Distance (posicionFinal, posicionInicial));
					velocidadActual = velocidadMaxima * transform.parent.localScale.y;
					if(!adelante){
						adelante = true;
						transform.parent.localPosition = posicionInicial;
					}
					//transform.parent.localPosition = posicionInicial;
				}
			}
			transform.parent.localPosition += orientacion * velocidadActual * Time.deltaTime; //Vector3.Lerp (rigidbody.velocity, transform.parent.forward * 100f, Time.deltaTime );
		}
		else GetComponent<AudioSource>().Stop ();
	}
}
