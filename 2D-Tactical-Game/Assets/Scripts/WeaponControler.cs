using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControler : MonoBehaviour
{

    public float degreesOfDeadZone = 0.25f;

    public Transform weaponPivot;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public PlayerSettings playerSettings;

    private Vector3 mousePosition;

    void Update()
    {
        AimToMouse();
        
        //Shoot if we leftclick on the mouse
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            playerSettings.EndTurn();
        }

    }

    void Shoot()
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
