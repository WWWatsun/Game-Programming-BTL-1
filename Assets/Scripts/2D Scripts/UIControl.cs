using TMPro;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    [SerializeField] private GameObject StartingUI;
    [SerializeField] private GameObject PausedUI;

    [SerializeField] private GameObject PlayingUI;
    [SerializeField] private TMP_Text PlayingText;

    [SerializeField] private GameObject ResultUI;
    [SerializeField] private TMP_Text ResultText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static UIControl Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void DisplayStartingUI()
    {
        StartingUI.SetActive(true);
        PausedUI.SetActive(false);
        PlayingUI.SetActive(false);
        ResultUI.SetActive(false);
    }

    public void DisplayPlayingUI(int score, int combo, int miss, int whip, float gameTime)
    {
        StartingUI.SetActive(false);
        PausedUI.SetActive(false);
        PlayingUI.SetActive(true);
        ResultUI.SetActive(false);
        PlayingText.text = $"Score: {score}\nCombo: {combo}\nMiss:{miss}\nWhip: {whip}\nGame Time: {gameTime}s";
    }

    public void DisplayPausedUI()
    {
        StartingUI.SetActive(false);
        PausedUI.SetActive(true);
        PlayingUI.SetActive(false);
        ResultUI.SetActive(false);
    }

    public void DisplayResultUI(int score, int hit, int combo, int miss, int whip)
    {
        StartingUI.SetActive(false);
        PausedUI.SetActive(false);
        PlayingUI.SetActive(false);
        ResultUI.SetActive(true);
        ResultText.text = $"Score: {score}\nHit: {hit}\nMiss: {miss}\nWhip: {whip}\nMax Combo: {combo}";
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
