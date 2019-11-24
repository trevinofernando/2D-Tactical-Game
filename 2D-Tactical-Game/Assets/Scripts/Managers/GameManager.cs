using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public CameraController cam;
    public GameObject soldierPrefab;
    public GameObject planeHealerPrefab;
    public GameObject planeCratesPrefab;
    public Vector3 PlaneSpawnPoint;
    public Vector3 spawnOffset = new Vector3(-18, 10, 0);
    private Vector3[] spawnLocations;
    public MapInitializer mapInitializer;
    public Canvas gameOverCanvas;
    public Canvas pauseMenuCanvas;
    public SunScript sun;
    public GameObject projectile;
    public EnvironmentManager environmentManager;

    //Teams related variables
    [System.NonSerialized]
    public GameObject[,] teams; //[TeamID, SoldierID]
    public int deadTeamsCounter = 0; 
    public int[] teamsHealth; //[TeamID]
    public int[,] soldiersHealth; //[TeamID, SoldierID]
    public int currTeamTurn = 0; //index of current teams turn
    private int currTeamTurnWhenPause; //index of current teams turn when the game was paused
    public int[] currSoldierTurn; //[TeamID] to keep track of which soldier is next depending on team

    //Time related variables
    public float turnClock = 0f;
    private float turnClockWhenPause;
    public float gameClock = 0f;
    private float gameClockWhenPause;
    public float timeWatchingDamage = 2f;
    private bool coroutineStarted = false;
    private bool coroutineEnded = false;
    private IEnumerator coroutineTurnClock;
    private IEnumerator coroutineGameClock;

    //STATES
    public GameState gameState = GameState.LoadingScene;
    public GameState prevGameState = GameState.LoadingScene;
    public bool suddenDeath = false;
    public bool isTurnFinished = false;
    private bool isTurnFinishedWhenPaused;
    public bool isOneTeamAlive = false;
    public bool stateSaved = false;
    public bool hazardsWereCalled = false;
    public int winningTeamID;
    public string winningTeamName;
    public bool isArsenalOpen = false;

    //Temporary Variables
    private const int GauntletCursorCode = 0;
    private GameObject go;
    private PlaneManager pm;
    private GameManager thisGM;
    private PlayerSettings ps;

    public enum GameState
    {
        LoadingScene,
        TurnInProgress,
        TurnTransition,
        WhatchingShot,
        WhatchingDamage,
        Pause,
        GameOver
    }

    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        
        gameState = GameState.LoadingScene;
        isTurnFinished = false; //Set initial state
        Time.timeScale = 1.0f; //Set normal time for game to run

        PlaneSpawnPoint  = new Vector3(GLOBALS.mapXMax + 50f, GLOBALS.mapYMax + 20f, 0);

        //Initialize array to hold each soldier object team[Team][Avatar]
        teams = new GameObject[GLOBALS.numTeams, GLOBALS.teamSize];
        
        //Initialize array to hold health of each soldier of each team
        soldiersHealth = new int[GLOBALS.numTeams, GLOBALS.teamSize];

        //Initialize array to hold health of each team
        teamsHealth = new int[GLOBALS.numTeams]; //default value is already 0

        //Initialize array to keep track of turn of soldiers, since each team will have a different order
        currSoldierTurn = Enumerable.Repeat(-1, GLOBALS.numTeams).ToArray(); //initialize Array with -1
        currTeamTurn = -1; //-1 since we always add 1 when starting a turn

        //Get reference to this GameManager component on this object
        thisGM = gameObject.GetComponent<GameManager>();
        GLOBALS.GM = thisGM;

        /****** TODO: Call Map generator and get spawn locations *******/
        spawnLocations = mapInitializer.GenerateSpawns();
        //mapInitializer = gameObject.GetComponent<MapInitializer>();

        int count = 0;

        //Create Teams
        for (int i = 0; i < GLOBALS.numTeams; i++)
        {
            List<int> subArsenal = new List<int>();
            for(int weaponCode = 0; weaponCode < GLOBALS.arsenalAmmo.Count; weaponCode++){
                subArsenal.Add(GLOBALS.arsenalAmmo[weaponCode]);
            }
            PlayerSettings.arsenalAmmo.Add(subArsenal);

            for(int j = 0; j < GLOBALS.teamSize; j++)
            {
                //Spawn Player
                teams[i , j] = Instantiate(soldierPrefab, spawnLocations[count++], transform.rotation);

                //teams[i, j].GetComponent<AIController>().GM = thisGM;
                
                ps = teams[i, j].GetComponent<PlayerSettings>();
                ps.gameManager = thisGM; //Self reference to each soldier to keep contact
                ps.cam = cam; //pass camera reference
                ps.SetColor(GLOBALS.teamColors[i]);
                ps.teamID = i; //set team id (unique for each team)
                ps.ID = j; //set player id (unique inside each team)
                ps.iAmAI = GLOBALS.isTeamAI[i]; //Mark Soldier as AI or not

                if(GLOBALS.teams[i] != null)
                {
                    ps.nameGiven = GLOBALS.teams[i].roster[j].playerName;
                }
                else
                {
                    ps.nameGiven = GLOBALS.teamNames[i, j];//GLOBALS.teams[i].roster[j].playerName;
                }

                //Set Health of player
                teams[i, j].GetComponent<DamageHandler>().SetHealth(GLOBALS.healthPerAvatar);
                soldiersHealth[i, j] = GLOBALS.healthPerAvatar;
                
                //Add Heath of this soldier to the team health
                teamsHealth[i] += GLOBALS.healthPerAvatar;
                
                //Deactivate movement script.
                teams[i, j].GetComponent<PlayerSettings>().isMyTurn = false;
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
        coroutineStarted = false;
        gameState = GameState.TurnTransition;
    }


    void Update()
    {

        //Check if game is over
        deadTeamsCounter = 0;
        winningTeamID = -1;
        for (int i = 0; i < GLOBALS.numTeams; i++)
        {
            if(teamsHealth[i] <= 0)
            {
                deadTeamsCounter++;
            }
            else if(winningTeamID == -1 || teamsHealth[i] > teamsHealth[winningTeamID])
            {
                winningTeamID = i;
            }
        }

        //Check if there is 1 or less players alive
        if(deadTeamsCounter >= GLOBALS.numTeams - 1)
        {
            if(gameState == GameState.GameOver || gameState == GameState.LoadingScene)
            {
                gameState = GameState.LoadingScene; //change state to prevent repetitive calls to gameOver
            }
            else
            {
                gameState = GameState.GameOver;
            }
        }

        //Check for Pause input if we are not in the pause state
        if(Input.GetKeyDown(KeyCode.P) && gameState != GameState.Pause){
            prevGameState = gameState; //save prevGameState
            gameState = GameState.Pause; //Change to Pause state
            pauseMenuCanvas.gameObject.SetActive(true); //enable Pause Menu
        }

        //State Machine
        switch (gameState)
        {
            case GameState.TurnTransition:
                // Start timer for next turn.
                if (!coroutineStarted)
                {
                    coroutineStarted = true;
                    coroutineTurnClock = SetTurnClock(GLOBALS.timeBetweenTurns); //time between turns should be 1 to 5 sec
                    StartCoroutine(coroutineTurnClock);

                    
                }

                if (isTurnFinished)
                {
                    coroutineStarted = false; //Reset coroutine check
                    isTurnFinished = false; //Reset check before changing state
                    gameState = GameState.TurnInProgress;//change State

                    //Find out which team is next in turn. By looping thru the number of teams - 1
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

                    // Activate movement Script for player or AI to play and tell camera
                    go = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                    if(go != null)
                    {
                        //**************TODO*********************************
                        //Check for Human/AI

                        go.GetComponent<PlayerSettings>().isMyTurn = true;

                        //Tell camera which player is next in turn
                        cam.soldier = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                        cam.shouldFollowTarget = true;
                        cam.SetZoom(10f);
                    }
                    else
                    {
                        Debug.LogError("Next Soldier in turn is dead, this state should never be reached");
                    }


                }

                break;

            case GameState.TurnInProgress:
                //Start timer to change state
                if (!coroutineStarted)
                {
                    coroutineStarted = true;
                    coroutineTurnClock = SetTurnClock(GLOBALS.timePerTurn); //timePerTurn should be 30 - 120 sec
                    StartCoroutine(coroutineTurnClock);
                }

                //Check if turn suddenly stops because premature death or self injure and StopCoroutine()
                if (isTurnFinished){
                    
                    coroutineStarted = false; //Reset coroutine check

                    //Stop Coroutine just in case of premature death or self injure
                    StopCoroutine(coroutineTurnClock);

                    //If there is a projectile, follow it
                    if(projectile != null){
                        cam.soldier = projectile;
                        cam.shouldFollowTarget = true; //follow projectile
                        gameState = GameState.WhatchingShot;
                    }else{
                        gameState = GameState.TurnTransition;
                    }

                    //Deactivate movement Script for player or AI to play
                    go = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                    if (go != null)
                    {
                        ps = go.GetComponent<PlayerSettings>();
                        if(ps != null)
                            ps.EndTurn(); //call clean up function for players end turn
                        //else 
                            //we do nothing since the player is gone anyway
                    }

                    isTurnFinished = false; //Reset check before changing state. THIS must be CALLED at the END of this state!
                }

                break;

            case GameState.WhatchingShot:
                //Once projectile is null, change state
                if(projectile == null){
                    gameState = GameState.WhatchingDamage;
                }else if(cam.soldier == null){
                    cam.soldier = projectile; //this is in case the initial projectile releases more projectiles
                }
                break;

            case GameState.WhatchingDamage:
                 //Start timer to change state
                if (!coroutineStarted)
                {
                    coroutineStarted = true;
                    coroutineTurnClock = SetTurnClock(timeWatchingDamage); //timePerTurn should be 30 - 120 sec
                    StartCoroutine(coroutineTurnClock);
                    coroutineEnded = false;
                }

                if (coroutineEnded)
                {
                    coroutineStarted = false; //Reset coroutine check
                    isTurnFinished = false; //Reset check before changing state
                    gameState = GameState.TurnTransition;
                }
                break;

            case GameState.Pause:
                //Adjust timer to not being affected by the pause and save the current game state
                if(!stateSaved){
                    stateSaved = true; //Update flag
                    Time.timeScale = 0.0f; //Stop time

                    turnClockWhenPause = turnClock; //Save turnClock
                    StopCoroutine(coroutineTurnClock); //And stop the turn Coroutine

                    gameClockWhenPause = gameClock; //Save gameClock
                    StopCoroutine(coroutineGameClock); //And stop the game Coroutine

                    isTurnFinishedWhenPaused = isTurnFinished; //Save turn state
                    currTeamTurnWhenPause = currTeamTurn; //Save current Team Turn

                    //Check for out of bounce errors
                    if(currTeamTurn < 0 || currSoldierTurn[currTeamTurn] < 0)
                        return;

                    //Force player to change to the gauntlet weapon
                    go = teams[currTeamTurn, currSoldierTurn[currTeamTurn]];
                    if (go != null && !GLOBALS.isTeamAI[currTeamTurn])
                    {
                        WeaponController wc = go.GetComponent<WeaponController>();
                        if(wc != null)
                            wc.ChangeWeapon((int)WeaponCodes.Gauntlet); // Weapon Code for the gauntlet is 0
                    }
                    
                }
                else if (Input.GetKeyDown(KeyCode.P) || !pauseMenuCanvas.gameObject.activeInHierarchy){
                    stateSaved = false; //reset flag
                    
                    isTurnFinished = isTurnFinishedWhenPaused; //Recover this variable state

                    //Check if the coroutine was in progress or not, this only happens in exactly 1 frame
                    if(!isTurnFinishedWhenPaused){
                        //Resume gameClock Timer
                        coroutineGameClock = SetGameClock(gameClockWhenPause);
                        StartCoroutine(coroutineGameClock);
                        //Resume turnClock Timer
                        coroutineTurnClock = SetTurnClock(turnClockWhenPause); //time between turns should be 1 to 5 sec
                        StartCoroutine(coroutineTurnClock);
                    }

                    Time.timeScale = 1.0f; //Unfreeze time
                    gameState = prevGameState; //Resume the game
                    pauseMenuCanvas.gameObject.SetActive(false);//disable Pause Menu
                }
                break;

            case GameState.LoadingScene:
                //***************************TODO**************************
                /*
                 *
                */
                break;

            case GameState.GameOver:

                Debug.Log("Game Over");
                
                //Get reference to GameOverScreen component
                GameOverScreen gos = gameOverCanvas.GetComponent<GameOverScreen>();
                
                //pass the teamID
                gos.winningTeamID = winningTeamID;

                if(winningTeamID != -1){
                    //Pass the name of the winning team if there is one
                    if(GLOBALS.teams[winningTeamID] != null)
                    {
                        gos.winningTeamName = GLOBALS.teams[winningTeamID].teamName;
                    }

                    //pass winning team color
                    gos.winTeamColor = GLOBALS.teamColors[winningTeamID];
                }

                //Enable the game over canvas to trigger the end of the game
                gameOverCanvas.gameObject.SetActive(true);

                break;

            default:
                Debug.LogError("Invalid State reached... but HOW??!!");
                break;
        }
    }

    public IEnumerator SetGameClock(float waitTime)
    {
        gameClock = waitTime;
        //Debug.Log("Timer for GAME Clock started with " + waitTime + " Seconds");
        while (true)
        {
            
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
        //Debug.Log("Timer for GAME Clock finished");
        suddenDeath = true;
    }

    public IEnumerator SetTurnClock(float waitTime)
    {
        turnClock = waitTime;
        //Debug.Log("Timer for TURN Clock started with " + waitTime + " Seconds");
        while (true)
        {
            yield return new WaitForSeconds(1.0f);//wait one second

            if (turnClock > 0)
            {
                turnClock--;//update the clock timer

                if(!hazardsWereCalled && gameState == GameState.TurnTransition && (int) turnClock == (int) waitTime - 2){
                    Random.InitState(System.DateTime.Now.Millisecond);
                    hazardsWereCalled = true; //set flag
                    if(environmentManager.isReady)
                    {
                        environmentManager.DeployHazard();
                    }
                    //Chose random player
                    //go = teams[Random.Range(0, GLOBALS.numTeams), Random.Range(0, GLOBALS.teamSize)];
                }
                
            }
            else
            {
                //Time reached 0, break while loop
                break;
            }
        }
        //Debug.Log("Timer for TURN Clock finished");
        hazardsWereCalled = false;
        isTurnFinished = true;
        coroutineEnded = true;
    }
}
