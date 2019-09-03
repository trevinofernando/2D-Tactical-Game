using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    //public bool isGrounded;
    //public Transform feetPos;
    //public float checkRadious;
    //public LayerMask whatIsGround;

    private float moveDirection;
    private Rigidbody2D rb;
    private Animator anim;
    void Start()
    {
        //Get reference to rigidbody of this player object
        rb = GetComponent<Rigidbody2D>();
        //Get reference to Animator component of this player object
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(moveDirection > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            anim.SetBool("isWalking", true);
        }
        else if(moveDirection < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        //Check with an invisible circle in feet position for gorund overlap
        //isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadious, whatIsGround);
    }

    void FixedUpdate()
    {
        //Left arrow = -1, Right arrow = 1
        //if it feels weird, try removing "Raw"
        moveDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);


    }
}
