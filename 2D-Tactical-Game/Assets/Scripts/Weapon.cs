using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int weaponCode = 0;
    /*
     * 0    = Empty
     * 1    = Bazooka
     * 2    = 
     * 3    =
     * 4    =
     * 5    =
     * 6    =
     * 7    =
     * 8    =
     * 9    =
     * 10   =
    */
    
    public PlayerSettings playerSettings;
    public WeaponControler WeaponControler;
    public Transform firePoint1;
    public GameObject[] projectilePrefab;

    private bool canShoot = true;



    void Update()
    {
        //if weapon is not equiped, then ignore all code
        //if is not the players turn, then ignore all code
        if (playerSettings.isMyTurn)
        {
            switch(weaponCode)
            {
                case 0:
                    break;
                case 1:
                    //Shoot if we leftclick on the mouse
                    if (Input.GetButtonDown("Fire1") && canShoot)
                    {
                        Invoke("EndTurn", 3);
                        canShoot = false;
                        WeaponControler.Shoot(projectilePrefab[weaponCode], firePoint1);
                    }
                    break;
                default:
                    break;
            }
            
        }
    }

    void EndTurn()
    {
        canShoot = true;
        playerSettings.EndTurn();
    }

}
