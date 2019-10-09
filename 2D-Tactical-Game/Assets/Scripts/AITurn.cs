using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurn : MonoBehaviour
{
    public GameObject[,] teams; //[TeamID, Soldier ID]
    private int numTeams;
    private int numPlayers;
    public PlayerSettings playerSettings;
    private GlobalVariables GLOBALS;

    //Make sure to use playerSettings.EndTurn() to clean up
    private RaycastHit2D raycast;
    private int myTeamId;
    private int myPlayerId;
    private GameObject otherPlayerSetting;

    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        numTeams = GLOBALS.numTeams;
        numPlayers = GLOBALS.teamSize;
        playerSettings = gameObject.GetComponent<PlayerSettings>();
        myTeamId = playerSettings.teamID;
        myPlayerId = playerSettings.ID;
    }


    
    
}
