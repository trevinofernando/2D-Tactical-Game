using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    public int ID;
    public int teamID;
    public string nameGiven;
    public bool isMyTurn = false;
    public bool iAmAI = false;
    public SpriteRenderer bodySprite;
    public Weapon weaponScript;
    public Animator anim;
    public RectTransform nameTransform;
    public TextMeshPro nameAndHealthBar;
    
    //The arsenal is static to have only one instance of this variable in 
    //all of the Player Settings classes
    [System.NonSerialized]public static List<List<int>> arsenalAmmo = new List<List<int>>(); //[TeamID][WeaponCode]
    [System.NonSerialized] public GameManager gameManager;
    [System.NonSerialized] public CameraController cam;
    [System.NonSerialized]public GameObject thisGameObject;
    private AIController AICont;
    private WeaponController weaponContr;
    private Rigidbody2D rb;
    private bool textLookingLeft = false;
    private int angle;
    private PlayerSettings ps;

    void Start()
    {
        //Get reference to weaponController and AIController script, and Rigidbody component
        weaponContr = GetComponent<WeaponController>();
        AICont = GetComponent<AIController>();
        rb = GetComponent<Rigidbody2D>();
        thisGameObject = gameObject;

        //Initialize the player health/name
        nameAndHealthBar.SetText(nameGiven + "\n" + gameManager.soldiersHealth[teamID, ID]);
    }
    public void SetColor(Color newColor)//RBGA
    {
        bodySprite.color = newColor;
    }

    public void EndTurn()
    {
        if (isMyTurn)
        {
            //Tell gameManager the turn is done
            if(gameManager != null)
            {
                gameManager.isTurnFinished = true;
            }
        }
        
        //Revert Animations to idle
        if(anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isJumping", false);
        }

        //Change to default weapon
        weaponContr.ChangeWeapon((int)WeaponCodes.Gauntlet);
        AICont.curWeapon = (int)WeaponCodes.Gauntlet;

        //Reset these variable in case of sudden turn ending
        weaponScript.canChangeWeapons = true;
        weaponScript.canShoot = true;
        weaponScript.fireTriggered = false; //reset flag for next turn
        weaponScript.targetSelected = false;//reset flag for next turn
        
        //Reset AI State machine to waiting
        AICont.curState = AIController.AIState.WaitingForTurn;

        //Let all scripts in this player know the turn is done, so they stop updating
        isMyTurn = false;
    }

    public bool HaveAmmo(int weaponCode){ 
        //Check if the shared arsenal have ammo on the weapon selected
        return arsenalAmmo[teamID][weaponCode] > 0;
    }

    public void UpdateAmmo(int weaponCode, int ammoChange){
        //Increase or decrease the ammo by ammoChange units
        arsenalAmmo[teamID][weaponCode] += ammoChange;
    }

    public int AmmoCount(int weaponCode){
        //This is a look up function to check how much ammo there is on the given weapon
        return arsenalAmmo[teamID][weaponCode];
    }

    public void UpdateHealth(int newHealth)
    {
        //Ignore negative values
        newHealth = Mathf.Max(newHealth, 0);

        //subtract current health of soldier from team health
        gameManager.teamsHealth[teamID] -= gameManager.soldiersHealth[teamID, ID];

        //update individual soldier health
        gameManager.soldiersHealth[teamID, ID] = newHealth;

        //add ack the new soldier health
        gameManager.teamsHealth[teamID] += gameManager.soldiersHealth[teamID, ID];

        //update the nameplate
        nameAndHealthBar.SetText(nameGiven + "\n" + newHealth);
        //nameAndHealthBar.SetText(nameGiven + "\n" + gameManager.soldiersHealth[teamID, ID]);
    }

    public void FlipName(bool playerLookingLeft){
        angle = 0;
        if(playerLookingLeft){
            //Check if text is currently looking left
            if(!textLookingLeft){
                //Flip text
                angle = 180;
                textLookingLeft = true;//set the flag
            }
        }else{
            if(textLookingLeft){
                //Flip text
                angle = 180;
                textLookingLeft = false;//set the flag
            }
        }
        //Flip text to Always read from left to right
        nameTransform.Rotate(new UnityEngine.Vector3(0, angle, 0));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(!isMyTurn && other.transform.tag == "Player"){
            //ps = other.transform.GetComponent<PlayerSettings>();
            if(other.transform.GetComponent<PlayerSettings>().isMyTurn){
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(!isMyTurn && other.transform.tag == "Player"){
            //Check if the other player turn is 
            ps = other.transform.GetComponent<PlayerSettings>();
            if(ps == null || gameManager.gameState != GameManager.GameState.TurnInProgress){
                return;
            }
            if(!ps.isMyTurn && ps.ID == gameManager.currSoldierTurn[gameManager.currTeamTurn]){
                UnfreezePlayer(rb);
            }
        }
    }
    

    private void OnCollisionExit2D(Collision2D other) {
        if(!isMyTurn && other.transform.tag == "Player"){
            if(other.transform.GetComponent<PlayerSettings>().isMyTurn){
                UnfreezePlayer(rb);
            }
        }
    }

    private void UnfreezePlayer(Rigidbody2D _rb){
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}