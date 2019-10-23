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

    private void OnEnable() {
        if(winnerTextHolder == null){
            //This can happen if another canvas in enabled and this winnerTextHolder is disabled
            return;
        }
        if(winningTeamID == -1){
            winnerTextHolder.text = "DRAW";
        }else{
            //if not empty name show that name, else show team number
            if(winningTeamName != "")
            {
                winnerTextHolder.text = winningTeamName;
            }
            else
            {
                winnerTextHolder.text = "Team #" + (winningTeamID + 1).ToString();
            }
            //set the text to be the color of the winning team 
            winnerTextHolder.color = winTeamColor;
        }
    
    }

    void Update()
    {
        /*
        if(!gameObject.enabled)
            return;
        */

        //check for escape key to go back to the menu
        if(Input.GetKey(KeyCode.Escape))
        {
            //Back to the menu
            SceneManager.LoadScene(0);
        }
    }
}
