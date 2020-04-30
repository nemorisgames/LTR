using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObjects : MonoBehaviour
{
    public float LightpushPower = 0.5f;
    public float HeavypushPower = 0.1f;

    public Transform polvo;

    public LayerMask pushLayers = -1;

    public int ciclo = 0;

    private CharacterController controller;
    public AudioClip sonidoPush;
    private AudioSource audioSource;

    void Awake(){
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
        if(hit.gameObject.tag == "Bloque") {
            Rigidbody body = hit.collider.attachedRigidbody;
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
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            if(Mathf.Abs(pushDir.x) > Mathf.Abs(pushDir.z)) pushDir.z = 0;
            else pushDir.x = 0;
            body.AddForce(pushDir * LightpushPower * 2f);
            print(body.velocity.magnitude);
            body.velocity = Vector3.ClampMagnitude(body.velocity, 1.2f);
            if(ciclo % 20 == 0 || ciclo == 0){
                Instantiate(polvo, body.transform.position, polvo.rotation);
            }
            if(ciclo % 40 == 0 || ciclo == 0){
                audioSource.PlayOneShot(sonidoPush);
            }
            ciclo ++;
        }
    }
}
