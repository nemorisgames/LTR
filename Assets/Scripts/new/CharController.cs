using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public Transform sombrero;
    public GameObject sombreroPrefab;
    private bool muerto = false;
    private bool salirTemplo = false;
    private bool terminado = false;
    public GameObject disparo;
    public bool canControl = true;
    public bool useFixedUpdate = true;
    public Animator animacion;
    public Vector3 inputMoveDirection = Vector3.zero;
    public bool inputJump = false;
    public CharacterMotorMovement movement = new CharacterMotorMovement();
    public CharacterMotorJumping jumping = new CharacterMotorJumping();
    public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
    public CharacterMotorSliding sliding = new CharacterMotorSliding();
    [HideInInspector]
    public bool grounded = true;
    public Vector3 groundNormal = Vector3.zero;
    private Vector3 lastGroundNormal;
    public Vector3 posicionSegura;
    private CharacterController controller;
    public AudioClip sonidoSalto, sonidoMuerte;
    public MeshRenderer proteccionEscudo;
    private Vector3 saltoMuerte = Vector3.up * 5f;
    public bool activo = true;
    public bool protegido = false;
    public float tiempoProteccion = 0f;
    private Collider col;
    private AudioSource audioSource;

    void Awake(){
        controller = GetComponent<CharacterController>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void UpdateFunction(){
        if(muerto){
            transform.position += saltoMuerte * Time.deltaTime;
            saltoMuerte -= Vector3.up * 3f * Time.deltaTime; 
            return;
        }
        if(salirTemplo){
            transform.position += Vector3.up * 8f * Time.deltaTime;
            return;
        }
        if(terminado){ 
            movement.velocity = Vector3.up * movement.velocity.y;
        }
        // We copy the actual velocity into a temporary variable that we can manipulate.
        Vector3 velocity = movement.velocity;
        
        // Update velocity based on input
        velocity = ApplyInputVelocityChange(velocity);
        
        // Apply gravity and jumping force
        velocity = ApplyGravityAndJumping (velocity);
        
        // Moving platform support
        Vector3 moveDistance = Vector3.zero;
        if (MoveWithPlatform()) {
            Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
            moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
            if (moveDistance != Vector3.zero && IsGroundedTest())
                controller.Move(moveDistance);
            
            // Support moving platform rotation as well:
            Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
            Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
            
            var yRotation = rotationDiff.eulerAngles.y;
            if (yRotation != 0) {
                // Prevent rotation of the local up vector
                transform.Rotate(0, yRotation, 0);
            }
        }

        // Save lastPosition for velocity calculation.
        Vector3 lastPosition = transform.position;
        
        // We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
        Vector3 currentMovementOffset = velocity * Time.deltaTime;
        
        // Find out how much we need to push towards the ground to avoid loosing grouning
        // when walking down a step or over a sharp change in slope.
        float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
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
        Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
        movement.velocity = (transform.position - lastPosition) / Time.deltaTime;
        Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);
        
        // The CharacterController can be moved in unwanted directions when colliding with things.
        // We want to prevent this from influencing the recorded velocity.
        if (oldHVelocity == Vector3.zero) {
            movement.velocity = new Vector3(0, movement.velocity.y, 0);
        }
        else {
            float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
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
            
            OnFall();
            // We pushed the character down to ensure it would stay on the ground if there was any.
            // But there wasn't so now we cancel the downwards offset to make the fall smoother.
            transform.position += pushDownOffset * Vector3.up;
        }
        // We were not grounded but just landed on something
        else if (!grounded && IsGroundedTest()) {
            grounded = true;
            jumping.jumping = false;
            StartCoroutine(SubtractNewPlatformVelocity());
            
            SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
        }
        
        // Moving platforms support
        if (MoveWithPlatform()) {
            // Use the center of the lower half sphere of the capsule as reference point.
            // This works best when the character is standing on moving tilting platforms. 
            movingPlatform.activeGlobalPoint = transform.position + Vector3.up * (controller.center.y - controller.height*0.5f + controller.radius);
            movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
            
            // Support moving platform rotation as well:
            movingPlatform.activeGlobalRotation = transform.rotation;
            movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
        }
    }

    void FixedUpdate(){
        if (movingPlatform.enabled) {
            if (movingPlatform.activePlatform != null) {
                if (!movingPlatform.newPlatform) {
                    Vector3 lastVelocity = movingPlatform.platformVelocity;
                    
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

    void Update(){
        if(protegido){
            if(tiempoProteccion <= Time.timeSinceLevelLoad){ 
                protegido = false;
            }
            //print("proteccion " + (((tiempoProteccion - Time.timeSinceLevelLoad) / 3.0) * 0.5f));
            proteccionEscudo.material.color = new Color(116f / 255f, 215f / 255f, 1f, ((tiempoProteccion - Time.timeSinceLevelLoad) / 3.0f) * 0.5f);
        }
        if (!useFixedUpdate)
            UpdateFunction();
        if(!muerto && transform.position.y <= 0f){ 
            //print("no proteccion muerte");
            proteccionEscudo.material.color = new Color(116f / 255f, 215f / 255f, 1f, 0f);
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

    private Vector3 ApplyInputVelocityChange(Vector3 velocity){
        if (!canControl)
            inputMoveDirection = Vector3.zero;
        // Find desired velocity
        Vector3 desiredVelocity;
        if (grounded && TooSteep()) {
            // The direction we're sliding in
            desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
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
        float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
        Vector3 velocityChangeVector = (desiredVelocity - velocity);
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

    private Vector3 ApplyGravityAndJumping(Vector3 velocity){
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
                
                OnJump();
            }
            else {
                jumping.holdingJumpButton = false;
            }
        }
        
        return velocity;
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
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

    private IEnumerator SubtractNewPlatformVelocity(){
        // When landing, subtract the velocity of the new ground from the character's velocity
        // since movement in ground is relative to the movement of the ground.
        if (movingPlatform.enabled &&
            (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
            movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
        ) {
            // If we landed on a new platform, we have to wait for two FixedUpdates
            // before we know the velocity of the platform under the character
            if (movingPlatform.newPlatform) {
                Transform platform = movingPlatform.activePlatform;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (grounded && platform == movingPlatform.activePlatform)
                    yield return 1;
            }
            movement.velocity -= movingPlatform.platformVelocity;
        }
    }

    private bool MoveWithPlatform(){
        return (
            movingPlatform.enabled
            && (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
            && movingPlatform.activePlatform != null
        );
    }

    private Vector3 GetDesiredHorizontalVelocity(){
        // Find desired velocity
        Vector3 desiredLocalDirection = transform.InverseTransformDirection(inputMoveDirection);
        float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
        if (grounded) {
            // Modify max speed on slopes based on slope speed multiplier curve
            var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
            maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
        }
        return transform.TransformDirection(desiredLocalDirection * maxSpeed);
    }

    private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal){
        Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
	    return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }

    private bool IsGroundedTest(){
        return (groundNormal.y > 0.01);
    }

    public float GetMaxAcceleration(bool grounded){
        if (grounded)
            return movement.maxGroundAcceleration;
        else
            return movement.maxAirAcceleration;
    }

    public float CalculateJumpVerticalSpeed(float targetJumpHeight){
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
    }

    public bool IsJumping(){
        return jumping.jumping;
    }

    public bool IsSliding(){
        return grounded && sliding.enabled && TooSteep();
    }

    public bool IsTouchingCeiling(){
        return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
    }

    public bool IsGrounded(){
        return grounded;
    }

    public bool TooSteep(){
        return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
    }

    public Vector3 GetDirection(){
        return inputMoveDirection;
    }

    public void SetControllable(bool controllable){
        canControl = controllable;
    }

    public float MaxSpeedInDirection(Vector3 desiredMovementDirection){
        if (desiredMovementDirection == Vector3.zero)
            return 0;
        else {
            float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
            Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
            float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
            return length;
        }
    }

    public void activar(bool a){
        activo = a; 
	    canControl = activo;
    }

    public void proteccion(){
        tiempoProteccion = Time.timeSinceLevelLoad + 3.0f;
        protegido = true;
        print("proteccion inicio");
        proteccionEscudo.material.color = new Color(116f / 255f, 215f / 255f, 1f, 0.5f);
    }

    public void reset(){
        grounded = false;
        muerto = false;
        terminado = false;
        col.enabled = true;
        salirTemplo = false;
        animacion.SetTrigger("reset");
        transform.rotation = Quaternion.identity;
        sombrero.gameObject.SetActive(true);
        saltoMuerte = new Vector3(0f, 5f, 0f);
        movement.velocity = Vector3.zero;
        inputMoveDirection = Vector3.zero;
        movement.frameVelocity = Vector3.zero;
        GameObject obj = transform.Find("comprobacion").gameObject;
        ComprobacionSuelo comp = obj.GetComponent<ComprobacionSuelo>();
        GameObject ultimoBloque = comp.ultimoBloque;
        print(ultimoBloque.name);
        if(ultimoBloque.name.Contains("Plataforma")){
            print("encontrado");
            transform.position = ultimoBloque.transform.position + Vector3.up * 0.55f;
        }
        else transform.position = posicionSegura;
    }

    public void SetVelocity(Vector3 velocity){
        if(!activo) return;
        if(!inputJump){
            grounded = false;
            inputMoveDirection = velocity;
            //movement.velocity = velocity;
            movement.frameVelocity = Vector3.zero;
            SendMessage("OnExternalVelocity", SendMessageOptions.DontRequireReceiver);
        }
    }

    public IEnumerator OnRobar(){
        activar(false);
        terminado = true;
        //animacion.Play("Robar");
        animacion.SetTrigger("victoria");
        inputMoveDirection = Vector3.zero;
        movement.frameVelocity = Vector3.zero;
        yield return new WaitForSeconds(1.1f);
        Instantiate(disparo, transform.position + Vector3.up * 0.8f + transform.right * 0.25f + transform.forward * 0.1f, disparo.transform.rotation);
        yield return new WaitForSeconds(0.7f);
        col.enabled = false;
        salirTemplo = true;
    }

    public void OnDeath(){
        if(protegido) return;
        if(terminado) return;
        audioSource.PlayOneShot(sonidoMuerte);
        muerto = true;
        GameObject g = Instantiate(sombreroPrefab, sombrero.transform.position, transform.rotation);
        Rigidbody rb = g.GetComponent<Rigidbody>();
        rb.AddForce(Random.Range(-100f, 100f), 500f + Random.Range(-50f, 50f), Random.Range(-100f, 100f));
        rb.AddTorque(0f, Random.Range(300f, 800f), 0f);
        sombrero.gameObject.SetActive(false);
        //animacion.Play("Caer");
        Central.Instance.perdiste();
        //animacion.SetBool("lose", true);
        animacion.SetTrigger("muerte");
        print("death");
        col.enabled = false;
    }

    public void OnJump(){
        audioSource.PlayOneShot(sonidoSalto);
        print("saltando");
        animacion.SetBool("jumping", true);
        animacion.SetBool("grounded", false);
    }

    public void OnFall(){
        print("cayendo");
	    animacion.SetBool("falling", true);
    }

    public void Saltar(){
        if(!activo) return;
        print("saltar");
        inputJump = true;
    }
}
