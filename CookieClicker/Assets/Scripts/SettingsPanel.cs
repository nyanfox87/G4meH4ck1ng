using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI References")]
    public Slider speedSlider;
    public Text speedValueText;
    public Button resetButton;
    public Button hintButton;
    public Text hintText;

    private int currentHintIndex = 0;

    void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.minValue = 1f;
            speedSlider.maxValue = 20f;
            speedSlider.value = GameManager.Instance != null ? GameManager.Instance.baseDropSpeed : 2f;
            speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        }

        if (resetButton != null) resetButton.onClick.AddListener(OnReset);
        if (hintButton != null) hintButton.onClick.AddListener(OnHint);
        if (hintText != null) hintText.text = "";
    }

    private void OnSpeedChanged(float value)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.baseDropSpeed = value;
        }
        if (speedValueText != null)
        {
            speedValueText.text = "Speed: " + value.ToString("F1");
        }
    }

    private void OnReset()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HardReset();
        }
        if (FlagManager.Instance != null)
        {
            FlagManager.Instance.flag01_Solved = false;
            FlagManager.Instance.flag02_Solved = false;
            FlagManager.Instance.flag03_Solved = false;
            FlagManager.Instance.flag04_Solved = false;
            FlagManager.Instance.flag05_Solved = false;
        }
        if (speedSlider != null) speedSlider.value = 2f;
        if (hintText != null) hintText.text = "Progress reset!";
    }

    private void OnHint()
    {
        if (hintText != null && FlagManager.FlagHints.Length > 0)
        {
            hintText.text = FlagManager.FlagHints[currentHintIndex % FlagManager.FlagHints.Length];
            currentHintIndex++;
        }
    }
}
