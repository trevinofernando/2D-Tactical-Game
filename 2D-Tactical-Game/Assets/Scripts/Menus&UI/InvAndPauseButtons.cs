using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvAndPauseButtons : MonoBehaviour
{
    public GameManager gm;

    private GameObject currPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver ()
    {
        //Debug.Log("is this actually working?");
        currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
        currPlayer.GetComponent<WeaponController>().ChangeWeapon(0);        // change weapon to selected button
    }

    public void PauseGame ()
    {
        gm.prevGameState = gm.gameState; //save prevGameState
        gm.gameState = GameManager.GameState.Pause; //Change to Pause state
        gm.pauseMenuCanvas.gameObject.SetActive(true); //enable Pause Menu
    }
}
