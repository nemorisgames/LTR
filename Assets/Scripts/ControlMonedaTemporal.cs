using UnityEngine;
using System.Collections;

public class ControlMonedaTemporal : MonoBehaviour {
	float tiempoVida;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().AddForce(Random.Range(60f, 120f) * Mathf.Sign(Random.Range(-1, 1)), Random.Range(200f, 350f), Random.Range(60f, 120f) * Mathf.Sign(Random.Range(-1, 1)));
		tiempoVida = Time.timeSinceLevelLoad + 10f;
	}
	
	// Update is called once per frame
	void Update () {
		if (tiempoVida <= Time.timeSinceLevelLoad)
			Destroy (gameObject);
	}
}
