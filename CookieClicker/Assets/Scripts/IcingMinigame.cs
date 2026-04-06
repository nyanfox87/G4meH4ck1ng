using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class IcingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform drawArea;
    public Image drawCursor;
    public Text precisionText;
    public Text resultText;
    public Button backButton;
    public Button clearButton;

    [Header("Line Visuals")]
    public GameObject brushPrefab; 
    public float brushSize = 12f; // 奶油的大小
    public float spacing = 3f;   // 補點的間距，越小越平滑但效能消耗越高

    [Header("Circle Settings")]
    public float targetRadius = 150f;
    public Vector2 circleCenter = Vector2.zero;

    // 數據追蹤
    private List<Vector2> currentStrokePoints = new List<Vector2>();
    private List<GameObject> allVisualSegments = new List<GameObject>();
    private bool isDrawing = false;
    private Vector2 lastPoint; // 記錄上一幀的位置
    public float drawPrecision = 0f;

    void Start()
    {
        if (resultText != null) resultText.text = "Trace the cookie! Drawings stay until you Clear.";
        if (backButton != null) backButton.onClick.AddListener(GoBack);
        if (clearButton != null) clearButton.onClick.AddListener(OnClearButtonClicked);
    }

    void Update()
    {
        if (drawArea == null) return;

        // 取得滑鼠座標
        Vector2 mousePos = Pointer.current.position.ReadValue();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawArea, mousePos, null, out localPoint);

        // 1. 按下：開始新筆劃
        if (Pointer.current.press.wasPressedThisFrame)
        {
            isDrawing = true;
            lastPoint = localPoint;
            SpawnBrushPoint(localPoint);
            currentStrokePoints.Add(localPoint);
        }

        // 2. 按住：持續繪圖
        if (Pointer.current.press.isPressed && isDrawing)
        {
            UpdateDrawCursor(localPoint);

            float dist = Vector2.Distance(lastPoint, localPoint);
            if (dist > spacing)
            {
                // 計算需要補多少點來讓軌跡連續
                int segments = Mathf.FloorToInt(dist / spacing);
                for (int i = 1; i <= segments; i++)
                {
                    Vector2 lerpPoint = Vector2.Lerp(lastPoint, localPoint, (float)i / segments);
                    SpawnBrushPoint(lerpPoint);
                    currentStrokePoints.Add(lerpPoint);
                    CalculatePrecision();
                }
                lastPoint = localPoint;
            }
        }

        // 3. 放開：結束並評分
        if (Pointer.current.press.wasReleasedThisFrame && isDrawing)
        {
            isDrawing = false;
            CalculatePrecision();
        }

        if (precisionText != null)
            precisionText.text = "Precision: " + drawPrecision.ToString("F1") + "%";
    }

    private void SpawnBrushPoint(Vector2 pos)
    {
        if (brushPrefab == null) return;
        GameObject go = Instantiate(brushPrefab, drawArea);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(brushSize, brushSize);
        allVisualSegments.Add(go);
    }

    private void UpdateDrawCursor(Vector2 pos)
    {
        // if (drawCursor != null) drawCursor.rectTransform.anchoredPosition = pos;
    }

    // private void CalculatePrecision()
    // {
    //     float totalError = 0f;
    //     foreach (Vector2 pt in currentStrokePoints)
    //     {
    //         float dist = Vector2.Distance(pt, circleCenter);
    //         float error = Mathf.Abs(dist - targetRadius) / targetRadius;
    //         totalError += error;
    //     }

    //     float avgError = totalError / currentStrokePoints.Count;
    //     drawPrecision = Mathf.Clamp((1f - avgError) * 100f, 0f, 100f);

    //     if(precisionText != null) precisionText.text = "Precision: " + drawPrecision.ToString("F1") + "%";

    //     if (drawPrecision < 98f) {
    //         resultText.text = "Try to be more smooth with your circle (>98%)";
    //     }
    //     else if (currentStrokePoints.Count < 100) {
    //         resultText.text = "Draw more to earn the flag";
    //     }
    //     else {
    //         resultText.text = "Perfect! This is your flag: " + FlagManager.FLAG_04;
    //         if (FlagManager.Instance != null) FlagManager.Instance.UnlockFlag04();

    //     }
    // }

    private void CalculatePrecision()
{
    // 如果點數太少（例如剛開始畫），不進行計算，避免除以零
    if (currentStrokePoints.Count < 10) return;

    // 1. 計算所有點到「指定中心點(circleCenter)」的平均距離
    // 這個平均距離就是玩家目前畫出來的「圓形半徑」
    float sumDistance = 0f;
    foreach (Vector2 pt in currentStrokePoints)
    {
        sumDistance += Vector2.Distance(pt, circleCenter);
    }
    float averageRadius = sumDistance / currentStrokePoints.Count;

    // 安全檢查：如果玩家畫得太小（幾乎是一個點），則不判定通過
    if (averageRadius < 10f) 
    {
        drawPrecision = 0f;
        resultText.text = "The circle is too small!";
        return;
    }

    // 2. 計算每個點相對於「平均半徑」的誤差百分比
    float totalError = 0f;
    foreach (Vector2 pt in currentStrokePoints)
    {
        float currentDist = Vector2.Distance(pt, circleCenter);
        // 誤差計算公式：|當前距離 - 平均半徑| / 平均半徑
        // 這代表這個點偏離「理想圓周」的程度
        float error = Mathf.Abs(currentDist - averageRadius) / averageRadius;
        totalError += error;
    }

    // 3. 換算成百分比精確度
    float avgError = totalError / currentStrokePoints.Count;
    // 使用 Mathf.Clamp 確保數值在 0-100 之間
    drawPrecision = Mathf.Clamp((1f - avgError) * 100f, 0f, 100f);

    // 4. 更新 UI 顯示
    if (precisionText != null) 
        precisionText.text = "Shape Accuracy: " + drawPrecision.ToString("F1") + "%";

    // 5. 判定過關條件
    // 條件 A: 精確度必須高於 95% (可以根據難度調整，95% 已經算很圓了)
    if (drawPrecision < 99f) 
    {
        resultText.text = "Try to keep the distance from the center steady!";
    }
    // 條件 B: 玩家必須畫了足夠多的點 (代表畫了足夠長的弧度，防止只畫一小段圓弧就過關)
    else if (currentStrokePoints.Count < 150) 
    {
        resultText.text = "Keep drawing to complete the shape!";
    }
    else 
    {
        // 條件 C: 額外檢查（可選）- 檢查首尾是否接近，確保是一個「封閉」的圓
        float startEndDist = Vector2.Distance(currentStrokePoints[0], currentStrokePoints[currentStrokePoints.Count - 1]);
        if (startEndDist > averageRadius * 0.8f) // 如果頭尾距離太遠，代表圓沒畫完
        {
            resultText.text = "Almost there! Close the circle.";
        }
        else
        {
            resultText.text = "Great circle! Flag: " + FlagManager.FLAG_04;
            if (FlagManager.Instance != null) FlagManager.Instance.UnlockFlag04();
        }
    }
}

    // 普通函數用來啟動協程
    public void OnClearButtonClicked()
    {
        StartCoroutine(FullResetCanvasRoutine());
    }

    // 真正的清除邏輯改為 IEnumerator
    public IEnumerator FullResetCanvasRoutine()
    {
        // 1. 先清除數據與物件
        ClearAllLogic();
        if (resultText != null) resultText.text = "Clearing...";

        // 2. 等待 0.5 秒 (注意：WaitForSeconds 需要 float，所以是 0.5f)
        yield return new WaitForSeconds(0.5f); 

        // 3. 再次確保清除並更新文字
        ClearAllLogic();
        if (resultText != null) resultText.text = "Canvas Cleared!";
    }

    // 將重複的清除邏輯抽出來
    private void ClearAllLogic()
    {
        foreach (var seg in allVisualSegments)
        {
            if (seg != null) Destroy(seg);
        }
        allVisualSegments.Clear();
        currentStrokePoints.Clear();
        drawPrecision = 0f;
    }

    private void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}