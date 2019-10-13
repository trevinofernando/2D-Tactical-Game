using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public GameObject tilePrefab;
    private string themeName;
    private float positionOffSet = 2.0f;
    private int numPlayers;
    private int numTiles;
    private int[,] mapState;
    private List<GameObject> gameObjects;
    private int xCoordinate;
    private int yCoordinate;
    private int scaleToPlayersX = 4;
    private int scaleToPlayersY = 3;
    private int xMax;
    private int yMax;
    private List<Vector3> spawnPoints;
    private int alternate;

    //Values for map state:
    //0 is an empty 2*2 area
    //1 is an occupied 2*2 area




    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        transform.position = new Vector3(0f, 0f, 0f);
        gameObjects = new List<GameObject>();
        numPlayers = GLOBALS.numTeams * GLOBALS.teamSize;
        spawnPoints = new List<Vector3>();
        //GenerateMap();
    }

    public Vector3[] GenerateSpawnLocations()
    {

        for (int x = 0; x < xMax; x++)
        {
            int randomY = Random.Range(3, yMax+1);

            if (CanSpawn(x, randomY))
            {
                //Debug.Log("Spawn point is (" + x + "," + randomY + ")");
                spawnPoints.Add(new Vector3(x * positionOffSet, randomY * positionOffSet, 0));
            }
        }

        //return spawnPoints.ToArray();
        return spawnPoints.OrderBy(a => System.Guid.NewGuid()).ToArray();

    }

    public bool CanSpawn(int x, int y)
    {
        if (y < yMax - 1)
        { 
            if (mapState[x, y] == 0 && mapState[x, y+1] == 0)
                return true;
        }
        return false;
    }

    public void RandomlyPickPlatform()
    {

        switch(alternate)
        {
            case 0:
                PlaceFlatPlatform();
                if(Random.Range(0, 2) > 0)
                {
                    xCoordinate += 2;
                    yCoordinate -= 2;
                    PlaceFlatPlatform();
                }
                
                break;
            case 1:
                PlaceLayeredPlatform();
                xCoordinate++;
                yCoordinate--;
                break;
            case 2:
                PlacePyramid();
                xCoordinate++;
                break;
            default:
                //PlaceOneTile();
                //Debug.Log("Skipping tiles");
                SkipTiles();
                yCoordinate -= 8;
                break;
        }

        //alternate++;
        //alternate %= 3;

    }


    public Vector3[] GenerateMap()
    {
        //How big the map will be based on number of players
        

        xMax = numPlayers * scaleToPlayersX;
        yMax = numPlayers * scaleToPlayersY;

        //Debug.Log("X max boundaries " + xMax + " Y max boundaries " + yMax);
        mapState = new int[xMax, yMax];

        //Base Platform
        for (int y = 0; y < yMax / 4; y++)
        {
            for (int x = 0; x < xMax; x++)
            {
                CreateTile(x, y);
                mapState[x, y] = 1;
                numTiles++;
                yCoordinate = y;
                xCoordinate = x;
            }
        }

        //Update to where the base platform leaves off
        xCoordinate = Random.Range(0, 4);
        yCoordinate += 2;
        //Debug.Log("X is at " + xCoordinate + " Y is at " + yCoordinate);

        //Add tiles
        for (int i = 0; i < numPlayers /2 ; i++)
        {
            RandomlyPickPlatform();
        }

        xCoordinate = 0;
        yCoordinate -= 5;


        return GenerateSpawnLocations();
    }

    /*Tile Platform Generators*/

    //Creates tile at given coordinate
    private void CreateTile(int x, int y)
    {
        if(xCoordinate < xMax && yCoordinate < yMax)
        {
            if (mapState[x, y] == 0)
            {
                GameObject newTile = Instantiate(tilePrefab, transform.position + new Vector3(x * positionOffSet, y * positionOffSet, 0), transform.rotation);
                newTile.transform.SetParent(transform);
                gameObjects.Add(newTile);
                mapState[x, y] = 1;
            }
        }

        
    }


    private void IteratePosition()
    {
        //Iterate x coordinate and check if you need to reset x and increase y
        xCoordinate++;
        if (xCoordinate >= xMax)
        {
            xCoordinate = 0;
            yCoordinate++;
        }
    }

    //Skips between 1 and 3 tiles in the x and y direction
    private void SkipTiles()
    {
        int skipX = Random.Range(0, 2);
        int skipY = Random.Range(0, 1);

        xCoordinate += skipX;
        yCoordinate += skipY;
        IteratePosition();
    }

    private void PlaceOneTile()
    {
        CreateTile(xCoordinate, yCoordinate);
        IteratePosition();
    }

    //Places a (2-5) x 1 size platform
    private void PlaceFlatPlatform()
    {
        int sizeX = Random.Range(2, 6);

        for (int i = 0; i <= sizeX; i++)
        {
            CreateTile(xCoordinate, yCoordinate);
            IteratePosition();
        }


        xCoordinate++;
        yCoordinate += 2;
    }

    //Places a (2-5) x 2 size platform
    private void PlaceLayeredPlatform()
    {
        int sizeX = Random.Range(2, 6);
        int sizeY = 2;

        for (int i = 0; i <= sizeY; i++)
        {
            for (int j = 0; j <= sizeX; j++)
            {
                CreateTile(xCoordinate, yCoordinate);
                IteratePosition();
            }
        }
    }

    //Places stairs from 3 to 9 wide/high
    private void PlaceStairs()
    {
        int stairHeight = Random.Range(3, 10);

        for (int i = 0; i < stairHeight; i++)
        {
            CreateTile(xCoordinate++, yCoordinate++);
        }
    }

    //Places a pyramid from 3-5 height/width
    private void PlacePyramid()
    {
        int sizeWidthHeight = Random.Range(3, 6);

        //Upwards rightwards movement
        for (int i = 0; i < sizeWidthHeight; i++)
        {
            CreateTile(xCoordinate++, yCoordinate++);
        }

        yCoordinate--;

        //Downwards rightward movement
        for (int j = 0; j < sizeWidthHeight; j++)
        {
            CreateTile(xCoordinate++, yCoordinate--);
        }
    }

    /*Prop Generators*/

    //Randomly places props near 
    private void GenerateProps()
    {

    }

    //Places a small prop 1x1
    private void PlaceSmallProp()
    {

    }

    //Places a medium prop 1x2
    private void PlaceMediumProp()
    {

    }

    //Places a large prop 1x3
    private void PlaceLargeProp()
    {

    }

    //Spawns boss onto map
    private void GenerateBoss()
    {

    }

}
