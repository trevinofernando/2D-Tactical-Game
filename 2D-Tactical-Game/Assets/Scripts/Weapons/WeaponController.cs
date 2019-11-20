using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float degreesOfDeadZone = 0.25f;

    public Transform weaponPivot;
    public GameObject weapon;
    [System.NonSerialized] public PlayerSettings playerSettings;

    private SpriteRenderer sprRenderer;
    public Sprite[] weaponSprites;
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
    */
    private Weapon weaponScript;
    public int currWeapon = 0;
    public int prevWeapon = 0;
    public int numWeapons;
    private GameObject go;

    private Vector3 mousePosition;

    void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();
        sprRenderer = weapon.GetComponent<SpriteRenderer>();
        weaponScript = weapon.GetComponent<Weapon>();
        numWeapons = weaponSprites.Length;
    }

    void Update()
    {
        //Only run this code if the game isn't paused, the soldier is not AI and if it is this soldiers turn
        if (playerSettings.isMyTurn && !playerSettings.iAmAI && Time.timeScale != 0.0f && !playerSettings.gameManager.isArsenalOpen)
        {
            AimToMouse();
            if (weaponScript.canChangeWeapons)
            {
                prevWeapon = currWeapon;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    do{
                        currWeapon = (currWeapon + 1) % numWeapons; //next weapon
                    }while(!playerSettings.HaveAmmo(currWeapon));
                }
                else if(Input.GetKeyDown(KeyCode.Q))
                {
                    do{
                        currWeapon = (currWeapon + numWeapons - 1) % numWeapons; //previews weapon
                    }while(!playerSettings.HaveAmmo(currWeapon));
                }
            
                //Only call this method if we actually change weapons
                if(prevWeapon != currWeapon)
                {
                    ChangeWeapon(currWeapon);
                }
            }
        }

    }

    public bool ChangeWeapon(int weaponChoice) //returns true if the weapon change was possible
    {
        if(!playerSettings.HaveAmmo(weaponChoice)){
            return false;
        }
        if(prevWeapon == weaponChoice && !playerSettings.iAmAI){
            return true;
        }
        if(weaponChoice != (int)WeaponCodes.Gauntlet){
            AudioManager.instance.Play("Weapon_Swap");
        }
        prevWeapon = currWeapon; //To keep track of what weapon to go back if out of ammo
        currWeapon = weaponChoice; //This is needed if an external script calls this method
        weaponScript.weaponCode = weaponChoice; //update weapon script
        sprRenderer.sprite = weaponSprites[weaponChoice]; //change to corresponding sprite
        //Only change cursor if not an AI player
        if(!playerSettings.iAmAI){
            CrosshairManager.Instance.SetCrosshairTo(weaponChoice);
        }
        return true;
    }

    public void Shoot(GameObject projectilePrefab, Vector3 firePoint, Quaternion direction, bool cameraShouldFollow = true, Transform target = null, int numItemsToDrop = -1, int dropArea = -1)
    {
        if(projectilePrefab != null )
        {
            go = Instantiate(projectilePrefab, firePoint, direction);

            if(cameraShouldFollow){//Don't follow the sniper or shotgun or melee weapons
                playerSettings.gameManager.projectile = go;//Tell GM what projectile is  in the game, to tell the camera to follow it
            } 
            
            //Specific targeting info for some prefabs like plane and homing bazooka
            if(target != null)
            {
                PlaneManager pm = go.GetComponent<PlaneManager>();
                if(pm != null){
                    pm.GM = playerSettings.gameManager;
                    pm.SetTarget(target.position, numItemsToDrop, dropArea);
                    return;
                }
                HomingBomb hb = go.GetComponent<HomingBomb>();
                if(hb != null)
                {
                    hb.target = target.position;
                    return;
                }
            }
        }
    }

    void AimToMouse()
    {
        //Find Mouse position in monitor and then translate that to a point in the world
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Find diff in x and y
        float xDiff = mousePosition.x - weaponPivot.position.x;
        float yDiff = mousePosition.y - weaponPivot.position.y;

        //Calculate angle with 2D tangent formula and change from radians to degrees
        float zRotation = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
        
        AimTo(zRotation, xDiff);
    }

    public void AimTo(float _zRotation, float _xDiff){
        if (_xDiff > 0 + degreesOfDeadZone)
        {
            //We should be facing right
            transform.eulerAngles = new Vector3(0, 0, 0);
            playerSettings.FlipName(false);
            //Rotate gun to point at mouse
            weaponPivot.rotation = Quaternion.Euler(0, 0, _zRotation);
        }
        else if(_xDiff < 0 - degreesOfDeadZone)
        {
            //We should be facing left
            transform.eulerAngles = new Vector3(0, 180, 0);
            playerSettings.FlipName(true);
            //Rotate gun to point at mouse but gun is upside down, so flip 180 on x
            //And compansate fliping x by inverting z rotation
            weaponPivot.rotation = Quaternion.Euler(180, 0, -_zRotation);
        }
    }
}
