using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{

    


    private void Start()
    {
        
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

    }

}
