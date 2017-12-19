using UnityEngine;
using System.Collections;

public class ControlGestos : MonoBehaviour {
	Central central;
	public CN2DControllerNemoris2 js;
	// Use this for initialization
	void Start () {
		central = gameObject.GetComponent<Central> ();
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
		//print ("detectado " + d.delta);
		if (js.utilizandose) return;
		if(d.pos.x < Screen.width * 0.8f && d.pos.x > Screen.width * 0.2f && d.pos.y < Screen.height * 0.9f && d.pos.y > Screen.height * 0.1f)
			//no hacer que gire mas rapido o el personaje se mete dentro de los bloques
			StartCoroutine(central.girarEscena( - 0.10f * Mathf.Clamp(d.delta.x, -12f, 12f)));// * 960f / (float)Screen.width));

	}

	void OnPinchE(float val){
		central.zoom (val);
	}
	
	/*void OnSwipe(SwipeInfo sw){
		print ("swipe " + sw.startPoint);
		//position the projectile object at the start point of the swipe
		Vector3 p=Camera.main.ScreenToWorldPoint(new Vector3(sw.startPoint.x, sw.startPoint.y, 35));
		projectileObject.position=p;
		
		//clear the projectile current velocity before apply a new force in the swipe direction, take account of the swipe speed
		body.velocity=new Vector3(0, 0, 0);
		body.AddForce(new Vector3(sw.direction.x, 0, sw.direction.y) * sw.speed*0.0035f);
		
		//show the swipe info 
		string labelText="Swipe Detected\n\n";
		labelText+="direction: "+sw.direction+"\n";
		labelText+="angle: "+sw.angle.ToString("f1")+"\n";
		labelText+="speed: "+sw.speed.ToString("f1")+"\n";
		label.text=labelText;
		
		//if the label is previous cleared, re-initiate the coroutine to clear it
		if(labelTimer<0){
			StartCoroutine(ClearLabel());
		}
		//else just extend the timer
		else labelTimer=5;
		
		StartCoroutine(ShowSwipeIndicator(sw));
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}
}
