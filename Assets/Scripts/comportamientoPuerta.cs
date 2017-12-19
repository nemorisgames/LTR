using UnityEngine;
using System.Collections;

public class comportamientoPuerta : MonoBehaviour {

	bool activa = true;

	// Use this for initialization
	void Start () {
		
	}

	public void activar(bool a){
		activa = a;
	}
	
	// Update is called once per frame
	void Update () {
		if (activa) {
			//if (renderer.enabled){
				transform.parent.GetComponent<Collider>().enabled = activa;
				GetComponent<Renderer>().material.color = new Color(0.3f, 0.5f, 1f, 1f);
			//}
		} else {
			//if (renderer.enabled){
				transform.parent.GetComponent<Collider>().enabled = activa;
				GetComponent<Renderer>().material.color = new Color(0.3f, 0.5f, 1f, 0.5f);
			//}
		}
	}
}
