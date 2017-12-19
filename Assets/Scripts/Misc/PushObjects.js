// Script added to a player for it to be able to push rigidbodies around.
#pragma strict
#pragma implicit
#pragma downcast

// How hard the player can push
var LightpushPower = 0.5;
var HeavypushPower = 0.1;

var polvo:Transform;

// Which layers the player can push
// This is useful to make unpushable rigidbodies
var pushLayers : LayerMask = -1;
var ciclo:int = 0;

// pointer to the player so we can get values from it quickly
private var controller : CharacterController;

var sonidoPush:AudioClip;
     
function Start () {
    controller = GetComponent (CharacterController);
     
}
     
function OnControllerColliderHit (hit : ControllerColliderHit) {
	//print(hit.gameObject.tag);
    if(hit.gameObject.tag == "Bloque") {
	    var body : Rigidbody = hit.collider.attachedRigidbody;
	    // no rigidbody
	    if (body == null || body.isKinematic)
	    return;
	     
	    // Only push rigidbodies in the right layers
	    var bodyLayerMask = 1 << body.gameObject.layer;
	    if ((bodyLayerMask & pushLayers.value) == 0)
	    return;
	     
	    // We dont want to push objects below us
	    if (hit.moveDirection.y < -0.3)
	    return;
	     
	    // Calculate push direction from move direction, we only push objects to the sides
	    // never up and down
	    
	    //var pushDir:Vector3;
	    //if(
	    var pushDir = Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
	    if(Mathf.Abs(pushDir.x) > Mathf.Abs(pushDir.z)) pushDir.z = 0;
	    else pushDir.x = 0;
	    body.AddForce(pushDir * LightpushPower * 2f);
	    print(body.velocity.magnitude);
	    body.velocity = Vector3.ClampMagnitude(body.velocity, 1.2f);
	    if(ciclo % 20 == 0 || ciclo == 0){
	    	Instantiate(polvo, body.transform.position, polvo.rotation);
	    }
	    if(ciclo % 40 == 0 || ciclo == 0){
	    	GetComponent.<AudioSource>().PlayOneShot(sonidoPush);
	    }
	    ciclo ++;
    }
}