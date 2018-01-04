#pragma strict
#pragma implicit
#pragma downcast

var sombrero:Transform;
var sombreroPrefab:Transform;

private var muerto:boolean = false;
private var salirTemplo:boolean = false;
private var terminado:boolean = false;
var disparo : Transform;
// Does this script currently respond to input?
var canControl : boolean = true;

var useFixedUpdate : boolean = true;

var animacion : Animator;

// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
// Very handy for organization!

// The current global direction we want the character to move in.
@System.NonSerialized
var inputMoveDirection : Vector3 = Vector3.zero;

// Is the jump button held down? We use this interface instead of checking
// for the jump button directly so this script can also be used by AIs.
@System.NonSerialized
var inputJump : boolean = false;

var movement : CharacterMotorMovement = CharacterMotorMovement();


var jumping : CharacterMotorJumping = CharacterMotorJumping();

var movingPlatform : CharacterMotorMovingPlatform = CharacterMotorMovingPlatform();

var sliding : CharacterMotorSliding = CharacterMotorSliding();

@System.NonSerialized
var grounded : boolean = true;

@System.NonSerialized
var groundNormal : Vector3 = Vector3.zero;

private var lastGroundNormal : Vector3 = Vector3.zero;

private var tr : Transform;

var posicionSegura : Vector3;

private var controller : CharacterController;
var sonidoSalto:AudioClip;
var sonidoMuerte:AudioClip;
var proteccionEscudo:GameObject;

function Awake () {
	proteccionEscudo = transform.Find("proteccion").gameObject;
	proteccionEscudo.SetActive(false);
	controller = GetComponent (CharacterController);
	tr = transform;
}

private function UpdateFunction () {
	if(muerto){
		//transform.position += saltoMuerte * Time.deltaTime;
		//saltoMuerte -= Vector3.up * 3f * Time.deltaTime; 
		return;
	}
	if(salirTemplo){
		transform.position += Vector3.up * 8f * Time.deltaTime;
		return;
	}
	if(terminado){ 
		movement.velocity = Vector3(0f, movement.velocity.y, 0f);
		//return;
	}
	// We copy the actual velocity into a temporary variable that we can manipulate.
	var velocity : Vector3 = movement.velocity;
	
	// Update velocity based on input
	velocity = ApplyInputVelocityChange(velocity);
	
	// Apply gravity and jumping force
	velocity = ApplyGravityAndJumping (velocity);
	
	// Moving platform support
	var moveDistance : Vector3 = Vector3.zero;
	if (MoveWithPlatform()) {
		var newGlobalPoint : Vector3 = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
		moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
		if (moveDistance != Vector3.zero && IsGroundedTest())
			controller.Move(moveDistance);
		
		// Support moving platform rotation as well:
        var newGlobalRotation : Quaternion = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
        var rotationDiff : Quaternion = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
        
        var yRotation = rotationDiff.eulerAngles.y;
        if (yRotation != 0) {
	        // Prevent rotation of the local up vector
	        tr.Rotate(0, yRotation, 0);
        }
	}
	
	// Save lastPosition for velocity calculation.
	var lastPosition : Vector3 = tr.position;
	
	// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
	var currentMovementOffset : Vector3 = velocity * Time.deltaTime;
	
	// Find out how much we need to push towards the ground to avoid loosing grouning
	// when walking down a step or over a sharp change in slope.
	var pushDownOffset : float = Mathf.Max(controller.stepOffset, Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
	if (grounded)
		currentMovementOffset -= pushDownOffset * Vector3.up;
	
	// Reset variables that will be set by collision function
	movingPlatform.hitPlatform = null;
	groundNormal = Vector3.zero;
	
   	// Move our character!
	movement.collisionFlags = controller.Move (currentMovementOffset);
	
	movement.lastHitPoint = movement.hitPoint;
	lastGroundNormal = groundNormal;
	
	if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform) {
		if (movingPlatform.hitPlatform != null) {
			movingPlatform.activePlatform = movingPlatform.hitPlatform;
			movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
			movingPlatform.newPlatform = true;
		}
	}
	
	// Calculate the velocity based on the current and previous position.  
	// This means our velocity will only be the amount the character actually moved as a result of collisions.
	var oldHVelocity : Vector3 = new Vector3(velocity.x, 0, velocity.z);
	movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
	var newHVelocity : Vector3 = new Vector3(movement.velocity.x, 0, movement.velocity.z);
	
	// The CharacterController can be moved in unwanted directions when colliding with things.
	// We want to prevent this from influencing the recorded velocity.
	if (oldHVelocity == Vector3.zero) {
		movement.velocity = new Vector3(0, movement.velocity.y, 0);
	}
	else {
		var projectedNewVelocity : float = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
		movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
	}
	
	if (movement.velocity.y < velocity.y - 0.001) {
		if (movement.velocity.y < 0) {
			// Something is forcing the CharacterController down faster than it should.
			// Ignore this
			movement.velocity.y = velocity.y;
		}
		else {
			// The upwards movement of the CharacterController has been blocked.
			// This is treated like a ceiling collision - stop further jumping here.
			jumping.holdingJumpButton = false;
		}
	}
	
	// We were grounded but just loosed grounding
	if (grounded && !IsGroundedTest()) {
		grounded = false;
		// Apply inertia from platform
		if (movingPlatform.enabled &&
			(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
			movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		) {
			movement.frameVelocity = movingPlatform.platformVelocity;
			movement.velocity += movingPlatform.platformVelocity;
		}
		
		SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
		// We pushed the character down to ensure it would stay on the ground if there was any.
		// But there wasn't so now we cancel the downwards offset to make the fall smoother.
		tr.position += pushDownOffset * Vector3.up;
	}
	// We were not grounded but just landed on something
	else if (!grounded && IsGroundedTest()) {
		grounded = true;
		jumping.jumping = false;
		SubtractNewPlatformVelocity();
		//print("landed");
		SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
	}
	
	// Moving platforms support
	if (MoveWithPlatform()) {
		// Use the center of the lower half sphere of the capsule as reference point.
		// This works best when the character is standing on moving tilting platforms. 
		movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height*0.5 + controller.radius);
		movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
		
		// Support moving platform rotation as well:
        movingPlatform.activeGlobalRotation = tr.rotation;
        movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
	}
}

function FixedUpdate () {
	//if(muerto) return;
	if (movingPlatform.enabled) {

		if (movingPlatform.activePlatform != null) {
			if (!movingPlatform.newPlatform) {
				var lastVelocity : Vector3 = movingPlatform.platformVelocity;
				
				movingPlatform.platformVelocity = (
					movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
					- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
				) / Time.deltaTime;
			}
			movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
			movingPlatform.newPlatform = false;
		}
		else {
			movingPlatform.platformVelocity = Vector3.zero;	
		}
	}
	
	if (useFixedUpdate)
		UpdateFunction();
}

var saltoMuerte:Vector3 = new Vector3(0f, 5f, 0f);

function Update () {
	if(protegido){
		if(tiempoProteccion <= Time.timeSinceLevelLoad){ 
			proteccionEscudo.SetActive(false);
		 	protegido = false;
		}
	}
	if (!useFixedUpdate)
		UpdateFunction();
	if(!muerto && transform.position.y <= 0f){ 
		proteccionEscudo.SetActive(false);
		protegido = false;
		OnDeath();
	}
	if(!muerto && canControl){
		animacion.SetBool("grounded", IsGrounded());
		if(IsGrounded()){
			animacion.SetBool("falling", false);
			animacion.SetBool("jumping", false);
			animacion.SetFloat("velocidad", GetDirection().magnitude);
			posicionSegura = transform.position;
		}
	}
}

private function ApplyInputVelocityChange (velocity : Vector3) {	
	if (!canControl)
		inputMoveDirection = Vector3.zero;
	// Find desired velocity
	var desiredVelocity : Vector3;
	if (grounded && TooSteep()) {
		// The direction we're sliding in
		desiredVelocity = Vector3(groundNormal.x, 0, groundNormal.z).normalized;
		// Find the input movement direction projected onto the sliding direction
		var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
		// Add the sliding direction, the spped control, and the sideways control vectors
		desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
		// Multiply with the sliding speed
		desiredVelocity *= sliding.slidingSpeed;
	}
	else
		desiredVelocity = GetDesiredHorizontalVelocity();
	
	if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer) {
		desiredVelocity += movement.frameVelocity;
		desiredVelocity.y = 0;
	}
	
	if (grounded)
		desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
	else
		velocity.y = 0;
	
	// Enforce max velocity change
	var maxVelocityChange : float = GetMaxAcceleration(grounded) * Time.deltaTime;
	var velocityChangeVector : Vector3 = (desiredVelocity - velocity);
	if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
		velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
	}
	// If we're in the air and don't have control, don't apply any velocity change at all.
	// If we're on the ground and don't have control we do apply it - it will correspond to friction.
	if (grounded || canControl)
		velocity += velocityChangeVector * 2f;
	
	if (grounded) {
		// When going uphill, the CharacterController will automatically move up by the needed amount.
		// Not moving it upwards manually prevent risk of lifting off from the ground.
		// When going downhill, DO move down manually, as gravity is not enough on steep hills.
		velocity.y = Mathf.Min(velocity.y, 0);
	}
	
	return velocity;
}

private function ApplyGravityAndJumping (velocity : Vector3) {
	
	if (!inputJump || !canControl) {
		jumping.holdingJumpButton = false;
		jumping.lastButtonDownTime = -100;
	}
	
	if (inputJump && jumping.lastButtonDownTime < 0 && canControl)
		jumping.lastButtonDownTime = Time.time;
	
	if (grounded)
		velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
	else {
		velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
		
		// When jumping up we don't apply gravity for some time when the user is holding the jump button.
		// This gives more control over jump height by pressing the button longer.
		if (jumping.jumping && jumping.holdingJumpButton) {
			// Calculate the duration that the extra jump force should have effect.
			// If we're still less than that duration after the jumping time, apply the force.
			if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
				// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
			}
		}
		
		// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
		velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
	}
		
	if (grounded) {
		// Jump only if the jump button was pressed down in the last 0.2 seconds.
		// We use this check instead of checking if it's pressed down right now
		// because players will often try to jump in the exact moment when hitting the ground after a jump
		// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
		// it's confusing and it feels like the game is buggy.
		if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.1)) {
			grounded = false;
			jumping.jumping = true;
			jumping.lastStartTime = Time.time;
			jumping.lastButtonDownTime = -100;
			jumping.holdingJumpButton = true;
			
			// Calculate the jumping directionApplyInputVelocityChange
			if (TooSteep())
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
			else
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
			
			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
			velocity.y = 0;
			velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
			
			// Apply inertia from platform
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
				movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
			) {
				movement.frameVelocity = movingPlatform.platformVelocity;
				velocity += movingPlatform.platformVelocity;
			}
			
			SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
		}
		else {
			jumping.holdingJumpButton = false;
		}
	}
	
	return velocity;
}

function OnControllerColliderHit (hit : ControllerColliderHit) {
	if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
		if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
			groundNormal = hit.normal;
		else
			groundNormal = lastGroundNormal;
		
		movingPlatform.hitPlatform = hit.collider.transform;
		movement.hitPoint = hit.point;
		movement.frameVelocity = Vector3.zero;
	}
}

private function SubtractNewPlatformVelocity () {
	// When landing, subtract the velocity of the new ground from the character's velocity
	// since movement in ground is relative to the movement of the ground.
	if (movingPlatform.enabled &&
		(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
		movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
	) {
		// If we landed on a new platform, we have to wait for two FixedUpdates
		// before we know the velocity of the platform under the character
		if (movingPlatform.newPlatform) {
			var platform : Transform = movingPlatform.activePlatform;
			yield WaitForFixedUpdate();
			yield WaitForFixedUpdate();
			if (grounded && platform == movingPlatform.activePlatform)
				yield 1;
		}
		movement.velocity -= movingPlatform.platformVelocity;
		movement.velocity -= movement.velocity/3;
	}
}

private function MoveWithPlatform () : boolean {
	return (
		movingPlatform.enabled
		&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
		&& movingPlatform.activePlatform != null
	);
}

private function GetDesiredHorizontalVelocity () {
	// Find desired velocity
	var desiredLocalDirection : Vector3 = tr.InverseTransformDirection(inputMoveDirection);
	var maxSpeed : float = MaxSpeedInDirection(desiredLocalDirection);
	if (grounded) {
		// Modify max speed on slopes based on slope speed multiplier curve
		var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
		maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
	}
	return tr.TransformDirection(desiredLocalDirection * maxSpeed);
}

private function AdjustGroundVelocityToNormal (hVelocity : Vector3, groundNormal : Vector3) : Vector3 {
	var sideways : Vector3 = Vector3.Cross(Vector3.up, hVelocity);
	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
}

private function IsGroundedTest () {
	return (groundNormal.y > 0.01);
}

function GetMaxAcceleration (grounded : boolean) : float {
	// Maximum acceleration on ground and in air
	if (grounded)
		return movement.maxGroundAcceleration;
	else
		return movement.maxAirAcceleration;
}

function CalculateJumpVerticalSpeed (targetJumpHeight : float) {
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
}

function IsJumping () {
	return jumping.jumping;
}

function IsSliding () {
	return (grounded && sliding.enabled && TooSteep());
}

function IsTouchingCeiling () {
	return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
}

function IsGrounded () {
	return grounded;
}

function TooSteep () {
	return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
}

function GetDirection () {
	return inputMoveDirection;
}

function SetControllable (controllable : boolean) {
	canControl = controllable;
}

// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
// The function returns the length of the resulting vector.
function MaxSpeedInDirection (desiredMovementDirection : Vector3) : float {
	if (desiredMovementDirection == Vector3.zero)
		return 0;
	else {
		var zAxisEllipseMultiplier : float = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
		var temp : Vector3 = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
		var length : float = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
		return length;
	}
}

var activo:boolean = true;

function activar(a:boolean){
	activo = a; 
	canControl = activo;
}

var protegido:boolean = false;
var tiempoProteccion:float = 0f;
function proteccion(){
	tiempoProteccion = Time.timeSinceLevelLoad + 3.0;
	protegido = true;
	proteccionEscudo.SetActive(true);
}

function reset(){
	grounded = false;
	muerto = false;
	terminado = false;
	GetComponent.<Collider>().enabled = true;
	salirTemplo = false;
	animacion.SetTrigger("reset");
	transform.rotation = Quaternion.identity;
	sombrero.gameObject.SetActive(true);
	saltoMuerte = new Vector3(0f, 5f, 0f);
	movement.velocity = Vector3.zero;
	inputMoveDirection = Vector3.zero;
	movement.frameVelocity = Vector3.zero;
	var obj:GameObject = transform.Find("comprobacion").gameObject;
	var comp:ComprobacionSuelo = obj.GetComponent(ComprobacionSuelo);
	var ultimoBloque:GameObject = comp.ultimoBloque;
	print(ultimoBloque.name);
	if(ultimoBloque.name.Contains("Plataforma")){
		print("encontrado");
		transform.position = ultimoBloque.transform.position + Vector3.up * 0.55;
	}
	else transform.position = posicionSegura;
}

function SetVelocity (velocity : Vector3) {
	if(!activo) return;
	if(!inputJump){
		grounded = false;
		inputMoveDirection = velocity;
		//movement.velocity = velocity;
		movement.frameVelocity = Vector3.zero;
		SendMessage("OnExternalVelocity", SendMessageOptions.DontRequireReceiver);
	}
}

function OnRobar(){
	activar(false);
	terminado = true;
	//animacion.Play("Robar");
	animacion.SetTrigger("victoria");
	inputMoveDirection = Vector3.zero;
	movement.frameVelocity = Vector3.zero;
	yield WaitForSeconds(1.1);
	Instantiate(disparo, transform.position + Vector3.up * 0.8 + transform.right * 0.25 + transform.forward * 0.1, disparo.rotation);
	yield WaitForSeconds(0.7);
	GetComponent.<Collider>().enabled = false;
	salirTemplo = true;
}

function OnDeath(){
	if(protegido) return;
	if(terminado) return;
	GetComponent.<AudioSource>().PlayOneShot(sonidoMuerte);
	muerto = true;
	//var g:Transform = Instantiate(sombreroPrefab, sombrero.transform.position, transform.rotation);
	//g.gameObject.GetComponent.<Rigidbody>().AddForce(Random.Range(-100f, 100f), 500f + Random.Range(-50f, 50f), Random.Range(-100f, 100f));
	//g.gameObject.GetComponent.<Rigidbody>().AddTorque(0f, Random.Range(300f, 800f), 0f);
	sombrero.gameObject.SetActive(false);
	//animacion.Play("Caer");
	GameObject.Find("_central").SendMessage("perdiste");
	//animacion.SetBool("lose", true);
	animacion.SetTrigger("muerte");
	print("death");
	GetComponent.<Collider>().enabled = false;
}

function OnJump(){
	GetComponent.<AudioSource>().PlayOneShot(sonidoSalto);
	print("saltando");
	animacion.SetBool("jumping", true);
	animacion.SetBool("grounded", false);
	//animacion.Blend("Saltar");
}
function OnFall(){
	print("cayendo");
	animacion.SetBool("falling", true);
	//animacion.SetBool("jumping", true);
}

function Saltar(){
	if(!activo) return;
	print("saltar");
	inputJump = true;
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterController)
@script AddComponentMenu ("Character/Character Motor")
