using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    
    public GameManager GM;
    public Animator anim;
    private Rigidbody2D rb;
    private PlayerSettings ps;
    private GlobalVariables GLOBALS;
    private WeaponController weaponContr;

    private List<Transform> targets = new List<Transform>();

    private AIState curState = AIState.WaitingForTurn;


    public enum AIState
    {
        WaitingForTurn,
        PickingTarget,
        Moving,
        Aiming,
        Shooting,
        Pause
    }

    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        //Get reference to rigidbody of this player object
        rb = GetComponent<Rigidbody2D>();
        //Get reference to PlayerSettings component of this player object
        ps = GetComponent<PlayerSettings>();
        //Get reference to WeaponController component of this player object
        weaponContr = GetComponent<WeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!ps.isMyTurn)
            return; //not my turn, so exit
        
        switch (curState)
        {
            case(AIState.WaitingForTurn):
                if(ps.isMyTurn){
                    curState = AIState.PickingTarget;
                }
                break;
            case(AIState.PickingTarget):
                for(int i = 0; i < GLOBALS.numTeams; i++){
                    //Check if this team is alive, else skip it
                    if(GM.teamsHealth[i] <= 0)
                        continue; //team is dead, move on
                    
                    //Check if this team is the AI team
                    if(ps.teamID == i)
                        continue; //Don't target allies
                    
                    for(int j = 0; j < GLOBALS.teamSize; j++){
                        //Check if this Soldier is alive, else skip it
                        if(GM.soldiersHealth[i,j] <= 0)
                            continue; //Soldier is dead, move on
                        
                        //Add Soldier to possible targets to shoot
                        targets.Add(GM.teams[i,j].transform);

                        
                    }
                }

                //We now have all possible targets, now we need to sort them
                //***************************TODO**************************
                //Use Insert on above forloop to sort

                break;
            case(AIState.Moving):
                //***************************TODO**************************

                break;
            case(AIState.Aiming):
                //***************************TODO**************************

                break;
            case(AIState.Shooting):
                //***************************TODO**************************

                break;
            case(AIState.Pause):
                //***************************TODO**************************

                break;

            default:
                Debug.LogError("Invalid State reached... but HOW??!!");
                break;
        }
    }
}
