using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;


public class BirdScript : MonoBehaviour
{


    public Transform target;
    public Transform birdGraphics;

    /*
    public float checkStuck = 0.1f;
    private bool stuckInY = false;
    private bool stuckInX = false;
    private bool moveUp = false;
    private bool moveRight = false;
    private bool moveDown = false;
    private bool moveLeft = false;
    public float unstuckForce = 100f;
    */


    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    private Path path;
    private int currentWayPoint = 0;
    private GlobalVariables GLOBALS;
    private Seeker seeker;
    private Rigidbody2D rb;
    private System.Random rand;

    private Vector3 bottomLeft;
    private Vector3 bottomRight;
    private Vector3 topLeft;
    private Vector3 topRight;

    private Vector3[] targets;

    private Vector3 previousPosition;


    // Start is called before the first frame update
    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        rand = new System.Random();

        targets = new Vector3[4];
        bottomLeft = new Vector3(0, 0, 0);
        bottomRight = new Vector3(GLOBALS.mapXMax, 0, 0);
        topLeft = new Vector3(0, GLOBALS.mapYMax, 0);
        topRight = new Vector3(GLOBALS.mapXMax, GLOBALS.mapYMax, 0);
        targets[0] = bottomLeft;
        targets[1] = bottomRight;
        targets[2] = topLeft;
        targets[3] = topRight;

        UpdatePath();
        //StartCoroutine(CheckIfStuck());
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }


    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;


        if (currentWayPoint >= path.vectorPath.Count)
        {
            target = FindNewTarget();
            UpdatePath();
            rb.velocity = new Vector3();
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        /*
        if (stuckInX)
        {
            stuckInX = false;
            if (CheckDirectionForObject(Vector3.right))
                force += Vector2.left * unstuckForce;
            else
                force += Vector2.right * unstuckForce;

            //int leftOrRight = rand.Next(2);
            //force += (leftOrRight > 0 ? Vector2.left : Vector2.right) * unstuckForce;
        }

        if (stuckInY)
        {
            stuckInY = false;
            if (CheckDirectionForObject(Vector3.up))
                force += Vector2.down * unstuckForce;
            else
                force += Vector2.up * unstuckForce;
            //int upOrDown = rand.Next(2);
            //force += (upOrDown > 0 ? Vector2.up : Vector2.down) * unstuckForce;
        }
        */

        /*

        if(moveUp)
        {
            force += Vector2.up * unstuckForce;
            moveUp = false;
        }
        else if(moveRight)
        {
            force += Vector2.right * unstuckForce;
            moveRight = false;
        }
        else if (moveDown)
        {
            force += Vector2.down * unstuckForce;
            moveDown = false;
        }
        else if (moveLeft)
        {
            force += Vector2.left * unstuckForce;
            moveLeft = false;
        }
        */

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }

        if (rb.velocity.x >= 0.1f)
        {
            birdGraphics.localScale = new Vector3(-1f, 1f, 1f);
        }

        else if (rb.velocity.x <= -0.1f)
        {
            birdGraphics.localScale = new Vector3(1f, 1f, 1f);
        }



    }


    Transform FindNewTarget()
    {
        GameObject tempObject = new GameObject();
        Transform tempTrans = tempObject.transform;
        tempTrans.SetPositionAndRotation(targets[rand.Next(4)], Quaternion.identity);
        return tempTrans;
    }

    Transform FindTargetInDirection(Vector3 direction)
    {
        GameObject tempObject = new GameObject();
        Transform tempTrans = tempObject.transform;
        tempTrans.SetPositionAndRotation(direction + Vector3.Scale(direction, new Vector3(5, 5, 0)), Quaternion.identity);
        return tempTrans;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector3 currentPosition = gameObject.transform.position;

        if (other.transform.tag == "Player" || other.transform.tag == "Ground")
            Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<Collider2D>());


        /*
        if (other.transform.position.x < currentPosition.x)
        {
            moveRight = true;
        }
        else if (other.transform.position.x > currentPosition.x)
        {
            moveLeft = true;
        }
        else if (other.transform.position.y < currentPosition.y)
        {
            moveUp = true;
        }
        else
            moveDown = true;
        */
    }

    /*
    IEnumerator CheckIfStuck()
    {
        previousPosition = gameObject.transform.position;

        if (CheckIfStuckX())
        {
            stuckInX = true;
        }

        if (CheckIfStuckY())
        {
            stuckInY = true;
        }

        yield return new WaitForSeconds(1);
    }


    bool CheckIfStuckX()
    {
        if (Math.Abs(previousPosition.x - gameObject.transform.position.x) < checkStuck)
        {
            Debug.Log("Stuck in X Direction");
            return true;
        }
        return false;
    }

    bool CheckIfStuckY()
    {
        if (Math.Abs(previousPosition.y - gameObject.transform.position.y) < checkStuck)
        {
            Debug.Log("Stuck in Y Direction");
            return true;
        }

        return false;
    }

    bool CheckDirectionForObject(Vector3 direction)
    {
        Vector3 currentPosition = gameObject.transform.position;
        RaycastHit2D raycast = Physics2D.Raycast(currentPosition, (direction - currentPosition).normalized);

        if (Math.Abs(raycast.transform.position.x - currentPosition.x) < 0.5f ||
            Math.Abs(raycast.transform.position.y - currentPosition.y) < 0.5f)
            return true;
        return false;
    }
    */

    
}
