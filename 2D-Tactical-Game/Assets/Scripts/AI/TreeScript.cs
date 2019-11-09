using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{

    Vector3 downLeft = new Vector3(-2, -2, 0);
    Vector3 downRight = new Vector3(2, -2, 0);
    public GameObject projectilePrefab;
    public float pushForce = 2500f;
    public float angularVelocity = 200f;
    public float autoDestroyOnHeight = -50f;


    void Update()
    {
        if (gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }

    }

    void Shoot()
    {
        GameObject leftCoconut = Instantiate(projectilePrefab, transform.position + downLeft, Quaternion.identity);
        GameObject rightCoconut = Instantiate(projectilePrefab, transform.position + downRight, Quaternion.identity);

        Rigidbody2D leftrb = leftCoconut.transform.GetComponent<Rigidbody2D>();
        Rigidbody2D rightrb = rightCoconut.transform.GetComponent<Rigidbody2D>();
        
        leftrb.angularVelocity = -angularVelocity;
        rightrb.angularVelocity = angularVelocity;
        leftrb.AddForce(new Vector2(-1, -0.5f) * pushForce);
        rightrb.AddForce(new Vector2(1, -0.5f) * pushForce);
    }

}
