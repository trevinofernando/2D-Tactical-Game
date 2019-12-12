using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public GameManager gm;
    public WeaponCodes wc;
    public GameObject weaponMenu;
    public TextMeshProUGUI ammo;
    public Button btn;

    private GameObject currPlayer;
    private Color red = new Color32(150,0,0,255);
    private Color black = new Color32(0,0,0,255);

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
                
                int ammoCount = currPlayer.GetComponent<PlayerSettings>().AmmoCount((int)wc);
                ammo.SetText(ammoCount.ToString("00"));
                
                var color = btn.colors;
                if(ammoCount <= 0){
                    color.normalColor = red;
                }else{
                    color.normalColor = black;
                }
                btn.colors = color;

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
