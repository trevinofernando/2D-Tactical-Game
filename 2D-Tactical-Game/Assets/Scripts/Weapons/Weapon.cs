using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector3 PlaneSpawnPoint  = new Vector3(200f, 65f, 0);
    public GameObject targetSprite;
    public int weaponCode = 0;
    /*
     * 0    = Empty
     * 1    = Bazooka
     * 2    = Sniper
     * 3    = Homing Bazooka
     * 4    = Grenade
     * 5    = Holy Grenade
     * 6    = PlaneBomber
     * 7    = BFG 9000 (Doom gun)
     * 8    = 
     * 9    = 
     * 10   = 
    */
    public float endTurnDelay = 0f;
    public PlayerSettings playerSettings;
    public WeaponController WeaponController;
    public Transform firePoint1;
    public Transform firePoint2;
    public GameObject[] projectilePrefab;
    public bool canChangeWeapons = true;

    public  bool canShoot = true;
    private bool targetSelected = false;
    private GameObject go;
    private Rigidbody2D rb;
    private  Vector3 mousePos;

    private void Start() {
        rb = playerSettings.thisGameObject.GetComponent<Rigidbody2D>();
    }



    void Update()
    {
        //if weapon is not equipped, then ignore all code
        //if is not the players turn, then ignore all code
        //if the game is paused, then ignore all code
        if (playerSettings.isMyTurn && !playerSettings.iAmAI && Time.timeScale != 0.0f)
        {
            if (Input.GetButtonDown("Fire1") && canShoot){
                switch (weaponCode)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                        canShoot = false;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation);
                        playerSettings.UpdateAmmo(weaponCode, -1);
                        EndTurn();
                        break;
                    case 3:
                        if (!targetSelected)
                        {
                            AudioManager.instance.Play("Target_Acquired");
                            targetSelected = true;
                            canChangeWeapons = false;
                            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            mousePos.z = 0;
                            go = Instantiate(targetSprite, mousePos, Quaternion.identity);
                            Destroy(go, playerSettings.gameManager.turnClock); //just in case the turn ends suddenly
                        }
                        else
                        {
                            targetSelected = false;
                            canShoot = false;
                            WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, go.transform);
                            playerSettings.UpdateAmmo(weaponCode, -1);
                            if (go != null)
                            {
                                Destroy(go, 5f);
                            }
                            EndTurn();
                        }  
                        break;
                    case 6:
                        canShoot = false;
                        
                        AudioManager.instance.Play("Target_Acquired");
                        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = 0;

                        go = Instantiate(targetSprite, mousePos, Quaternion.identity);
                        WeaponController.Shoot(projectilePrefab[weaponCode], PlaneSpawnPoint, Quaternion.identity, go.transform);
                        playerSettings.UpdateAmmo(weaponCode, -1);
                        if (go != null)
                        {
                            Destroy(go, 10f);
                        }
                        
                        EndTurn();
                        break;
                    case 7:
                        canShoot = false;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint2.position, firePoint2.rotation);
                        playerSettings.UpdateAmmo(weaponCode, -1);
                        Invoke("UnfreezePosition", 7f);
                        EndTurn();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void EndTurn()
    {
        canShoot = true;
        canChangeWeapons = true;
        playerSettings.EndTurn();
    }

    private void UnfreezePosition(){
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

}
