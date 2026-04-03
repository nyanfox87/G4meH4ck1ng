using UnityEngine;
using UnityEngine.UI;

public class BakeMinigame : MonoBehaviour
{
    [Header("Slider Settings")]
    public Slider bakeSlider;
    public Image sliderFill;
    public Text positionText;
    public Text resultText;
    public Button backButton;

    // === Public for memory scanning (intentional) ===
    public float f_BakePos = 0f;
    public bool isStopped = false;

    [Header("Speed")]
    public float sliderSpeed = 30f;
    private float direction = 1f;

    void Start()
    {
        if (bakeSlider != null)
        {
            bakeSlider.minValue = 0f;
            bakeSlider.maxValue = 100f;
        }
        if (resultText != null) resultText.text = "";
        if (backButton != null) backButton.onClick.AddListener(GoBack);
    }

    void Update()
    {
        // Mouse click stops the slider
        if (Input.GetMouseButtonDown(0) && !isStopped)
        {
            isStopped = true;
            CheckResult();
        }

        // Escape resumes
        if (Input.GetKeyDown(KeyCode.Escape) && isStopped)
        {
            isStopped = false;
            if (resultText != null) resultText.text = "";
        }

        if (!isStopped)
        {
            // Bounce slider back and forth
            f_BakePos += sliderSpeed * direction * Time.deltaTime;
            if (f_BakePos >= 100f) { f_BakePos = 100f; direction = -1f; }
            if (f_BakePos <= 0f) { f_BakePos = 0f; direction = 1f; }
        }

        // Update UI
        if (bakeSlider != null) bakeSlider.value = f_BakePos;
        if (positionText != null) positionText.text = f_BakePos.ToString("F2");

        // Update slider color based on zone
        UpdateSliderColor();
    }

    private void UpdateSliderColor()
    {
        if (sliderFill == null) return;

        if (f_BakePos >= 48f && f_BakePos <= 52f)
            sliderFill.color = new Color(1f, 0.65f, 0f); // Orange
        else if ((f_BakePos >= 40f && f_BakePos < 48f) || (f_BakePos > 52f && f_BakePos <= 60f))
            sliderFill.color = Color.yellow;
        else
            sliderFill.color = Color.gray;
    }

    private void CheckResult()
    {
        if (Mathf.Approximately(f_BakePos, 50.00f))
        {
            if (resultText != null) resultText.text = "PERFECT! " + FlagManager.FLAG_03;
            if (FlagManager.Instance != null) FlagManager.Instance.UnlockFlag03();
        }
        else
        {
            float diff = Mathf.Abs(f_BakePos - 50f);
            if (resultText != null) resultText.text = "Missed by " + diff.ToString("F2") + " - Try again! (Press Esc)";
        }
    }

    private void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
