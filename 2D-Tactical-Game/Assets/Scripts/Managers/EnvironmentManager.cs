using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    public int numHazards = 2;
    public Vector3 planeSpawnPoint;
    public GameObject coconutBomberPrefab;
    public GameManager GM;
    public SunScript sun;
    
    private int hazardCode = 1;
    private GameObject go;
    private GlobalVariables GLOBALS;


    /*
     * 1    Sun
     * 2    Plane 
     * 3    Player Bush 
     * 4    Land (Wolf and Mountain Lion)
     */


    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        //DeployHazard();
    }

    public void DeployHazard()
    {
        hazardCode = rand.Next(1, numHazards+1);

        Debug.Log("Deploying a hazard");

        switch(hazardCode)
        {
            case 1:
                this.go = GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
                while(this.go == null)
                    this.go = GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
                sun.Shoot(this.go.transform.position);
                AudioManager.instance.Play("Short_Choir");
                break;
            case 2:
                planeSpawnPoint = new Vector3(GLOBALS.mapXMax + 30f, GLOBALS.mapYMax + 20f, 0);
                GameObject go = Instantiate(coconutBomberPrefab, planeSpawnPoint, Quaternion.identity);
                PlaneManager pm = go.transform.GetComponent<PlaneManager>();
                pm.SetTarget(new Vector2((GLOBALS.mapXMax / 2) + Random.Range(-20f, 20f), Random.Range(15f, 40f)), (int)Random.Range(3, 6), (GLOBALS.mapXMax / 2) - (int)Random.Range(0f, 30f));
                break;
            case 3:
                Debug.Log("Bush!");
                break;
            case 4:
                Debug.Log("Land Animal!");
                break;
            default:
                Debug.LogError("Impossible, there's not that many enivronmental hazards!");
                break;
        }
    }
}
