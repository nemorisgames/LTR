using UnityEngine;
using System.Collections;

public class General : MonoBehaviour {

	private Vector2 lastPos;
	private bool dragging=false;
	private bool draggingAlt=false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IPHONE || UNITY_ANDROID
			if(Input.touchCount>0){
				foreach(Touch touch in Input.touches){
					if(touch.phase==TouchPhase.Began){
						Gesture.Down(touch.position);
					}
					else if(touch.phase==TouchPhase.Ended){
						Gesture.Up(touch.position);
					}
					else{
						Gesture.On(touch.position);
					}
				}
			}
			
			if(Input.touchCount==1){
				Touch touch=Input.touches[0];
				if(touch.phase == TouchPhase.Moved){
					if(!dragging) dragging=true;
					DragInfo dragInfo=new DragInfo(0, touch.position, touch.deltaPosition);
					Gesture.Dragging(dragInfo);
				}
				//lastPos=touch.position;
			}
			else{
				if(dragging){
					dragging=false;
					Gesture.DraggingEnd(lastPos);
				}
			}
		#endif
			
		#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
			if(Input.GetMouseButtonDown(0)){
				Gesture.Down(Input.mousePosition);
				lastPos=Input.mousePosition;
			}
			if(Input.GetMouseButton(0)){
				Gesture.On(Input.mousePosition);
				
				Vector2 curPos=Input.mousePosition;
				Vector2 delta=curPos-lastPos;
				
				if(Mathf.Abs((delta).magnitude)>0){
					dragging=true;
					DragInfo dragInfo=new DragInfo(1, curPos, delta);
					Gesture.Dragging(dragInfo);
				}
				
				lastPos=Input.mousePosition;
			}
			if(Input.GetMouseButtonUp(0)){
				Gesture.Up(Input.mousePosition);
				
				if(dragging){
					dragging=false;
					Gesture.DraggingEnd(Input.mousePosition);
				}
			}
			
			
			if(Input.GetMouseButtonDown(1)){
				Gesture.DownAlt(Input.mousePosition);
				lastPos=Input.mousePosition;
			}
			else if(Input.GetMouseButton(1)){
				Gesture.OnAlt(Input.mousePosition);
				Vector2 curPos=Input.mousePosition;
				Vector2 delta=curPos-lastPos;
				
				if(Mathf.Abs((delta).magnitude)>0){
					draggingAlt=true;
					DragInfo dragInfo=new DragInfo(2, curPos, delta);
					Gesture.Dragging(dragInfo);
				}
				
				lastPos=Input.mousePosition;
			}
			else if(Input.GetMouseButtonUp(1)){
				Gesture.Up(Input.mousePosition);
				
				if(draggingAlt){
					draggingAlt=false;
					Gesture.DraggingEnd(Input.mousePosition);
				}
			}
		#endif
	}
	
	
}
