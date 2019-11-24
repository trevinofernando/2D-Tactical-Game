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
    private GlobalVariables GLOBALS;
    private GameManager GM;
    private MapSize mapSize;
    private MapTheme mapTheme;
    
    //Keep reference of each object for respawning function
    private List<GameObject> zones;
    private List<int> platformRowNums;
    private int skipPlatform;
    private int numPlatforms;
    
    private int numRows;
    private int numCols;
    private System.Random rand;

    //Position spawning offsets
    private Vector3 xPlatformOffset = new Vector3(10, 0, 0);
    private int yLargeBaseOffset = 16;
    private int yMediumBaseOffset = 10;
    private int ySmallBaseOffset = 6;
    private Vector3 xZoneOffset;
    private Vector3 yZoneOffset;

    //Self Reference
    public static MapGenerator Instance { get; private set; }

    void Awake()
    {
        //This will only pass once at the beginning of the game 
        if (Instance == null)
        {
            //Self reference
            Instance = this;
            //Make this object persistent
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy duplicate instance created by changing Scene
            Destroy(gameObject);
        }


        GLOBALS = GlobalVariables.Instance;
        mapSize = GLOBALS.mapSize;
        mapTheme = GLOBALS.mapTheme;
        platformRowNums = new List<int>();
        zones = new List<GameObject>();
        rand = new System.Random();
        xZoneOffset = new Vector3(xZoneSize + 3, 0, 0);
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
            zones.Add(Instantiate(PickRandomZone(), spawnPosition, Quaternion.identity));
            spawnPosition += xZoneOffset;
            for (int x = 1; x < numCols; x++)
            {
                spawnPosition += xPlatformOffset;
                zones.Add(Instantiate(PickRandomZone(), spawnPosition, Quaternion.identity));
                spawnPosition += xZoneOffset;
            }
            spawnPosition += yZoneOffset;
            spawnPosition.x = 0;
        }
    }

    GameObject PickRandomZone()
    {
        switch(mapTheme)
        {
            case MapTheme.Desert:
                return desertZonePrefabs[rand.Next(desertZonePrefabs.Length)];
            case MapTheme.Forest:
                return forestZonePrefabs[rand.Next(forestZonePrefabs.Length)];
            default:
                return new GameObject();
        }
    }

    void SpawnBasePlatforms()
    {
        
        SpawnOneColBasePlatform(new Vector3(0, 0, 0));
        
        Vector3 spawnPosition = new Vector3(60, 0, 0);
        
        for(int i = 1; i < numCols; i++)
        {
            Instantiate(platformVert, spawnPosition + new Vector3(5, 0, 0), Quaternion.identity);
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
                    Vector3 offSet = new Vector3(0, i*2, 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            case MapSize.Medium:
                for (int i = 0; i < 3; i++)
                {
                    Vector3 offSet = new Vector3(0, i*2 , 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            case MapSize.Small:
                for (int i = 0; i < 2; i++)
                {
                    Vector3 offSet = new Vector3(0, i*2, 0);
                    Instantiate(twoHighBasePlatform, spawnPosition + offSet, Quaternion.identity);
                }
                break;
            default:
                Debug.LogError("Map size not Initalized yet!");
                break;
        }
    }


    void RespawnZone()
    {

    }

    //Returns a list of the zone indices in order
    List<Zone> GetZoneRespawnPriorities()
    {
        List<Zone> respawnPriorities = new List<Zone>();


        return respawnPriorities;
    }


}
