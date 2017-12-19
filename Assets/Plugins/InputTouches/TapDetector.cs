using UnityEngine;
using System.Collections;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class TapDetector : MonoBehaviour {

	private float tapStartTime=0;
	//private bool touched=false;
	
	private enum _DTapState{Clear, Tap1, Complete}
	private _DTapState dTapState=_DTapState.Clear;
	private Vector2 lastTouchPos;
	
	private enum _ChargeState{Clear, Charged}
	private _ChargeState chargeState=_ChargeState.Clear;
	private float chargedValue=0;
	
	private bool longTap;
	private Vector2 startPos;
	
	private bool posShifted;
	
	private float lastShortTapTime;
	private Vector2 lastShortTapPos;
	
	public float shortTapTime=0.2f;
	public float longTapTime=0.8f;
	public float maxLTapSpacing=10;
	public float doubleTapTime=0.5f;
	public float maxDTapPosSpacing=50;
	public float minChargeTime=0.15f;
	public float maxChargeTime=2.0f;
	
	private bool firstTouch=true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IPHONE || UNITY_ANDROID
			if(Input.touchCount==1){
				Vector2 touchPos=Input.touches[0].position;
				
				//touch down
				if(firstTouch){
					firstTouch=false;
					tapStartTime=Time.time;
					startPos=touchPos;
					longTap=false;
					posShifted=false;
				}
				//on touch
				else{
					if(Vector2.Distance(Input.mousePosition, startPos)>maxLTapSpacing) posShifted=true;
					
					if(Time.time-tapStartTime>minChargeTime){
						if(chargeState==_ChargeState.Clear) chargeState=_ChargeState.Charged;
						chargedValue=Mathf.Min(1, (Time.time-tapStartTime)/maxChargeTime);
						ChargedInfo cInfo=new ChargedInfo(touchPos, chargedValue);
						Gesture.Charging(cInfo);
					}
					
					if(!longTap && !posShifted && Time.time-tapStartTime>1f){
						longTap=true;
						Gesture.LongTap(startPos);
					}
				}
				
				lastTouchPos=touchPos;
			}
			else if(Input.touchCount==0){
				//touch up
				if(!firstTouch){
					firstTouch=true;
					if(Time.time-tapStartTime<shortTapTime){
						if(Time.time-lastShortTapTime<doubleTapTime){
							if(dTapState==_DTapState.Clear){
								dTapState=_DTapState.Tap1;
							}
							else if(dTapState==_DTapState.Tap1){
								if(Vector2.Distance(lastTouchPos, lastShortTapPos)<maxDTapPosSpacing){
					
									dTapState=_DTapState.Clear;
									
									Gesture.DoubleTap((startPos+lastShortTapPos)/2);
								}
							}
						}
						else{
							dTapState=_DTapState.Tap1;
						}
						
						lastShortTapTime=Time.time;
						lastShortTapPos=lastTouchPos;
						Gesture.ShortTap(startPos);
					}
				}
				
				if(chargeState==_ChargeState.Charged){
					ChargedInfo cInfo=new ChargedInfo(lastTouchPos, chargedValue);
					Gesture.ChargeEnd(cInfo);
					
					chargedValue=0;
					chargeState=_ChargeState.Clear;
				}
			}
		#endif
			
		#if (!UNITY_IPHONE && !UNITY_ANDROID) //|| UNITY_EDITOR
			if(Input.GetMouseButtonDown(0)){
				tapStartTime=Time.time;
				startPos=Input.mousePosition;
				longTap=false;
				posShifted=false;
			}
			
			if(Input.GetMouseButton(0)){
				
				if(Vector2.Distance(Input.mousePosition, startPos)>5) posShifted=true;
				
				if(Time.time-tapStartTime>minChargeTime){
					if(chargeState==_ChargeState.Clear) chargeState=_ChargeState.Charged;
					chargedValue=Mathf.Min(1, (Time.time-tapStartTime)/maxChargeTime);
					ChargedInfo cInfo=new ChargedInfo(Input.mousePosition, chargedValue);
					Gesture.Charging(cInfo);
				}
				
				if(!longTap && !posShifted && Time.time-tapStartTime>1f){
					longTap=true;
					Gesture.LongTap(startPos);
				}
				
				lastTouchPos=Input.mousePosition;
			}
			
			if(Input.GetMouseButtonUp(0)){
				if(Time.time-tapStartTime<shortTapTime){
						
					if(Time.time-lastShortTapTime<doubleTapTime){
						if(dTapState==_DTapState.Clear){
							dTapState=_DTapState.Tap1;
						}
						else if(dTapState==_DTapState.Tap1){
							if(Vector2.Distance(lastTouchPos, lastShortTapPos)<10){
				
								dTapState=_DTapState.Clear;
								
								Gesture.DoubleTap((startPos+lastShortTapPos)/2);
								
							}
						}
					}
					else{
						dTapState=_DTapState.Tap1;
					}
					
					lastShortTapTime=Time.time;
					lastShortTapPos=Input.mousePosition;
					Gesture.ShortTap(startPos);
					
				}
				
				if(chargeState==_ChargeState.Charged){
					ChargedInfo cInfo=new ChargedInfo(Input.mousePosition, chargedValue);
					Gesture.ChargeEnd(cInfo);
					
					chargedValue=0;
					chargeState=_ChargeState.Clear;
				}
			}
		#endif
	}
}
