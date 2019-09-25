using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public GameObject tilePrefab;
    public string themeName;
    public float positionOffSet = 2.0f;
    public int numPlayers;

    //Values for map state:
    //0 is an empty 2*2 area
    //1 is an occupied 2*2 area


    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        transform.position = new Vector3(0f,0f,0f);
    }

    public Vector3[] generateMap()
    {
        int[ , ] mapState = new int[100, 100];
        Vector3[] spawnLocations = new Vector3[GLOBALS.numTeams * GLOBALS.teamSize];

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                Instantiate(tilePrefab, transform.position + new Vector3(i * positionOffSet, j * positionOffSet, 0), transform.rotation);
                mapState[i, j] = 1;
            }
        }


        return spawnLocations;
    }
}
