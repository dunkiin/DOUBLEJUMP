using System.Collections;
using System.Collections.Generic;
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

    // [SerializeField] private AudioSource jumpSoundEffect;


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

            sprite.flipX = dir > 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        float topY = transform.position.y + boxCol.offset.y + (boxCol.size.y * 0.5f);

        float playerY = collision.transform.position.y;

        if (playerY > topY)
        {
            Kill();

            // bounce the player
            if (collision.rigidbody != null) {
                collision.rigidbody.linearVelocity = new Vector2(collision.rigidbody.linearVelocity.x, 10f);
            }
        }
        else
        {
            collision.gameObject.GetComponent<PlayerLife>()?.Die();
        }
    }

    private void Kill()
    {
        isDead = true;
        chasing = false;
        rb.linearVelocity = Vector2.zero;
        boxCol.enabled = false;

        // play the death sound effect
        anim.SetTrigger("hit");
        Destroy(gameObject, 1f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
