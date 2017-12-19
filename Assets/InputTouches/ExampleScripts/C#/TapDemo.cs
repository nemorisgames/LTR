using UnityEngine;
using System.Collections;

public class TapDemo : MonoBehaviour {

	public ParticleSystem Indicator;
	
	public Transform shortTapObj;
	public Transform longTapObj;
	public Transform doubleTapObj;
	public Transform chargeObj;
	
	public TextMesh chargeTextMesh;
	
	private bool latchOnCursor;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnEnable(){
		Gesture.onShortTapE += OnShortTap;
		Gesture.onLongTapE += OnLongTap;
		Gesture.onDoubleTapE += OnDoubleTap;
		
		Gesture.onChargingE += OnCharging;
		Gesture.onChargeEndE += OnChargeEnd;
		
		Gesture.onDraggingE += OnDragging;
		Gesture.onDraggingEndE += OnDraggingEnd;
	}
	
	void OnDisable(){
		Gesture.onShortTapE -= OnShortTap;
		Gesture.onLongTapE -= OnLongTap;
		Gesture.onDoubleTapE -= OnDoubleTap;
		
		Gesture.onChargingE -= OnCharging;
		Gesture.onChargeEndE -= OnChargeEnd;
		
		Gesture.onDraggingE -= OnDragging;
		Gesture.onDraggingEndE -= OnDraggingEnd;
	}
	
	//called when a short tap event is ended
	void OnShortTap(Vector2 pos){
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			if(hit.collider.transform==shortTapObj){
				//place the indicator at the object position and assign a random color to it
				Indicator.transform.position=shortTapObj.position;
				Indicator.startColor=GetRandomColor();
				//emit a set number of particle
				Indicator.Emit(30);
			}
		}
	}
	
	//called when a long tap event is ended
	void OnLongTap(Vector2 pos){
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			if(hit.collider.transform==longTapObj){
				//place the indicator at the object position and assign a random color to it
				Indicator.transform.position=longTapObj.position;
				Indicator.startColor=GetRandomColor();
				//emit a set number of particle
				Indicator.Emit(30);
			}
		}
	}
	
	//called when a double tap event is ended
	void OnDoubleTap(Vector2 pos){
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			if(hit.collider.transform==doubleTapObj){
				//place the indicator at the object position and assign a random color to it
				Indicator.transform.position=doubleTapObj.position;
				Indicator.startColor=GetRandomColor();
				//emit a set number of particle
				Indicator.Emit(30);
			}
		}
	}
	
	//called when a charging event is detected
	void OnCharging(ChargedInfo cInfo){
		Ray ray = Camera.main.ScreenPointToRay(cInfo.pos);
		RaycastHit hit;
		//use raycast at the cursor position to detect the object
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			if(hit.collider.transform==chargeObj){
				//display the charged percentage on screen
				chargeTextMesh.text="Charging "+(cInfo.percent*100).ToString("f1")+"%";
			}
		}
	}
	
	//called when a charge event is ended
	void OnChargeEnd(ChargedInfo cInfo){
		Ray ray = Camera.main.ScreenPointToRay(cInfo.pos);
		RaycastHit hit;
		//use raycast at the cursor position to detect the object
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			if(hit.collider.transform==chargeObj){
				//place the indicator at the object position and assign a random color to it
				Indicator.transform.position=chargeObj.position;
				Indicator.startColor=GetRandomColor();
				
				//adjust the indicator speed with respect to the charged percent
				Indicator.startSpeed=1+3*cInfo.percent;
				//emit a set number of particles with respect to the charged percent
				Indicator.Emit((int)(10+cInfo.percent*75f));
				
				//reset the particle speed, since it's shared by other event
				StartCoroutine(ResumeSpeed());
			}
		}
		chargeTextMesh.text="HoldToCharge";
	}
	
	//reset the particle emission speed of the indicator
	IEnumerator ResumeSpeed(){
		yield return new WaitForSeconds(Indicator.startLifetime);
		Indicator.startSpeed=2;
	}
	
	public Transform dragObj;
	public TextMesh dragTextMesh;
	
	void OnDragging(DragInfo dragInfo){
		if(latchOnCursor){
			ObjToCursor(dragInfo);
		}
		else{
			Ray ray = Camera.main.ScreenPointToRay(dragInfo.pos);
			RaycastHit hit;
			//use raycast at the cursor position to detect the object
			if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
				if(hit.collider.transform==dragObj){
					latchOnCursor=true;
					dragObj.localScale*=1.1f;
					
					ObjToCursor(dragInfo);
				}
			}
		}
	}
	
	void ObjToCursor(DragInfo dragInfo){
		Vector3 p=Camera.main.ScreenToWorldPoint(new Vector3(dragInfo.pos.x, dragInfo.pos.y, 30));
		dragObj.position=p;
		
		if(dragInfo.type==0){
			//display the charged percentage on screen
			dragTextMesh.text="Dragging with finger";
		}
		if(dragInfo.type==1){
			//display the charged percentage on screen
			dragTextMesh.text="Dragging with mouse1";
		}
		if(dragInfo.type==2){
			//display the charged percentage on screen
			dragTextMesh.text="Dragging with mouse2";
		}
	}
	
	void OnDraggingEnd(Vector2 pos){
		//drop the dragObj
		if(latchOnCursor){
			latchOnCursor=false;
			dragObj.localScale*=10f/11f;
			
			Vector3 p=Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 30));
			dragObj.position=p;
			dragTextMesh.text="DragMe";
		}
	}
	
	
	//return a random color when called
	private Color GetRandomColor(){
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
	

	private bool instruction=false;
	void OnGUI(){
		if(!instruction){
			if(GUI.Button(new Rect(10, 55, 130, 35), "Instruction On")){
				instruction=true;
			}
		}
		else{
			if(GUI.Button(new Rect(10, 55, 130, 35), "Instruction Off")){
				instruction=false;
			}
			
			GUI.Box(new Rect(10, 100, 200, 65), "");
			
			GUI.Label(new Rect(15, 105, 190, 65), "interact with each object using the interaction stated on top of each of them");
		}
	}

}
