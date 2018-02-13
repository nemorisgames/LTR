using UnityEngine;
using System.Collections;

public class ComportamientoIdolo : MonoBehaviour {
	Central c;
	// Use this for initialization
	bool finished = false;
	public GameObject bloqueGO;
	GameObject bloque;
	Quaternion rotation;
	GameObject idolo;
	void Start () {
		GameObject cent = GameObject.Find ("_central");
		if(cent != null) c = cent.GetComponent<Central> ();
	}

	void Awake(){
		idolo = transform.Find("idolo").gameObject;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player") && !finished) {
			c.estado = 3;
			other.SendMessage("OnRobar");
			print ("terminado");
			//Destroy(gameObject);
			finished = true;
			bloque = (GameObject)Instantiate(bloqueGO,transform.position,Quaternion.identity);
			bloque.transform.position = new Vector3(bloque.transform.position.x, bloque.transform.position.y -0.85f, bloque.transform.position.z);
			bloque.transform.localScale = transform.lossyScale;
			bloque.transform.parent = other.transform;
			rotation = bloque.transform.rotation;
			other.GetComponent<CN2DControllerNemoris2>().EnableRotation(false);
			idolo.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//transform.position -= Vector3.up * Time.deltaTime * 0.05f;
		if(finished)
			bloque.transform.rotation = rotation;
	}

	public void restart(){
		finished = false;
		idolo.SetActive(true);
	}
}
