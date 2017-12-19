using UnityEngine;
using System.Collections;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class SwipeDetector : MonoBehaviour {

	private Vector2 swipeDir;
	private Vector2 initVector;
	
	private Vector2 lastTouchPos;
	private Vector2 lastMousePos;
	//private Vector2 lastMousePosAlt;
	
	private enum _SwipeState{None, Start, Swiping, End}
	private _SwipeState swipeState;
	//private _SwipeState swipeStateAlt;
	
	private float timeStartSwipe;
	//private float timeStartSwipeAlt;
	private Vector2 swipeStartPos;
	//private Vector2 swipeStartPosAlt;
	
	public float maxSwipeDuration=0.25f;
	public float minSpeed=150;
	public float minDistance=15;
	public float maxDirectionChange=35;
	
	//private Swipe swipe;
	//private Swipe swipeAlt;
	
	private bool firstTouch=true;
	
	
	void SwipeStart(Vector2 pos){
		//GameMessage.DisplayMessage("swipe start");
		timeStartSwipe=Time.time;
		swipeStartPos=pos;
		swipeState=_SwipeState.Swiping;
		//Gesture.SwipeStart(pos);
	}
	
	
	void SwipeEnd(Vector2 pos){
		
		swipeState=_SwipeState.End;
		swipeDir=pos-swipeStartPos;
		
		if((swipeDir).magnitude<minDistance) {
			//Debug.Log("too short");
			//GameMessage.DisplayMessage("too short");
			return;
		}
			
		//GameMessage.DisplayMessage("swipe end "+pos);
		
		SwipeInfo sw=new SwipeInfo(swipeStartPos, pos, swipeDir, timeStartSwipe);
		Gesture.Swipe(sw);
	}
	
	
	// Use this for initialization
	void Start () {
		swipeDir=Vector2.zero;
		lastTouchPos=Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
		#if UNITY_IPHONE || UNITY_ANDROID
			if(Input.touchCount==1){
				
				Touch touch=Input.touches[0];
				Vector2 curPos=touch.position;
				
				if(firstTouch){
					firstTouch=false;
					lastTouchPos=curPos;
				}

				Vector2 curVector=curPos-lastTouchPos;

				if(Mathf.Abs(curVector.magnitude)>0){

					if(swipeState==_SwipeState.None){
						SwipeStart(curPos);
						//initVector=curPos-lastTouchPos;
						initVector=curVector;
					}
					else if(swipeState==_SwipeState.Swiping){
						//GameMessage.DisplayMessage("swiping");
						if(Time.time-timeStartSwipe>maxSwipeDuration){
							//GameMessage.DisplayMessage("duration due");
							SwipeEnd(curPos);
						}
						//check angle
						if(Mathf.Abs(Vector2.Angle(initVector, curVector))>maxDirectionChange){
							//GameMessage.DisplayMessage("angle is too wide "+initVector+"   "+curVector);
							SwipeEnd(curPos);
						}
						//check speed
						if(Mathf.Abs((curVector).magnitude/(Time.time-timeStartSwipe))<minSpeed){
							//GameMessage.DisplayMessage("too slow");
							SwipeEnd(curPos);
						}
						
					}
					
					//Gesture.Dragging(touch.deltaPosition);
					lastTouchPos=curPos;
				}
				
				
				
			}
			else{
				if(!firstTouch) firstTouch=true;
				
				if(swipeState==_SwipeState.Swiping){
					SwipeEnd(lastTouchPos);
				}
				else if(swipeState==_SwipeState.End){
					swipeState=_SwipeState.None;
				}
				//lastTouchPos=new Vector2(-9999, -9999);
			}
		#endif
			
		#if (!UNITY_IPHONE && !UNITY_ANDROID) //|| UNITY_EDITOR

			if(Input.GetMouseButton(0)){
				Vector2 curPos=Input.mousePosition;
				Vector2 delta=curPos-lastMousePos;
				
				if(Mathf.Abs((delta).magnitude)>0){
					//Debug.Log(curPos+"   "+lastMousePos+"   "+delta+"   "+Mathf.Abs((delta).magnitude));
					if(swipeState==_SwipeState.None){
						SwipeStart(curPos);
						initVector=delta;
					}
					else if(swipeState==_SwipeState.Swiping){
						if(Time.time-timeStartSwipe>maxSwipeDuration){
							//Debug.Log("duration due");
							//GameMessage.DisplayMessage("duration due");
							SwipeEnd(curPos);
						}
						//check angle
						Vector2 curVector=curPos-swipeStartPos;
						if(Mathf.Abs(Vector2.Angle(initVector, curVector))>maxDirectionChange){
							//Debug.Log("angle is too wide");
							//GameMessage.DisplayMessage("angle is too wide");
							SwipeEnd(curPos);
						}
						//check speed
						if(Mathf.Abs((curVector).magnitude/(Time.time-timeStartSwipe))<minSpeed){
							//Debug.Log("too slow");
							//GameMessage.DisplayMessage("too slow");
							SwipeEnd(curPos);
						}
					}
					
					lastMousePos=curPos;
				}
				
				//count+=1;
			}
			
			
			if(Input.GetMouseButtonUp(0)){
				if(swipeState==_SwipeState.Swiping){
					SwipeEnd(lastMousePos);
				}
				swipeState=_SwipeState.None;
			}
			
			lastMousePos=Input.mousePosition;
		#endif
	}

	
}

//
//public class Swipe{
//	public Vector2 startPoint;
//	public Vector2 endPoint;
//	
//	public Vector2 direction;
//	public float angle;
//	
//	public float duration;
//	public float speed;
//	
//	public Swipe(Vector2 p1, Vector2 p2, Vector2 dir, float startT){
//		startPoint=p1;
//		endPoint=p2;
//		direction=dir;
//		angle=Gesture.VectorToAngle(dir);
//		duration=Time.time-startT;
//		speed=dir.magnitude/duration;
//	}
//}