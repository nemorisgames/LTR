using UnityEngine;
using System.Collections;

public class ComportamientoIdolo : MonoBehaviour {
	Central c;
	// Use this for initialization
	void Start () {
		GameObject cent = GameObject.Find ("_central");
		if(cent != null) c = cent.GetComponent<Central> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player")) {
			c.estado = 3;
			other.GetComponent<CharController>().OnRobar();
			print ("terminado");
			//Destroy(gameObject);
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position -= Vector3.up * Time.deltaTime * 0.05f;
	}
}
