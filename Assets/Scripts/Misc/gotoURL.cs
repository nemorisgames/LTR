using UnityEngine;
using System.Collections;

public class gotoURL : MonoBehaviour {
	public string miURL = "http://www.nemorisgames.com/marker.jpg";
	// Use this for initialization
	void Start () {
	
	}

	void OnClick(){
		Application.OpenURL (miURL);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
