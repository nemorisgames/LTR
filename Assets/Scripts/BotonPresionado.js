#pragma strict

var interval:float = 0.25f;

var mIsPressed:boolean = false;
var mNextClick:float = 0f;
private var motor : CharacterMotorNemoris;

// Use this for initialization
function Awake () {
	motor = GameObject.FindWithTag("Player").GetComponent(CharacterMotorNemoris);
}

function OnPress (isPressed:boolean) { 
	mIsPressed = isPressed; 
	mNextClick = Time.realtimeSinceStartup + interval; 
}
    
function Update (){
	motor.inputJump = false;
	if(Input.GetKeyDown(KeyCode.Space)){
		mIsPressed = true; 
		mNextClick = Time.realtimeSinceStartup + interval; 
		print("jump");
	}
	if(Input.GetKeyUp(KeyCode.Space)){
		mIsPressed = false; 
		print("jump2");
	}
	
	if (mIsPressed && Time.realtimeSinceStartup < mNextClick)
	{
	    mNextClick = Time.realtimeSinceStartup + interval;

	    // Do what you need to do, or simply:
	    //SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
	    motor.inputJump = true;
	}
	
}

function Start () {

}