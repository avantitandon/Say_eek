using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int score = 0;
    [SerializeField] private int lives = 3;

    [Header("UI References")]
    private bool isPaused = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Handle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void CollectItem()
    {
        score += 10;
        Debug.Log($"Score: {score}");
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log($"Lives remaining: {lives}");
        
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        score = 0;
        lives = 3;
    }

    private void GameOver()
    {
        Debug.Log($"Game Over! Final Score: {score}");
        Time.timeScale = 0f;
    }

    public int GetScore() => score;
    public int GetLives() => lives;
}
