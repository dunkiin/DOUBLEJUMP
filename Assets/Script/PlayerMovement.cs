using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    // Stores the horizontal movement direction, with 0f as the initial value (stationary)
    private float directionX = 0f;

    [SerializeField] private float movingSpeed = 7f;

    // Upward force applied when the player jumps
    [SerializeField] private float jumpForce = 14f;

    // hitedGround
    [SerializeField] private LayerMask jumpableGround;

    private enum MovementState { idle, running, jumping, falling };


    [SerializeField] private AudioSource jumpSoundEffect;

    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get horizontal input (left or right) and update
        directionX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(directionX * movingSpeed, rb.linearVelocity.y);

        // Check if the Jump button is pressed and the player is on the ground
        if (Input.GetButtonDown("Jump") && hitedGround())
        {
            jumpSoundEffect.Play();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Vector3 is more for 3D, the 3 meaning is X,Y,Z
        }

        UpdateAnmationState();

    }

    // update the anmation state
    private void UpdateAnmationState()
    {

        MovementState state;

        // check moving to the right
        if (directionX > 0f)
        {
            //anim.SetBool("running", true);
            state = MovementState.running;

            // ensure the sprite is facing right
            sprite.flipX = false;

        }
        // check moving to the left
        else if (directionX < 0f)
        {
            //anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            //anim.SetBool("running", false);
            state = MovementState.idle;
        }

        // check player is moving jumping
        if (rb.linearVelocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        // check player is falling
        else if (rb.linearVelocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        // set the animation param 'state' to the integer representation
        anim.SetInteger("state", (int)state);
    }

    private bool hitedGround()
    {
        // casted downwards by a small distance (0.1f), to detect if it hits a layer defined as 'jumpableGround
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}