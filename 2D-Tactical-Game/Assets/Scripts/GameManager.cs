using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public GameObject soldierPrefab;
    public Vector3 spawnOffset = new Vector3(-18, 10, 0);
    private Vector3[] spawnLocations;

    //Teams related variables
    private GameObject[,] teams; //[TeamID][SoldierID]
    public int deadTeamsCounter = 0; 
    public int[] teamsHealth; //[TeamID]
    public int[,] soldiersHealth; //[TeamID, SoldierID]
    public int currTeamTurn = 0; //index of current teams turn
    public int[] currSoldierTurn; //[TeamID] to keep track of which soldier is next depending on team

    //Time related variables
    public float turnClock = 0f;
    public float gameClock = 0f;
    private bool corutineStarted = false;
    private IEnumerator coroutineTurnClock;
    private IEnumerator coroutineGameClock;

    //STATES
    public GameState gameState = GameState.LoadingScene;
    public bool suddenDeath = false;
    public bool isTurnFinished = false;
    public bool isOneTeamAlive = false;
    public AIState aiState = AIState.WaitingForTurn;

    //Temporary Variables
    private GameObject go;
    private GameManager thisGM;
    private PlayerSettings ps;

    public enum GameState
    {
        LoadingScene,
        TurnInProgress,
        TurnTransition,
        AITurn,
        Pause,
        GameOver
    }

    public enum AIState
    {
        WaitingForTurn,
        PickingTarget,
        WeaponSelect,
        Aiming,
        Shooting,
        Pause
    }

    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        gameState = GameState.LoadingScene;
        isTurnFinished = false; //Set initial state

        /****** TODO: Call Map generator and get spawn locations *******/
        // spawnLocations = GenerateMap(GLOBALS.numTeams * GLOBALS.teamSize);


        //Initialize array to hold each soldier object team[Team][Avatar]
        teams = new GameObject[GLOBALS.numTeams, GLOBALS.teamSize];
        
        //Initialize array to hold healt of each soldier of each team
        soldiersHealth = new int[GLOBALS.numTeams, GLOBALS.teamSize];

        //Initialize array to hold healt of each team
        teamsHealth = new int[GLOBALS.numTeams]; //default value is already 0

        //Initialize array to keep track of turn of soldiers, since each team will have a different order
        currSoldierTurn = new int[GLOBALS.numTeams]; //default value is already 0
        currTeamTurn = 0;

        //Get reference to this GameManager component on this object
        thisGM = gameObject.GetComponent<GameManager>();

        //Create Teams
        for (int i = 0; i < GLOBALS.numTeams; i++)
        {
            for(int j = 0; j < GLOBALS.teamSize; j++)
            {
                spawnOffset.x += 2; //temporary offset until map generation is done
                //Spawn Player
                teams[i , j] = Instantiate(soldierPrefab, transform.position + spawnOffset, transform.rotation);
                //temporary spawn location until map generation is done
                //teams[i , j] = Instantiate(avatarPrefab, spawnLocations[i * GLOBALS.numTeams + j], transform.rotation);

                teams[i, j].GetComponent<PlayerSettings>().gameManager = thisGM;

                ps = teams[i, j].GetComponent<PlayerSettings>();
                //Set color of player
                ps.SetColor(GLOBALS.teamColors[i]);
                ps.teamID = i;
                ps.ID = j;
                ps.nameGiven = GLOBALS.teamNames[i,j]; //[TeamID, SoldierID]

                //Set Health of player
                teams[i, j].GetComponent<DamageHandler>().SetHealth(GLOBALS.healthPerAvatar);
                soldiersHealth[i, j] = GLOBALS.healthPerAvatar;
                
                //Add Heath of this soldier to the team health
                teamsHealth[i] += GLOBALS.healthPerAvatar;
                
                //Deactivate movement script.
                teams[i, j].GetComponent<PlayerMovement>().enabled = false;

            }
        }

        //Check if GLOBALS.TimePerGame have a valid time
        if (GLOBALS.TimePerGame <= 0)
        {
            GLOBALS.TimePerGame = (float)(GLOBALS.timePerTurn * GLOBALS.numTeams * 5f); //around 5 turns per team 
        }
        //Start gameClock Timer
        coroutineGameClock = SetGameClock(GLOBALS.TimePerGame);
        StartCoroutine(coroutineGameClock);
        corutineStarted = false;
        gameState = GameState.TurnTransition;
    }


    void Update()
    {

        //Check if game is over
        deadTeamsCounter = 0;
        for (int i = 0; i < GLOBALS.numTeams; i++)
        {
            if(teamsHealth[i] <= 0)
            {
                deadTeamsCounter++;
            }
        }

        if(deadTeamsCounter >= GLOBALS.numTeams - 1)
        {
            gameState = GameState.GameOver;
        }

        switch (gameState)
        {
            case GameState.TurnTransition:
                // Start timer for next turn.
                if (!corutineStarted)
                {
                    corutineStarted = true;
                    coroutineTurnClock = SetTurnClock(GLOBALS.timeBetweenTurns); //time between turns should be 1 to 5 sec
                    StartCoroutine(coroutineTurnClock);
                }

                //***************************TODO**************************
                //Chance of Enviroment Hazard activation.

                if (isTurnFinished)
                {
                    corutineStarted = false; //Reset coroutine check
                    isTurnFinished = false; //Reset check before changing state
                    gameState = GameState.TurnInProgress;//change State

                    // Find out which team is next in turn. By looping thru numteams - 1
                    //If we don't get to the break statement, then game is over
                    isOneTeamAlive = true;
                    for (int i = 0; i < GLOBALS.numTeams - 1; i++)
                    {
                        //Give turn to next team
                        currTeamTurn = (currTeamTurn + 1) % GLOBALS.numTeams;

                        //Check if next team is dead
                        if (teamsHealth[currTeamTurn] > 0)
                        {
                            //A team with acceptable health was found
                            isOneTeamAlive = false;
                            break;
                        }
                    }

                    if (isOneTeamAlive)
                    {
                        //Check if all teams are dead by checking the last team
                        currTeamTurn = (currTeamTurn + 1) % GLOBALS.numTeams;

                        if (teamsHealth[currTeamTurn] <= 0)
                        {
                            //Everyone died
                            //This is the flag for the game over screen
                            currTeamTurn = -1;
                            //else nextTeamTurn stays the same
                        }

                        //Trigger GameOver state
                        gameState = GameState.GameOver;
                    }

                    //Depending on the next Teams turn, find out which soldier is next on their team
                    for (int j = 0; j < GLOBALS.teamSize; j++)
                    {
                        //go to the next player alive in that turn
                        currSoldierTurn[currTeamTurn] = (currSoldierTurn[currTeamTurn] + 1) % GLOBALS.teamSize;
                        //check if Soldier is alive before choosing it for next turn
                        if (soldiersHealth[currTeamTurn, currSoldierTurn[currTeamTurn]] > 0)
                        {
                            break;
                        }
                    }

                    // Activate movement Script for player or AI to play
                    go = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                    if(go != null)
                    {
                        go.GetComponent<PlayerMovement>().enabled = true;
                    }
                    else
                    {
                        Debug.LogError("Next Soldier in turn is dead, this state should never be reached");
                    }

                    //***************************TODO**************************
                    //Tell Camara to focus on this player

                }

                break;

            case GameState.TurnInProgress:
                //Start timer to change state
                if (!corutineStarted)
                {
                    corutineStarted = true;
                    coroutineTurnClock = SetTurnClock(GLOBALS.timePerTurn); //timePerTurn should be 30 - 120 sec
                    StartCoroutine(coroutineTurnClock);
                }

                //Check if turn suddently stops because premature death or self injure and StopCoroutine()
                if (isTurnFinished){

                    corutineStarted = false; //Reset coroutine check
                    isTurnFinished = false; //Reset check before changing state
                    gameState = GameState.TurnTransition;

                    //Stop Coroutine just in case of premature death or self injure
                    StopCoroutine(coroutineTurnClock);

                    //Deactivate movement Script for player or AI to play
                    go = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                    if (go != null)
                    {
                        go.GetComponent<PlayerMovement>().enabled = false; 
                        //else we do nothing since the player is gone anyway
                    }
                }

                break;

            case GameState.Pause:
                //***************************TODO**************************
                //Adjust timer to not being afected by the pause
                
                break;

            case GameState.LoadingScene:
                //***************************TODO**************************
                /*
                 *
                */
                break;

            case GameState.GameOver:
                //***************************TODO**************************
                /*
                 *
                */
                break;

            default:
                Debug.LogError("Invalid State reached... but HOW??!!");
                break;
        }
    }

    public IEnumerator SetGameClock(float waitTime)
    {
        gameClock = waitTime;
        Debug.Log("Timer for GAME Clock started with " + waitTime + " Seconds");
        while (true)
        {
            //***************************TODO**************************: 
            //update Clock GUI
            if (gameClock > 0)
            {
                gameClock--;//update the clock timer
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(1.0f);//wait one second
        }
        Debug.Log("Timer for GAME Clock finished");
        suddenDeath = true;
    }

    public IEnumerator SetTurnClock(float waitTime)
    {
        turnClock = waitTime;
        Debug.Log("Timer for TURN Clock started with " + waitTime + " Seconds");
        while (true)
        {
            //***************************TODO**************************: 
            //update Clock GUI
            if (turnClock > 0)
            {
                turnClock--;//update the clock timer
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(1.0f);//wait one second
        }
        Debug.Log("Timer for TURN Clock finished");
        isTurnFinished = true;
    }
}
