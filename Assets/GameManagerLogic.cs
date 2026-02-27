using UnityEngine;
using TMPro;

public class GameManagerLogic : MonoBehaviour
{
    public static GameManagerLogic Instance;

    public GameObject target;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    public float gameDuration = 60f;
    private float timeLeft;

    private int score = 0;
    private int hits = 0;
    private int misses = 0;

    private bool gameActive = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = gameDuration;
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            EndGame();
        }
    }

    public void RegisterHit(float reactionTime, float ttl)
    {
        hits++;

        int basePoints = 100;
        int bonus = Mathf.RoundToInt(Mathf.Max(0, (ttl - reactionTime) / ttl * 50));

        score += basePoints + bonus;

        Debug.Log("Score: " + score);
    }

    public void RegisterMiss()
    {
        misses++;
        Debug.Log("Miss!");
    }

    void EndGame()
    {
        gameActive = false;
        target.SetActive(false);

        float accuracy = (hits + misses) > 0 ?
            (float)hits / (hits + misses) * 100f : 0f;

        Debug.Log("===== GAME OVER =====");
        Debug.Log("Score: " + score);
        Debug.Log("Hits: " + hits);
        Debug.Log("Misses: " + misses);
        Debug.Log("Accuracy: " + accuracy + "%");
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    //void UpdateHUD()
    //{
    //    timeText.text = "Time: " + Mathf.Ceil(timeLeft).ToString();
    //    scoreText.text = "Score: " + score.ToString();
    //}
}