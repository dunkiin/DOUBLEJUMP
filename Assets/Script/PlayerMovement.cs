using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    private float directionX = 0f;
    [SerializeField] private float movingSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    // hitedGound
    [SerializeField] private LayerMask jumpableGround;

    private enum MovementState { idle, running, jumping, falling };
    
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
        // user left or right
        directionX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(directionX * movingSpeed, rb.velocity.y);

        // user jump
        if (Input.GetButtonDown("Jump") && hitedGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Vector3 is more for 3D, the 3 meaning is X,Y,Z
        }

        UpdateAnmationState();

    }

    private void UpdateAnmationState()
    {

        MovementState state;

        if (directionX > 0f)
        {
            //anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = false;

        }
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

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool hitedGround()
    {
        // casted downwards by a small distance (0.1f), to detect if it hits a layer defined as 'jumpableGround
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}