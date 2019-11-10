using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Animator anim;
    public Transform weaponPivot;
    public Weapon weaponScript;

    //References to other scripts
    [System.NonSerialized]
    public GameManager GM;
    private Rigidbody2D rb;
    private PlayerSettings ps;
    private GlobalVariables GLOBALS;
    private WeaponController weaponContr;

    private List<Transform> targets = new List<Transform>();

    //Temporal variables
    private RaycastHit2D hit;
    private Vector2 origin;
    private Vector2 direction;
    private Vector3 dir3;
    private Transform target;

    public AIState curState = AIState.WaitingForTurn;

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
        //Return if this player isn't an AI player
        if(!ps.iAmAI){
            return;
        }
        switch (curState)
        {
            case(AIState.WaitingForTurn):
                //Change state when player starts turn
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

                //We now have all possible targets, now we need to sort them:

                //sort by distance to player ascending
                targets = targets.OrderBy(x => Vector2.Distance(this.transform.position,x.position)).ToList();
                //sort by teams health decending
                targets = targets.OrderBy(x => -GM.teamsHealth[x.GetComponent<PlayerSettings>().teamID]).ToList();
                //sort by soldiers health decending
                targets = targets.OrderBy(x => -x.GetComponent<DamageHandler>().health).ToList();

                foreach (Transform tar in targets){
                    //Find direction vector from self to target and normalize it to length 1
                    dir3 = tar.position - transform.position;
                    dir3.Normalize();
                    dir3.z = 0;

                    //Direct cast from Vector3s to Vector2s
                    origin = transform.position;
                    direction = dir3;

                    //Send a ray and store the information in hit
                    hit = Physics2D.Raycast(origin, direction);
                    
                    //Check if we are hitting a player and that player is not in our team
                    if(hit.transform.tag == "Player" && ps.teamID != hit.transform.GetComponent<PlayerSettings>().teamID){
                        target = hit.transform;
                        break;
                    }
                }

                if(target == null){
                    break;
                }
                //Find diff in x and y
                float xDiff = target.position.x - weaponContr.weaponPivot.position.x;
                float yDiff = target.position.y - weaponContr.weaponPivot.position.y;

                //Calculate angle to rotate with 2D tangent formula and change from radians to degrees
                float zRotation = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
                
                curState = AIState.Aiming;
                break;
            case(AIState.Moving):
                //***************************TODO**************************

                break;
            case(AIState.Aiming):
                //***************************TODO**************************
                //move 
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
