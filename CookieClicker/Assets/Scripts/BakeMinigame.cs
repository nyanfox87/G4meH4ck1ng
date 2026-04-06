using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

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
        // 1. STOP: Click Left Mouse OR Press Space
        bool stopInput = (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) || 
                         (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        if (stopInput && !isStopped)
        {
            isStopped = true;
            // Start the delayed check instead of calling it directly
            StartCoroutine(DelayedCheck(0.5f)); 
        }

        // 2. RESUME: Press Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isStopped)
        {
            isStopped = false;
            if (resultText != null) resultText.text = "";
            StopAllCoroutines(); // Stop any pending checks if they hit Esc early
        }

        if (!isStopped)
        {
            f_BakePos += sliderSpeed * direction * Time.deltaTime;
            if (f_BakePos >= 100f) { f_BakePos = 100f; direction = -1f; }
            if (f_BakePos <= 0f) { f_BakePos = 0f; direction = 1f; }
        }

        // Update UI
        if (bakeSlider != null) bakeSlider.value = f_BakePos;
        if (positionText != null) positionText.text = f_BakePos.ToString("F2");

        UpdateSliderColor();
    }

    // This is the new Coroutine for the delay
    IEnumerator DelayedCheck(float delay)
    {
        if (resultText != null) resultText.text = "Checking..."; // Optional: Tell player it's processing
        
        yield return new WaitForSeconds(delay); // Wait for 0.5 seconds
        
        CheckResult();
    }

    private void UpdateSliderColor()
    {
        if (sliderFill == null) return;

        // We use "else" to ensure only one color is applied at a time
        if (f_BakePos >= 48f && f_BakePos <= 52f)
        {
            // Perfect Zone: Orange
            sliderFill.color = new Color(1f, 0.65f, 0f, 1f); 
        }
        else if ((f_BakePos >= 40f && f_BakePos < 48f) || (f_BakePos > 52f && f_BakePos <= 60f))
        {
            // Near Zone: Yellow
            sliderFill.color = Color.yellow;
        }
        else
        {
            // Fail Zone: Gray (Darker gray so it's visible)
            sliderFill.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        }
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
