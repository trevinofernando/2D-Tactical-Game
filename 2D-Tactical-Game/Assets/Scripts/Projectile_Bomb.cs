using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bomb : MonoBehaviour
{
    public float launchForce = 10f;
    public int damage = 30;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    void Start()
    {
        rb.AddForce(transform.right * launchForce);
    }

    void OnTriggerEnter2D(Collider2D colInfo)
    {
        Debug.Log("Projectile Hit!");
        DamageHandler target = colInfo.GetComponent<DamageHandler>();

        if(target != null)
        {
            target.TakeDamage(damage);
        }
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
