using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{

    private GlobalVariables GLOBALS;
    private System.Random rand;
    private float maxHeight;
    private float startingY;
    private float startingX;
    private float velocityY;
    private float velocityX;
    private Vector2 velocity;
    private Rigidbody2D rb;
    private PlayerMovement playerMov;

    public double maxSpeedY;
    public double maxSpeedX;
    public bool moveVertically;
    public bool moveHorizontally;


    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        rand = new System.Random();
        maxHeight = GLOBALS.mapYMax;
        startingY = transform.position.y;
        startingX = transform.position.x;
        if(moveVertically)
        {
            velocityY = (float)(rand.NextDouble() * (maxSpeedY - maxSpeedY/2) + maxSpeedY/2);
        }
        if(moveHorizontally)
        {
            velocityX = (float)(rand.NextDouble() * (maxSpeedX - maxSpeedX/2) + maxSpeedX/2);
        }
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    //Update to support movement in x
    void FixedUpdate()
    {
        float currentHeight = transform.position.y;

        if(currentHeight >= maxHeight)
        {
            velocity = new Vector2(velocityX, -velocityY);
        }
        else if(currentHeight <= 0)
        {
            velocity = new Vector2(velocityX, velocityY);
        }

        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.tag == "Player"){
            other.transform.GetComponent<PlayerMovement>().standingOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.transform.tag == "Player"){
            other.transform.GetComponent<PlayerMovement>().standingOnPlatform = false;
        }
    }
}
