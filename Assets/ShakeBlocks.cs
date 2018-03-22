using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBlocks : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider c){
		Debug.Log(c.name);
		if(c.tag == "Bloque"){
			//c.GetComponent<Bloque>().EnableShake(true);
		}
	}

	/*void OnTriggerStay(Collider c){
		if(c.tag == "Bloque"){
			c.GetComponent<Bloque>().EnableShake(true);
		}
	}*/

	void OnTriggerExit(Collider c){
		if(c.tag == "Bloque"){
			//c.GetComponent<Bloque>().EnableShake(false);
		}
	}
}
