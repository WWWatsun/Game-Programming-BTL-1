using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D Instance;

    [SerializeField] TargetScript_2D target;

    [SerializeField] float gameTime = 60f;

    [SerializeField] int baseScore = 100;
    [SerializeField] int missPenalty = 50;
    [SerializeField] int whipPenalty = 20;
    [SerializeField] int comboBonus = 10;

    [SerializeField] int combo = 0;
    [SerializeField] int score = 0;
    [SerializeField] int hit = 0;
    [SerializeField] int miss = 0;
    [SerializeField] int whip = 0;

    [SerializeField] int maxCombo = 0;


    public enum GameState
    {
        START,
        PLAYING,
        PAUSED,
        RESULT
    }
    public GameState state;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        state = GameState.START;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.START)
        {
            UIControl.Instance.DisplayStartingUI();
        }
        else if (state == GameState.PLAYING)
        {
            UIControl.Instance.DisplayPlayingUI(score, combo, miss, whip, gameTime);
            gameTime -= Time.deltaTime;
            gameTime = Mathf.Max(0f, gameTime);
            if (gameTime == 0)
            {
                state = GameState.RESULT;
                TargetPool.Instance.CleanUp(target);
            }
        }
        else if (state == GameState.PAUSED)
        {
            UIControl.Instance.DisplayPausedUI();
        }
        else if (state == GameState.RESULT)
        {
            UIControl.Instance.DisplayResultUI(score, hit, maxCombo, miss, whip);
        }

        Time.timeScale = (state == GameState.PLAYING ? 1f : 0f);
    }

    //State setting
    public void StartPlayingGame()
    {
        state = GameState.PLAYING;
        PlayerScript.Instance.SetPlaying(true);
        target = TargetPool.Instance.StartSpawning();
    }

    public void PauseGame()
    {
        if (state == GameState.PLAYING)
        {
            state = GameState.PAUSED;
        }

        else if (state == GameState.PAUSED)
        {
            state = GameState.PLAYING;
        }
    }

    public void RestartGame()
    {
        this.SetDefaultParameters();
        state = GameState.PLAYING;
        TargetPool.Instance.StartSpawning();
        TargetPool.Instance.SetDefaultParameters();
    }

    public GameState GetGameState()
    {
        return state;
    }

    //Hit profiles

    public void RegisterHit()
    {
        score += baseScore + combo * comboBonus;
        hit++;
        combo++;
        maxCombo = Mathf.Max(maxCombo, combo);
        TargetPool.Instance.DecreaseTTL();
        TargetPool.Instance.StartSpawning();
    }

    public void RegisterMiss()
    {
        Debug.Log("Miss");
        score -= missPenalty;
        miss++;
        combo = 0;
        TargetPool.Instance.IncreaseTTL();
        TargetPool.Instance.StartSpawning();
    }

    public void RegisterWhip()
    {
        score -= whipPenalty;
        whip++;
        combo = 0;
    }

    //Utils
    private void SetDefaultParameters()
    {
        gameTime = 60f;
        score = 0;
        combo = 0;
        hit = 0;
        miss = 0;
        whip = 0;
    }

}
