using UnityEngine;
using System.Collections;

public class RinoController : MonoBehaviour
{
    public float detectionRange = 8f;         // distance to start chasing
    public float walkSpeed = 3f;         // initial run speed
    public float chargeSpeed = 12f;        // speed during charge
    public float chargeDelay = 0.8f;       // delay before full charge

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D boxCol;

    private bool chasing = false;
    private bool isCharging = false;
    private bool isDead = false;
    private int dir = 1;

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
        // when we detect the player...
        if (!chasing && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            chasing = true;
            dir = (player.position.x > transform.position.x) ? 1 : -1;
            anim.SetInteger("state", 1);        // Running state
                                                // store the handle so we can cancel it later
            chargeRoutine = StartCharge();
            StartCoroutine(chargeRoutine);
        }
    }

    IEnumerator StartCharge()
    {
        yield return new WaitForSeconds(chargeDelay);
        if (!isDead)
        {
            isCharging = true;
        }
    }

    void FixedUpdate()
    {
        if (!chasing || isDead) return;
        float speed = isCharging ? chargeSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        sprite.flipX = dir > 0; // face the movement direction
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // handle player collisions first
        if (collision.gameObject.CompareTag("Player"))
        {
            bool stomped = false;
            foreach (ContactPoint2D cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f) { stomped = true; break; }
            }
            if (stomped)
            {
                StompedByPlayer(collision);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerLife>()?.Die();
            }
            return;
        }

        // handle terrain collision during charge
        if (isCharging && collision.gameObject.CompareTag("Terrain"))
        {
            ContactPoint2D cp = collision.contacts[0];
            if (Mathf.Abs(cp.normal.x) > 0.5f)
            {
                // stop the coroutine so it can’t flip back on
                if (chargeRoutine != null)
                    StopCoroutine(chargeRoutine);

                HitWall();
            }
        }
    }

    void StompedByPlayer(Collision2D collision)
    {
        isDead = true;
        chasing = false;
        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        boxCol.enabled = false;
        anim.SetTrigger("hit");    // stomp hit animation

        // bounce player
        if (collision.rigidbody != null)
        {
            collision.rigidbody.linearVelocity = new Vector2(collision.rigidbody.linearVelocity.x, 10f);
        }

        StartCoroutine(DestroyAfterAnimation("Rino_Hit"));
    }

    void HitWall()
    {
        isDead = false;
        chasing = false;
        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("hitwall"); // wall hit animation
    }

    IEnumerator DestroyAfterAnimation(string clipName)
    {
        float length = 0f;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                length = clip.length;
                break;
            }
        }
        yield return new WaitForSeconds(length);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
