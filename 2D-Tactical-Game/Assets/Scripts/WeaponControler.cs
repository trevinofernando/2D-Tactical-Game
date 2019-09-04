using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControler : MonoBehaviour
{

    public Transform firePoint;
    public GameObject projectilePrefab;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
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
}
