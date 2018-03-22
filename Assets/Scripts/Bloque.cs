using UnityEngine;
using System.Collections;

public class Bloque : MonoBehaviour {
	public Vector3 posicion;
	//0: bloque normal
	public int tipo = 0;
	public bool randomRotacion = false;
	public bool shaking = false;
	public bool noShake = false;
	Transform renderBlock;

	void Awake(){
		renderBlock = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
	}

	// Use this for initialization
	void Start () {
		posicion = transform.position;
		aplicarRotacion ();
		shaking = false;
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
		if(transform.position.y < 0.5f && transform.position.y > -0.25f && !shaking) StartCoroutine(shake());
	}

	IEnumerator shake(){
		if(shaking || noShake)
			yield break;
		shaking = true;
		Vector3 pos = renderBlock.position;
		float target = Random.Range(0f,1f)/35f;
		pos.y += target;
		renderBlock.position = pos;
		yield return new WaitForSeconds(0.05f);
		pos.y -= target;
		renderBlock.position = pos;
		yield return new WaitForSeconds(0.05f);
		renderBlock.localPosition = new Vector3(0f,-0.23f,0f);
		shaking = false;
	}

	public void ResetBlock(){
		if(noShake)
			return;
		StopAllCoroutines();
		shaking = false;
		renderBlock.localPosition = new Vector3(0f,-0.23f,0f);
	}
}
