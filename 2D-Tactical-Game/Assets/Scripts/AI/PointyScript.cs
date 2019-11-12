using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyScript : MonoBehaviour
{

    public int damage = 10;
    public float autoDestroyOnHeight = -50f;
    public float pushBackForceTurn = 4000f;
    public float pushBackForceNotTurn = 500f;
    private Vector3 direction;


    void Update()
    {
        if (gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Check if tag match to target
        if (other.transform.tag == "Player")
        {


            Transform otherTransform = other.transform;

            //get Damage Handler component
            DamageHandler dh = otherTransform.GetComponent<DamageHandler>();
            if (dh != null)
            {
                //pass negative damage to heal
                dh.TakeDamage(damage, 0);

                if (gameObject.transform.position.x > otherTransform.position.x)
                    direction = new Vector3(-1, 0, 0);
                else
                    direction = new Vector3(1, 0, 0);

                Rigidbody2D rb = otherTransform.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector3(0, 0, 0);

                    PlayerSettings ps = otherTransform.GetComponent<PlayerSettings>();

                    //Check if it's the player's turn
                    if(!ps.isMyTurn)
                        rb.AddForce(direction * pushBackForceNotTurn);
                    else
                        rb.AddForce(direction * pushBackForceTurn);
                }

            }
        }
    }

    



}
