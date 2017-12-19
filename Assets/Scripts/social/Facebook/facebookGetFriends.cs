using UnityEngine;
using System.Collections;

public class facebookGetFriends : MonoBehaviour {
	GameObject redesSociales;
	// Use this for initialization
	void Start () {
		redesSociales = GameObject.Find("RedesSociales");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void getFriends(){
		redesSociales.SendMessage("getFriends");
	}
}
