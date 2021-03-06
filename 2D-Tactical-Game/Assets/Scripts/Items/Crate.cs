﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public string objectTagToInteract = "Player";
    public GameObject impactEffect;
    public int predeterminedWeaponToSpawn = -1;

    [System.NonSerialized]
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
        "Mjolnir",
        "Infinity Gauntlet",
        "Teleport Grenade",
        "Hadouken",
        "Mine",
        "Bang Pistol",
        "Space Boots",
        "ThunderGun",
        "Weak Stone",
        "Normal Stone",
        "Hard Stone",
        "Plane Nuke",
        "Hulk Fists",
        "MissingName",
        "MissingName",
        "MissingName",
    };
    private int weaponCode;

    private void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);
        if(predeterminedWeaponToSpawn == -1){
            weaponCode = Random.Range(1, GlobalVariables.Instance.arsenalAmmo.Count);
        }else{
            weaponCode = predeterminedWeaponToSpawn;
        }
    }

    public void ChangeCreateContentTo(int weaponChoice){
        if(weaponChoice < GlobalVariables.Instance.arsenalAmmo.Count){
            weaponCode = weaponChoice;
        }
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
                if(weaponCode >= (int)WeaponCodes.Weak_Stone && weaponCode <= (int)WeaponCodes.Hard_Stone){
                    ps.UpdateAmmo(weaponCode, + 5);
                }else{
                    ps.UpdateAmmo(weaponCode, + 1);
                }
                
                AudioManager.instance.Play("Weapon_PickUp");
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
