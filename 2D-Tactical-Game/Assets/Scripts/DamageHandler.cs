﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int health = 100;
    public bool iAmDead = false;
    public GameObject deathEffect;
    public PlayerMovement movementControls;
    [System.NonSerialized] public PlayerSettings ps;
    private Animator anim;



    void Start()
    {
        ps = GetComponent<PlayerSettings>();
        if(ps != null)
        {
            health = GlobalVariables.Instance.healthPerAvatar;
            //Get reference to animator component
            anim = GetComponent<Animator>();
            movementControls = GetComponent<PlayerMovement>();
        }
    }

    public void SetHealth(int _health)
    {
        health = _health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (ps != null) //if we are a player
        {
            //updateHeatlh() will take care of negative values
            ps.UpdateHeatlh(health);


            //Check if player haven't triggered the death sequence, if not then take damage
            if (!iAmDead)
            {
                anim.SetTrigger("takeDamage");
            }

            //If player health below 0 then trigger death sequence
            if (health <= 0 && !iAmDead)
            {
                anim.SetTrigger("die");
                iAmDead = true;

                //Disable PlayerMovement script to stop player from moving during animation in case of killing himself
                if (movementControls != null)
                {
                    movementControls.enabled = false;
                }
                Die();
            }
        }
        else //we must be a tile, prop, etc.
        {
            if (health <= 0)
            {
                DestroyProp();
            }
        }
    }

    void DestroyProp()
    {
        //************TODO******************
        //Update Map array in Game Manager

        //Added this just in case we want to add a deathEffect later, like an explosion
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
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
