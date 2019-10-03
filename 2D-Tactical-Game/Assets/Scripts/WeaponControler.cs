using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControler : MonoBehaviour
{

    public float degreesOfDeadZone = 0.25f;

    public Transform weaponPivot;
    public GameObject weapon;
    public PlayerSettings playerSettings;

    private SpriteRenderer sprRenderer;
    public Sprite[] weaponSprites;
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
    private Weapon weaponScript;
    private int currWeapon = 0;
    private int prevWeapon = 0;
    private int numWeapons;

    private Vector3 mousePosition;

    void Start()
    {
        sprRenderer = weapon.GetComponent<SpriteRenderer>();
        weaponScript = weapon.GetComponent<Weapon>();
        numWeapons = weaponSprites.Length;
    }

    void Update()
    {
        if (playerSettings.isMyTurn)
        {
            AimToMouse();

            prevWeapon = currWeapon;

            if (Input.GetKeyDown(KeyCode.E))
            {
                currWeapon = (currWeapon + 1) % numWeapons; //next weapon
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                currWeapon = (currWeapon + numWeapons - 1) % numWeapons; //previews weapon
            }
            
            if(prevWeapon != currWeapon)
            {
                ChangeWeapon();
            }
        }

    }

    void ChangeWeapon()
    {
        weaponScript.weaponCode = currWeapon; //update weapon script
        sprRenderer.sprite = weaponSprites[currWeapon]; //change to corresponding sprite
    }

    public void Shoot(GameObject projectilePrefab, Transform firePoint)
    {
        if(projectilePrefab != null )
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
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

        //Calculate angule with 2D tangent formula and change from radians to degrees
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
