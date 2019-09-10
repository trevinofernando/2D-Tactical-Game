using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    public float distanceOffsetX = 0.17f;
    public float distanceOffsetY = 0.4f;
    public bool isGrounded;
    public LayerMask ground;
    public bool isJumping = false;

    private float moveDirection;
    private Rigidbody2D rb;
    private Animator anim;

    /*
     We will make an invisible rectangle to check if it overlaps with anything under 
     the player for that we need the CapsuleCollider2D and get it's dimensions.

            ____
           /    \
          |      | <- CapsuleCollider2D
          |      |
          \______/
          [      ] <- Small Rectangle Here

    To draw this small rectangle under our player, we need to get the bottom left 
    corner of the collider which will be our top left corner of the rectangle and 
    similarly for the bottom right corner + an offset (distanceOffsetToGround)
    */
    private CapsuleCollider2D colliderInfo;
    private float colliderHalfWidth;
    private float colliderHalfHeight;
    private Vector2 centerToBottmLeftCorner;
    private Vector2 centerToBottmRightCorner;
    private Vector2 playerPosition;

    void Start()
    {
        //Get reference to rigidbody of this player object
        rb = GetComponent<Rigidbody2D>();
        //Get reference to Animator component of this player object
        anim = GetComponent<Animator>();
        //Get reference to CapsuleCollider2D component of this player object
        colliderInfo = GetComponent<CapsuleCollider2D>();

        //colliderInfo.bounds.center.x

        //Get half of the width of the capsule collider
        colliderHalfWidth = colliderInfo.size.x / 2;
        colliderHalfHeight = colliderInfo.size.y / 2;

        //Pre-compute offset distante of corners of rectangle under player
        //So we can just add it to the current position of our player and 
        //and get the right coordinate
        centerToBottmLeftCorner = new Vector2(-colliderHalfWidth + distanceOffsetX, -colliderHalfHeight);
        centerToBottmRightCorner = new Vector2(colliderHalfWidth - distanceOffsetX, -colliderHalfHeight - distanceOffsetY);
    }

    void Update()
    {

        /*  HORIZIONTAL Movement Animations */
        if (moveDirection > 0) //Check for Right arrow or "d"
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            anim.SetBool("isWalking", true);
        }
        else if(moveDirection < 0) //Check for Left arrow or "a"
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

    }

    void FixedUpdate()
    {
        //Direct cast from Vector3 to Vector2
        playerPosition = transform.position;

        //Check for any overlap area under the players feet
        isGrounded = Physics2D.OverlapArea(
            playerPosition + centerToBottmLeftCorner, 
            playerPosition + centerToBottmRightCorner,
            ground);

        //Left arrow = -1, Right arrow = 1
        //if it feels weird, try removing "Raw"
        moveDirection = Input.GetAxisRaw("Horizontal");


        //Move The Horiziontal axis
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);

        //Check if we are in the ground
        if (isGrounded)
        {
            //If we are on the ground, then we are not jumping
            isJumping = false;
            anim.SetBool("isJumping", false);

            //But we can jump, so check for Up arrow, Space or "w"
            //And check if we are relatively not moving up or down
            //but if theres a ramp we could be moving up and down but no faster than "speed"
            if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs( rb.velocity.y) < speed)
            {
                Debug.Log("Jump");
                anim.SetTrigger("takeOff");
                anim.SetBool("isJumping", true);
                isJumping = true;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

    }

}
