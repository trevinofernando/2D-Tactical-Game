using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public int ID;
    public int teamID;
    public string nameGiven;
    public bool isMyTurn = false;
    public SpriteRenderer bodySprite;
    public GameManager gameManager;
    public CameraController cam;
    private Animator anim;

    void Start()
    {
        //Get reference to Animator component of this player object
        anim = GetComponent<Animator>();
    }
    public void SetColor(Color newColor)//RBGA
    {
        bodySprite.color = newColor;
    }

    public void EndTurn()
    {
        if(anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isJumping", false);
        }
        if(gameManager != null)
            gameManager.isTurnFinished = true;
        isMyTurn = false;
    }

    public void UpdateHeatlh(int newHealth)
    {
        //Ignore negative values
        newHealth = Mathf.Max(newHealth, 0);

        //subtract current health of soldier from team health
        gameManager.teamsHealth[teamID] -= gameManager.soldiersHealth[teamID, ID];

        //update individual soldier health
        gameManager.soldiersHealth[teamID, ID] = newHealth;

        //add ack the new soldier health
        gameManager.teamsHealth[teamID] += gameManager.soldiersHealth[teamID, ID];
    }

}
