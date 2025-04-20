using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    [SerializeField] private float chaseSpeed = 3f; // Speed when chasing the player
    [SerializeField] private float detectionRange = 5f; // Range within which the enemy can detect the player
    [SerializeField] private float returnSpeed = 2f; // Speed at which the enemy returns to the original position

    private bool isChasing = false; // Flag to check if the enemy is chasing the player
    private Vector3 originalPosition; // Store the enemy's original position
    private GameObject player; // Reference to the player

    private enum EnemyState { Idle, Running };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");  // Cache the player reference
        originalPosition = transform.position; // Store the enemy's initial position
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is chasing, continue chasing the player
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            // If the player is not in range, return to the original position
            ReturnToOriginalPosition();
            DetectPlayer();
        }

        UpdateAnimationState();
    }

    private void DetectPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            if (distanceToPlayer < detectionRange)
            {
                isChasing = true; // Start chasing if the player is within detection range
            }
            else
            {
                isChasing = false;
            }
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            // Move the enemy towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * chaseSpeed);

            // Flip the sprite based on the player's position
            if (player.transform.position.x < transform.position.x)
            {
                sprite.flipX = true;  // Flip sprite to face left
            }
            else
            {
                sprite.flipX = false;  // Flip sprite to face right
            }
        }
    }

    private void ReturnToOriginalPosition()
    {
        // If the player is not within range, move the enemy back to the original position
        if (Vector2.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, Time.deltaTime * returnSpeed);
        }
        else
        {
            isChasing = false; // Stop chasing once the enemy returns to its original position
        }
    }

    private void UpdateAnimationState()
    {
        // Update the animation state based on chasing or idle
        if (isChasing)
        {
            anim.SetInteger("state", (int)EnemyState.Running); // Update to Running state
        }
        else
        {
            anim.SetInteger("state", (int)EnemyState.Idle); // Update to Idle state
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize the detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerLife>().Die();
        }
    }
}
