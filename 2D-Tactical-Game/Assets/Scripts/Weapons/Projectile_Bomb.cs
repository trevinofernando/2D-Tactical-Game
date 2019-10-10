using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bomb : MonoBehaviour
{
    public int directHitDamage = 10;
    public int damage = 40;
    public float launchForce = 1000f;
    public float explosionForce = 1000f;
    public float extraSideForce = 10f;
    public float explosionRadius = 5f;
    public GameObject impactEffect;
    public float autoDestroyOnHeight = -50f;
    private Rigidbody2D rb;
    private float travelingDirection;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        //Add initial force once to make a parabolic trajectory
        rb.AddForce(transform.right * launchForce);
    }

    void Update()
    {
        if(gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        //Calculate projectile traveling direction by using it's velocity on each frame
        travelingDirection = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        //Set the rotation of the projectile to point to it's current direction for a more realistic effect.
        transform.rotation = Quaternion.Euler(0, 0, travelingDirection);
    }
    void OnTriggerEnter2D(Collider2D colInfo)
    {
        //Debug.Log("Projectile Hit!");

        //Look for a DamageHandler script in object collided
        DamageHandler target = colInfo.GetComponent<DamageHandler>();

        //Make sure target has a DamageHandler script, and if so then inflict damage.
        if(target != null)
        {
            target.TakeDamage(directHitDamage);
        }

        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Get all colliders in a circular area of radius "explosionRadious"
        Collider2D[] collisions = Physics2D.OverlapCircleAll( transform.position, explosionRadius);

        //Loop thru each collider
        foreach(Collider2D col in collisions)
        {
            //get rigidbody and add a repulsive force
            rb = col.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Rigidbody2DExtension.AddExplosionForce(rb, transform.position, explosionRadius, explosionForce, extraSideForce);
            }

            //get damageHandler script and apply damage depending on distance
            target = col.GetComponent<DamageHandler>();
            if(target != null)
            {
                var dir = (col.transform.position - transform.position);
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                target.TakeDamage( (int)(damage * wearoff));
            }
        }

        Destroy(gameObject);
    }
}
