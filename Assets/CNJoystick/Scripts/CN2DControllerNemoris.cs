using UnityEngine;
using System.Collections;

public class CN2DControllerNemoris : MonoBehaviour
{
    public CNJoystick movementJoystick;

    private Transform transformCache;

	public bool enSuelo = false;
	public bool saltando = false;
	public bool bajando = false;

	Central centralScript;

	public bool activado = false;

    // Use this for initialization
    void Awake()
    {
		centralScript = GameObject.Find ("_central").GetComponent<Central> ();
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
        // It's actually 2D vector
        //transformCache.position = transformCache.position + relativeVector;
        //FaceMovementDirection(relativeVector);
		GetComponent<Rigidbody>().AddForce (transform.forward * 400f * relativeVector.y * Time.deltaTime, ForceMode.Force);
		GetComponent<Rigidbody>().AddTorque (Vector3.up * Time.deltaTime * relativeVector.x * 20f);
    }

    private void FaceMovementDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.1)
        {
            transform.up = direction;
        }
    }
	
    protected virtual void StopMoving()
    {
        
    }

    protected virtual void StartMoving()
    {

    }

	public void activar(bool a){
		activado = a;
	}
	
	public void salto(){
		if (enSuelo && !saltando) { 
			GetComponent<Rigidbody>().AddForce (Vector3.up * 5f, ForceMode.Impulse);
			saltando = true;
			bajando = false;
		}
	}

	void Update(){
		if (!activado)
			return;
		bajando = GetComponent<Rigidbody>().velocity.y < -0.05f;
		//print (rigidbody.velocity.y);
		RaycastHit hit;
		Ray downRay = new Ray(transform.position, -Vector3.up);
		if (Physics.Raycast(downRay, out hit)) {
			enSuelo = (hit.distance < 0.131);
			if(hit.collider.gameObject.name.Contains("BloqueFinal")){
			centralScript.estado = 3;
			}
		}
		if(Input.GetAxisRaw("Vertical") != 0f) GetComponent<Rigidbody>().AddForce (transform.forward * 400f * Input.GetAxisRaw("Vertical") * Time.deltaTime, ForceMode.Force);
		if(Input.GetAxisRaw("Horizontal") != 0f)GetComponent<Rigidbody>().AddTorque (Vector3.up * Time.deltaTime * Input.GetAxisRaw("Horizontal") * 20f);
		if (Input.GetButtonUp ("Jump") && enSuelo) salto ();

		if (saltando && enSuelo && bajando)
			saltando = false; 
			
	}

}
