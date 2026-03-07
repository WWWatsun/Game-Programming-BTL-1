using System.Collections;
using UnityEngine;

public class GameManager2D : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private int scoreOnHit = 100;
    [SerializeField] private int comboBonus = 30;
    [SerializeField] private int speedBonus = 50;
    [SerializeField] private float speedBonusThreshold = 1f;
    [SerializeField] private int missPenalty = 50;

    private int score, hit, miss;
    private int comboCount, highestCombo;

    private float totalReactionTime;
    private float averageReactionTime = -1f;
    private float bestReactionTime = float.MaxValue;

    private float currentTime;

    public enum GameState { START, PLAYING, PAUSED, RESULTS, SETTINGS }
    private GameState currentState = GameState.START;
    private GameState previousState = GameState.START;

    public static GameManager2D Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        currentState = GameState.START;
        Time.timeScale = 0f;
    }

    void Update()
    {
        // UI flow
        if (UIManager.Instance != null)
        {
            if (currentState == GameState.START) UIManager.Instance.ShowTutorialUI();
            else if (currentState == GameState.PLAYING)
            {
                currentTime = Mathf.Max(currentTime - Time.deltaTime, 0f);
                UIManager.Instance.ShowMainGameUI(score, hit, miss, comboCount, currentTime);
            }
            else if (currentState == GameState.PAUSED) UIManager.Instance.ShowPauseUI();
            else if (currentState == GameState.RESULTS)
            {
                float accuracy = hit + miss > 0 ? (float)hit / (hit + miss) * 100f : 0f;
                averageReactionTime = hit > 0 ? totalReactionTime / hit : -1f;

                // UIManager/SummaryUI của bạn hiện vẫn có "whip" -> 2D cho whip = 0
                UIManager.Instance.ShowSummaryUI(score, hit, miss, 0, accuracy, highestCombo, averageReactionTime, bestReactionTime);
            }
            else if (currentState == GameState.SETTINGS) UIManager.Instance.ShowSettingsUI();
        }

        Time.timeScale = (currentState == GameState.PLAYING) ? 1f : 0f;
    }

    private IEnumerator GameLoop()
    {
        currentTime = gameDuration;
        ResetGameStats();

        // 2D spawner
        TargetBool2D.Instance?.StartSpawning();

        yield return new WaitForSeconds(gameDuration);

        TargetBool2D.Instance?.StopSpawning();
        currentState = GameState.RESULTS;
        StopAllCoroutines();
    }

    private void ResetGameStats()
    {
        score = 0; hit = 0; miss = 0;
        comboCount = 0; highestCombo = 0;
        totalReactionTime = 0f;
        averageReactionTime = -1f;
        bestReactionTime = float.MaxValue;
    }

    private void CalculateScoreOnHit(float reactionTime)
    {
        score += scoreOnHit;

        if (comboCount > highestCombo) highestCombo = comboCount;

        if (comboCount > 1)
            score += comboBonus * (comboCount - 1);

        if (reactionTime < speedBonusThreshold)
            score += speedBonus;
    }

    // ------- Public API (giống 3D để UI dễ gọi) -------
    public void StartGame()
    {
        currentState = GameState.PLAYING;
        StartCoroutine(GameLoop());
    }

    public void PauseGame()
    {
        if (currentState == GameState.PLAYING) currentState = GameState.PAUSED;
        else if (currentState == GameState.PAUSED) currentState = GameState.PLAYING;
    }

    public void OpenSettings()
    {
        if (currentState != GameState.SETTINGS)
        {
            previousState = currentState;
            currentState = GameState.SETTINGS;
        }
        else currentState = previousState;
    }

    public void RegisterHit(float reactionTime)
    {
        hit++;
        comboCount++;
        CalculateScoreOnHit(reactionTime);

        totalReactionTime += reactionTime;
        if (reactionTime < bestReactionTime) bestReactionTime = reactionTime;
    }

    public void RegisterMiss()
    {
        miss++;
        comboCount = 0;
        score = Mathf.Max(score - missPenalty, 0);
    }

    public GameState GetGameState() => currentState;
}