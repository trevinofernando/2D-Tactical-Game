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
    public float groundDistance = 0.2f;

    //private float 
    private float moveDirection;
    private Rigidbody2D rb;
    public Animator anim;
    private PlayerSettings ps;

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
        //anim = GetComponent<Animator>();
        //Get reference to PlayerSettings component of this player object
        ps = GetComponent<PlayerSettings>();
        //Get reference to CapsuleCollider2D component of this player object
        colliderInfo = GetComponent<CapsuleCollider2D>();

        //colliderInfo.bounds.center.x

        //Get half of the width of the capsule collider
        colliderHalfWidth = colliderInfo.size.x / 2;
        colliderHalfHeight = colliderInfo.size.y / 2;

        //Pre-compute offset distante of corners of rectangle under player
        //So we can just add it to the current position of our player and 
        //and get the right coordinate
        centerToBottmLeftCorner = new Vector2(-colliderHalfWidth - colliderInfo.offset.x + distanceOffsetX, -colliderHalfHeight + colliderInfo.offset.y - 0.5f);
        centerToBottmRightCorner = new Vector2(colliderHalfWidth + colliderInfo.offset.x - distanceOffsetX, -colliderHalfHeight + colliderInfo.offset.y - distanceOffsetY -0.5f);
    }

    void Update()
    {
        if (ps.isMyTurn)
        {
            if (moveDirection == 0) 
            {
                anim.SetBool("isWalking", false);
            }
            else
            {
                anim.SetBool("isWalking", true);
            }
        }
    }

    void FixedUpdate()
    {
        if (ps.isMyTurn)
        {
            //Freeze rotation on Z and unfreeze position on X
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
            
            //Direct cast from Vector3 to Vector2
            playerPosition = transform.position;

            //Check for any overlap area under the players feet
            isGrounded = Physics2D.OverlapArea(playerPosition + centerToBottmLeftCorner, playerPosition + centerToBottmRightCorner);
            //isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance);


            //Left arrow = -1, Right arrow = 1
            //if it feels weird, try removing "Raw"
            moveDirection = Input.GetAxisRaw("Horizontal");


            //Move The Horizontal axis
            if(moveDirection != 0)
            {
                //By not calling this when movement is 0, this stops bugs responsible for weird physics movements where the x axis is constrained
                rb.velocity = new Vector2(moveDirection * speed * Time.deltaTime, rb.velocity.y );
            }else{
                rb.velocity = new Vector2(0, rb.velocity.y );
            }


            //Check if we are in the ground
            if (isGrounded)
            {
                //If we are on the ground, then we are not jumping
                if(rb.velocity.y < 0.1)
                    anim.SetBool("isJumping", false);

                //But we can jump, so check for Up arrow, Space or "w"
                //And check if we are relatively not moving up or down
                //but if theres a ramp we could be moving up and down but no faster than "speed"
                if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs( rb.velocity.y) < 5f)
                {
                    //Debug.Log("Jump");
                    anim.SetTrigger("takeOff");
                    anim.SetBool("isJumping", true);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }else{
                anim.SetBool("isJumping", true);
            }
        }
    }

}
