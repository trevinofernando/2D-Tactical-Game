using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    private GlobalVariables GLOBALS;

    private Vector3 smallBackgroundSpawnPoint = new Vector3(65, 65, 0);
    private Vector2Int smallCoordinate = new Vector2Int(135, 70);

    private Vector3 mediumBackgroundSpawnPoint = new Vector3(130, 75, 0);
    private Vector2Int mediumCoordinate = new Vector2Int(280, 110);

    private Vector3 largeBackgroundSpawnPoint = new Vector3(170, 150, 0);
    private Vector2Int largeCoordinate = new Vector2Int(425, 175);

    //Desert Background
    public GameObject largeDesert;
    public GameObject mediumDesert;
    public GameObject smallDesert;

    //Forest Background
    public GameObject largeForest;
    public GameObject mediumForest;
    public GameObject smallForest;


    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        DetermineMapSize();
        PlaceBackground();
    }

    void DetermineMapSize()
    { 

        switch(GLOBALS.mapSize)
        {
            case MapSize.Large:
                GLOBALS.mapXMax = largeCoordinate.x;
                GLOBALS.mapYMax = largeCoordinate.y;
                break;
            case MapSize.Medium:
                GLOBALS.mapXMax = mediumCoordinate.x;
                GLOBALS.mapYMax = mediumCoordinate.y;
                break;
            case MapSize.Small:
                GLOBALS.mapXMax = smallCoordinate.x;
                GLOBALS.mapYMax = smallCoordinate.y;
                break;
            default:
                break;
        }
    }

    void PlaceBackground()
    {
        switch(GLOBALS.mapTheme)
        {
            case MapTheme.Desert:
                switch(GLOBALS.mapSize)
                {
                    case MapSize.Large:
                        Instantiate(largeDesert, new Vector3(), Quaternion.identity);
                        break;
                    case MapSize.Medium:
                        Instantiate(mediumDesert, new Vector3(), Quaternion.identity);
                        break;
                    case MapSize.Small:
                        Instantiate(smallDesert, new Vector3(), Quaternion.identity);
                        break;
                    default:
                        break;
                }
                break;
            case MapTheme.Forest:
                switch (GLOBALS.mapSize)
                {
                    case MapSize.Large:
                        Instantiate(largeForest, largeBackgroundSpawnPoint, Quaternion.identity);
                        break;
                    case MapSize.Medium:
                        Instantiate(mediumForest, mediumBackgroundSpawnPoint, Quaternion.identity);
                        break;
                    case MapSize.Small:
                        Instantiate(smallForest, smallBackgroundSpawnPoint, Quaternion.identity);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public Vector3[] GenerateSpawns(float sizeX, float sizeY, bool checkForColliderBelow)
    {
        List<Vector3> spawns = new List<Vector3>();
        int numSpawns = 0;

        for(int x = 4; x < GLOBALS.mapXMax -5; x+=3)
        {
            if(!SafeToSpawn(x))
            {
                continue;
            }

            for(int y = 2; y < GLOBALS.mapYMax -5; y+=4)
            {
                Vector2 tempStart = new Vector2(x, y);
                Vector2 tempEnd = new Vector2(x + sizeX, y + sizeY);
                Collider2D[] tempColliders = Physics2D.OverlapAreaAll(tempStart, tempEnd);
                if (tempColliders.Length == 0)
                {
                    spawns.Add(tempStart);
                    numSpawns++;
                }
            }
        }

        var rand = new System.Random();

        var shuffledSpawns = new List<Vector3>(spawns.Count);

        while (spawns.Count > 0)
        {
            var i = rand.Next(spawns.Count);
            shuffledSpawns.Add(spawns[i]);
            spawns.RemoveAt(i);
        }

        return shuffledSpawns.ToArray();
    }
    /*Avoid 58-78, 135-153, 210-228, 286-305, 360-384 
     * For platform areas 
     */
    bool SafeToSpawn(int x)
    {
        if ((x > 58 && x < 78) || (x > 135 && x < 153) || (x > 210 && x < 228) || (x > 286 && x < 305) || (x > 360 && x < 384))
            return false;
        return true;
    }

}


