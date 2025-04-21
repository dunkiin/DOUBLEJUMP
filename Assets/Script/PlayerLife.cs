using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{

    // reference
    private Rigidbody2D rb;
    private Animator anim;


    [SerializeField] private AudioSource deathSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Check all contact normals ¨C if any point upward, it's a stomp, so ignore
            foreach (var cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f)
                {
                    return;
                }
            }
            Die();
        }
    }


    public void Die() 
    {

        // play death sound
        deathSoundEffect.Play();

        // After death, body type will change to static body
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
