using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class BushAI : MonoBehaviour
{


    public Transform[] wanderTargets;
    public Transform target;
    public Transform bushGraphics;

    public float speed = 350f;
    public float nextWaypointDistance = 3f;
    public float checkAheadOffsetX = 0.5f;
    public float checkAheadOffsetY = -0.5f;
    public float jumpForce = 17f;

    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    public BushState bushState = BushState.Wandering;

    public enum BushState
    {
        Standing,
        Wandering,
        Seeking,
        Targeting,
        Attacking
    }

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();


        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        {
            Debug.Log("Seeker is done");
            if(reachedEndOfPath)
                target = FindNewWanderTarget();
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        /*
        if(bushState.Equals(BushState.Wandering) && path == null)
        {
            target = FindNewWanderTarget();
            UpdatePath();
        }
        */

        if(currentWayPoint >= path.vectorPath.Count)
        {
            Debug.Log("Reached end of path!");
            reachedEndOfPath = true;
            return;
        }else
        {
            reachedEndOfPath = false;
        }


        if (CheckIfShouldJump())
            Jump();


        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }

        if(force.x <= 0.01f)
        {
            bushGraphics.localScale = new Vector3(-1f, 1f, 1f);
        }else if(force.x >= -0.01f)
        {
            bushGraphics.localScale = new Vector3(1f, 1f, 1f);
        }

    }


    bool CheckIfShouldJump()
    {

        float nextWaypointY = path.vectorPath[currentWayPoint].y;
        float currentPositionY = transform.position.y;

        //Check if jumping is needed vertically
        if (Math.Abs(currentPositionY - nextWaypointY) >= 0.1f)
        {
            if(CheckIfSomethingToJumpTo(gameObject, speed ,jumpForce) && !CheckIfSomethingInWayOfJumping())
                return true;
        }
            

        //Check if jumping is needed horizontally
        Vector2 startSpot = new Vector2();
        Vector2 endSpot = new Vector2();
        Collider2D horizontalCheck = Physics2D.OverlapArea(startSpot, endSpot);
        if (!horizontalCheck)
        {
            if(CheckIfSomethingToJumpTo(gameObject, speed, jumpForce) && !CheckIfSomethingInWayOfJumping())
                return true;
        }
            

        return false;

    }

    bool CheckIfSomethingToJumpTo(GameObject go, float xForce, float yForce)
    {

        //Check if something can be landed on in the projectile arc from current y -> current y


        //There's something to jump to above current y position


        //There's something to jump to at current y position


        //There's something to jump to below current y position


        //There's nothing to jump to




        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        Transform transform = go.GetComponent<Transform>();

        float mass = rb.mass;
        float velocityX = xForce / mass;
        float velocityY = yForce / mass;

        float travelingDirection = Mathf.Atan2(velocityY, velocityX);

        float velocity = (float)Math.Sqrt(Math.Pow(velocityY, 2.0f) +  Math.Pow(velocityX, 2.0f));

        float jumpDistanceX = (float)(Math.Pow(velocity, 2.0f) * Math.Sin(2.0f * travelingDirection)) / 9.8f;

        Vector2 startSpot = new Vector2(transform.position.x + jumpDistanceX, transform.position.y + checkAheadOffsetY);
        Vector2 endSpot = new Vector2(transform.position.x + jumpDistanceX + checkAheadOffsetX, transform.position.y);

        Collider2D check = Physics2D.OverlapArea(startSpot, endSpot);

        if (check)
            return true;


        //Continue to check along the projectile path

        return false;
    }

    bool CheckIfSomethingInWayOfJumping()
    {
        //Ray trace somewhere in the jump motion
        return false;
    }


    Transform FindNewWanderTarget()
    {
        System.Random r = new System.Random();
        int index = r.Next(0, wanderTargets.Length);
        Debug.Log("Targeting wander " + index);
        return wanderTargets[index];
    }

    void Shoot()
    {

    }

    void Jump()
    {
        //Remove friction while jumping?
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

}
