using UnityEngine;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;


    private void Start()
    {
        // Ensure hidden and unpaused at start
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        // Only listen if we have a panel to toggle
        if (pausePanel == null) return;

        // Toggle pause when the configured key is pressed
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                OnReturnToGameButton();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void OnReturnToGameButton()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public void OnExitGameButton()
    {
        Debug.Log("exit game");
        Application.Quit();
    }
}
