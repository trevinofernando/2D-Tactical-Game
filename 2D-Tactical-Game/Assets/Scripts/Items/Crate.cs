using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public string objectTagToInteract = "Player";
    public GameObject impactEffect;
    public string[] weaponNames = 
    {
        "Gauntlet",
        "Bazooka",
        "Sniper",
        "Homing Bazooka",
        "Grenade",
        "Holy Grenade",
        "PlaneBomber",
        "BFG9000",
        "Shotgun",
        "MissingName",
        "MissingName",
        "MissingName",
        "MissingName"
    };
    private int weaponCode;

    private void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);
        weaponCode = Random.Range(1, GlobalVariables.Instance.arsenalAmmo.Count);
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        //Check if tag match to target
        if(other.transform.tag == objectTagToInteract){

            Random.InitState(System.DateTime.Now.Millisecond);
            weaponCode = Random.Range(1, GlobalVariables.Instance.arsenalAmmo.Count);
            Debug.Log(weaponNames[weaponCode] + "++");

            //get Player Settings component
            PlayerSettings ps = other.transform.GetComponent<PlayerSettings>();
            if(ps != null){
                //pass negative damage to heal
                ps.UpdateAmmo(weaponCode, + 1);

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
    private void FixedUpdate() {
        if(transform.position.y < -50){
            Destroy(gameObject);
        }
    }
}
