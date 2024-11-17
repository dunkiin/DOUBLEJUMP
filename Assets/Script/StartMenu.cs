using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StartMenu : MonoBehaviour
{

    public void StartGame()
    {
        // clicked the button, button including index element
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
