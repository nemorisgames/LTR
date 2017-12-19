using UnityEngine;
using System.Collections;

public class facebookInvitar : MonoBehaviour {
	GameObject redesSociales;
	// Use this for initialization
	void Start () {
		redesSociales = GameObject.Find("RedesSociales");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void inviteFriends(){
		redesSociales.SendMessage("inviteFriends");
	}
}
