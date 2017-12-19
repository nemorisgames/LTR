using UnityEngine;
using System.Collections;

public class ControlMoneda : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.Rotate (0f, 0f,Random.Range(0f, 360f));
	}

	void OnTriggerEnter(Collider c){
		if (c.CompareTag ("Player")) {
			GameObject.Find("_central").SendMessage("tomarOro");
			//Destroy (gameObject);
			gameObject.SetActive(false);
			if(transform.parent.gameObject.tag != "Bloque") 
				Destroy (transform.parent.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0f, 0f, 300f * Time.deltaTime);
	}
}
