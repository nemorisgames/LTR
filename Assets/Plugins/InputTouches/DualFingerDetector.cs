using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class DualFingerDetector : MonoBehaviour {

	//for rotate
	private Vector2 initPos1=Vector3.zero;
	private Vector2 initPos2=Vector3.zero;
	private Vector2 initGradient;
	
	private Vector2 lastPos1;
	private Vector2 lastPos2;
	
	private bool firstTouch=true;
	
	private int currentStart=0;
	private List<float> rotVals=new List<float>();
	
	private float curAngle;
	private float prevAngle;
	
	private Vector2 lastTouchPos1;
	private Vector2 lastTouchPos2;
	
	
	//for tap
	private float tapStartTime=0;
	
	private enum _DTapState{Clear, Tap1, Complete}
	private _DTapState dTapState=_DTapState.Clear;
	
	private bool longTap;
	private Vector2 startPos;
	
	private bool posShifted;
	
	private float lastShortTapTime;
	private Vector2 lastShortTapPos;
	
	public float shortTapTime=0.2f;
	public float longTapTime=0.8f;
	public float doubleTapTime=0.5f;
	public float maxDTapPosSpacing=50;
	public float minChargeTime=0.2f;
	public float maxChargeTime=2.0f;
	
	private bool charged=false;
	private bool dragging=false;
	
	
	void Update(){
		if(Input.touchCount==2){
			Touch touch1=Input.touches[0];
			Touch touch2=Input.touches[1];
			
			Vector2 pos1 = touch1.position; 
			Vector2 pos2 = touch2.position; 
			
			Vector2 delta1 = pos1-lastTouchPos1;
			Vector2 delta2 = pos2-lastTouchPos2;
			
			
			if(firstTouch){
				firstTouch=false;
				
				//for rotate
				initPos1=pos1;
				initPos2=pos2;
				initGradient=(pos1-pos2).normalized;
				
				float curX=pos1.x-pos2.x;
				float curY=pos1.y-pos2.y;
				float prevAngle=Gesture.VectorToAngle(new Vector2(curX, curY));
				
				//for tap
				tapStartTime=Time.time;
				startPos=(pos1+pos2)/2;
				longTap=false;
				posShifted=false;
			}
			else{
				
				
				if(Vector2.Distance(Input.mousePosition, startPos)>5) posShifted=true;
					
				if(Time.time-tapStartTime>minChargeTime){
					charged=true;
					float chargeValue=Mathf.Min(1, (Time.time-tapStartTime)/maxChargeTime);
					ChargedInfo cInfo=new ChargedInfo((pos1+pos2)/2, chargeValue);
					Gesture.DFCharging(cInfo);
				}
				
				if(!longTap && !posShifted && Time.time-tapStartTime>1f){
					longTap=true;
					Gesture.DFLongTap((pos1+pos2)/2);
				}
			}
			
			
			if(touch1.phase==TouchPhase.Moved && touch2.phase==TouchPhase.Moved){
				
				float dot = Vector2.Dot(delta1, delta2);
				if(dot<0){
					Vector2 grad1=(pos1-initPos1).normalized;
					Vector2 grad2=(pos2-initPos2).normalized;
					
					float dot1=Vector2.Dot(grad1, initGradient);
					float dot2=Vector2.Dot(grad2, initGradient);
					
					//rotate				
					if(dot1<0.7f && dot2<0.7f){
						
						float curX=pos1.x-pos2.x;
						float curY=pos1.y-pos2.y;
						float curAngle=Gesture.VectorToAngle(new Vector2(curX, curY));
						float val=Mathf.DeltaAngle(curAngle, prevAngle);
						
						if(Mathf.Abs(val)>0) AddRotVal(val);
						float valueAvg=GetAverageValue();
						
						Gesture.Rotate(valueAvg);
						
						prevAngle=curAngle;
					}
					//pinch
					else{
						Vector2 curDist=pos1-pos2;
						Vector2 prevDist=(pos1-delta1)-(pos2-delta2);
						float pinch=prevDist.magnitude-curDist.magnitude;
						
						Gesture.Pinch(pinch);
					}
				}
				
				//drag
				if(dot>2){
					dragging=true;
					
					Vector2 posAvg=(pos1+pos2)/2;
					Vector2 dir=(delta1+delta2)/2;
					DragInfo dragInfo=new DragInfo(-1, posAvg, dir);
					Gesture.DualFingerDragging(dragInfo);
				}
			}
			
			lastTouchPos1=pos1;
			lastTouchPos2=pos2;
			
			
		}
		else if(Input.touchCount==0){
			//~ if(!firstTouch){
				//~ firstTouch=true;
			//~ }
			
			if(!firstTouch){
				firstTouch=true;
				
				//for tap
				if(Time.time-tapStartTime<shortTapTime){
					if(Time.time-lastShortTapTime<doubleTapTime){
						if(dTapState==_DTapState.Clear){
							dTapState=_DTapState.Tap1;
						}
						else if(dTapState==_DTapState.Tap1){
							if(Vector2.Distance(lastTouchPos1, lastShortTapPos)<maxDTapPosSpacing){
				
								dTapState=_DTapState.Clear;
								
								Gesture.DFDoubleTap((startPos+lastShortTapPos)/2);
							}
						}
					}
					else{
						dTapState=_DTapState.Tap1;
					}
					
					lastShortTapTime=Time.time;
					lastShortTapPos=(lastTouchPos1+lastTouchPos2)/2;
					Gesture.DFShortTap(startPos);
					
				}
				
				if(dragging){
					dragging=false;
					Gesture.DualFingerDraggingEnd((lastTouchPos1+lastTouchPos2)/2);
				}
				
				if(charged){
					charged=false;
					float chargeValue=Mathf.Min(1, (Time.time-tapStartTime)/maxChargeTime);
					ChargedInfo cInfo=new ChargedInfo((lastTouchPos1+lastTouchPos2)/2, chargeValue);
					Gesture.DFChargeEnd(cInfo);
				}
			}
		
			
		}
		
	}
	
	
	void AddRotVal(float val){
		if(rotVals.Count<10){
			rotVals.Add(val);
		}
		else{
			rotVals[currentStart]=val;
			
			currentStart+=1;
			if(currentStart>=rotVals.Count) currentStart=0;
		}
	}
	
	float GetAverageValue(){
		float valTotal=0;
		foreach(float val in rotVals){
			valTotal+=val;
		}
		
		return valTotal/rotVals.Count;
	}
	

}
