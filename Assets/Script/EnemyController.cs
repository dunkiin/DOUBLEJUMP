using UnityEngine;


public class EnemyController : MonoBehaviour
{
    
    public float detectionRange = 5f;
    public float moveSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D boxCol;

    private bool chasing = false;
    private bool isDead = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isDead) return;

        chasing = Vector2.Distance(transform.position, player.position) <= detectionRange;
        anim.SetInteger("state", chasing ? 1 : 0);
    }

    void FixedUpdate()
    {
        if (chasing && !isDead)
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            sprite.flipX = dir < 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // Look at the first contact point's normal to see if the player hit us from above
        ContactPoint2D cp = collision.GetContact(0);
        if (cp.normal.y > 0.5f)
        {
            // Player jumped on us
            KillChicken();

            // Optional: bounce the player up
            Rigidbody2D prb = collision.rigidbody;
            if (prb != null)
                prb.linearVelocity = new Vector2(prb.linearVelocity.x, 10f);
        }
        else
        {
            // Side collision: hurt the player
            var life = collision.gameObject.GetComponent<PlayerLife>();
            if (life != null)
                life.Die();
        }
    }

    private void KillChicken()
    {
        isDead = true;
        chasing = false;
        rb.linearVelocity = Vector2.zero;
        boxCol.enabled = false;       // disable our BoxCollider2D

        anim.SetTrigger("hit");
        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
