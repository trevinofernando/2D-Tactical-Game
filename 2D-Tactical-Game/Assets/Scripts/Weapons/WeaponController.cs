using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public float degreesOfDeadZone = 0.25f;

    public Transform weaponPivot;
    public GameObject weapon;
    public PlayerSettings playerSettings;
    [System.NonSerialized] public CrosshairManager crosshairs;

    private SpriteRenderer sprRenderer;
    public Sprite[] weaponSprites;
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
    private Weapon weaponScript;
    public int currWeapon = 0;
    public int prevWeapon = 0;
    public int numWeapons;
    private GameObject go;

    private Vector3 mousePosition;

    void Start()
    {
        sprRenderer = weapon.GetComponent<SpriteRenderer>();
        weaponScript = weapon.GetComponent<Weapon>();
        numWeapons = weaponSprites.Length;
    }

    void Update()
    {
        //Only run this code if the game isn't paused, the soldier is not AI and if it is this soldiers turn
        if (playerSettings.isMyTurn && !playerSettings.iAmAI && Time.timeScale != 0.0f)
        {
            AimToMouse();
            if (weaponScript.canChangeWeapons)
            {
                prevWeapon = currWeapon;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currWeapon = (currWeapon + 1) % numWeapons; //next weapon
                }
                else if(Input.GetKeyDown(KeyCode.Q))
                {
                    currWeapon = (currWeapon + numWeapons - 1) % numWeapons; //previews weapon
                }
            
                //Only call this method if we actuallt change weapons
                if(prevWeapon != currWeapon)
                {
                    ChangeWeapon(currWeapon);
                }
            }
        }

    }

    public void ChangeWeapon(int weaponChoice)
    {
        currWeapon = weaponChoice; //This is needed if an external script calls this method
        weaponScript.weaponCode = weaponChoice; //update weapon script
        sprRenderer.sprite = weaponSprites[weaponChoice]; //change to corresponding sprite
        crosshairs.SetCrosshairTo(weaponChoice);
    }

    public void Shoot(GameObject projectilePrefab, Transform firePoint, Quaternion direction, Transform target = null)
    {
        if(projectilePrefab != null )
        {
            go = Instantiate(projectilePrefab, firePoint.position, direction);
            if(target != null)
            {
                HomingBomb hb = go.GetComponent<HomingBomb>();
                if(hb != null)
                {
                    hb.target = target.position;
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
        
        if (xDiff > 0 + degreesOfDeadZone)
        {
            //We should be facing right
            transform.eulerAngles = new Vector3(0, 0, 0);
            //Rotate gun to point at mouse
            weaponPivot.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        else if(xDiff < 0 - degreesOfDeadZone)
        {
            //We should be facing left
            transform.eulerAngles = new Vector3(0, 180, 0);
            //Rotate gun to point at mouse but gun is upside down, so flip 180 on x
            //And compansate fliping x by inverting z rotation
            weaponPivot.rotation = Quaternion.Euler(180, 0, -zRotation);
        }

    }
}
