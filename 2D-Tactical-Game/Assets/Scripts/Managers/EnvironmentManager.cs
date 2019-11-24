using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Hazard
{
    Sun = 0,
    Coconut_Bomber = 1,
    Plane_Healer = 2,
    Plane_Crates = 3,
    Player_Bush = 4,
    Tree = 5, 
    Wizard = 6
}

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    //Hazards
    public int numHazards = 7;
    public Vector3 planeSpawnPoint;
    public Vector3 sunSpawnPoint;
    public GameObject coconutBomberPrefab;
    public GameObject healthPlanePrefab;
    public GameObject cratePlanePrefab;
    public GameObject sunPrefab;
    public GameObject wizardPrefab;
    public GameObject palmPrefab;

    public bool isReady = false;
    
    public CameraController cam;
    private GameObject sun;
    private SunScript sunScript;

    private Hazard hazard;
    private GameObject go;
    private GlobalVariables GLOBALS;
    


    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        InitHazards();
        //DeployHazard();
    }

    public void DeployHazard()
    {
        hazard = (Hazard)rand.Next(0, numHazards+6);


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
            case Hazard.Tree:
                //DeployTree();
                break;
            case Hazard.Wizard:
                break;
            default://Do nothing
                break;
        }
    }

    void InitHazards()
    {
        PlaceSun();
        //SpawnTrees();
        isReady = true;
    }

    void PlaceSun()
    {
        switch(GLOBALS.mapSize)
        {
            case MapSize.Large:
                sunSpawnPoint = new Vector3(440, 195, 0);
                break;
            case MapSize.Medium:
                sunSpawnPoint = new Vector3(295, 130, 0);
                break;
            case MapSize.Small:
                sunSpawnPoint = new Vector3(150, 90, 0);
                break;
        }
        
        this.sun = Instantiate(sunPrefab, sunSpawnPoint, Quaternion.identity);
        sunScript = this.sun.transform.GetComponent<SunScript>();
    }

    void DeployPlane(GameObject prefab)
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

    void DeploySun()
    {
        this.go = GLOBALS.GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
        while (this.go == null)
            this.go = GLOBALS.GM.teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
        cam.soldier = this.go;
        cam.shouldFollowTarget = true;
        cam.SetZoom(30f);
        sunScript.Shoot(this.go.transform.position);
        AudioManager.instance.Play("Short_Choir");
    }

    void DeployTree()
    {
        //trees[rand.Next(0, numTrees+1)].transform.GetComponent<TreeScript>().Shoot();
    }

    void DeployWizard()
    {

    }
}
