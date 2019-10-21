﻿using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public int ID;
    public int teamID;
    public string nameGiven;
    public bool isMyTurn = false;
    public bool iAmAI = false;
    public SpriteRenderer bodySprite;
    public Weapon weaponScript;
    [System.NonSerialized] public GameManager gameManager;
    [System.NonSerialized] public CameraController cam;
    public Animator anim;
    private WeaponController weaponContr;
    private Rigidbody2D rb;

    void Start()
    {
        //Get reference to weaponController script and Rigidbody component
        weaponContr = GetComponent<WeaponController>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetColor(Color newColor)//RBGA
    {
        bodySprite.color = newColor;
    }

    public void EndTurn()
    {
        if (isMyTurn)
        {
            //Revert Animations to idle
            if(anim != null)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isJumping", false);
            }

            //Change to default weapon
            weaponContr.ChangeWeapon(0);

            //Reset these variable in case of sudden turn ending
            weaponScript.canChangeWeapons = true;
            weaponScript.canShoot = true;

            //Tell gameManager the turn is done
            if(gameManager != null)
            {
                gameManager.isTurnFinished = true;
            }
        }

        //Let all scripts in this player know the turn is done, so they stop updating
        isMyTurn = false;
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
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(!isMyTurn && other.transform.tag == "Player"){
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(!isMyTurn && other.transform.tag == "Player"){
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}