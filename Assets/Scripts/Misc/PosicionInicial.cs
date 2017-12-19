using UnityEngine;
using System.Collections;

public class PosicionInicial : MonoBehaviour {
	public Vector3 posicion;
	// Use this for initialization
	void Start () {
		posicion = transform.position;
	}

	public void reset(){
		transform.position = posicion;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
