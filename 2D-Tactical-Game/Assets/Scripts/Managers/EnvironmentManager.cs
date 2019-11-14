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
    Tree = 5
}

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    public int numHazards = 5;
    public Vector3 planeSpawnPoint;
    public Vector3 sunSpawnPoint;
    public GameObject coconutBomberPrefab;
    public GameObject healthPlanePrefab;
    public GameObject cratePlanePrefab;
    public GameObject sunPrefab;
    private GameObject sun;
    private SunScript sunScript;
    private GameObject[] trees;
    private int numTrees = 6;
    public bool isReady = false;

    private Hazard hazard;
    private GameObject go;
    private GlobalVariables GLOBALS;
    public CameraController cam;


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
            default://Do nothing
                break;
        }
    }

    void InitHazards()
    {
        PlaceSun();
        //GetTrees();
        isReady = true;
    }

    void PlaceSun()
    {
        if (GLOBALS.mapSize == MapSize.Small)
        {
            sunSpawnPoint = new Vector3(155, 120, 0);
        }
        else
            sunSpawnPoint = new Vector3(315, 225, 0);
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

    void GetTrees()
    {
        
        trees = new GameObject[numTrees];
        var gameObjects = FindObjectsOfType<GameObject>();
        int count = 0;
        foreach(GameObject go in gameObjects)
        {
            Debug.Log("Looking for a tree...");
            if(go.transform.GetComponent<TreeScript>())
            {
                trees[count++] = go;
                Debug.LogError("Hey look a tree!");
            }

        }
        /*
        trees[0] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree");
        trees[1] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree1");
        trees[2] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree2");
        trees[3] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree3");
        trees[4] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree4");
        trees[5] = GameObject.Find("/Sand Large Environment/Ground/Palm Tree5");
        */
        if(trees[0] != null)
            Debug.LogError("Got the trees!");
    }

    void DeployTree()
    {
        trees[rand.Next(0, numTrees+1)].transform.GetComponent<TreeScript>().Shoot();
    }
}
