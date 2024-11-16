using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemsCollector : MonoBehaviour
{

    // set the dynamic number in components
    [SerializeField] private Text cherriesText;

    [SerializeField] private AudioSource collectionSoundEffect;

    private int cherries = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            // collecting music
            collectionSoundEffect.Play();  

            // player touch it, items disappear
            Destroy(collision.gameObject);
            cherries++;
            cherriesText.text = "Cherries: " + cherries;
        }
    }
}
