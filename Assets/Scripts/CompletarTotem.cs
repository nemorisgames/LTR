using UnityEngine;
using System.Collections;

public class CompletarTotem : MonoBehaviour {
	public GameObject[] piezas;
	public UILabel piecesHave;
	public UILabel piecesLeft;
	public Transform totem;
	// Use this for initialization
	void Start () {
		Time.timeScale = 1f;
		for (int i = 0; i < piezas.Length; i++) {
			if(piezas[i] != null && i + 1 <= (PlayerPrefs.GetInt("escenasPasadas", 0)) - 1) piezas[i].SetActive(true);
		}
		piecesHave.text = "you have " + Mathf.Clamp((PlayerPrefs.GetInt("escenasPasadas", 0)) - 1, 0, 20) + " pieces!";
		piecesLeft.text = (20 - (PlayerPrefs.GetInt("escenasPasadas", 0) - 1)) + " pieces left";
	}

	void OnEnable(){
		Gesture.onDraggingE += OnDrag;
		Gesture.onPinchE += OnPinchE;
		//Gesture.onSwipeE += OnSwipe;
	}
	
	void OnDisable(){
		Gesture.onDraggingE -= OnDrag;
		Gesture.onPinchE -= OnPinchE;
		//Gesture.onSwipeE -= OnSwipe;
	}
	
	void OnDrag(DragInfo d){
		if(d.pos.x < Screen.width * 0.8f && d.pos.x > Screen.width * 0.2f && d.pos.y < Screen.height * 0.9f && d.pos.y > Screen.height * 0.1f)
			girar (d.delta.x);
	}

	public void continuar(){
		Application.LoadLevel ("SeleccionEscena");
	}

	void girar(float g){
		totem.Rotate(0f, - g, 0f);
	}
	
	void OnPinchE(float val){
		zoom (val);
	}

	void zoom(float z){
		totem.localScale = (Mathf.Clamp(z + totem.localScale.x, 0.5f, 1.3f)) * Vector3.one;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("ArrowsHorizontal") != 0f)
			girar (Input.GetAxis ("ArrowsHorizontal") * Time.deltaTime * 100f);
		if (Input.GetAxis ("ArrowsVertical") != 0f)
			zoom (Input.GetAxis ("ArrowsVertical") * Time.deltaTime * 1f);

	}
}
