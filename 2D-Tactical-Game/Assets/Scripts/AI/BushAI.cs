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

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

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

}
