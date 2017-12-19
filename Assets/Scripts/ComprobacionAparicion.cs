using UnityEngine;
using System.Collections;

public class ComprobacionAparicion : MonoBehaviour {
	public Renderer objeto;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (objeto.enabled) {
				if (GetComponent<Rigidbody>().IsSleeping ())
						GetComponent<Rigidbody>().WakeUp ();
		} else {
				if (!GetComponent<Rigidbody>().IsSleeping ())
						GetComponent<Rigidbody>().Sleep ();
		}

	}
}
