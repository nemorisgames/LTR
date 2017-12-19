using UnityEngine;
using System.Collections;

public class sombra : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (Physics.Raycast (transform.parent.position, -Vector3.up, out hit, 5f, ~(1 << 11))) {
			transform.position = new Vector3(transform.parent.position.x, hit.point.y + 0F, transform.parent.position.z);
		}
	}
}
