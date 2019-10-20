using System.IO.Pipes;
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
        switch (curState)
        {
            case(AIState.WaitingForTurn):
                if(ps.isMyTurn){
                    curState = AIState.PickingTarget;
                }
                break;
            case(AIState.PickingTarget):
                //***************************TODO**************************
                
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
