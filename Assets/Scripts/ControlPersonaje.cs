using UnityEngine;
using System.Collections;

public class ControlPersonaje : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void controlAdelante(){
		transform.Translate (transform.forward);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
