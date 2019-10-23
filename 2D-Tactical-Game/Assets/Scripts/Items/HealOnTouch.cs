using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOnTouch : MonoBehaviour
{
    public int healthBonus = 25;
    public string objectTagToHeal = "Player";
    public GameObject impactEffect;
    
    private void OnCollisionEnter2D(Collision2D other) {
        //Debug.Log(other.transform.tag);
        //Check if tag match to target
        if(other.transform.tag == objectTagToHeal){
            //get Damage Handler component
            DamageHandler dh = other.transform.GetComponent<DamageHandler>();
            if(dh != null){
                //pass negative damage to heal
                dh.TakeDamage(-healthBonus, 0);

                AudioManager.instance.Play("Magic_PickUp");
                //Spawn some VFX when picked up
                if(impactEffect != null)
                {
                    Instantiate(impactEffect, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }
}
