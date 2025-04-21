using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float detectionRange = 5f;
    public float moveSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D col;

    private bool chasing = false;
    private bool isDead = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isDead) return;

        float dist = Vector2.Distance(transform.position, player.position);
        chasing = dist <= detectionRange;

        // Set animation: 0 = Idle, 1 = Run
        anim.SetInteger("state", chasing ? 1 : 0);
    }

    void FixedUpdate()
    {
        if (chasing && !isDead)
        {
            // Move horizontally toward player
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            sprite.flipX = dir < 0; // flip sprite so it faces movement
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize detection range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Did player land on top?
            float playerY = collision.transform.position.y;
            float chickenY = transform.position.y;

            if (playerY > chickenY + 0.5f)
            {
                // Hit from above: kill chicken
                KillChicken();

                // Bounce the player up a bit
                var playerRb = collision.rigidbody;
                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
            }
            else
            {
                // Side hit: kill the player
                var life = collision.gameObject.GetComponent<PlayerLife>();
                if (life != null)
                    life.Die();
            }
        }
    }

    private void KillChicken()
    {
        isDead = true;
        chasing = false;

        // Stop movement & disable collider so it can't hit you again
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;

        // Play hit animation
        anim.SetTrigger("hit");

        // Destroy after the animation has time to play
        Destroy(gameObject, 0.5f);
    }
}
