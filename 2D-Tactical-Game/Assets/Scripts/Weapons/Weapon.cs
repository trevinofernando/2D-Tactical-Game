using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject targetSprite;
    public int weaponCode = 0;
    private Vector3 PlaneSpawnPoint;
    /*
     * 0    = Gauntlet
     * 1    = Bazooka
     * 2    = Sniper
     * 3    = Homing Bazooka
     * 4    = Grenade
     * 5    = Holy Grenade
     * 6    = PlaneBomber
     * 7    = BFG 9000 (Doom gun)
     * 8    = Shotgun
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
    private Vector3 mousePos;
    private float zoomAmount;

    private void Start() {
        rb = playerSettings.thisGameObject.GetComponent<Rigidbody2D>();
        //PlaneSpawnPoint  = new Vector3(GlobalVariables.Instance.mapXMax + 30f, GlobalVariables.Instance.mapYMax + 25f, 0);
        PlaneSpawnPoint  = new Vector3(GlobalVariables.Instance.mapXMax + 30f, GlobalVariables.Instance.mapYMax - 25f, 0);
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
                    case 0:// Gauntlet
                        break;
                    case 1:// Bazooka
                    case 2:// Sniper
                    case 4:// Grenade
                    case 8://Shotgun
                        canShoot = false; //set flag
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1); //decrement the ammo on this weapon
                        EndTurn();
                        break;
                    case 5:// Holy Grenade
                        zoomAmount = 20f; //set desired zoom
                        Invoke("SetZoom", 3f); //wait for explosion then zoom out if necessary
                        canShoot = false;//set flag
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        EndTurn();
                        break;
                    case 3:// Homing Bazooka
                        if (!targetSelected)
                        {
                            AudioManager.instance.Play("Target_Acquired");
                            targetSelected = true; //set flag
                            canChangeWeapons = false; //set flag
                            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //capture mouse location
                            mousePos.z = 0; //clean z value
                            go = Instantiate(targetSprite, mousePos, Quaternion.identity);//spawn target mark
                            Destroy(go, playerSettings.gameManager.turnClock); //just in case the turn ends suddenly
                        }
                        else
                        {
                            targetSelected = false;//set flag
                            canShoot = false;//set flag
                            WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, go.transform);//call method to spawn prefab
                            playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                            if (go != null)
                            {
                                Destroy(go, 5f);
                            }
                            EndTurn();
                        }  
                        break;
                    case 6: //PlaneBomber
                        canShoot = false;
                        
                        AudioManager.instance.Play("Target_Acquired");
                        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = 0;

                        go = Instantiate(targetSprite, mousePos, Quaternion.identity);
                        WeaponController.Shoot(projectilePrefab[weaponCode], PlaneSpawnPoint, Quaternion.identity, go.transform);
                        zoomAmount = 30f;
                        SetZoom();
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        if (go != null)
                        {
                            Destroy(go, 10f);
                        }
                        
                        EndTurn();
                        break;
                    case 7: //BFG900
                        canShoot = false;//set flag
                        //freeze player in place to play animation smoothly
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint2.position, firePoint2.rotation);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        Invoke("UnfreezePosition", 7f); //unfreeze player after animation
                        zoomAmount = 100f; //set desired zoom
                        Invoke("SetZoom", 5f); //wait for animation then zoom out
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
        canShoot = true; //reset flag for next turn
        canChangeWeapons = true; //reset flag next turn
        playerSettings.EndTurn(); //call master clean up function for all other scripts 
    }

    private void UnfreezePosition(){
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; //rotation on Z should be freeze at all time
    }

    private void SetZoom(){
        playerSettings.cam.SetZoom(zoomAmount);
    }

}
