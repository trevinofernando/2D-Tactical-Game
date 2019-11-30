using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    //References to other scripts
    private Animator anim;
    private GameManager GM;
    private Rigidbody2D rb;
    private DamageHandler dh;
    private PlayerSettings ps;
    private Weapon weaponScript;
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
    private float angle_1;
    private float angle_2;
    private float currentRotation;
    private static float[] launchForces;
    private static float[] velocity;
    //private bool coroutineFinished = true;

    public int curWeapon = 0;
    public AIState curState = AIState.WaitingForTurn;

    public enum AIState
    {
        WaitingForTurn,
        PickingTarget,
        tryingStraightShot,
        tryingParabolicShot,
        Moving,
        Aiming,
        Shooting,
        Pause
    }

    void Start()
    {
        //Get reference to Globals
        GLOBALS = GlobalVariables.Instance;
        //Get a reference to Game Manager
        GM = GLOBALS.GM; 


        //Get reference to PlayerSettings component of this player object
        ps = GetComponent<PlayerSettings>();
        weaponScript = ps.weaponScript; //Reference to the Weapon script from PlayerSettings
        anim = ps.anim;//Reference to the Animator component from PlayerSettings

        //Initialize array of 0's of size = the # of weapons
        launchForces = new float[GLOBALS.arsenalAmmo.Count]; 
        //Populate all weapon launch forces array of the projectiles needed
        launchForces[(int)WeaponCodes.Grenade] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Grenade].GetComponent<Grenade>().launchForce;
        launchForces[(int)WeaponCodes.Bazooka] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Bazooka].GetComponent<Projectile_Bomb>().launchForce;
        launchForces[(int)WeaponCodes.Mine] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Mine].GetComponent<LandMineScript>().launchForce;
        launchForces[(int)WeaponCodes.Holy_Grenade] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Holy_Grenade].GetComponent<Grenade>().launchForce;
        launchForces[(int)WeaponCodes.Homing_Bazooka] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Homing_Bazooka].GetComponent<HomingBomb>().launchForce;
        launchForces[(int)WeaponCodes.Teleport_Grenade] = ps.weaponScript.projectilePrefab[(int)WeaponCodes.Teleport_Grenade].GetComponent<TeleportGrenade>().launchForce;
        
        //Initialize array of 0's of size = the # of weapons
        velocity = new float[GLOBALS.arsenalAmmo.Count]; 
        for(int i = 0; i < GLOBALS.arsenalAmmo.Count;i++){
            if(ps.weaponScript.projectilePrefab[i] == null){
                continue;
            }
            rb = ps.weaponScript.projectilePrefab[i].GetComponent<Rigidbody2D>();
            if(rb == null){
                continue;
            }
            velocity[i] = (launchForces[i] / rb.mass) * Time.fixedDeltaTime;
        }

        //Get reference to rigidbody of this player object
        rb = GetComponent<Rigidbody2D>();

        //Get reference to DamageHandler component of this player object
        dh = GetComponent<DamageHandler>();

        //Get reference to WeaponController component of this player object
        weaponContr = GetComponent<WeaponController>();
        degreesOfDeadZone = weaponContr.degreesOfDeadZone;
    }

/*
    private void FixedUpdate() {
        onSyncWithFixedUpdate = true;
    }

    // Update is called once per frame
    private void Update()
    {
        //This will sync with FixedUpdate with the intention of pausing the game and 
        if(!onSyncWithFixedUpdate){
            return;
        }else{
            onSyncWithFixedUpdate = false;
        }
*/

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
                    //Debug.Log("starting turn");
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
                        if(GM.teams[i,j] != null){
                            targets.Add(GM.teams[i,j].transform);
                        }
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
                //Debug.Log("tryingStraightShot");
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
                
                
                if(target == null){//change state TODO
                    Debug.Log("AI Player says: No target in sight");
                    //Set target to be the closest one
                    target = targets.First();

                    //Change state and wait 
                    curState = AIState.Pause;
                    StartCoroutine(ChangeStateIn(AIState.tryingParabolicShot, 1f));
                    //leave state
                    break;
                }
                

                zRotation = CalculateStraightShotAngle(target);

                //Find a default weapon to fall back to
                if(TryWeapon(WeaponCodes.Sniper))               {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Shotgun))         {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Holy_Grenade))    {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Hadouken))        {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.BFG9000))         {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Mine))            {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Grenade))         {/*Nothing to do*/}
                else if(TryWeapon(WeaponCodes.Bazooka))         {/*Nothing to do*/}
                else   {TryWeapon(WeaponCodes.Bang_Pistol);}

                //-------Default Weapon has been set---------//

                //If distance between target is small, then prioritize other short range weapons
                if(Vector2.Distance(target.position, transform.position) < 10f){

                    //Try to use Shotgun, else use the Hadouken, else ThunderGun
                    if(TryWeapon(WeaponCodes.Shotgun))          {/*Nothing to do*/}
                    else if(TryWeapon(WeaponCodes.Hadouken))    {/*Nothing to do*/}
                    else if(TryWeapon(WeaponCodes.ThunderGun))  {/*Nothing to do*/}
                    else                                        {/*Keep default weapon*/}

                    //If the angle is good for launching, then prioritize ThunderGun
                    if(zRotation > 30f && zRotation < 60f){
                        TryWeapon(WeaponCodes.ThunderGun);
                    }

                    //If target is in melee range, then prioritize Mjolnir
                    if(Vector2.Distance(target.position, transform.position) < 4f){
                        //Close enough for Mjolnir
                        TryWeapon(WeaponCodes.Mjolnir);
                    }
                }
                
                float rotationOffset = 0f;
                switch (curWeapon)
                {
                    case (int)WeaponCodes.Mjolnir:
                    case (int)WeaponCodes.ThunderGun:
                        rotationOffset = 7f;
                        break;

                    case (int)WeaponCodes.Hadouken:
                        rotationOffset = 1.5f;
                        break;

                    case (int)WeaponCodes.Mine:
                    case (int)WeaponCodes.Bazooka:
                    case (int)WeaponCodes.Grenade:
                    case (int)WeaponCodes.Holy_Grenade:
                    case (int)WeaponCodes.Teleport_Grenade:
                        //**************TODO********************
                        break;
                    default:
                        break;
                }
                
                if(Mathf.Abs(zRotation) < 90f){
                    zRotation += rotationOffset;//raise weapon x degrees up
                }else{
                    zRotation -= rotationOffset;//raise weapon x degrees up
                }

                //Change state and wait 
                curState = AIState.Pause;
                StartCoroutine(ChangeStateIn(AIState.Aiming, 1f));
                break;

            case(AIState.tryingParabolicShot):
                //Debug.Log("tryingParabolicShot");

                //This is for BFG9000, ThunderGun and sets yDiff and xDiff 
                zRotation = CalculateStraightShotAngle(target);

                
                if(TryWeapon(WeaponCodes.ThunderGun) && Vector2.Distance(target.position, transform.position) < 10f && zRotation > 10f && zRotation < 70f)
                {/*In ThunderGun range and optimal angle range*/}
                else if(TryWeapon(WeaponCodes.BFG9000) && target.GetComponent<DamageHandler>().health < 50)
                {/*The target has low health*/}
                else if(TryWeapon(WeaponCodes.Infinity_Gauntlet) && dh.health < 50 && dh.health != GM.teamsHealth[ps.teamID])
                {/*AI has low health and is not the last man of its team*/}
                else{
                    angle_1 = angle_2 = 0f;
                    if(CalculateParabolicShotAngles(target, WeaponCodes.Bazooka) && TryWeapon(WeaponCodes.Bazooka))                         {/*Nothing to do*/}
                    else if(CalculateParabolicShotAngles(target, WeaponCodes.Holy_Grenade) && TryWeapon(WeaponCodes.Holy_Grenade))          {/*Nothing to do*/}
                    else if(CalculateParabolicShotAngles(target, WeaponCodes.Mine) && TryWeapon(WeaponCodes.Mine))                          {/*Nothing to do*/}
                    else if(CalculateParabolicShotAngles(target, WeaponCodes.Grenade) && TryWeapon(WeaponCodes.Grenade))                    {/*Nothing to do*/}
                    else if(CalculateParabolicShotAngles(target, WeaponCodes.Teleport_Grenade) && TryWeapon(WeaponCodes.Teleport_Grenade))  {/*Nothing to do*/}
                    else{TryWeapon(WeaponCodes.Bang_Pistol);}

                    //Debug.Log(zRotation);
                    //Debug.Log(angle_1);
                    //Debug.Log(angle_2);
                    zRotation = angle_1;
                    /*
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
                    */
                }
                
                //Change state and wait 
                curState = AIState.Pause;
                StartCoroutine(ChangeStateIn(AIState.Aiming, 1f));
                break;

            case(AIState.Aiming):
                //Check if gun is close to the correct angle
                if(Mathf.Abs(currentRotation - (360 + zRotation) % 360) < 5f){
                    //Debug.Log("Finished Aiming");
                    weaponContr.AimTo(zRotation, xDiff);
                    curState = AIState.Pause;
                    StartCoroutine(ChangeStateIn(AIState.Shooting, 1f));
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
                //Debug.Log("Shooting " + curWeapon);
                curState = AIState.Pause;
                StartCoroutine(ChangeStateIn(AIState.WaitingForTurn, 1f));
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

    private float CalculateStraightShotAngle(Transform _target){
        //Find diff in x and y
        xDiff = _target.position.x - weaponContr.weaponPivot.position.x;
        yDiff = _target.position.y - weaponContr.weaponPivot.position.y;

        //Calculate angle to rotate with 2D tangent formula and change from radians to degrees
        return Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
    }

    private bool CalculateParabolicShotAngles(Transform _target, WeaponCodes weaponCode){
        //Source position
        float x1 = weaponContr.weaponPivot.position.x;
        float y1 = weaponContr.weaponPivot.position.y;

        //Target position
        float x = _target.position.x;
        float y = _target.position.y;

        //Target position normalized by moving source to (0,0)
        x = x - x1;
        y = y - y1;

        //Grab Velocity for specific projectile
        float v = velocity[(int)weaponCode];

        //Get gravity from Unity Project Settings (assuming there is only gravity on the y axis)
        float g = Physics2D.gravity.y;

        //Check if the target is reachable
        //discriminant = v^4 - g(gx^2 + 2yv^2)
        float discriminant = v*v*v*v - g * (g*x*x + 2*y*v*v);
        if(discriminant < 0){
            //In this case the target is unreachable 
            return false;
        }
        //Theta = ( v^2 (+-) sqrt(discriminant) ) / ( gx )
        angle_1 =  Mathf.Atan2((v * v + Mathf.Sqrt(discriminant)) , (g * x)) * Mathf.Rad2Deg; //Low trajectory
        angle_2 =  Mathf.Atan2((v * v - Mathf.Sqrt(discriminant)) , (g * x)) * Mathf.Rad2Deg; //High trajectory
        /*
        if(x < 0){
            if(y < 0){//3rd quadrant
                //1st to 3rd quadrant
                angle_1 -= 180f;
                angle_2 -= 180f;
            }else{//2nd quadrant
                //4th to 2nd quadrant
                angle_1 += 180f;
                angle_2 += 180f;
            }
        }
        */
        return true;
    }

    private bool TryWeapon(WeaponCodes weaponCode){
        if(ps.HaveAmmo((int)weaponCode)){//Check if we have Ammo of given weapon
            //Equip weapon
            curWeapon = (int)weaponCode;
            weaponContr.ChangeWeapon((int)weaponCode);
            return true;
        }
        return false;
    }
}
