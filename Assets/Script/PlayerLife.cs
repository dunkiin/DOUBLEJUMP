using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{

    private Animator anim;

    private void Start()
    {
        anim = GetComponect<Animator>();
    }

    private void OnCollisionEneter2D(Collision2D collision)
    {
        if (collision.gameObject.CompateTag("Trap"))
        {
            Die();
        }
    }


    private void Die()
    {
        anim.SetTrigger("death");
    }

}
