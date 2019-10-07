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



    private List<GameObject> DecideTargets()
    {
        List<GameObject> targets = new List<GameObject>();

        for(int i = 0; i < numTeams; i++)
        {
            if(i != myTeamId)
            {
                for (int j = 0; j < numPlayers; j++)
                {
                    if(teams[i, j] != null)
                    {
                        if(IsShootable(teams[i, j].transform.position))
                        {
                            targets.Add(teams[i, j]);
                        }

                    }
                }
            }
            
        }

        return targets;
    }

    //Checks if you can ray cast to an enemy player
    private bool IsShootable(Vector3 otherPosition)
    {
        raycast = Physics2D.Raycast(transform.position, (otherPosition - transform.position).normalized);

        return raycast.transform.position == otherPosition;
    }



    
    
}
