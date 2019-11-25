using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float delayTime = 0f;
    public Transform localPosition;
    public Vector3 offsetFromSelf = Vector3.zero;
    public Vector3 worldPosition = Vector3.zero;

    void Start()
    {
        StartCoroutine(Timer(delayTime));
    }

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(localPosition != null){
            Instantiate(objectToSpawn, localPosition.position + offsetFromSelf, Quaternion.identity);
        }else{
            Instantiate(objectToSpawn, worldPosition + offsetFromSelf, Quaternion.identity);
        }
    }
}
