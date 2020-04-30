using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{

}

[System.Serializable]
public class CharacterMotorMovement{
    public float maxForwardSpeed = 10f;
    public float maxSidewaysSpeed = 10f;
    public float maxBackwardsSpeed = 10f;

    public AnimationCurve slopeSpeedMultiplier;
    
    public float maxGroundAcceleration = 30f;
    public float maxAirAcceleration = 20f;

    public float gravity = 10f;
    public float maxFallSpeed = 20f;
    [HideInInspector]
    public CollisionFlags collisionFlags;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 frameVelocity = Vector3.zero;
    [HideInInspector]
    public Vector3 hitPoint = Vector3.zero;
    [HideInInspector]
    public Vector3 lastHitPoint = new Vector3(Mathf.Infinity,0,0);
}

public enum MovementTransferOnJump{
    None,
    InitTransfer,
    PermaTransfer,
    PermaLocked
}

[System.Serializable]
public class CharacterMotorJumping{
    public bool enabled = true;
    public float baseHeight = 1f;
    public float extraHeight = 4.1f;
    public float perpAmount = 0;
    public float steepPerpAmount = 0.5f;
    [HideInInspector]
    public bool jumping = false;
    [HideInInspector]
    public bool holdingJumpButton = false;
    [HideInInspector]
    public float lastStartTime = 0;
    [HideInInspector]
    public float lastButtonDownTime = -100;
    [HideInInspector]
    public Vector3 jumpDir = Vector3.up;
}

[System.Serializable]
public class CharacterMotorMovingPlatform{
    public bool enabled = true;
    public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;
    [HideInInspector]
    public Transform hitPlatform;
    [HideInInspector]
    public Transform activePlatform;
    [HideInInspector]
    public Vector3 activeLocalPoint;
    [HideInInspector]
    public Vector3 activeGlobalPoint;
    [HideInInspector]
    public Quaternion activeLocalRotation;
    [HideInInspector]
    public Quaternion activeGlobalRotation;
    [HideInInspector]
    public Matrix4x4 lastMatrix;
    [HideInInspector]
    public Vector3 platformVelocity;
    [HideInInspector]
    public bool newPlatform;
}

[System.Serializable]
public class CharacterMotorSliding{
    public bool enabled = true;
    public float slidingSpeed = 15;
    public float sidewaysControl = 1;
    public float speedControl = 1;
}