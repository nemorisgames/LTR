using UnityEngine;
using System.Collections;

public class comportamientoEspinas : MonoBehaviour {

	public float tiempoReposo = 2f;
	float tiempoReposoActual;
	bool arriba = true;
	public bool movil = true;
	bool activa = false;

	// Use this for initialization
	void Start () {
		
	}

	public void activar(bool a){
		activa = a;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (activa?0f:0.9f));
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			other.GetComponent<CharController>().OnDeath();
			//collider.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!activa) {
			if(movil){
				if(tiempoReposoActual <= 0){
					transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + (arriba?-2f:2f) * Time.deltaTime);
					if(transform.localScale.z < 0f){ transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, arriba?0f:0.9f); arriba = false; tiempoReposoActual = Time.time + tiempoReposo;}
					if(transform.localScale.z > 0.9f){ transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, arriba?0f:0.9f); arriba = true; tiempoReposoActual = Time.time + tiempoReposo; }
				}
				else{
					if(tiempoReposoActual < Time.time) tiempoReposoActual = 0f;
				}
			}
		}
	}
}
