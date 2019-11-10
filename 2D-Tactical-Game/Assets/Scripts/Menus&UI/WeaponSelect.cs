using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public GameManager gm;
    public int weaponIndex;
    public GameObject weaponMenu;
    public TextMeshProUGUI ammo;

    private GameObject currPlayer;


    // Start is called before the first frame update
    void Start()
    {
        // update this sprite (maybe do in global unsire!)

        // figure out status of menu
        

    }

    // Update is called once per frame
    void Update()
    {
        // this doesnt work here because the obj is disabled when you turn it off lol
        
        
    }

    public void CallWeaponSwap()
    {
        // call ChangeWeapon
        currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
        currPlayer.GetComponent<WeaponController>().ChangeWeapon(weaponIndex);        // change weapon to selected button
    }

    public void OnEnable()
    {
        if(gm.gameState == GameManager.GameState.TurnInProgress){
            gm.isArsenalOpen = true;
            currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
            ammo.SetText(currPlayer.GetComponent<PlayerSettings>().AmmoCount(weaponIndex).ToString("00"));
            currPlayer.GetComponent<WeaponController>().ChangeWeapon((int)WeaponCodes.Gauntlet);        // change weapon to selected button
        }else{
            //Can't change weapons when no turn is in progress
            weaponMenu.gameObject.transform.GetChild(0).gameObject.SetActive(false); //disable arsenal Menu
        }
    }

    private void OnDisable() {
        gm.isArsenalOpen = false;
    }
}
