using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    public GameManager gm;
    public int weaponIndex;
    public GameObject weaponMenu;

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
        currPlayer = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];  // find the current player
        currPlayer.GetComponent<WeaponController>().ChangeWeapon(0);        // change weapon to selected button
    }
}
