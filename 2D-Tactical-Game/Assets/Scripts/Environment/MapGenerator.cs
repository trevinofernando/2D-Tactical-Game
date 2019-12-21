using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Prefabs for map components
    public GameObject[] desertZonePrefabs;
    public GameObject[] forestZonePrefabs;
    public GameObject oneHighBasePlatform;
    public GameObject twoHighBasePlatform;
    public GameObject platformVert;
    public GameObject platformHoriz;

    //How many zones of each map for different sizes
    public int smallZoneRows;
    public int mediumZoneRows;
    public int largeZoneRows;
    public int smallZoneColumns;
    public int mediumZoneColumns;
    public int largeZoneColumns;

    public int xZoneSize;
    public int yZoneSize;

    //Probabilities for added randomness
    public double platformSpawnProb;
    public double emptyZoneProb;

    //References to global variables
    public GlobalVariables GLOBALS; //made this public to allow for running game from Fernando's scene
    private GameManager GM;
    private MapSize mapSize;
    private MapTheme mapTheme;
    
    //Keep reference of each object for respawning function
    private List<GameObject> zoneObjects;
    private List<int> zonePrefabNum;
    private List<int> platformRowNums;
    private List<int> startingColliderPlatform;
    private int skipPlatform;
    private int numPlatforms;
    private int zoneToRespawn;
    GameObject zonePrefab;
    Vector3 spawnSpot;

    private int numRows;
    private int numCols;
    private System.Random rand;

    //Position spawning offsets
    private Vector3 xPlatformOffset = new Vector3(10, 0, 0);
    private int yLargeBaseOffset = 18;
    private int yMediumBaseOffset = 12;
    private int ySmallBaseOffset = 8;
    private Vector3 xZoneOffset;
    private Vector3 yZoneOffset;

    public EnvironmentManager environmentManager;

    public LayerMask layerMask;


    void Awake()
    {
        if(GlobalVariables.Instance != null){//Else use the default or previous instance
            GLOBALS = GlobalVariables.Instance;
        }
        mapSize = GLOBALS.mapSize;
        mapTheme = GLOBALS.mapTheme;
        platformRowNums = new List<int>();
        zoneObjects = new List<GameObject>();
        zonePrefabNum = new List<int>();
        startingColliderPlatform = new List<int>();
        rand = new System.Random();
        xZoneOffset = new Vector3(xZoneSize + 6, 0, 0);
        yZoneOffset = new Vector3(0, yZoneSize + 3, 0);

        switch (mapSize)
        {
            case MapSize.Large:
                numRows = largeZoneRows;
                numCols = largeZoneColumns;
                break;
            case MapSize.Medium:
                numRows = mediumZoneRows;
                numCols = mediumZoneColumns;
                break;
            case MapSize.Small:
                numRows = smallZoneRows;
                numCols = smallZoneColumns;
                break;
            default:
                Debug.LogError("Map size not Initalized yet!");
                break;
        }

        GenerateMap();
    }

    void GenerateMap()
    {
        SpawnBasePlatforms();

        Vector3 spawnPosition = new Vector3();
        switch (mapSize)
        {
            case MapSize.Large:
                spawnPosition += new Vector3(0, yLargeBaseOffset, 0);
                break;
            case MapSize.Medium:
                spawnPosition += new Vector3(0, yMediumBaseOffset, 0);
                break;
            case MapSize.Small:
                spawnPosition += new Vector3(0, ySmallBaseOffset, 0);
                break;
            default:
                break;
        }



        for (int y = 0; y < numRows; y++)
        {
            GameObject zone = Instantiate(PickRandomZone(), spawnPosition, Quaternion.identity);
            zoneObjects.Add(zone);
            InitZoneNumColliders(zone);
            spawnPosition += xZoneOffset;
            for (int x = 1; x < numCols; x++)
            {
                if(rand.NextDouble() <= platformSpawnProb && y > 0)
                {
                    Instantiate(platformVert, spawnPosition + new Vector3(2, 0, 0), Quaternion.identity);
                }

                spawnPosition += xPlatformOffset;
                GameObject zonee = Instantiate(PickRandomZone(), spawnPosition, Quaternion.identity);
                zoneObjects.Add(zonee);
                InitZoneNumColliders(zonee);
                spawnPosition += xZoneOffset;
            }
            spawnPosition += yZoneOffset;
            spawnPosition.x = 0;
        }
    }

    void InitZoneNumColliders(GameObject go)
    {
        int childCount = 0;
        Vector2 tempStart = go.transform.position;
        Vector2 tempEnd = tempStart + new Vector2(60, 30);
        Collider2D[] tempColliders = Physics2D.OverlapAreaAll(tempStart, tempEnd);
        foreach (Collider2D collider in tempColliders)
        {
            childCount++;
        }
        startingColliderPlatform.Add(childCount);
    }

    GameObject PickRandomZone()
    {
        int zoneNum;
        switch(mapTheme)
        {
            case MapTheme.Desert:
                zoneNum = rand.Next(desertZonePrefabs.Length);
                zonePrefabNum.Add(zoneNum);
                return desertZonePrefabs[zoneNum];
            case MapTheme.Forest:
                zoneNum = rand.Next(forestZonePrefabs.Length);
                zonePrefabNum.Add(zoneNum);
                return forestZonePrefabs[zoneNum];
            default:
                return new GameObject();
        }
    }

    void SpawnBasePlatforms()
    {
        
        SpawnOneColBasePlatform(new Vector3(0, 0, 0));
        
        Vector3 spawnPosition = new Vector3(66, 0, 0);
        
        for(int i = 1; i < numCols; i++)
        {
            Instantiate(platformVert, spawnPosition + new Vector3(2, 0, 0), Quaternion.identity);
            spawnPosition += xPlatformOffset;
            SpawnOneColBasePlatform(spawnPosition);
            spawnPosition += xZoneOffset;


        }
    }

    void SpawnOneColBasePlatform(Vector3 spawnPosition)
    {
       switch(mapSize)
       {
            case MapSize.Large:
                for(int i = 0; i < 5; i++)
                {
                    Vector3 offSet = new Vector3(0, i*4, 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            case MapSize.Medium:
                for (int i = 0; i < 3; i++)
                {
                    Vector3 offSet = new Vector3(0, i*4 , 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            case MapSize.Small:
                for (int i = 0; i < 2; i++)
                {
                    Vector3 offSet = new Vector3(0, i*4, 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            default:
                Debug.LogError("Map size not Initalized yet!");
                break;
        }
    }


    public void RespawnZone()
    {
        zoneToRespawn = GetMostDestroyedZone();
        if (MapTheme.Desert == mapTheme && zoneToRespawn > desertZonePrefabs.Length ||
            MapTheme.Forest == mapTheme && zoneToRespawn > forestZonePrefabs.Length )
            return;
        if (MapTheme.Forest == mapTheme)
            zonePrefab = forestZonePrefabs[zonePrefabNum[zoneToRespawn]];
        if (MapTheme.Desert == mapTheme)
            zonePrefab = desertZonePrefabs[zonePrefabNum[zoneToRespawn]];
        if (zonePrefab == null)
            return;
        spawnSpot = zoneObjects[zoneToRespawn].transform.position;
        this.environmentManager.DeployWizard(spawnSpot);
        Invoke("CreateZone", 11f);
    }

    private void CreateZone()
    {
        GameObject toDelete = zoneObjects[zoneToRespawn];
        zoneObjects.Insert(zoneToRespawn, Instantiate(zonePrefab, spawnSpot, Quaternion.identity));
        if (toDelete != null)
        {
            Destroy(toDelete);
        }
    }

    //Returns the index of zoneObjects zone that has the least number of colliders
    int GetMostDestroyedZone()
    {
        int mostDestroyedIndex = 0;
        double leastChildren = 1e9;

        for(int i = 0; i < zoneObjects.Count; i++)
        {
            GameObject go = zoneObjects[i];
            if (go == null)
                continue;
            //bool hasPlayer = false;
            int childCount = 0;
            Vector2 tempStart = go.transform.position;
            Vector2 tempEnd = tempStart + new Vector2(60, 30);
            Collider2D[] tempColliders = Physics2D.OverlapAreaAll(tempStart, tempEnd);
            foreach(Collider2D collider in tempColliders)
            {
                childCount++;
            }
            if (i >= startingColliderPlatform.Count)
                continue;
            double ratio = (double)childCount / startingColliderPlatform[i];
            if(leastChildren >ratio)
            {
                mostDestroyedIndex = i;
                leastChildren = ratio;
            }
        }
        return mostDestroyedIndex;
    }

    int GetNumChildren(Transform parent)
    {
        int numChildren = 0;
        foreach(Transform child in parent)
        {
            numChildren += child.childCount;
        }
        return numChildren;
    }
}
