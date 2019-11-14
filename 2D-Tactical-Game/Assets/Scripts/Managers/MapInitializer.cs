﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    //Default map size
    public MapSize mapSize = MapSize.Large;
    public MapTheme mapTheme = MapTheme.Desert;
    private GlobalVariables GLOBALS;
    public GameObject tilePrefab;
    private List<GameObject> gameObjects;
    private List<Vector3> spawnPoints;
    private int xMax;
    private int yMax;
    private Vector3 smallMapSpawnPoint = new Vector3(70, 80, 0);
    private Vector2Int smallCoordinate = new Vector2Int(100, 50);

    private Vector3 mediumMapSpawnPoint = new Vector3(105, 47, 0);
    private Vector2Int mediumCoordinate = new Vector2Int(200, 130);

    private Vector3 largeMapSpawnPoint = new Vector3(170, 150, 0);
    private Vector2Int largeCoordinate = new Vector2Int(290, 100);

    //Desert Maps
    public GameObject largeDesert;
    public GameObject mediumDesert;
    public GameObject smallDesert;

    //Forest Maps
    public GameObject largeForest;
    public GameObject mediumForest;
    public GameObject smallForest;

    //Beach maps
    public GameObject largeBeach;
    public GameObject mediumBeach;
    public GameObject smallBeach;

    void Awake()
    {
        GLOBALS = GlobalVariables.Instance;
        GLOBALS.mapSize = MapSize.Large;
        DetermineMapTheme();
        DetermineMapSize();
        GLOBALS.mapXMax = xMax;
        GLOBALS.mapYMax = yMax;
        
    }

    void DetermineMapSize()
    { 



        switch(GLOBALS.mapSize)
        {
            case MapSize.Large:
                mapSize = MapSize.Large;
                xMax = largeCoordinate.x;
                yMax = largeCoordinate.y;
                break;
            case MapSize.Medium:
                mapSize = MapSize.Medium;
                xMax = mediumCoordinate.x;
                yMax = mediumCoordinate.y;
                break;
            default:
                mapSize = MapSize.Small;
                xMax = smallCoordinate.x;
                yMax = smallCoordinate.y;
                break;
        }
    }

    void DetermineMapTheme()
    {
        switch(GLOBALS.mapTheme)
        {
            case MapTheme.Forest:
                mapTheme = MapTheme.Forest;
                break;
            case MapTheme.Beach:
                mapTheme = MapTheme.Beach;
                break;
            default:
                mapTheme = MapTheme.Desert;
                break;
        }
    }

    public Vector3[] GenerateMap()
    {

        //Debug.LogError("The map size is " + mapSize);
        mapSize = MapSize.Large;

        switch(mapTheme)
        {
            case MapTheme.Beach:
                if(mapSize.Equals(MapSize.Large))
                {
                    Instantiate(largeBeach, largeMapSpawnPoint, Quaternion.identity);
                }
                else if(mapSize.Equals(MapSize.Medium))
                {
                    Instantiate(mediumBeach, mediumMapSpawnPoint, Quaternion.identity);
                }
                else 
                {
                    Instantiate(smallBeach, smallMapSpawnPoint, Quaternion.identity);
                }
                break;

            case MapTheme.Forest:
                if (mapSize.Equals(MapSize.Large))
                {
                    Instantiate(largeForest, largeMapSpawnPoint, Quaternion.identity);
                }
                else if (mapSize.Equals(MapSize.Medium))
                {
                    Instantiate(mediumForest, mediumMapSpawnPoint, Quaternion.identity);
                }
                else
                {
                    Instantiate(smallForest, smallMapSpawnPoint, Quaternion.identity);
                }
                break;

            default:
                if (mapSize.Equals(MapSize.Small))
                {
                    Instantiate(smallDesert, smallMapSpawnPoint, Quaternion.identity);
                }
                /*
                else if (mapSize.Equals(MapSize.Medium))
                {
                    Instantiate(mediumDesert, mediumMapSpawnPoint, Quaternion.identity);
                }
                */
                else
                {
                    Instantiate(largeDesert, largeMapSpawnPoint, Quaternion.identity);
                }
                break;
        }

        return GenerateSpawns();
    }



    private Vector3[] GenerateSpawns()
    {
        List<Vector3> spawns = new List<Vector3>();
        float soldierSizeX = 1.503002f;
        float soldierSizeY = 2.663491f;
        int numSpawns = 0;

        for(int x = 2; x < xMax; x+=4)
        {
            for(int y = 2; y < yMax; y+=4)
            {
                Vector2 tempStart = new Vector2(x, y);
                Vector2 tempEnd = new Vector2(x + soldierSizeX, y + soldierSizeY);
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



}
