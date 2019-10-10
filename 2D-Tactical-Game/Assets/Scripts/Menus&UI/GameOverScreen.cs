using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{

    public int winningTeamID;
    public string winningTeamName;
    public Color winTeamColor;
    public TextMeshProUGUI winnerTextHolder;

    void Start()
    {
        Debug.Log("Game Over Canvas active");
        Debug.Log(winningTeamID);
        Debug.Log(winningTeamName);
        Debug.Log(winTeamColor);
        if(winningTeamName != "")
        {
            winnerTextHolder.text = winningTeamName;
        }
        else
        {
            winnerTextHolder.text = "Team #" + winningTeamID.ToString();
        }
        winnerTextHolder.color = winTeamColor;

    }


    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            //Back to the menu
            SceneManager.LoadScene(0);
        }
    }
}
