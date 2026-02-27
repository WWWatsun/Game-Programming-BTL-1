using UnityEngine;
using TMPro;

public class OtherUIManager : MonoBehaviour
{
    public TargetSpawner spawner;
    public static OtherUIManager Instance;
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI finalScoreText;
    private bool isPaused = false;

    private int score;
    private float timeLeft;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    //void Update()
    //{
    //    if (isPlaying)
    //    {
    //        timeLeft -= Time.deltaTime;
    //        timeText.text = "Time: " + Mathf.Ceil(timeLeft);

    //        if (timeLeft <= 0)
    //        {
    //            GameOver();
    //        }
    //    }
    //}
    void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;   // Dừng game

        pausePanel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;   // Chạy lại

        pausePanel.SetActive(false);
    }
    void Update()
    {
        if (isPlaying)
        {
            timeLeft -= Time.deltaTime;
            timeText.text = "Time: " + Mathf.Ceil(timeLeft);

            if (timeLeft <= 0)
            {
                GameOver();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void StartGame()
    {
        score = 0;
        timeLeft = 10f;
        isPlaying = true;

        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);

        scoreText.text = "Score: 0";

        spawner.ResetSpawner();
        spawner.SpawnTarget();   // spawn 1 target duy nhất
    }

    public void AddScore(int amount)
    {
        if (!isPlaying) return;

        score += amount;
        scoreText.text = "Score: " + score;
        Debug.Log("Score Updated: " + score);
        //spawner.SpawnTarget();
    }

    void GameOver()
    {
        isPlaying = false;

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        finalScoreText.text = "Final Score: " + score;
    }

    public void ShowMainMenu()
    {
        isPlaying = false;
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void AddScoreWithBonus(float ttl, float reactionTime)
    {
        if (!isPlaying) return;

        int basePoints = 100;
        int bonusCap = 50;

        float bonusRatio = Mathf.Max(0, ttl - reactionTime) / ttl;
        int bonus = Mathf.FloorToInt(bonusRatio * bonusCap);

        int totalPoints = basePoints + bonus;

        score += totalPoints;

        scoreText.text = "Score: " + score;
    }

    private int missCount = 0;

    public void AddMiss()
    {
        missCount++;

        // Optional: trừ nhẹ
        score -= 10;
        if (score < 0) score = 0;

        scoreText.text = "Score: " + score;
    }
}