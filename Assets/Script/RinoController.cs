using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class RinoController : MonoBehaviour
{
    [Header("Speeds & Timings")]
    public float detectionRange = 8f;   // when the player is this close, start moving
    public float walkSpeed = 3f;   // initial run speed
    public float chargeSpeed = 12f;  // full‑on charge speed
    public float chargeDelay = 0.8f; // delay before going from walk to charge

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

        // Lock Y‑position & rotation so Rino stays on platform
        rb.constraints = RigidbodyConstraints2D.FreezePositionY
                       | RigidbodyConstraints2D.FreezeRotation;
        boxCol.isTrigger = false;
    }

    void Update()
    {
        if (isDead) return;

        // Detect player
        if (!chasing &&
            Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            chasing = true;
            dir = player.position.x > transform.position.x ? 1 : -1;
            anim.SetInteger("state", 1);   // Rino_Running
            StartCoroutine(StartCharge());
        }
    }

    IEnumerator StartCharge()
    {
        // Run for a bit before charging
        yield return new WaitForSeconds(chargeDelay);
        if (!isDead)
            isCharging = true;
    }

    void FixedUpdate()
    {
        if (!chasing || isDead) return;

        float speed = isCharging ? chargeSpeed : walkSpeed;
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        sprite.flipX = dir > 0; // face direction of movement
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // --- Player collision: stomp vs side hit ---
        if (collision.gameObject.CompareTag("Player"))
        {
            bool stomped = false;
            foreach (var cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f)
                {
                    stomped = true;
                    break;
                }
            }

            if (stomped)
            {
                StompedByPlayer();
                if (collision.rigidbody != null)
                    collision.rigidbody.velocity =
                        new Vector2(collision.rigidbody.velocity.x, 10f);
            }
            else
            {
                // Side hit: kill player
                collision.gameObject.GetComponent<PlayerLife>()?.Die();
            }
            return;
        }

        // --- Wall collision during charge: slam ---
        if (isCharging)
        {
            foreach (var cp in collision.contacts)
            {
                if (Mathf.Abs(cp.normal.x) > 0.5f)
                {
                    HitWall();
                    return;
                }
            }
        }
    }

    void StompedByPlayer()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        boxCol.enabled = false;
        anim.SetTrigger("hit");  // Rino_Hit
        StartCoroutine(DestroyAfterAnimation("Rino_Hit"));
    }

    void HitWall()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        boxCol.enabled = false;
        anim.SetTrigger("hitwall");  // Rino_HitWall
        StartCoroutine(DestroyAfterAnimation("Rino_HitWall"));
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
}
