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
    private DamageHandler dh;
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
    private int curWeapon = 0;
    //private bool coroutineFinished = true;

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
        //Get reference to DamageHandler component of this player object
        dh = GetComponent<DamageHandler>();
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
                    curWeapon = (int)WeaponCodes.Gauntlet; //Return to default weapon
                    weaponContr.ChangeWeapon((int)WeaponCodes.Gauntlet); //Gauntlet
                    curState = AIState.Pause;
                    StartCoroutine(ChangeStateIn(AIState.PickingTarget, 2f));
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

                curState = AIState.Pause;
                StartCoroutine(ChangeStateIn(AIState.tryingStraightShot, 1f));
                break;

            case(AIState.tryingStraightShot):
                //Reset target
                target = null; 
                //Loop thru all enemies and chose the first one with direct line of sight
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
                
                
                if(target == null){
                    Debug.Log("AI Player says: No target in sight");
                    //Set target to be the closest one
                    target = targets.First();
                    weaponContr.ChangeWeapon((int)WeaponCodes.Gauntlet);
                }
                else{
                    //Find a default weapon to fall back to
                    if(ps.HaveAmmo((int)WeaponCodes.Sniper)){//Check if we have Sniper Ammo
                        //Set Sniper as default choice
                        curWeapon = (int)WeaponCodes.Sniper;
                        weaponContr.ChangeWeapon((int)WeaponCodes.Sniper);
                    }
                    else if(ps.HaveAmmo((int)WeaponCodes.Shotgun)){//Check if we have Shotgun Ammo
                        //Set Shotgun as default choice if no Sniper Ammo
                        curWeapon = (int)WeaponCodes.Shotgun;
                        weaponContr.ChangeWeapon((int)WeaponCodes.Shotgun);
                    }
                    else if(ps.HaveAmmo((int)WeaponCodes.Holy_Grenade)){//Check if we have Holy Grenade Ammo
                        curWeapon = (int)WeaponCodes.Holy_Grenade;
                        weaponContr.ChangeWeapon((int)WeaponCodes.Holy_Grenade);
                    }
                    else if(ps.HaveAmmo((int)WeaponCodes.Grenade)){//Check if we have Grenade Ammo
                        curWeapon = (int)WeaponCodes.Grenade;
                        weaponContr.ChangeWeapon((int)WeaponCodes.Grenade);
                    }
                    else{//If all fails, then use Bazooka
                        curWeapon = (int)WeaponCodes.Bazooka;
                        weaponContr.ChangeWeapon((int)WeaponCodes.Bazooka);
                    }
                    //-------Default Gun has been set
                    //If distance is small, prioritize Shotgun
                    if(Vector2.Distance(target.position, transform.position) < 10f){
                        //Try to use Shotgun, else use the Hadouken else, Grenade launcher
                        if(ps.HaveAmmo((int)WeaponCodes.Shotgun)){
                            weaponContr.ChangeWeapon((int)WeaponCodes.Shotgun);
                        }else if(ps.HaveAmmo((int)WeaponCodes.Hadouken)){
                            weaponContr.ChangeWeapon((int)WeaponCodes.Hadouken);
                        }else{
                            weaponContr.ChangeWeapon((int)WeaponCodes.Grenade);
                        }
                        if(!weaponContr.ChangeWeapon((int)WeaponCodes.Shotgun)){
                            weaponContr.ChangeWeapon((int)WeaponCodes.Grenade);
                        }
                        if(Vector2.Distance(target.position, transform.position) < 4f){
                            //Close enough for Mjolnir
                            curWeapon = (int)WeaponCodes.Mjolnir;
                            weaponContr.ChangeWeapon((int)WeaponCodes.Mjolnir);
                        }
                    }
                }

                //Find diff in x and y
                xDiff = target.position.x - weaponContr.weaponPivot.position.x;
                yDiff = target.position.y - weaponContr.weaponPivot.position.y;

                //Calculate angle to rotate with 2D tangent formula and change from radians to degrees
                zRotation = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;

                switch (curWeapon)
                {
                    case (int)WeaponCodes.Gauntlet:

                        if(ps.HaveAmmo((int)WeaponCodes.BFG9000) && target.GetComponent<DamageHandler>().health < 50){
                            //Use BFG9000 if possible
                            weaponContr.ChangeWeapon((int)WeaponCodes.BFG9000);
                        }
                        else if(ps.HaveAmmo((int)WeaponCodes.Infinity_Gauntlet) && dh.health < 50 && dh.health != GM.teamsHealth[ps.teamID]){
                            //Use Infinity_Gauntlet if no BFG9000
                            weaponContr.ChangeWeapon((int)WeaponCodes.Infinity_Gauntlet);
                        }
                        else{
                            //If target is roughly on the same height, use grenades
                            if(Mathf.Abs(yDiff) < 3f){
                                weaponContr.ChangeWeapon((int)WeaponCodes.Grenade);
                            }else if(yDiff > 0){//If target is above player, use Bazooka
                                weaponContr.ChangeWeapon((int)WeaponCodes.Bazooka);
                            }else{//Else a grenade
                                weaponContr.ChangeWeapon((int)WeaponCodes.Grenade);
                            }

                            //Compansate for for gravity
                            if(Mathf.Abs(zRotation) < 90f){
                                zRotation += 25f;//raise the hammer 5 degrees up
                            }else{
                                zRotation -= 25f;//raise the hammer 5 degrees up
                            }
                        }
                        break;
                    case (int)WeaponCodes.Mjolnir:
                        if(Mathf.Abs(zRotation) < 90f){
                            zRotation += 5f;//raise the hammer 5 degrees up
                        }else{
                            zRotation -= 5f;//raise the hammer 5 degrees up
                        }
                        break;
                    default:
                        break;
                }
                
                //Change state and wait 
                curState = AIState.Pause;
                StartCoroutine(ChangeStateIn(AIState.Aiming, 1f));
                break;
            case(AIState.Aiming):
                
                //Check if gun is close to the correct angle
                if(Mathf.Abs(currentRotation - (360 + zRotation) % 360) < 5f){
                    Debug.Log("AI Player says: Shooting");
                    curState = AIState.Pause;
                    StartCoroutine(ChangeStateIn(AIState.Shooting, 1f));
                    weaponContr.AimTo(zRotation, xDiff);
                    break;
                }

                //Rotate Weapon
                currentRotation = (currentRotation + 4) % 360;
                
                if (Mathf.Cos(currentRotation * Mathf.Deg2Rad) > 0)
                {
                    //We should be facing right
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    ps.FlipName(false);
                    //Rotate gun to point at mouse
                    weaponContr.weaponPivot.rotation = Quaternion.Euler(0, 0, currentRotation);
                }
                else
                {
                    //We should be facing left
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    ps.FlipName(true);
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
        //coroutineFinished = true;
    }
}
