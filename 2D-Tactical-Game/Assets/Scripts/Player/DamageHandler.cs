using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int health = 100;
    public bool isPlayer = false;
    public bool iAmDead = false;
    public bool dropCrate = false;
    public GameObject cratePrefab;
    public GameObject deathEffect;
    [System.NonSerialized] public PlayerSettings ps;
    public Animator anim;
    public SpriteRenderer sprRend;
    private int baseHealth;
    private PlayerMovement movementControls;



    void Start()
    {
        baseHealth = health;
        if(isPlayer){
            ps = GetComponent<PlayerSettings>();
        }
            
        if(ps != null)
        {
            health = GlobalVariables.Instance.healthPerAvatar;
            //Get reference to animator component
            //anim = GetComponent<Animator>();
            movementControls = GetComponent<PlayerMovement>();
        }
    }

    public void SetHealth(int _health)
    {
        health = _health;
    }

    public void TakeDamage(int damageToPlayers, int damageToProps = -1, int damageToPlayersPercent = 0, int damageToPropsPercent = 0)
    {
        if(damageToProps == -1){
            damageToProps = damageToPlayers;
        }
        if(isPlayer){
            health -= (int)((float)damageToPlayers + (float)health * damageToPlayersPercent / 100f);
        }else{
            health -= (int)((float)damageToProps + (float)health * damageToPropsPercent / 100f);
        }
        

        //if we are a player
        if (ps != null) 
        {
            //updateHealth() will take care of negative values
            ps.UpdateHealth(health);

            //if the damage is negative, then player was healed, no need to continue
            if(damageToPlayers > 0 || damageToPlayersPercent > 0){
                //start sound
                AudioManager.instance.Play("Take_Damage");

                //Tell camera to look at the player
                ps.cam.soldier = gameObject;
                ps.cam.shouldFollowTarget = true;

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
        }
        else //we must be a tile, prop, etc.
        {
            if(sprRend != null){
                float normalizedColor = ((health * 155f) / baseHealth + 100f) / 255f; //This will give a value between 100 and 255
                sprRend.color = new Color(normalizedColor, normalizedColor, normalizedColor, 1f);
            }
            
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
        if(transform.tag == "Ground"){
            //GlobalVariables.Instance.mapState[(int)transform.position.x / 2, (int)transform.position.y / 2] = 0;
        }

        if(dropCrate)
        {
            Instantiate(cratePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void Die()
    {
        AudioManager.instance.Play("Fatality");
        
        //Added this just in case we want to add a deathEffect later
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (ps.isMyTurn)
        {
            ps.EndTurn();
        }

        //wait 6 seconds before destroyng this game object, to let the animations play
        //WARNING, this is a calculated time, if death animation changes, then this code needs to be generalized
        //but for now this will do the trick just fine
        Destroy(gameObject, 6f);
    }
}
