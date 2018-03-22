using UnityEngine;
using System.Collections;

public class ComportamientoIdolo : MonoBehaviour {
	Central c;
	// Use this for initialization
	bool finished = false;
	public GameObject [] bloques; //0: gold, 1: rock, 2: alfombra
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
			GameObject bloqueGO = null;
			switch(PlayerPrefs.GetInt("escenaActual",1)){
				case 8:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				bloqueGO = bloques[1];
				break;
				case 9:
				bloqueGO = bloques[2];
				break;
				default:
				bloqueGO = bloques[0];
				break;
			}
			bloque = (GameObject)Instantiate(bloqueGO,transform.position,Quaternion.identity);
			StartCoroutine(showBlock(bloque));
			bloque.transform.position = new Vector3(bloque.transform.position.x, bloque.transform.position.y -0.85f, bloque.transform.position.z);
			bloque.transform.localScale = transform.lossyScale;
			bloque.transform.parent = other.transform;
			rotation = bloque.transform.rotation;
			other.GetComponent<CN2DControllerNemoris2>().EnableRotation(false);
			idolo.SetActive(false);
		}
	}

	//1 - 7, 10, 19, 20, 21: gold
	//8, 14, 15, 16, 17, 18 : rock
	//9: alfombra
	
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

	IEnumerator showBlock(GameObject bloque){
		yield return new WaitForSeconds(1.1f);
		bloque.GetComponentInChildren<MeshRenderer>().enabled = true;
	}

	

}
