using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvAndPauseButtons : MonoBehaviour
{
    public GameManager gm;

    private GameObject currPlayer;
    private Vector3 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Buttons"), LayerMask.NameToLayer("Ground"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Buttons"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Buttons"), LayerMask.NameToLayer("Default"), true);
    }

    // Update is called once per frame
    void Update()
    {
        //Only run code if a turn is in progress
        if(gm.gameState == GameManager.GameState.TurnInProgress){
            //Stop player from changing the weapon of an AI player
            if(GlobalVariables.Instance.isTeamAI[gm.currTeamTurn]){
                Debug.Log("AI, can't chnge weapon");
                return; //Not a human turn, so leave
            }
            //Find Mouse position in monitor and then translate that to a point in the world
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Debug.Log("passed");
            //Check if mouse is in the boundaries of the button
            if (Mathf.Abs(transform.position.x - mousePosition.x) < 3f && Mathf.Abs(transform.position.y - mousePosition.y) < 1f){
                currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
                currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)WeaponCodes.Gauntlet);        // change weapon to selected button
            }
        }
    }
/* 
    void OnMouseOver ()
    {
        //Debug.Log("is this actually working?");
        if(gm.gameState == GameManager.GameState.TurnInProgress){
            currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
            currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)WeaponCodes.Gauntlet);        // change weapon to selected button
        }
    }
*/
    public void PauseGame ()
    {
        gm.prevGameState = gm.gameState; //save prevGameState
        gm.gameState = GameManager.GameState.Pause; //Change to Pause state
        gm.pauseMenuCanvas.gameObject.SetActive(true); //enable Pause Menu
    }
}
