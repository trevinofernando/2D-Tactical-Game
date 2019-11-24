using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOptionsMenu : MonoBehaviour
{
    // the team object array
    public SampleTeam[] teams = new SampleTeam[8];
    private GlobalVariables GLOBALS;
    
    // object variables tied to team submenu
    public TMP_InputField[] namesTMP;
    public GameObject[] namesObjects;
    public TMP_InputField teamNameTMP;
    public TextMeshProUGUI teamTitle;
    public TextMeshProUGUI numTeamsTMP;
    public TextMeshProUGUI teamSizeTMP;
    public TextMeshProUGUI turnTimerTMP;
    public TextMeshProUGUI gameTimerTMP;
    public TextMeshProUGUI playerHealthTMP;
    public Toggle isAIToggle;

    // object variables tied to advanced submenu
    public Toggle supplyCrateToggle;
    public Dropdown teamColorIndex;
    public Dropdown gameModeIndex; // 1 is LTS, 2 is BTC. if 0 choose 1.

    // object variables tied to map submenu
    public Dropdown mapThemeDropdown;
    public Dropdown mapSizeDropdown;

    // we need to transfer these to global vars
    private int teamSize = 8;
    private int numTeams = 8;
    private float turnTimer;
    private float gameTimer;
    private int playerHealth;
    private bool enableSupplyCrates;
    private int gameMode;
    private int mapTheme;
    private int mapSize;

    public int currentTeamShowing;

    public Color[] teamColors =
    {
        new Color(000, 000, 000, 000), // 0 select color
        new Color(255, 000, 000, 255), // 1 red
        new Color(000, 000, 255, 255), // 2 blue
        new Color(000, 255, 000, 255), // 3 green
        new Color(255, 255, 000, 255), // 4 yellow
        new Color(255, 127, 000, 255), // 5 orange
        new Color(127, 000, 255, 255), // 6 purple
        new Color(063, 031, 000, 255), // 7 brown
        new Color(255, 000, 255, 255), // 8 pink
    };

    public void Start ()
    {
        GLOBALS = GlobalVariables.Instance;

        for (int teamNumber = 0; teamNumber < numTeams; teamNumber++)
        {
            teams[teamNumber] = new SampleTeam(teamSize);
            teams[teamNumber].teamName = ("Team " + (teamNumber + 1).ToString());
            teams[teamNumber].isAI = false;

            for (int playerNumber = 0; playerNumber < teamSize; playerNumber++)
            {
                teams[teamNumber].roster[playerNumber].playerName = ("Player " + (playerNumber + 1).ToString());
                namesTMP[playerNumber].text = teams[teamNumber].roster[playerNumber].playerName;
            }
        }

        // initialize team name field
        teamNameTMP.text = teams[0].teamName;
        teamTitle.text = ("Team 1:");

        // initialize numTeams slider
        numTeams = 2;
        numTeamsTMP.text = numTeams.ToString();
        
        // initialize teamSize slider
        teamSize = 4;
        teamSizeTMP.text = teamSize.ToString();
        
        // initialize the sliders
        SetPlayersPerTeam(4);
        SetTurnTimer(6);
        SetGameTimer(1);
        SetPlayerHealth(100);
    }

    public void NextTeam ()
    {
        SavePlayerFields();
        if (currentTeamShowing < numTeams - 1)
        {
            currentTeamShowing++;
            UpdateTeamNames();
        }

    }

    public void PreviousTeam ()
    {
        SavePlayerFields();
        if (currentTeamShowing > 0)
        {
            currentTeamShowing--;
            UpdateTeamNames();
        }
    }

    public void UpdateTeamNames ()
    {
        teamNameTMP.text = teams[currentTeamShowing].teamName;
        teamTitle.text = ("Team " + (currentTeamShowing + 1).ToString() + ":");
        isAIToggle.isOn = teams[currentTeamShowing].isAI;
        teamColorIndex.value = teams[currentTeamShowing].teamColorIndex;
        for (int playerNumber = 0; playerNumber < teamSize; playerNumber++)
        {
            namesTMP[playerNumber].text = teams[currentTeamShowing].roster[playerNumber].playerName;
        }
    }

    public void SavePlayerFields ()
    {
        teams[currentTeamShowing].teamName = teamNameTMP.text;
        teams[currentTeamShowing].isAI = isAIToggle.isOn;
        Debug.Log("Color is " + teamColorIndex.value);
        teams[currentTeamShowing].teamColorIndex = teamColorIndex.value;
        for (int playerNumber = 0; playerNumber < teamSize; playerNumber++)
        {
            teams[currentTeamShowing].roster[playerNumber].playerName = namesTMP[playerNumber].text;
        }
    }

    public void SetNumberOfTeams (float sliderValue)
    {
        numTeams = (int)sliderValue;
        numTeamsTMP.text = numTeams.ToString();
        if (currentTeamShowing >= numTeams)
        {
            PreviousTeam();
        }
    }

    public void SetPlayersPerTeam (float sliderValue)
    {
        teamSize = (int)sliderValue;
        teamSizeTMP.text = teamSize.ToString();
        // hide the remaining player slots
        for (int hideElement = 7; (hideElement - teamSize) >= 0; hideElement--)
        {
            namesObjects[hideElement].SetActive(false);
        }
        for (int showElement = 0; showElement < teamSize; showElement++)
        {
            namesObjects[showElement].SetActive(true);
        }
    }

    public void SetTurnTimer (float sliderValue)
    {
        turnTimer = sliderValue * 5;
        turnTimerTMP.text = (((int)turnTimer).ToString() + " seconds");
    }

    public void SetGameTimer(float sliderValue)
    {
        gameTimer = sliderValue * 5;
        gameTimerTMP.text = (((int)gameTimer).ToString() + " minutes");
    }

    public void SetPlayerHealth(float sliderValue)
    {
        playerHealth = (int)sliderValue;
        playerHealthTMP.text = (playerHealth.ToString() + " HP");
    }

    public void FindAdequateColor (int invalidTeam)
    {
        int checker = 0;

        // rotate through each color
        for (int colorIndex = 1; colorIndex < 9; colorIndex++)
        {
            //Debug.Log("Checking color " + colorIndex);
            checker = 0;
            // check each team's color to see if its taken
            for (int teamNumber = 0; teamNumber < numTeams; teamNumber++)
            {
                //Debug.Log("Checking team " + invalidTeam + " with team " + teamNumber);
                if (teamNumber == invalidTeam)
                {
                    //Debug.Log("Same team, NEXT!!!");
                    continue;
                }
                if (colorIndex == teams[teamNumber].teamColorIndex)
                {
                    //Debug.Log("Same color, TOSS IT OUT!!");
                    checker++;
                    break;
                }
                //Debug.Log("made it through the for loop");
            }

            //Debug.Log("checking the checker of size " + checker);
            if (checker == 0)
            {
                //Debug.Log("Looks like we found a match, bois. colorIndex is " + colorIndex);
                teams[invalidTeam].teamColorIndex = colorIndex;
                break;
            }
            //Debug.Log("changing the color now.");
        }
    }

    public void CheckForColorConflicts ()
    {
        // needs to check every team to see if there is a duplicate color choice
        for (int alphaTeam = 0; alphaTeam < numTeams; alphaTeam++)
        {
            if (teams[alphaTeam].teamColorIndex == 0)
                FindAdequateColor(alphaTeam);

            for (int betaTeam = alphaTeam + 1; betaTeam < numTeams; betaTeam++)
            {
                //Debug.Log("Checking team " + alphaTeam + " with team " + betaTeam);
                if (teams[alphaTeam].teamColorIndex == teams[betaTeam].teamColorIndex)
                {
                    //Debug.Log("SAME COLOR!! Fixing team " + betaTeam);
                    FindAdequateColor(betaTeam);
                }
            }
        }
    }

    public void UpdateEverythingElse ()
    {
        // game mode, supply crates
        gameMode = gameModeIndex.value;
        if (gameMode == 0)
            gameMode++;

        enableSupplyCrates = supplyCrateToggle.isOn;

        // map theme, map size
        mapTheme = mapThemeDropdown.value;
        if (mapTheme == 0)
            mapTheme++;

        mapSize = mapSizeDropdown.value;
        if (mapSize == 0 || mapSize == 2)
            mapSize++;

        //Debug.Log(mapTheme.ToString());
        //Debug.Log(mapSize.ToString());
    }

    public void SetOfficialTeamColors ()
    {
        for (int teamNumber = 0; teamNumber < numTeams; teamNumber++)
        {
            teams[teamNumber].teamColor = teamColors[teams[teamNumber].teamColorIndex];
        }
    }
    public void BeginGame ()
    {
        //Debug.Log("START MATCH");
        // check for color conflicts
        //Debug.Log("Checking for color conflicts...");
        CheckForColorConflicts();
        //Debug.Log("Successfully resolved any color conflicts.");

        // set team colors officially
        SetOfficialTeamColors();
        //Debug.Log("Successfully established official team colors.");

        // fetch the advanced options that haven't been updated yet
        UpdateEverythingElse();
        //Debug.Log("Successfully updated everything else.");

        GLOBALS.numTeams = numTeams;
        GLOBALS.teamSize = teamSize;
        GLOBALS.healthPerAvatar = playerHealth;
        GLOBALS.timePerTurn = turnTimer;
        GLOBALS.TimePerGame = gameTimer * 60;   // convert seconds to minutes.
        GLOBALS.gameMode = gameMode;

        

        GLOBALS.teamColors = new Color[numTeams];
        GLOBALS.isTeamAI = new bool[numTeams];
        for (int teamNumber = 0; teamNumber < numTeams; teamNumber++)
        {
            GLOBALS.teams[teamNumber] = teams[teamNumber];
            GLOBALS.teamColors[teamNumber] = teams[teamNumber].teamColor;
            GLOBALS.isTeamAI[teamNumber] = teams[teamNumber].isAI;
        }

        //Debug.Log("Successfully updated Globals");

        // swap the scenes
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // transfer everything to global variables
        // need to transfer:
        // - number of human teams / AI teams (can reference a method that establishes this)
        // - all of the team names AND player names in the form of a 2D array (can reference teams[] in a method)
        // - the team colors (for loop, teams[i].teamColor)
    }
}
