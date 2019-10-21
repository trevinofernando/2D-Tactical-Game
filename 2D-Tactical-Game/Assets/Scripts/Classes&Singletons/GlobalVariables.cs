﻿using UnityEngine;

//Singleton implementation for global variables

public class GlobalVariables : MonoBehaviour
{
    //Self Reference
    public static GlobalVariables Instance { get; private set; }

    //*********GLOBAL VARIABLES*********//

    //MAP related variables
    public int[][] mapState;



    //TEAMS related variables
    public int numTeams = 4;
    public int numPlayersHuman = 4;
    public int numPlayersNPCs = 0;
    public int teamSize = 4;
    public int healthPerAvatar = 100;

    // redundant
    public string[,] teamNames = new string[4,4]; //[Team, Soldier]

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
    public float timeBetweenTurns = 3f; // 3 seconds
    public float TimePerGame = 60f * 20f; // 20 minutes



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