using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bomb : MonoBehaviour
{
    public float launchForce = 10f;
    public int damage = 30;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    private bool travelingUp;
    private float travelingDirection;
    void Start()
    {
        //Add initial force once to make a parabolic trajectory
        rb.AddForce(transform.right * launchForce);
    }

    void Update()
    {
        if(gameObject.transform.position.y <= -50f)
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
        Debug.Log("Projectile Hit!");

        //Look for a DamageHandler script in object collided
        DamageHandler target = colInfo.GetComponent<DamageHandler>();

        //Make sure target has a DamageHandler script, and if so then inflict damage.
        if(target != null)
        {
            target.TakeDamage(damage);
        }

        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
