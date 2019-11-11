using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadouken : MonoBehaviour
{
    public int damageToPlayer = 40;
    public int damageToProps = 40;
    public float launchForce = 500f;
    public float upForce = 500f;
    public float impactForce = 2000f;
    public GameObject impactEffect;
    public float autoDestroyTimer = 10f;
    private Rigidbody2D rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        AudioManager.instance.Play("Hadouken");
        //Add initial force once to make a parabolic trajectory
        rb.AddForce(transform.right * launchForce * rb.mass);
        Destroy(gameObject, autoDestroyTimer);
    }

    void OnTriggerEnter2D(Collider2D colInfo)
    {
        
        //Explosion Sound
        AudioManager.instance.Play("Slap");
        
        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Look for a DamageHandler script in object collided
        DamageHandler dh = colInfo.GetComponent<DamageHandler>();
        //Make sure target has a DamageHandler script, and if so then inflict damage.
        if(dh != null)
        {
            dh.TakeDamage(damageToPlayer, damageToProps);
        }
        
        //get rigidbody and add a repulsive force
        rb = colInfo.transform.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.AddForce(transform.up * upForce);
            rb.AddForce(transform.right * impactForce);
        }
        
        Destroy(gameObject);
    }
}
