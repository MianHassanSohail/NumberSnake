using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TMP_Text finalScoreText;

    private bool gameActive = true;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        EventManager.Instance.Events.OnScoreChanged.AddListener(UpdateScore);
        EventManager.Instance.Events.OnGameOver.AddListener(HandleGameOver);
        EventManager.Instance.Events.OnLevelComplete.AddListener(HandleLevelComplete);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Events.OnScoreChanged.RemoveListener(UpdateScore);
        EventManager.Instance.Events.OnGameOver.RemoveListener(HandleGameOver);
        EventManager.Instance.Events.OnLevelComplete.RemoveListener(HandleLevelComplete);
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void HandleGameOver(int finalScore)
    {
        if (!gameActive) return;
        gameActive = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void HandleLevelComplete(int finalScore)
    {
        if (!gameActive) return;
        gameActive = false;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
