using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public int damageToPlayer = 50;
    public int damageToProps = 9999;
    public float pushBackForce = 500f;
    public float traceDuration = 1f;
    public Material mat;
    public Color startColor = new Color(1, 1, 0, 0.5f); //Yellow
    public Color endColor = new Color(1, 0, 0, 0.5f); //Red


    private RaycastHit2D hit;
    private Vector2 origin;
    private Vector2 direction;
    private Vector3 dir3;


    public void Shoot(Vector3 targetPosition)
    {
        //Debug.Log("Shooting " + targetPosition);
        
        //Find direction vector from self to target and normalize it to length 1
        dir3 = targetPosition - transform.position;
        dir3.Normalize();
        dir3.z = 0;

        //Direct cast from Vector3s to Vector2s
        origin = transform.position;
        direction = dir3;

        //Send a ray and store the information in hit
        hit = Physics2D.Raycast(origin, direction);

        //check if we hit something
        if (hit.transform != null)
        {

            //Draw a line from origin to the hit point
            DrawLine(transform.position, hit.point, startColor, endColor, traceDuration);

            //Get the DamageHandler component from the target hit
            DamageHandler dh = hit.transform.GetComponent<DamageHandler>();

            //Damage target if we can
            if (dh != null)
            {
                dh.TakeDamage(damageToPlayer, damageToPlayer);

                //get rigidbody to add a force to reflect the impact
                Rigidbody2D rb = hit.transform.GetComponent<Rigidbody2D>();
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
    }

    void DrawLine(Vector3 start, Vector3 end, Color color1, Color color2, float duration = 1f)
    {
        //This is a function to draw a custom line from script
        GameObject myLine = new GameObject(); //Instantiate Object

        //Move it to thestart position
        myLine.transform.position = start;

        //Add the LineRenderer to make it a line and get a reference to the line component
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();

        //Set sorting layer by name
        lr.sortingLayerName = "Projectile";


        //assign a custom material, probably a white default line material
        lr.material = mat;

        //Set start and end colors of the line
        lr.startColor = color1;
        lr.endColor = color2;

        //set the width of the line to something very small, since we are making a bullet
        lr.startWidth = 0.1f;
        lr.endWidth = 1f;

        //This will set the length of the line
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        //Destroy this line after some time, since this is a bullet trace
        GameObject.Destroy(myLine, duration);
    }
}
