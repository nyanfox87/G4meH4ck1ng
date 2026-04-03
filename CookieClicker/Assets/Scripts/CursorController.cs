using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 1. 必須加入這個命名空間

public class CursorController : MonoBehaviour
{
    // === Public for memory scanning (intentional) ===
    public float f_CursorX = 0f;
    public float f_CursorY = 0f;

    [Header("UI References")]
    public RectTransform cursorImage;
    public Text cursorXText;
    public Text cursorYText;
    public Canvas parentCanvas;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // 2. 使用新版 Input System 獲取滑鼠位置
        Vector3 mousePos = Mouse.current.position.ReadValue(); 
        
        f_CursorX = mousePos.x;
        f_CursorY = mousePos.y;

        // --- 以下移動 UI 和更新文字的邏輯保持不變 ---
        if (cursorImage != null && parentCanvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePos,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out localPoint
            );
            cursorImage.anchoredPosition = localPoint + new Vector2(50f, -50f);
        }

        if (cursorXText != null)
            cursorXText.text = "X: " + f_CursorX.ToString("F1");
        if (cursorYText != null)
            cursorYText.text = "Y: " + f_CursorY.ToString("F1");
    }
    
    void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
