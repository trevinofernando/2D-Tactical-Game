using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int damage = 60;
    public int timer = 3;
    public float launchForce = 1000f;
    public float explosionForce = 1000f;
    public float extraSideForce = 10f;
    public float explosionRadius = 4f;
    public GameObject impactEffect;
    public float autoDestroyOnHeight = -50f;
    private Rigidbody2D rb;
    private CircleCollider2D cirColider;
    private DamageHandler target;
    void Start()
    {
        AudioManager.instance.Play("Grenade_Launcher");
        
        //Add initial force once to make a parabolic trajectory
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * launchForce * rb.mass);

        StartCoroutine(Timer((float)timer));
    }

    void FixedUpdate()
    {
        if(gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }
        
        //Calculate projectile traveling direction by using it's velocity on each frame
        //rb.angularVelocity = rb.velocity.x * cirColider.radius;

        //Set the rotation of the projectile to point to it's current direction for a more realistic effect.
        //transform.rotation = Quaternion.Euler(0, 0, travelingDirection);
    }
    void Detonate()
    {
        //Explosion Sound
        AudioManager.instance.Play("Dark_Explosion");

        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Get all colliders in a circular area of radius "explosionRadius"
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

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Detonate();
    }
}
