using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f; // Duration of the game in seconds
    [SerializeField] private int scoreOnHit = 100;
    [SerializeField] private int comboBonus = 30;
    [SerializeField] private int speedBonus = 50;
    [SerializeField] private float speedBonusThreshold = 1f;
    [SerializeField] private int missPenalty = 50;
    [SerializeField] private int whipPenalty = 20;

    private int score = 0;
    private int hit = 0;
    private int miss = 0;
    private int whip = 0;

    private int comboCount = 0;
    private int highestCombo = 0;

    private float totalReactionTime = 0f;
    private float averageReactionTime = -1f;
    private float bestReactionTime = float.MaxValue;

    private float currentTime = 0f;

    public enum GameState
    {
        START,
        PLAYING,
        PAUSED,
        RESULTS,
        SETTINGS
    }
    private GameState currentState = GameState.START;
    private GameState previousState = GameState.START;

    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = GameState.START;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.START)
        {
            UIManager.Instance.ShowTutorialUI();
        }
        else if (currentState == GameState.PLAYING)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f); // Ensure time doesn't go negative
            UIManager.Instance.ShowMainGameUI(score, hit, miss, comboCount, currentTime);
        }
        else if (currentState == GameState.PAUSED)
        {
            UIManager.Instance.ShowPauseUI();
        }
        else if (currentState == GameState.RESULTS)
        {
            float accuracy = hit + miss > 0 ? (float)hit / (hit + miss) * 100f : 0f;
            averageReactionTime = hit > 0 ? totalReactionTime / hit : -1f;
            // fix lỗi 
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowSummaryUI(score, hit, miss, whip, accuracy, highestCombo, averageReactionTime, bestReactionTime);
            }        
        }
        else if (currentState == GameState.SETTINGS)
        {
            UIManager.Instance.ShowSettingsUI();
        }

        Time.timeScale = currentState == GameState.PLAYING? 1f : 0f; // Pause the game when not in PLAYING state
    }

    // -------------
    // Private methods
    // -------------

    private IEnumerator GameLoop()
    {
        Debug.Log("Starting Game");
        // FPS-only: Scene 2D may not have Player.
        if (Player.Instance != null)
        {
            Player.Instance.ResetPlayer();
            Player.Instance.StartGame();
        }
        // TargetBool.Instance.ResetTimeToLive();
        // TargetBool.Instance.StartSpawning();
        // -> Chỉnh GameManager.cs để 2D dùng TargetBool2D, 3D vẫn dùng TargetBool
        if (TargetBool2D.Instance != null) TargetBool2D.Instance.StartSpawning();
else if (TargetBool.Instance != null) TargetBool.Instance.StartSpawning();
        currentTime = gameDuration;
        ResetGameStats();

        yield return new WaitForSeconds(gameDuration);

        // End the game
        Debug.Log("Ending Game");
        if (Player.Instance != null)
        {
            Player.Instance.EndGame();
        }
        // TargetBool.Instance.StopSpawning();
        // -> Chỉnh GameManager.cs để 2D dùng TargetBool2D, 3D vẫn dùng TargetBool
        if (TargetBool2D.Instance != null) TargetBool2D.Instance.StopSpawning();
        else if (TargetBool.Instance != null) TargetBool.Instance.StopSpawning();
        currentState = GameState.RESULTS;
        StopAllCoroutines();
    }

    private void CalculateScoreOnHit(float reactionTime)
    {
        score += scoreOnHit;
        if (comboCount > highestCombo)
        {
            highestCombo = comboCount;
        }

        // Check for combo bonus
        if (comboCount > 1)
        {
            Debug.Log($"Combo bonus +{comboBonus * (comboCount - 1)}");
            score += comboBonus * (comboCount - 1);
        }

        // Check for speed bonus
        if (reactionTime < speedBonusThreshold)
        {
            Debug.Log($"Speed bonus +{speedBonus}");
            score += speedBonus;
        }
    }

    private void ResetGameStats()
    {
        score = 0;
        hit = 0;
        miss = 0;
        whip = 0;
        comboCount = 0;
        highestCombo = 0;
        totalReactionTime = 0f;
        averageReactionTime = -1f;
        bestReactionTime = float.MaxValue;
    }

    // -------------
    // Public methods
    // -------------

    public void StartGame()
    {
        StartCoroutine(GameLoop());
        currentState = GameState.PLAYING;
    }

    public void PauseGame()
    {
        if (currentState == GameState.PLAYING)
        {
            currentState = GameState.PAUSED;
            if (Player.Instance != null)
        {
            Player.Instance.EndGame();
        }
        }
        else if (currentState == GameState.PAUSED)
        {
            currentState = GameState.PLAYING;
            Player.Instance.StartGame();
        }
    }

    public void OpenSettings()
    {
        if (currentState != GameState.SETTINGS)
        {
            previousState = currentState;
            currentState = GameState.SETTINGS;
        }
        else
        {
            currentState = previousState;
        }
    }

    public void RegisterHit(float reactionTime)
    {
        hit++;
        comboCount++;
        CalculateScoreOnHit(reactionTime);

        // Update reaction time stats
        totalReactionTime += reactionTime;
        if (reactionTime < bestReactionTime)
        {
            bestReactionTime = reactionTime;
        }

        // Reduce time to live 
        // Reduce time-to-live only for 3D spawner (TargetBool).
        // 2D uses TargetBool2D difficulty by elapsed time, so NO touch TTL here.
        if (TargetBool2D.Instance == null && TargetBool.Instance != null)
        {
            TargetBool.Instance.ReduceTimeToLive();
        }    
    }

    public void RegisterMiss()
    {
        miss++;
        comboCount = 0;
        score = Mathf.Max(score - missPenalty, 0); // Ensure score doesn't go negative
        Debug.Log($"Miss penalty -{missPenalty}");
    }

    public void RegisterWhip()
    {
        whip++;
        score = Mathf.Max(score - whipPenalty, 0); // Ensure score doesn't go negative
        Debug.Log($"Whip!");
    }

    public GameState GetGameState()
    {
        return currentState;
    }
}
