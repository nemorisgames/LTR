using UnityEngine;
using System.Collections;

public class MovimientoBloqueInicio : MonoBehaviour {
	float velocidad = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Vector3.forward * velocidad * Time.deltaTime;
		if (transform.position.z > 10f)
			transform.position -= Vector3.forward * 12f;
	}
}
