using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int health = 100;
    public bool iAmDead = false;
    public GameObject deathEffect;
    private Animator anim;
    public PlayerMovement movementControls;
    public PlayerSettings playerSettings;



    void Start()
    {
        health = GlobalVariables.Instance.healthPerAvatar;
        
        //Get reference to animator component
        anim = GetComponent<Animator>();
        movementControls = GetComponent<PlayerMovement>();
    }

    public void SetHealth(int _health)
    {
        health = _health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(playerSettings != null)
        {
            //updateHeatlh() will take care of negative values
            playerSettings.updateHeatlh(health);
        }

        //Check if player haven't triggered the death sequence, if not then take damage
        if (!iAmDead && anim != null)
        {
            anim.SetTrigger("takeDamage");
        }
        
        //If player health below 0 then trigger death sequence
        if(health <= 0 && !iAmDead)
        {
            if(anim != null)
            {
                anim.SetTrigger("die");
            }
            iAmDead = true;

            //Disable PlayerMovement script to stop player from moving during animation in case of killing himself
            if(movementControls != null)
            {
                movementControls.enabled = false;
            }
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
        if(movementControls != null)
        {
            Destroy(gameObject, 6f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
