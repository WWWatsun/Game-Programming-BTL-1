using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private TMP_InputField SenInput;
    [SerializeField] private Slider SenSlider;
    [SerializeField] private TMP_InputField gameDurationInput;
    [SerializeField] private Slider gameDurationSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMouseSensitivityText()
    {
        if (Player.Instance != null)
        {
            Player.Instance.SetMouseSensitivity(float.Parse(SenInput.text));
        }
    }

    public void SetMouseSensitivitySlider()
    {
        if (Player.Instance != null)
        {
            Player.Instance.SetMouseSensitivity(SenSlider.value);
        }

        SenInput.text = SenSlider.value.ToString();
    }

    public void SetGameDurationText()
    {
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.SetGameDuration(float.Parse(gameDurationInput.text));
        } else if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.SetGameDuration(float.Parse(gameDurationInput.text));
        }
    }

    public void SetGameDurationSlider()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameDuration(gameDurationSlider.value);
        }
        else if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.SetGameDuration(gameDurationSlider.value);
        }

        gameDurationInput.text = gameDurationSlider.value.ToString();
    }
}
