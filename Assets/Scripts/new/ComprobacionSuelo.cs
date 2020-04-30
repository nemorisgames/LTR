using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComprobacionSuelo : MonoBehaviour
{
    public GameObject ultimoBloque = null;

    void OnTriggerStay(Collider c){
        if(c.CompareTag("Bloque"))
            ultimoBloque = c.gameObject;
    }
}
