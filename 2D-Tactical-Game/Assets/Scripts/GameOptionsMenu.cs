using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOptionsMenu : MonoBehaviour
{
 
    public SampleTeam[] teams = new SampleTeam[8];
    public TMP_InputField[] namesTMP;
    public GameObject[] namesObjects;
    public TMP_InputField teamTMP;
    public TextMeshProUGUI teamTitle;
    public TextMeshProUGUI numTeamsTMP;
    public TextMeshProUGUI teamSizeTMP;
    public TextMeshProUGUI turnTimerTMP;
    public TextMeshProUGUI gameTimerTMP;
    public TextMeshProUGUI playerHealthTMP;
    public Toggle isAIToggle;
    public Toggle supplyCrateToggle;
    public Dropdown teamColorIndex;
    public Dropdown gameModeIndex; // 1 is LTS, 2 is BTC. if 0 choose 1.
    public int teamSize = 8;
    public int numTeams = 8;
    public int currentTeamShowing;
    public int turnTimer;
    public int gameTimer;
    public int playerHealth;
    public int enableSupplyCrates;
    public int gameMode;

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

        teamTMP.text = teams[0].teamName;
        teamTitle.text = ("Team 1:");
        numTeams = 2;
        numTeamsTMP.text = numTeams.ToString();
        teamSize = 4;
        teamSizeTMP.text = teamSize.ToString();
        SetPlayersPerTeam(4);
        SetTurnTImer(6);
        SetGameTImer(1);
        SetPlayerHealth(100);
    }

    public void NextTeam ()
    {
        Debug.Log("Next");
        SavePlayerFields();
        if (currentTeamShowing < numTeams - 1)
        {
            currentTeamShowing++;
            UpdateTeamNames();
        }

    }

    public void PreviousTeam ()
    {
        Debug.Log("Back");
        SavePlayerFields();
        if (currentTeamShowing > 0)
        {
            currentTeamShowing--;
            UpdateTeamNames();
        }
    }

    public void UpdateTeamNames ()
    {
        teamTMP.text = teams[currentTeamShowing].teamName;
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
        teams[currentTeamShowing].teamName = teamTMP.text;
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
        Debug.Log(numTeams);
        if (currentTeamShowing >= numTeams)
        {
            PreviousTeam();
        }

    }

    public void SetPlayersPerTeam (float sliderValue)
    {
        teamSize = (int)sliderValue;
        teamSizeTMP.text = teamSize.ToString();
        Debug.Log(teamSize);
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

    public void SetTurnTImer (float sliderValue)
    {
        turnTimer = (int)sliderValue * 5;
        turnTimerTMP.text = (turnTimer.ToString() + " seconds");
    }

    public void SetGameTImer(float sliderValue)
    {
        gameTimer = (int)sliderValue * 5;
        gameTimerTMP.text = (gameTimer.ToString() + " minutes");
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
            checker = 0;
            // check each team's color to see if its taken
            for (int teamNumber = 0; teamNumber < numTeams; teamNumber++)
            {
                if (teamNumber == invalidTeam)
                {
                    continue;
                }
                if (teams[invalidTeam].teamColor == teams[teamNumber].teamColor)
                {
                    checker++;
                    break;
                }
            }

            if (checker == 0)
            {
                teams[invalidTeam].teamColor = teamColors[colorIndex];
            }

        }
    }

    public void BeginGame ()
    {
        Debug.Log("START MATCH");
        // transfer everything to global variables

        // this will be the call to transfer all the valiables to Global Vars
        // need to transfer:
        // - number of teams
        // - number of human teams / AI teams
        // - players per team
        // - health per player
        // - all of the team names AND player names in the form of a 2D array
        // - the team colors
        // - time per turn
        // - time of entire game
        // - *time between turns? probably not
        //
        // NEED TO CHECK FOR DUPLICATE COLORS OR UNSELECTED COLORS AND CHOOSE ACCORDINGLY
    }
}
