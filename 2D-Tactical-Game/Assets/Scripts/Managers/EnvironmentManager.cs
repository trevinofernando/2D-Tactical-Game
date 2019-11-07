using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    public GameObject[] environmentHazards;
    int hazardCode = 0;

    /*
     * 0    Sun
     * 1    Plane 
     * 2    Player Bush 
     * 3    Tree (Palm and Oak)
     * 4    Bird that doesn't attack, drops weapons/powerups
     * 5    Land (Wolf and Mountain Lion)
     * 6    Aquatic (Shark)
     */


    void Awake()
    {
        rand = new System.Random();
    }

    void Attack()
    {
        hazardCode = rand.Next(environmentHazards.Length);
        //environmentHazards[i].Shoot();

        switch(hazardCode)
        {
            case 0:

            case 1:

            case 2:

            case 3:

            case 4:

            default:
                break;
        }
    }
}
