using UnityEngine;
using TMPro;

public class SummaryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSummary(int score, int hit, int miss, int whip, float accuracy, int highestCombo, float averageReactionTime, float bestReactionTime, int highscore)
    {
        string summary = "";

        summary += $"{score}\n";

        if (hit == 0)
        {
            summary += "Must've been a skill issue.\n";
        } 
        else
        {
            summary += $"{hit}\n";
            
        }

        summary += $"{miss}\n{whip}\n{accuracy:F2}%\n{highestCombo}\n{averageReactionTime * 1000:F2}ms\n";

        if (bestReactionTime == float.MaxValue)
        {
            summary += "Maybe try to shoot next time.\n";
        }
        else
        {
            summary += $"{bestReactionTime * 1000:F2}ms\n";
        }

        summary += $"{highscore}";

        valueText.text = summary;
    }
}
