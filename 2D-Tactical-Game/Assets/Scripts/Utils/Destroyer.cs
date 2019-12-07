using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float timeToAutoDestroy = 1f;
    
    void Start()
    {
        Destroy(gameObject, timeToAutoDestroy);
    }
}
