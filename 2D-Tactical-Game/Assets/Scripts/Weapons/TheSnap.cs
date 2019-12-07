using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSnap : MonoBehaviour
{
    public int damageToPlayer = 0;
    public int damageToProps = 0;
    public int playerPercentHealthReduction = 50;
    public int propsPercentHealthReduction = 0;
    private int ID;
    private int teamID;
    private GameObject[,] players; //[TeamID, SoldierID]
    private DamageHandler dh;
    private GameManager GM;
    
    void Start()
    {
        AudioManager.instance.Stop("The Duel");
        AudioManager.instance.Play("I_Am_Inevitable");
        Invoke("Snap", 7.5f);
        GM = GlobalVariables.Instance.GM;
        ID = GM.currSoldierTurn[GM.currTeamTurn];
        teamID = GM.currTeamTurn;
    }

    private void Snap(){
        transform.parent = null;
        foreach (GameObject player in GM.teams)
        {
            if(player == null){
                continue;
            }
            if(teamID == player.GetComponent<PlayerSettings>().teamID){
                //No damage to teamates
                continue;
            }

            dh = player.GetComponent<DamageHandler>();
            if(dh != null){
                dh.TakeDamage(damageToPlayer, damageToProps, playerPercentHealthReduction, propsPercentHealthReduction);
            }
        }
        
        //Kill player that Snapped
        if(GM.teams[teamID, ID] != null){
            dh = GM.teams[teamID, ID].GetComponent<DamageHandler>();
            if(dh != null){
                dh.TakeDamage(9999, 0, 100, 0);
            }
        }
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        Invoke("PlayDuel", 3f);
        Destroy(gameObject, 3f);
    }

    private void PlayDuel(){
        AudioManager.instance.Play("The Duel");
    }
}
