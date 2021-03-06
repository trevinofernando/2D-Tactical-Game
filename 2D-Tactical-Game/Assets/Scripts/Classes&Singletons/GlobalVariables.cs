﻿using System;
using System.Collections.Generic;
using UnityEngine;


public enum MapSize
{
    Small = 1,
    Medium = 2,
    Large = 3
}

public enum MapTheme
{
    Desert = 1,
    Forest = 2,
    Beach = 3
}

//Singleton implementation for global variables

public class GlobalVariables : MonoBehaviour
{
    //Self Reference
    public static GlobalVariables Instance { get; private set; }

    //*********GLOBAL VARIABLES*********//

    //MAP related variables
    public int[,] mapState = new int[1000,1000];
    public List<int> arsenalAmmo = new List<int>();
    public MapSize mapSize;
    public MapTheme mapTheme;
    public int mapXMin = 0;
    public int mapYMin = 0;
    public int mapXMax;
    public int mapYMax;

    //TEAMS related variables
    public int numTeams = 4;
    public int numPlayersHuman = 4;
    public int numPlayersNPCs = 0;
    public int teamSize = 4;
    public int healthPerAvatar = 100;

    // redundant
    public string[,] teamNames = new string[4,4]; //[Team, Soldier]
    public bool[] isTeamAI = {false, false, false, false}; //[Team]

    public SampleTeam[] teams = new SampleTeam[8];
    public int gameMode = 1;

    public Color[] teamColors =
    {
        new Color(1, 0, 0, 1), //Red
        new Color(0, 1, 0, 1), //Green
        new Color(0, 0, 1, 1), //Blue
        new Color(1, 1, 1, 1) //White
    }; //RGBA


    //TIME related variables
    public float timePerTurn = 60f; //1 minute
    public float timeBetweenTurns = 5f; // 5 seconds
    public float TimePerGame = 60f * 20f; // 20 minutes

    [System.NonSerialized]
    public GameManager GM;

    private void Awake()
    {
        //This will only pass once at the beggining of the game 
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
    }
}

public enum WeaponCodes
{
    Gauntlet = 0,
    Bazooka = 1,
    Sniper = 2,
    Homing_Bazooka = 3,
    Grenade = 4,
    Holy_Grenade = 5,
    PlaneBomber = 6,
    BFG9000 = 7,
    Shotgun = 8,
    Mjolnir = 9,
    Infinity_Gauntlet = 10,
    Teleport_Grenade = 11,
    Hadouken = 12,
    Mine = 13,
    Bang_Pistol = 14,
    Space_Boots = 15,
    ThunderGun = 16,
    Weak_Stone = 17,
    Normal_Stone = 18,
    Hard_Stone = 19,
    Plane_Nuke = 20,
    Hulk_Punch = 21
}
