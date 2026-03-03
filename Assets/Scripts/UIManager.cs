using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Main menu UI")]
    [SerializeField] private GameObject homeUI;
    [SerializeField] private GameObject modeSelectUI;
    [SerializeField] private GameObject settingsUI;

    [Header("In-game UI")]
    [SerializeField] private MainGameUI mainGameUI;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private SummaryUI summaryUI;
    [SerializeField] private GameObject pauseUI;

    // Singleton instance
    public static UIManager Instance { get; private set; }

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMainGameUI(int score, int hit, int miss, int comboCount, float currentTime)
    {
        mainGameUI.gameObject.SetActive(true);
        tutorialUI.SetActive(false);
        summaryUI.gameObject.SetActive(false);
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);

        mainGameUI.UpdateStats(score, hit, miss, comboCount, currentTime);
    }

    public void ShowTutorialUI()
    {
        mainGameUI.gameObject.SetActive(false);
        tutorialUI.SetActive(true);
        summaryUI.gameObject.SetActive(false);
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    public void ShowSummaryUI(int score, int hit, int miss, int whip, float accuracy, int highestCombo, float averageReactionTime, float bestReactionTime)
    {
        mainGameUI.gameObject.SetActive(false);
        tutorialUI.SetActive(false);
        summaryUI.gameObject.SetActive(true);
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);

        summaryUI.UpdateSummary(score, hit, miss, whip, accuracy, highestCombo, averageReactionTime, bestReactionTime);
    }

    public void ShowPauseUI()
    {
        mainGameUI.gameObject.SetActive(false);
        tutorialUI.SetActive(false);
        summaryUI.gameObject.SetActive(false);
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void ShowSettingsUI()
    {
        if (pauseUI) pauseUI.SetActive(false);
        if (homeUI) homeUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void ShowHomeUI()
    {
        settingsUI.SetActive(false);
        homeUI.SetActive(true);
        modeSelectUI.SetActive(false);
    }

    public void ShowModeSelectUI()
    {
        settingsUI.SetActive(false);
        homeUI.SetActive(false);
        modeSelectUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
