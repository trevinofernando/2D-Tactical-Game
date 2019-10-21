﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject targetSprite;
    public int weaponCode = 0;
    /*
     * 0    = Empty
     * 1    = Bazooka
     * 2    = Sniper
     * 3    = Homing Bazooka
     * 4    = Grenade
     * 5    = Holy Grenade
     * 6    = 
     * 7    = 
     * 8    = 
     * 9    = 
     * 10   = 
    */
    public float endTurnDelay = 5f;
    public PlayerSettings playerSettings;
    public WeaponController WeaponController;
    public Transform firePoint1;
    public GameObject[] projectilePrefab;
    public bool canChangeWeapons = true;


    private RaycastHit hit;
    public  bool canShoot = true;
    private bool targetSelected = false;
    private DamageHandler dh;
    private GameObject go;



    void Update()
    {
        //if weapon is not equipped, then ignore all code
        //if is not the players turn, then ignore all code
        if (playerSettings.isMyTurn)
        {
            switch (weaponCode)
            {
                case 0:
                    break;
                case 1:
                case 2:
                case 4:
                case 5:
                    //Shoot if we leftclick on the mouse
                    if (Input.GetButtonDown("Fire1") && canShoot)
                    {
                        Invoke("EndTurn", endTurnDelay);
                        canShoot = false;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1, firePoint1.rotation);
                    }
                    break;
                case 3:
                    //Shoot if we leftclick on the mouse
                    if (Input.GetButtonDown("Fire1") && canShoot)
                    {
                        if (!targetSelected)
                        {
                            AudioManager.instance.Play("Target_Acquired");
                            targetSelected = true;
                            canChangeWeapons = false;
                            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            mousePos.z = 0;
                            go = Instantiate(targetSprite, mousePos, Quaternion.identity);
                            Destroy(go, 15f); //just in case the turn ends suddenly
                        }
                        else
                        {
                            Invoke("EndTurn", endTurnDelay);
                            targetSelected = false;
                            canShoot = false;
                            WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1, firePoint1.rotation, go.transform);
                            if (go != null)
                            {
                                Destroy(go, 5f);
                            }
                        }  
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void EndTurn()
    {
        canShoot = true;
        canChangeWeapons = true;
        playerSettings.EndTurn();
    }

}