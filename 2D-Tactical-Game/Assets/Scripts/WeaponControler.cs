using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControler : MonoBehaviour
{

    public float degreesOfDeadZone = 0.25f;

    public Transform parentPlayer;
    public Transform firePoint;
    public GameObject projectilePrefab;

    private Vector3 mousePosition;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        AimToMouse();
    }

    void FixedUpdate()
    {
        //TODO: update rotation to aim at mouse
    }

    void Shoot()
    {
        if(projectilePrefab != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.Log("ProjectilePrefab missing, can't shoot. Sorry :(");
        }
    }

    void AimToMouse()
    {
        //Find Mouse position in monitor and then translate that to a point in the world
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Find diff in x and y
        float xDiff = mousePosition.x - transform.position.x;
        float yDiff = mousePosition.y - transform.position.y;

        //Calculate angule with 2D tangent formula and change from radians to degrees
        float zRotation = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
        
        if (xDiff > 0 + degreesOfDeadZone)
        {
            //We should be facing right
            parentPlayer.eulerAngles = new Vector3(0, 0, 0);
            //Rotate gun to point at mouse
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        else if(xDiff < 0 - degreesOfDeadZone)
        {
            //We should be facing left
            parentPlayer.eulerAngles = new Vector3(0, 180, 0);
            //Rotate gun to point at mouse but gun is upside down, so flip 180 on x
            //And compansate fliping x by inverting z rotation
            transform.rotation = Quaternion.Euler(180, 0, -zRotation);
        }

    }
}
