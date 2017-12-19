using UnityEngine;
using System.Collections;

public class Bloque : MonoBehaviour {

	public Vector3 posicion;
	//0: bloque normal
	public int tipo = 0;
	public bool randomRotacion = false;

	// Use this for initialization
	void Start () {
		posicion = transform.position;
		aplicarRotacion ();
	}

	public void aplicarRotacion(){
		if (randomRotacion)
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range (0, 3) * 90, transform.eulerAngles.z);
	}

	public void setPosicion(Vector3 pos){
		posicion = pos;
		transform.position = new Vector3(pos.x, pos.y / 2f, pos.z);
	}

	public void setTipo(int t){
		tipo = t;
	}

	public void setParent(Transform t){
		transform.parent = t;
	}
	
	// Update is called once per frame
	void Update () {
		//if (gameObject.GetComponentInChildren<Renderer> ().enabled) { 
		//	transform.position -= Vector3.up * Time.deltaTime * 0.05f;
		//	if(transform.position.y < -1f) Destroy(gameObject);
		//}
	}
}
