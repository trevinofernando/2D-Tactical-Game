using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Hazard
{
    Sun = 0,
    Coconut_Bomber = 1,
    Plane_Healer = 2,
    Plane_Crates = 3,
    Player_Bush = 4
}

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    public int numHazards = 5;
    public Vector3 planeSpawnPoint;
    public GameObject coconutBomberPrefab;
    public GameObject healthPlanePrefab;
    public GameObject cratePlanePrefab;
    public SunScript sun;

    private Hazard hazard;
    private GameObject go;
    private GlobalVariables GLOBALS;
    private CameraController cam;



    

    


    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        cam = CameraController.instance;
        //DeployHazard();
    }

    public void DeployHazard()
    {
        hazard = (Hazard)rand.Next(0, numHazards+3);

        Debug.Log("Deploying a hazard");

        switch(hazard)
        {
            case Hazard.Sun:
                DeploySun();
                break;
            case Hazard.Coconut_Bomber:
                DeployPlane(coconutBomberPrefab);
                break;
            case Hazard.Plane_Healer: 
                DeployPlane(healthPlanePrefab);
                break;
            case Hazard.Plane_Crates: 
                DeployPlane(cratePlanePrefab);
                break;
            case Hazard.Player_Bush:
                break;
            default://Do nothing
                break;
        }
    }

    public void DeployPlane(GameObject prefab)
    {
        GLOBALS.GM.turnClock += (int)(GLOBALS.mapXMax / 100) + 5f;
        planeSpawnPoint = new Vector3(GLOBALS.mapXMax + 30f, GLOBALS.mapYMax + 20f, 0);
        this.go = Instantiate(prefab, planeSpawnPoint, Quaternion.identity);
        cam.soldier = this.go;
        cam.shouldFollowTarget = true;
        cam.SetZoom(100f);
        PlaneManager pm = this.go.transform.GetComponent<PlaneManager>();
        pm.SetTarget(new Vector2((GLOBALS.mapXMax / 2) + Random.Range(-20f, 20f), Random.Range(15f, 40f)), (int)Random.Range(3, 6), (GLOBALS.mapXMax / 2) - (int)Random.Range(0f, 30f));
    }

    public void DeploySun()
    {
        this.go = GLOBALS.GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
        while (this.go == null)
            this.go = GLOBALS.GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
        cam.soldier = this.go;
        cam.shouldFollowTarget = true;
        cam.SetZoom(30f);
        sun.Shoot(this.go.transform.position);
        AudioManager.instance.Play("Short_Choir");
    }
}
