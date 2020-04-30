using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonPresionado : MonoBehaviour
{
    public float interval = 0.25f;

    public bool mIsPRessed = false;
    public float mNextClick = 0;
    private CharController motor;

    void Awake(){
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>();
    }

    void OnPress(bool isPressed){
        mIsPRessed = isPressed;
        mNextClick = Time.realtimeSinceStartup + interval;
    }

    void Update(){
        motor.inputJump = false;
        if(Input.GetKeyDown(KeyCode.Space)){
            mIsPRessed = true;
            mNextClick = Time.realtimeSinceStartup + interval;
            print("jump");
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            mIsPRessed = false;
            print("jump2");
        }

        if(mIsPRessed && Time.realtimeSinceStartup < mNextClick){
            mNextClick = Time.realtimeSinceStartup + interval;
            motor.inputJump = true;
        }
    }
}
