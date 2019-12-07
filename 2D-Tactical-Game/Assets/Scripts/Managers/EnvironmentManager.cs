using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hazard
{
    /* Bad Hazards */
    Sun = 0,
    Coconut_Bomber = 1,
    Mine_Bomber = 2,
    Tree = 3,
    Wizard = 4,
    Player_Bush = 5,
    Bird = 6,
    /* Good Hazards */
    Plane_Healer = 7,
    Plane_Crates = 8
}

public class EnvironmentManager : MonoBehaviour
{

    System.Random rand;

    //Hazards
    private int numHazards = 9;
    public Vector3 planeSpawnPoint;
    public Vector3 sunSpawnPoint;
    public GameObject coconutBomberPrefab;
    public GameObject healthPlanePrefab;
    public GameObject cratePlanePrefab;
    public GameObject minePlanePrefab;
    public GameObject sunPrefab;
    public GameObject wizardPrefab;
    public GameObject palmPrefab;
    public GameObject cactusPrefab;

    public bool isReady = false;
    
    public CameraController cam;
    public MapGenerator mapGenerator;
    public MapInitializer mapInitializer;
    private GameObject sun;
    private SunScript sunScript;
    
    private bool goodHazard;
    public double badHazardChance;
    private Hazard hazard;
    private GameObject go;
    private GlobalVariables GLOBALS;

    private GameObject treePrefab;
    public List<GameObject> trees;
    private int numTrees;
    public int numTreesSmall;
    public int numTreesMedium;
    public int numTreesLarge;

    private GameObject pointyPrefab;
    private List<GameObject> pointies;
    private int numPointy;
    public int numPointySmall;
    public int numPointyMedium;
    public int numPointyLarge;

    
    
    
    
    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        trees = new List<GameObject>();
        InitHazards();
        //DeployHazard();
    }

    public void DeployHazard()
    {
        if(rand.NextDouble() >= badHazardChance)
        {
            hazard = (Hazard)rand.Next(0, 7);
        }
        else
        {
            hazard = (Hazard)rand.Next(7, numHazards + 2);
        }

        //hazard = Hazard.Wizard;

        switch(hazard)
        {
            /* Bad Hazards */
            case Hazard.Sun:
                DeploySun();
                break;
            case Hazard.Coconut_Bomber:
                DeployPlane(coconutBomberPrefab);
                break;
            case Hazard.Mine_Bomber:
                DeployPlane(minePlanePrefab);
                break;
            case Hazard.Tree:
                DeployTree();
                break;
            case Hazard.Wizard:
                mapGenerator.RespawnZone();
                break;
            case Hazard.Player_Bush:
                break;
            case Hazard.Bird:
                break;
            /* Good hazards */
            case Hazard.Plane_Healer: 
                DeployPlane(healthPlanePrefab);
                break;
            case Hazard.Plane_Crates: 
                DeployPlane(cratePlanePrefab);
                break;
            default://Do nothing
                break;
        }
    }

    void InitHazards()
    {
        switch (GLOBALS.mapSize)
        {
            case MapSize.Large:
                sunSpawnPoint = new Vector3(440, 195, 0);
                numTrees = numTreesLarge + rand.Next(3);
                numPointy = numPointyLarge + rand.Next(3);
                break;
            case MapSize.Medium:
                sunSpawnPoint = new Vector3(295, 130, 0);
                numTrees = numTreesMedium + rand.Next(2);
                numPointy = numPointyMedium + rand.Next(3);
                break;
            case MapSize.Small:
                sunSpawnPoint = new Vector3(150, 90, 0);
                numTrees = numTreesSmall + rand.Next(2);
                numPointy = numPointySmall + rand.Next(3);
                break;
        }
        switch (GLOBALS.mapTheme) 
        {
            case MapTheme.Desert:
                treePrefab = palmPrefab;
                pointyPrefab = cactusPrefab;
                break;
            case MapTheme.Forest:
                break;
        }

        PlaceSun();
        //SpawnTrees();
        //SpawnPointy();
        isReady = true;
    }

    void PlaceSun()
    {
        
        this.sun = Instantiate(sunPrefab, sunSpawnPoint, Quaternion.identity);
        sunScript = this.sun.transform.GetComponent<SunScript>();
    }

    void SpawnTrees()
    {
        trees = new List<GameObject>();
        Vector3[] treeSpawnLocations = mapInitializer.GenerateSpawns(5.18442f, 5.448769f, true);
        for(int i = 0; i < numTrees; i++)
        {
            trees.Add(Instantiate(treePrefab, treeSpawnLocations[i], Quaternion.identity));
        }
    }

    void SpawnPointy()
    {
        pointies = new List<GameObject>();
        Vector3[] pointySpawnLocations = mapInitializer.GenerateSpawns(1.391115f, 2.4288675f,true);
        for(int i = 0; i < numPointy; i++)
        {
            pointies.Add(Instantiate(pointyPrefab, pointySpawnLocations[i], Quaternion.identity));
        }
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

        for(int i = 0; i < trees.Count - rand.Next(trees.Count); i++)
        {
            if(trees[i] == null)
            {
                Debug.Log("Tree is null!");
                continue;
            }
            trees[i].transform.GetComponent<TreeScript>().Shoot();
        }

    }

    public void DeployWizard(Vector3 target)
    {
        GLOBALS.GM.turnClock += (int)(GLOBALS.mapXMax / 100) + 7f;
        GameObject go = Instantiate(wizardPrefab, target + new Vector3(30, 15, 0), Quaternion.identity);
        cam.soldier = go;
        cam.SetZoom(30f);
    }
}
