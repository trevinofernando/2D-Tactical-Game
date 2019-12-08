using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public GameManager gm;
    public WeaponCodes wc;
    public GameObject weaponMenu;
    public TextMeshProUGUI ammo;

    private GameObject currPlayer;

    public void CallWeaponSwap()
    {
        // call ChangeWeapon
        currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
        currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)wc);        // change weapon to selected button
    }

    public void OnEnable()
    {
        if(gm.gameState == GameManager.GameState.TurnInProgress){
            gm.isArsenalOpen = true;
            if(!GlobalVariables.Instance.isTeamAI[gm.currTeamTurn]){
                currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
                ammo.SetText(currPlayer.GetComponent<PlayerSettings>().AmmoCount((int)wc).ToString("00"));
                currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)WeaponCodes.Gauntlet);        // change weapon to gauntlet
            }
        }else{
            //Can't change weapons when no turn is in progress
            weaponMenu.gameObject.transform.GetChild(0).gameObject.SetActive(false); //disable arsenal Menu
        }
    }

    private void OnDisable() {
        gm.isArsenalOpen = false;
    }
}
