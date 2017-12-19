using UnityEngine;
using System.Collections;

public class MovimientoEscena : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position -= Vector3.up * Time.deltaTime * 0.05f;
	}
}
