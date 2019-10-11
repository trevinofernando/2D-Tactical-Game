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
        //if not empty name show that name, else show team number
        if(winningTeamName != "")
        {
            winnerTextHolder.text = winningTeamName;
        }
        else
        {
            winnerTextHolder.text = "Team #" + winningTeamID.ToString();
        }
        //set the text to be the color of the winning team 
        winnerTextHolder.color = winTeamColor;

    }


    void Update()
    {
        //check for escape key to go back to the menu
        if(Input.GetKey(KeyCode.Escape))
        {
            //Back to the menu
            SceneManager.LoadScene(0);
        }
    }
}
