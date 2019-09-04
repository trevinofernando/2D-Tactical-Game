using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int health = 100;
    public bool iAmDead = false;
    public GameObject deathEffect;
    private Animator anim;


    void Start()
    {
        //Get reference to animator component
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        //Check if player haven't triggered the death sequence, if not then take damage
        if (!iAmDead)
        {
            anim.SetTrigger("takeDamage");
        }

        //If player health below 0 then trigger death sequence
        if(health <= 0 && !iAmDead)
        {
            anim.SetTrigger("die");
            iAmDead = true;
            Die();
        }
        
    }

    void Die()
    {
        //Added this just in case we want to add a deathEffect later
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        //wait 6 seconds before destroyng this game object, to let the animations play
        //WARNING, this is a calculated time, if death animation changes, then this code needs to be generalized
        //but for now this will do the trick just fine
        Destroy(gameObject, 6f);
    }
}
