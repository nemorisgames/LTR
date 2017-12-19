using UnityEngine;
using System.Collections;

public class ComportamientoLava : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up, 2f * Time.deltaTime);
	}
}
