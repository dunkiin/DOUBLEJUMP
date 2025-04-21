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
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // see if any contact point has a mostly-upward normal
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
            // kill chicken & bounce player
            KillChicken();
            var prb = collision.rigidbody;
            if (prb != null)
                prb.velocity = new Vector2(prb.velocity.x, 10f);
        }
        else
        {
            // side‑hit → kill the player
            collision.gameObject.GetComponent<PlayerLife>()?.Die();
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
