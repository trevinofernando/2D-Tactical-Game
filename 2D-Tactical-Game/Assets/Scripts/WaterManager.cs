using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{

    public int Damage = 999999; //High number since it's meant to be an instakill
    public GameObject drowningEffect;

    void OnTriggerEnter2D(Collider2D colInfo)
    {
        //Debug.Log("Water Touched!");

        //get rigidbody
        Rigidbody2D rb = colInfo.attachedRigidbody;
        if(rb != null)
        {
            rb.gravityScale = 0.1f;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y) / 10;
        }

        //Look for a DamageHandler script in object collided
        DamageHandler target = colInfo.GetComponent<DamageHandler>();

        //Make sure target has a DamageHandler script, and if so then inflict damage.
        if (target != null)
        {
            target.TakeDamage(Damage);
        }

        //Create an impact effect like an explosion
        if (drowningEffect != null)
        {
            Instantiate(drowningEffect, transform.position, Quaternion.identity);
        }

    }
}
