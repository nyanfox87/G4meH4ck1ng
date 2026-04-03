using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IcingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform drawArea;
    public Image drawCursor;
    public Text precisionText;
    public Text resultText;
    public Button backButton;
    public Button clearButton;

    [Header("Circle Settings")]
    public float targetRadius = 150f;
    public Vector2 circleCenter = new Vector2(0, 0);

    // Tracking
    private List<Vector2> drawnPoints = new List<Vector2>();
    private bool isDrawing = false;
    public float drawPrecision = 0f;

    // Line rendering
    private List<GameObject> lineSegments = new List<GameObject>();

    void Start()
    {
        if (resultText != null) resultText.text = "Trace a circle around the cookie!";
        if (backButton != null) backButton.onClick.AddListener(GoBack);
        if (clearButton != null) clearButton.onClick.AddListener(ClearDrawing);
    }

    void Update()
    {
        if (drawArea == null) return;

        // Check if mouse is over draw area
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawArea, Input.mousePosition, null, out localPoint);

        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            drawnPoints.Clear();
            ClearLines();
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            drawnPoints.Add(localPoint);
            UpdateDrawCursor(localPoint);
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
            CalculatePrecision();
        }

        if (precisionText != null)
            precisionText.text = "Precision: " + drawPrecision.ToString("F1") + "%";
    }

    private void UpdateDrawCursor(Vector2 pos)
    {
        if (drawCursor != null)
        {
            drawCursor.rectTransform.anchoredPosition = pos;
        }
    }

    private void CalculatePrecision()
    {
        if (drawnPoints.Count < 10)
        {
            drawPrecision = 0f;
            if (resultText != null) resultText.text = "Draw a bigger circle! (Need more points)";
            return;
        }

        float totalError = 0f;
        for (int i = 0; i < drawnPoints.Count; i++)
        {
            float dist = Vector2.Distance(drawnPoints[i], circleCenter);
            float error = Mathf.Abs(dist - targetRadius) / targetRadius;
            totalError += error;
        }

        float avgError = totalError / drawnPoints.Count;
        drawPrecision = Mathf.Clamp((1f - avgError) * 100f, 0f, 100f);

        if (drawPrecision > 99f)
        {
            if (resultText != null) resultText.text = "PERFECT CIRCLE! " + FlagManager.FLAG_04;
            if (FlagManager.Instance != null) FlagManager.Instance.UnlockFlag04();
        }
        else if (drawPrecision > 80f)
        {
            if (resultText != null) resultText.text = "Good! " + drawPrecision.ToString("F1") + "% - try for 99%+";
        }
        else
        {
            if (resultText != null) resultText.text = drawPrecision.ToString("F1") + "% - Keep practicing!";
        }
    }

    private void ClearDrawing()
    {
        drawnPoints.Clear();
        ClearLines();
        drawPrecision = 0f;
        if (resultText != null) resultText.text = "Trace a circle around the cookie!";
    }

    private void ClearLines()
    {
        foreach (var seg in lineSegments)
        {
            if (seg != null) Destroy(seg);
        }
        lineSegments.Clear();
    }

    private void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
