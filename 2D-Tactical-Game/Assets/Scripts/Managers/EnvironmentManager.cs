using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    private GlobalVariables GLOBALS;
    public Vector3 planeSpawnPoint;
    public GameObject[] environmentHazards;
    public GameObject coconutBomberPrefab;
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
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        planeSpawnPoint = new Vector3(GLOBALS.mapXMax + 30f, GLOBALS.mapYMax + 20f, 0);
        //Attack();
    }

    void Attack()
    {
        //hazardCode = rand.Next(environmentHazards.Length);
        hazardCode = 1;
        //environmentHazards[i].Shoot();

        switch(hazardCode)
        {
            case 0:

            case 1:
                GameObject coconutBomber = Instantiate(coconutBomberPrefab, planeSpawnPoint, Quaternion.identity);
                PlaneManager pm = coconutBomber.transform.GetComponent<PlaneManager>();
                pm.SetTarget(new Vector2((GLOBALS.mapXMax / 2) + Random.Range(-20f, 20f), Random.Range(15f, 40f)), (int)Random.Range(3, 6), (GLOBALS.mapXMax / 2) - (int)Random.Range(0f, 30f));
                break;
            case 2:

            case 3:

            case 4:

            default:
                break;
        }
    }
}
