using System.Collections;
using System.Collections.Generic;
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
    private int scaleToPlayersX = 50;
    private int scaleToPlayersY = 4;
    private int xMax = 50;
    private int yMax = 20;

    //Values for map state:
    //0 is an empty 2*2 area
    //1 is an occupied 2*2 area


    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        transform.position = new Vector3(0f, 0f, 0f);
        gameObjects = new List<GameObject>();
        numPlayers = GLOBALS.numTeams * GLOBALS.teamSize;
        GenerateMap();
        //GenerateSpawnLocations();
        GenerateProps();
        GenerateBoss();
    }

    public void GenerateSpawnLocations()
    {
        Vector3[] spawnLocations = new Vector3[numPlayers];
        int numSpawnLocations = 0;
        bool skip = false;

        while (numSpawnLocations < numPlayers)
        {
            for (int x = 0; x < xMax; x++)
            {
                for (int y = scaleToPlayersY; y < yMax; y++)
                {
                    //Skips a tile 1/3 of the time
                    if (Random.Range(0, 3) == 0)
                        skip = true;

                    //Check if map is occupied in a space, and the space above, and something is below the player
                    if (mapState[x, y] == 0 && mapState[x, y + 1] == 0 && mapState[x, y - 1] == 1 && !skip)
                    {
                        spawnLocations[numSpawnLocations++] = new Vector3(x * positionOffSet, y * positionOffSet, 0);
                    }
                }
            }
        }
    }

    public void RandomlyPickPlatform()
    {
        int randomNumber = Random.Range(0, 10);

        if (randomNumber < 2)
            PlaceOneTile();
        else if (randomNumber >= 3 && randomNumber < 5)
            PlaceFlatPlatform();
        else if (randomNumber >= 6 && randomNumber < 8)
            PlaceLayeredPlatform();
        else if (randomNumber == 8)
            PlaceStairs();
        else if (randomNumber == 9)
            PlacePyramid();
        else
            SkipTiles();

    }


    public void GenerateMap()
    {
        //How big the map will be based on number of players
        mapState = new int[scaleToPlayersX * 5, scaleToPlayersY * 5];

        //Base Platform
        for (int y = 0; y < scaleToPlayersY; y++)
        {
            for (int x = 0; x < scaleToPlayersX; x++)
            {
                CreateTile(x, y);
                mapState[x, y] = 1;
                numTiles++;
                //Debug.Log("Placing tile at " + "(" + x + "," + y);
            }
        }

        //Update to where the base platform leaves off
        xCoordinate = 0;
        yCoordinate = scaleToPlayersY;

        //Add tiles
        for (int i = 0; i < 20; i++)
        {
            RandomlyPickPlatform();
        }

    }

    /*Tile Platform Generators*/

    //Creates tile at given coordinate
    private void CreateTile(int x, int y)
    {
        if (mapState[x, y] == 0)
        {
            GameObject newTile = Instantiate(tilePrefab, transform.position + new Vector3(x * positionOffSet, y * positionOffSet, 0), transform.rotation);
            gameObjects.Add(newTile);
            mapState[x, y] = 1;
        }
    }


    private void IteratePosition()
    {
        //Iterate x coordinate and check if you need to reset x and increase y
        xCoordinate++;
        if (xCoordinate >= scaleToPlayersX)
        {
            xCoordinate = 0;
            yCoordinate++;
        }
    }

    //Skips between 1 and 3 tiles in the x and y direction
    private void SkipTiles()
    {
        int skipX = Random.Range(0, 4);
        int skipY = Random.Range(0, 4);

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
