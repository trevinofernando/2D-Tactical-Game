using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeaponMenu : MonoBehaviour
{
    public GameObject weaponCanvas;
    public GameManager gm;
    public GameObject invButton;
    public GameObject pauseButton;

    private GameObject currPlayer;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Can't open menu when a turn is not in progress or if it's an AI turn
        if (gm.gameState == GameManager.GameState.TurnInProgress && !GlobalVariables.Instance.isTeamAI[gm.currTeamTurn])
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!weaponCanvas.gameObject.activeInHierarchy)
                {
                    weaponCanvas.SetActive(true);
                    invButton.SetActive(false);
                    pauseButton.SetActive(false);
                }
                else
                {
                    weaponCanvas.SetActive(false);
                    invButton.SetActive(true);
                    pauseButton.SetActive(true);
                }
            }
            else if (!weaponCanvas.gameObject.activeInHierarchy)
            {
                invButton.SetActive(true);
                pauseButton.SetActive(true);
            }
        }
        else if (gm.gameState == GameManager.GameState.Pause)
        {
            weaponCanvas.SetActive(false);
            invButton.SetActive(false);
            pauseButton.SetActive(false);
        }
        else
        {
            weaponCanvas.SetActive(false);
            invButton.SetActive(true);
            pauseButton.SetActive(true);
        }
    }

    void OnMouseOver ()
    {
        // this is never used anywhere btw
        currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
        if(!GlobalVariables.Instance.isTeamAI[gm.currTeamTurn]){
            currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)WeaponCodes.Gauntlet);        // change weapon to selected button
        }
    }
}
