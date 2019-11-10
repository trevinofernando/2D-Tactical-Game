using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Animator anim;
    public Weapon weaponScript;

    //References to other scripts
    [System.NonSerialized]
    public GameManager GM;
    private Rigidbody2D rb;
    private PlayerSettings ps;
    private GlobalVariables GLOBALS;
    private WeaponController weaponContr;

    private List<Transform> targets = new List<Transform>();
    private float degreesOfDeadZone;

    //Temporal variables
    private RaycastHit2D hit;
    private Vector2 origin;
    private Vector2 direction;
    private Vector3 dir3;
    private Transform target;
    private float xDiff;
    private float yDiff;
    private float tmp;
    private float zRotation;
    private float currentRotation;
    private bool coroutineFinished = true;

    public AIState curState = AIState.WaitingForTurn;

    public enum AIState
    {
        WaitingForTurn,
        PickingTarget,
        tryingStraightShot,
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
        degreesOfDeadZone = weaponContr.degreesOfDeadZone;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Return if this player isn't an AI player
        if(!ps.iAmAI){
            curState = AIState.WaitingForTurn;//Just in case something external ends the AI turn
            return;
        }
        switch (curState)
        {
            case(AIState.WaitingForTurn):
                //Change state when player starts turn
                if(ps.isMyTurn){
                    curState = AIState.Pause;
                    StartCoroutine(ChangeStateIn(AIState.PickingTarget, 3f));
                }
                break;
            case(AIState.PickingTarget):
                //Reset List
                targets.Clear(); 

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
                    dir3 = tar.position - weaponContr.weaponPivot.transform.position;
                    dir3.Normalize();
                    dir3.z = 0;

                    //Direct cast from Vector3s to Vector2s
                    origin = weaponContr.weaponPivot.transform.position;
                    direction = dir3;

                    //Send a ray and store the information in hit
                    hit = Physics2D.Raycast(origin + direction*1.5f, direction);
                    //Check if we are hitting a player and that player is not in our team
                    if(hit.transform.tag == "Player" && ps.teamID != hit.transform.GetComponent<PlayerSettings>().teamID){
                        Debug.Log("AI Player says: Found a Target!!");
                        target = hit.transform;
                        break;
                    }
                }

                weaponContr.ChangeWeapon(2); //Sniper
                
                if(target == null){
                    Debug.Log("AI Player says: No targets in range");
                    xDiff = 0;
                    zRotation = 0;
                    curState = AIState.Aiming;
                    break;
                }

                //Find diff in x and y
                xDiff = target.position.x - weaponContr.weaponPivot.position.x;
                yDiff = target.position.y - weaponContr.weaponPivot.position.y;

                //Calculate angle to rotate with 2D tangent formula and change from radians to degrees
                zRotation = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
                
                //StartCoroutine(ChangeStateIn(AIState.Aiming, 5f));
                curState = AIState.Aiming;
                break;
            case(AIState.tryingStraightShot):
                //***************************TODO**************************

                break;
            case(AIState.Aiming):
                //***************************TODO**************************
                
                //Check if gun is close to the correct angle
                if(Mathf.Abs(currentRotation - (360 + zRotation) % 360) < 5f){
                    Debug.Log("AI Player says: Shooting");
                    curState = AIState.Shooting;
                    weaponContr.AimTo(zRotation, xDiff);
                    break;
                }

                //Rotate Weapon
                currentRotation = (currentRotation + 1) % 360;
                
                if (Mathf.Cos(currentRotation * Mathf.Deg2Rad) > 0)
                {
                    //We should be facing right
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    //Rotate gun to point at mouse
                    weaponContr.weaponPivot.rotation = Quaternion.Euler(0, 0, currentRotation);
                }
                else
                {
                    //We should be facing left
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    //Rotate gun to point at mouse but gun is upside down, so flip 180 on x
                    //And compansate fliping x by inverting z rotation
                    weaponContr.weaponPivot.rotation = Quaternion.Euler(180, 0, -currentRotation);
                }

                break;
            case(AIState.Shooting):
                curState = AIState.WaitingForTurn;
                weaponScript.fireTriggered = true;
                break;
            case(AIState.Moving):
                //***************************TODO**************************
                break;
            case(AIState.Pause):
                //This is a dead state meant for the AI to just wait for some time
                //This state should only be called along with the coroutine ChangeStateIn()
                break;

            default:
                Debug.LogError("Invalid State reached... but HOW??!!");
                break;
        }
    }

    private IEnumerator ChangeStateIn(AIState state, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        curState = state;
        coroutineFinished = true;
    }
}
