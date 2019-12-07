using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject targetSprite;
    public Animator anim;
    [System.NonSerialized] public int weaponCode = 0;
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
     * 9    = Mjolnir
     * 10   = Infinity Gauntlet
     * 11   = Teleport Grenade
     * 12   = Hadouken
     * 13   = Mine
     * 14   = BangPistol
     * 15   = SpaceBoots
     * 16   = ThunderGun
     * 17   = Weak_Stone
     * 18   = Normal_Stone
     * 19   = Hard_Stone
     * 20   = Tactical_Nuke
    */
    public float endTurnDelay = 0f;
    public PlayerSettings playerSettings;
    public WeaponController WeaponController;
    public Transform firePoint1;
    public Transform firePoint2;
    public SpriteRenderer frontFeet;
    public SpriteRenderer backFeet;
    public float spaceBootTime = 15f;
    public Sprite spaceBootSprite;
    public Sprite normalFeetSprite;
    public GameObject[] projectilePrefab;
    [System.NonSerialized] public bool canChangeWeapons = true;

    [System.NonSerialized] public  bool canShoot = true;
    [System.NonSerialized] public  bool fireTriggered = false;
    [System.NonSerialized] public bool targetSelected = false;
    private GameObject go;
    private Rigidbody2D rb;
    private AIController aiContr;
    private Vector3 mousePos;
    private float zoomAmount;
    private IEnumerator coroutineSpaceBoots;
    private bool spaceBootsUsed = false;
    private float normalGravity;

    private void Start() {
        PlaneSpawnPoint  = new Vector3(GlobalVariables.Instance.mapXMax + 30f, GlobalVariables.Instance.mapYMax + 20f, 0);
        rb = playerSettings.thisGameObject.GetComponent<Rigidbody2D>();
        aiContr = playerSettings.thisGameObject.GetComponent<AIController>();
        normalGravity = rb.gravityScale;
    }



    void Update()
    {
        //if weapon is not equipped, then ignore all code
        //if is not the players turn, then ignore all code
        //if the game is paused, then ignore all code
        if (playerSettings.isMyTurn && Time.timeScale != 0.0f)
        {
            if(!playerSettings.iAmAI){
                fireTriggered = Input.GetButtonDown("Fire1");
            }
            if (fireTriggered && canShoot){
                switch (weaponCode)
                {
                    case (int)WeaponCodes.Bazooka: //Weapons that spawn a projectile and the camera follows the projectile
                    case (int)WeaponCodes.Grenade:
                    case (int)WeaponCodes.Teleport_Grenade:
                    case (int)WeaponCodes.Hadouken:
                        canShoot = false; //set flag
                        fireTriggered = false; //set flag
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, false, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1); //decrement the ammo on this weapon
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Sniper: //Weapons that don't need the camera to follow them
                    case (int)WeaponCodes.Shotgun:
                    case (int)WeaponCodes.ThunderGun:
                    case (int)WeaponCodes.Bang_Pistol:
                        canShoot = false; //set flag
                        fireTriggered = false; //set flag
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, true, false);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1); //decrement the ammo on this weapon
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Mine:
                        canShoot = false; //set flag
                        fireTriggered = false; //set flag
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, false, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1); //decrement the ammo on this weapon
                        Invoke("StopCameraFollow", 5f);
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Holy_Grenade:
                        canShoot = false;//set flag
                        fireTriggered = false;//set flag
                        zoomAmount = 20f; //set desired zoom
                        Invoke("SetZoom", 3f); //wait for explosion then zoom out if necessary
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, false, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        EndTurn();
                        break;
                        
                    case (int)WeaponCodes.Homing_Bazooka:
                        if (!targetSelected)
                        {
                            fireTriggered = false; //set flag
                            targetSelected = true; //set flag
                            canChangeWeapons = false; //set flag
                            AudioManager.instance.Play("Target_Acquired");
                            if(playerSettings.iAmAI){
                                mousePos = aiContr.target.position;
                            }else{
                                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //capture mouse location
                            }
                            mousePos.z = 0; //clean z value
                            go = Instantiate(targetSprite, mousePos, Quaternion.identity);//spawn target mark
                            Destroy(go, playerSettings.gameManager.turnClock); //just in case the turn ends suddenly
                        }
                        else
                        {
                            canShoot = false;//set flag
                            fireTriggered = false;//set flag
                            targetSelected = false;//set flag
                            WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, false, true, go.transform);//call method to spawn prefab
                            playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                            if (go != null)
                            {
                                Destroy(go, 5f);
                            }
                            EndTurn();
                        }  
                        break;

                    case (int)WeaponCodes.PlaneBomber:
                    case (int)WeaponCodes.Plane_Nuke:
                        canShoot = false;
                        fireTriggered = false;
                        
                        AudioManager.instance.Play("Target_Acquired");
                        if(playerSettings.iAmAI){
                            mousePos = aiContr.target.position;
                        }else{
                            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        mousePos.z = 0;

                        go = Instantiate(targetSprite, mousePos, Quaternion.identity);
                        if(mousePos.x > PlaneSpawnPoint.x / 2){
                            WeaponController.Shoot(projectilePrefab[weaponCode], new Vector3(0, PlaneSpawnPoint.y,0), Quaternion.identity, false, true, go.transform);
                        }else{
                            WeaponController.Shoot(projectilePrefab[weaponCode], PlaneSpawnPoint, Quaternion.identity, false, true, go.transform);
                        }
                        zoomAmount = 30f;
                        SetZoom();
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        if (go != null)
                        {
                            Destroy(go, 10f);
                        }
                        
                        EndTurn();
                        break;

                    case (int)WeaponCodes.BFG9000:
                        canShoot = false;//set flag
                        fireTriggered = false;//set flag
                        //freeze player in place to play animation smoothly
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint2.position, firePoint2.rotation, true, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        Invoke("UnfreezePosition", 7f); //unfreeze player after animation
                        zoomAmount = 100f; //set desired zoom
                        Invoke("SetZoom", 5f); //wait for animation then zoom out
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Mjolnir:
                        canShoot = false;//set flag
                        fireTriggered = false;//set flag
                        //freeze player in place to play animation smoothly
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, true, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        Invoke("UnfreezePosition", 2f); //unfreeze player after animation
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Infinity_Gauntlet:
                        canShoot = false;//set flag
                        fireTriggered = false;//set flag
                        //freeze player in place to play animation smoothly
                        AudioManager.instance.Stop("Carros_De_Fuego");
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        WeaponController.Shoot(projectilePrefab[weaponCode], transform.position, transform.rotation, true, true);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        zoomAmount = 5f;
                        SetZoom();
                        Invoke("UnfreezePosition", 7.5f); //unfreeze player after animation
                        zoomAmount = 25f; //set desired zoom
                        Invoke("SetZoom", 7.5f); //wait for animation then zoom out
                        EndTurn();
                        break;

                    case (int)WeaponCodes.Space_Boots:
                        fireTriggered = false;//set flag
                        if(spaceBootsUsed){
                            StopCoroutine(coroutineSpaceBoots);
                        }
                        spaceBootsUsed = true;
                        coroutineSpaceBoots = SpaceBootMode(spaceBootTime);
                        StartCoroutine(coroutineSpaceBoots);
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon

                        //Change weapon if we have no more ammo
                        if(!playerSettings.HaveAmmo((int)WeaponCodes.Space_Boots)){
                            WeaponController.ChangeWeapon((int)WeaponCodes.Gauntlet);
                        }
                        break;

                    case (int)WeaponCodes.Hulk_Punch:
                        fireTriggered = false;//set flag
                        anim.SetTrigger("Hulk_Punch");
                        WeaponController.Shoot(projectilePrefab[weaponCode], firePoint1.position, firePoint1.rotation, true, false);//call method to spawn prefab
                        playerSettings.UpdateAmmo(weaponCode, -1);//decrement the ammo on this weapon
                        break;

                    default:
                        //Here we have: Gauntlet, Weak_Stone, Normal_Stone, Hard_Stone
                        break;
                }
            }
        }
    }

    private IEnumerator SpaceBootMode(float waitTime)
    {
        rb.gravityScale = 1f;
        frontFeet.sprite = spaceBootSprite;
        backFeet.sprite = spaceBootSprite;
        AudioManager.instance.Play("Carros_De_Fuego");
        AudioManager.instance.Stop("The Duel");
        yield return new WaitForSeconds(waitTime);
        AudioManager.instance.Stop("Carros_De_Fuego");
        AudioManager.instance.Play("The Duel");
        frontFeet.sprite = normalFeetSprite;
        backFeet.sprite = normalFeetSprite;
        rb.gravityScale = normalGravity;
    }

    public void EndTurn()
    {
        canShoot = true; //reset flag for next turn
        spaceBootsUsed = false;
        fireTriggered = false; //reset flag for next turn
        targetSelected = false;//reset flag for next turn
        canChangeWeapons = true; //reset flag for next turn
        AudioManager.instance.Stop("Carros_De_Fuego");
        playerSettings.EndTurn(); //call master clean up function for all other scripts 
    }

    private void UnfreezePosition(){
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; //rotation on Z should be freeze at all time
    }

    private void StopCameraFollow(){
        GlobalVariables.Instance.GM.projectile = null;
    }

    private void SetZoom(){
        playerSettings.cam.SetZoom(zoomAmount);
    }


}
