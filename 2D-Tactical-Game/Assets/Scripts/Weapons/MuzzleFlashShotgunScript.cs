﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashShotgunScript : MonoBehaviour
{
    public int damageToPlayer = 20;
    public int damageToProps = 15;
    public float pushBackForce = 250f;
    public Color startColor = new Color(1, 1, 0, 1); //Yellow
    public Color endColor = new Color(1, 1, 1, 1); //White
    public Material mat;
    public float traceDuration = 0.1f;
    private RaycastHit2D [] hitList;
    private Vector2 origin;
    private Vector2 direction;
    private Vector2 direction2;
    private Vector2 direction3;
    private Vector3 dir3;

    void Start()
    {
        hitList = new RaycastHit2D[3];
        //Start Audio
        AudioManager.instance.Play("Shotgun");
        
        //Find direction vector from gun to mouse and normalize it to length 1
        //dir3 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir3 = transform.right;
        dir3.Normalize();
        dir3.z = 0;

        //Direct cast from Vector3s to Vector2s
        origin = transform.position;
        direction = dir3;
        direction2 = Quaternion.Euler(0, 0, -15) * direction;
        direction3 = Quaternion.Euler(0, 0, 15) * direction;
        //Send a ray and store the information in hit
        hitList[0] = Physics2D.Raycast(origin, direction);
        hitList[1] = Physics2D.Raycast(origin, direction2);
        hitList[2] = Physics2D.Raycast(origin, direction3);

        for (int i = 0; i < 3; i++) // for each projectile (3 of them)
        {
            //check if we hit something
            if (hitList[i].transform != null)
            {

                //Draw a line from origin to the hit point
                DrawLine(transform.position, hitList[i].point, startColor, endColor, traceDuration);

                //Get the DamageHandler component from the target hit
                DamageHandler dh = hitList[i].transform.GetComponent<DamageHandler>();

                //Damage target if we can
                if (dh != null)
                {
                    dh.TakeDamage(damageToPlayer, damageToProps);

                    //get rigidbody to add a force to reflect the impact
                    Rigidbody2D rb = hitList[i].transform.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        //Only add force if component have a rigidbody2D
                        rb.AddForce(direction * pushBackForce);
                    }
                }
            }
            else
            {
                //Draw line from origin in the direction of the mouse times a big constant
                DrawLine(transform.position, dir3 * 1000f, startColor, endColor, traceDuration);
            }

            //We don't need the Muzzle Flash any more, so destroy this object
            Destroy(gameObject, traceDuration);
        }
    }

    
    void DrawLine(Vector3 start, Vector3 end, Color color1, Color color2, float duration = 0.2f)
    {
        //This is a function to draw a custom line from script
        GameObject myLine = new GameObject(); //Instantiate Object
        
        //Move it to thestart position
        myLine.transform.position = start; 
        
        //Add the LineRenderer to make it a line and get a reference to the line component
        myLine.AddComponent<LineRenderer>(); 
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        
        //assign a custom material, probably a white default line material
        lr.material = mat;

        //Set sorting layer by name
        lr.sortingLayerName = "Projectile";

        //Set start and end colors of the line
        lr.startColor = color1;
        lr.endColor = color2;

        //set the width of the line to something very small, since we are making a bullet
        lr.startWidth = 0.01f;

        //This will set the length of the line
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        //Destroy this line after some time, since this is a bullet trace
        GameObject.Destroy(myLine, duration);
    }

}
