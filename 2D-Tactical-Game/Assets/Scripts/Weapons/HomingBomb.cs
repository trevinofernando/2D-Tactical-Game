using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBomb : MonoBehaviour
{
    
    public int directHitDamage = 10;
    public int damage = 30;
    public float launchForce = 1000f;
    public float explosionForce = 800f;
    public float extraSideForce = -5f;
    public float explosionRadius = 4f;
    public float speed = 100f;
    public float rotationSpeed = 250f;
    public float chaseTimer = 1f;
    public float autoDestroyOnHeight = -50f;
    public Vector3 target;
    public GameObject impactEffect;
    private Rigidbody2D rb;
    private Vector2 direction;
    private float rotationAmount;
    private bool chasingMode = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * launchForce);
        StartCoroutine(Timer(chaseTimer));
    }

    void Update()
    {
        if (gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (chasingMode)
        {
            
            direction = (Vector2)target - rb.position;
            direction.Normalize();

            rotationAmount = Vector3.Cross(direction, transform.right).z;
            rb.angularVelocity = -rotationAmount * rotationSpeed;

            rb.velocity = transform.right * speed;
            speed += 0.25f;
        }

    }

    void OnTriggerEnter2D(Collider2D colInfo)
    {
        //Debug.Log("Projectile Hit!");

        //Look for a DamageHandler script in object collided
        DamageHandler target = colInfo.GetComponent<DamageHandler>();

        //Make sure target has a DamageHandler script, and if so then inflict damage.
        if (target != null)
        {
            target.TakeDamage(directHitDamage);
        }

        //Create an impact effect like an explosion
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Get all colliders in a circular area of radius "explosionRadious"
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        //Loop thru each collider
        foreach (Collider2D col in collisions)
        {
            //get rigidbody and add a repulsive force
            rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Rigidbody2DExtension.AddExplosionForce(rb, transform.position, explosionRadius, explosionForce, extraSideForce);
            }

            //get damageHandler script and apply damage depending on distance
            target = col.GetComponent<DamageHandler>();
            if (target != null)
            {
                var dir = (col.transform.position - transform.position);
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                target.TakeDamage((int)(damage * wearoff));
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        chasingMode = true;
        //rb.gravityScale = 0.5f;
    }
}

