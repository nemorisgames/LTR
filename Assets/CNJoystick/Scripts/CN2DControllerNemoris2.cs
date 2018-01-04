using UnityEngine;
using System.Collections;

public class CN2DControllerNemoris2 : MonoBehaviour
{
    public CNJoystick movementJoystick;

    private Transform transformCache;

	public float velocidadMaxima = 10f;

	public Vector3 velocidad;

	public bool utilizandose = false;

	Transform escenaRoot;

    // Use this for initialization
    void Awake()
    {
		escenaRoot = GameObject.Find ("escenaRoot").transform;
        if (movementJoystick == null)
        {
            throw new UnassignedReferenceException("Please specify movement Joystick object");
        }
        movementJoystick.FingerTouchedEvent += StartMoving;
        movementJoystick.FingerLiftedEvent += StopMoving;
        movementJoystick.JoystickMovedEvent += Move;

        transformCache = transform;
    }

    // You can extend this class and override any of these virtual methods
    protected virtual void Move(Vector3 relativeVector)
    {
		if(relativeVector != Vector3.zero) relativeVector = Vector3.ClampMagnitude(relativeVector, 1f);
        // It's actually 2D vector
        //transformCache.position = transformCache.position + relativeVector;
        //FaceMovementDirection(relativeVector);
		//gameObject.SendMessage ("SetVelocity", velocidadMaxima * new Vector3(relativeVector.x, 0f, relativeVector.y));
		//print(Camera.main.transform.forward + " " + relativeVector);
		//Version 1
		transform.LookAt (transform.position + new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z) * relativeVector.x + new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z) * relativeVector.y);
		velocidad = velocidadMaxima * (new Vector3 (Camera.main.transform.right.x, 0f, Camera.main.transform.right.z) * relativeVector.x + new Vector3 (Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z) * relativeVector.y);
		//Version 2
		//transform.LookAt (transform.position + new Vector3(relativeVector.x, 0f, relativeVector.y));
		//transform.LookAt (transform.position + new Vector3(escenaRoot.right.x, 0f, escenaRoot.right.z) * relativeVector.x + new Vector3(escenaRoot.forward.x, 0f, escenaRoot.forward.z) * relativeVector.y);
		//velocidad = velocidadMaxima * (new Vector3 (escenaRoot.right.x, 0f, escenaRoot.right.z) * relativeVector.x + new Vector3 (escenaRoot.forward.x, 0f, escenaRoot.forward.z) * relativeVector.y);
    }

	void Update(){
		if (Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0f)
			Move (new Vector3(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0f));
		utilizandose = velocidad.magnitude > 0.05f;
        
		//print (velocidad);
		gameObject.SendMessage ("SetVelocity", velocidad);
		velocidad = Vector3.Lerp (velocidad, new Vector3(0f, 0f, 0f), 30f * Time.deltaTime);
		//if(velocidad.magnitude > 0f) velocidad = Vector3.ClampMagnitude(velocidad, Mathf.Clamp(velocidad.magnitude - 1000f * Time.deltaTime, 0f, velocidadMaxima));
		//if(Input.GetButton("Jump")) gameObject.SendMessage ("Saltar");
	}

    private void FaceMovementDirection(Vector3 direction)
    {
        /*if (direction.sqrMagnitude > 0.1)
        {
            transform.up = direction;
        }*/
    }

    protected virtual void StopMoving()
    {
    }

    protected virtual void StartMoving()
    {
    }

}
