using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // reference to the player object
    [SerializeField] private Transform player;

    // Update is called once per frame
    private void Update()
    {
        // Set the camera's position to follow the player's x and y position
        // Keep the camera's z position unchanged
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}
