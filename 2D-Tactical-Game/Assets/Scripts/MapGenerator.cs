using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public GameObject tilePrefab;
    private string themeName;
    public float positionOffSet = 2.0f;
    private int numPlayers;
    private int startingTiles;
    private int[,] mapState;
    private List<GameObject> gameObjects;

    //Values for map state:
    //0 is an empty 2*2 area
    //1 is an occupied 2*2 area


    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        transform.position = new Vector3(0f,0f,0f);
        gameObjects = new List<GameObject>();
        //numPlayers = GLOBALS.numTeams * GLOBALS.teamSize;
    }

    public Vector3[] GenerateMap()
    {
        //How big the map will be based on number of players
        int scaleToPlayersX = 50;
        int scaleToPlayersY = 4;
        

        mapState = new int[scaleToPlayersX * 2,scaleToPlayersY * 2];
        Vector3[] spawnLocations = new Vector3[numPlayers];

        //Base Platform
        for(int y = 0; y < scaleToPlayersY; y++)
        {
            for(int x = 0; x < 50; scaleToPlayersX++)
            {
                Instantiate(tilePrefab, transform.position + new Vector3(x * positionOffSet, y * positionOffSet, 0), transform.rotation);
                mapState[x, y] = 1;
                startingTiles++;
                //Debug.Log("Placing tile at " + "(" + x + "," + y);
            }
        }

        /*while(startingTiles < 100 )
        {



        }*/

        
        //Place props


        //Place map boss


        //Generate spawn locations


        return spawnLocations;
    }

    /*Tile Platform Generators*/

     //Skips tiles
     private void SkipTiles()
    {

    }

    //Places a (2-5) x 1 size platform
    private void PlaceFlatPlatform(int x, int y)
    {
        int sizeX = Random.Range(2, 6);
        int sizeY = 1;

        for(int i = 0; i <= sizeX; i++)
        {
            Instantiate(tilePrefab, transform.position + new Vector3(x + i * positionOffSet, y * positionOffSet, 0), transform.rotation);
        }
    }

    //Places a (2-5) x 2 size platform
    private void PlaceLayeredPlatform(int x, int y)
    {
        int sizeX = Random.Range(2, 6);
        int sizeY = 2;
    }

    //Places stairs from 3 to 9 wide/high
    private void PlaceStairs(int x, int y)
    {
        int stairHeight = Random.Range(3, 10);
    }

    //Places a pyramid from 3-5 height/width
    private void PlacePyramid(int x, int y)
    {
        int sizeWidthHeight = Random.Range(3, 6);
    }

    /*Prop Generators*/

    //Places a small prop 1x1
    private void PlaceSmallProp(int x, int y)
    {

    }

    //Places a medium prop 1x2
    private void PlaceMediumProp(int x, int y)
    {

    }

    //Places a large prop 1x3
    private void PlaceLargeProp(int x, int y)
    {

    }

    //Spawns boss onto map
    private void PlaceSpawnBoss(int x, int y)
    {

    }



}
