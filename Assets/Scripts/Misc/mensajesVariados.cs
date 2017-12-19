using UnityEngine;
using System.Collections;

public class mensajesVariados : MonoBehaviour {
	public string[] mensajes;
	UILabel label;
	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel> ();
	}

	void OnEnable(){
		if(label == null) label = GetComponent<UILabel> ();
		label.text = mensajes[Random.Range(0, mensajes.Length)];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
