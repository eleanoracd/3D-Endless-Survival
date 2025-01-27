using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    [SerializeField] private GameObject gameOverUI; 
    [SerializeField] private Text timeSurvivedText;

    private bool isGamePaused = false;
    private float timeSurvived = 0f;
    private bool isGameOver = false;

    [Header("Player Settings")]
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    private void Update()
    {
        if (isGameOver) return;

        if (!playerMovement.enabled && !isGameOver)
        {
            TriggerGameOver();
        }

        if (!isGamePaused)
        {
            timeSurvived += Time.deltaTime;
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (timeSurvivedText != null)
        {
            timeSurvivedText.text = $"Time Survived: {timeSurvived:F2} seconds";
        }

        PauseGame();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
