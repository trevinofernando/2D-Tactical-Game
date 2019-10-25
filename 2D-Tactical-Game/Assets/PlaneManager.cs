using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    public float speed = -15;
    public float dropArea = 5f;
    public int numItemsToDrop = 1;
    public bool randomizeDrops = false;
    public GameObject[] prefabCargo;

    //[System.NonSerialized]
    public Vector2[] dropPoints;

    public Vector2 target;
    private GameObject go;
    private Rigidbody2D rb;
    private int nextPrefabIndex;

    
    void Start()
    {
        //Can't allow static planes
        if(speed == 0){
            speed = -5;
        }

        //Sprite is always facing left, so if speed is positive, then flip image
        if(speed > 0){
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //Set Planes velocity
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
    }


    void Update()
    {
        //destroy object when horizontal edge is reached
        if(transform.position.x < -35f || transform.position.x > 150f){
            Destroy(gameObject);
        }
        //don't do anything if this is satisfied
        if(dropPoints == null || numItemsToDrop < 1){
            return;
        }
        
        //Check if we have reached passed the dropPoints, if  yes, then drop one item from the cargo
        if(transform.position.x * Mathf.Sign(speed) > dropPoints[numItemsToDrop - 1].x * Mathf.Sign(speed)){
            //decrease the number of Items to drop
            numItemsToDrop--;

            //Calculate the index of the next item to drop as a queue or random
            nextPrefabIndex = numItemsToDrop % prefabCargo.Length;
            if(randomizeDrops){
                nextPrefabIndex = Random.Range(0, prefabCargo.Length - 1);
            }
            //Spawn item from cargo
            go = Instantiate(prefabCargo[numItemsToDrop % prefabCargo.Length], transform.position, transform.rotation);
            //Get reference to RigidBody and set initial speed
            rb = go.GetComponent<Rigidbody2D>();
            if(rb != null){
                rb.velocity = new Vector2(speed, 0);
            }
        }
    }
    public void SetTarget(Vector2 _target, int _numItemsToDrop = -1, int _dropArea = -1){
        target = _target;

        //This two are in case we want to modify this values when calling this method
        if(_numItemsToDrop > 0){
            numItemsToDrop = _numItemsToDrop;
        }
        if(_dropArea > 0){
            dropArea = _dropArea;
        }

        //Initialize dropPoints array
        dropPoints = new Vector2[numItemsToDrop];
                
        if(numItemsToDrop == 1){
            dropPoints[0] = target;
        }else{
            //There are 2 or more drop points
            //Divide the area in evenly spaced points
            float spaceBetweenDrops = dropArea / numItemsToDrop - 1;
            //Calculate each spawn according to area
            for(int i = 0; i < numItemsToDrop; i++){
                //Find the DropPoints for the plane to release the cargo
                dropPoints[i] = FindDropPoint(new Vector2(_target.x + i * spaceBetweenDrops - dropArea / 2, _target.y));
            }
        }
        
    }

    private Vector2 FindDropPoint(Vector2 _target){
        //Assuming target is below plane
        float yDiff = transform.position.y - _target.y;
        float time = yDiff / -Physics2D.gravity.y;
        float xDiff = speed * time;
        //Flip sign on xDiff since we need to drop the cargo before target
        return new Vector2(_target.x - xDiff, yDiff);
    }
}
