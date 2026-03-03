using UnityEngine;
using TMPro;

public class MainGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text timeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateStats(int score, int hit, int miss, int comboCount, float currentTime)
    {
        statsText.text = $"Score: {score}\nHit: {hit}\nMiss: {miss}\nCombo: {comboCount}";
        timeText.text = $"{currentTime:F1}s";
    }
}
