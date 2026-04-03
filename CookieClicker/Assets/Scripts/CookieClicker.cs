using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CookieClicker : MonoBehaviour
{
    [Header("Cookie Settings")]
    public int centsPerClick = 1;

    [Header("Drop Animation")]
    public GameObject fallingCookiePrefab;
    public Transform dropSpawnPoint;

    // === Public for memory injection lab ===
    public bool isPressed = false;

    // === Drop speed (public for memory scanning) ===
    public float f_DropSpeed = 2f;

    private Button cookieButton;
    private Vector3 originalScale;

    private float hideTimer = 0f;
    private bool isHidden = false;
    private Queue<float> clickTimestamps = new Queue<float>();

    private bool isWarning = false;
    private float warningTimer = 0f;
    private float lastClickTime = -100f;

    void Start()
    {
        originalScale = transform.localScale;
        cookieButton = GetComponent<Button>();
        if (cookieButton != null)
        {
            cookieButton.onClick.AddListener(OnCookieClick);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null)
        {
            f_DropSpeed = GameManager.Instance.CalculateDropSpeed();
        }
    }

    public void OnCookieClick()
    {
        if (isHidden) return;
        if (GameManager.Instance == null) return;

        float currentTime = Time.time;
        lastClickTime = currentTime;
        clickTimestamps.Enqueue(currentTime);
        while(clickTimestamps.Count > 0 && currentTime - clickTimestamps.Peek() > 1f)
        {
            clickTimestamps.Dequeue();
        }

        if (clickTimestamps.Count >= 10)
        {
            if (!isWarning)
            {
                isWarning = true;
                warningTimer = 5f;
                Image img = GetComponent<Image>();
                if (img != null) img.color = Color.red;
                clickTimestamps.Clear();
            }
            else
            {
                // Clear to prevent constant processing during warning
                clickTimestamps.Clear();
            }
        }

        isPressed = true;
        GameManager.Instance.isPressed = true;
        GameManager.Instance.AddCents(centsPerClick);

        // Spawn falling cookie
        if (fallingCookiePrefab != null && dropSpawnPoint != null)
        {
            GameObject cookie = Instantiate(fallingCookiePrefab, dropSpawnPoint.position, Quaternion.identity, transform.parent);
            FallingCookie fc = cookie.GetComponent<FallingCookie>();
            if (fc != null)
            {
                fc.fallSpeed = f_DropSpeed * 50f;
            }
            Destroy(cookie, 3f);
        }

        StopAllCoroutines();
        StartCoroutine(ClickEffect());
    }

    private System.Collections.IEnumerator ClickEffect()
    {
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = originalScale;
    }

    void Update()
    {
        if (isHidden)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                isHidden = false;
                Image img = GetComponent<Image>();
                if (img != null) 
                {
                    img.enabled = true;
                    img.color = Color.white;
                }
                if (cookieButton != null) cookieButton.interactable = true;
                transform.localScale = originalScale;
            }
        }
        else if (isWarning)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
            {
                isWarning = false;
                
                // Disappear if there was a click in the last 2 seconds
                if (Time.time - lastClickTime < 2f)
                {
                    isHidden = true;
                    hideTimer = 10f;
                    Image img = GetComponent<Image>();
                    if (img != null) img.enabled = false;
                    if (cookieButton != null) cookieButton.interactable = false;
                }
                else // Recover normal
                {
                    Image img = GetComponent<Image>();
                    if (img != null) img.color = Color.white;
                }
            }
        }

        if (Mouse.current != null)
        {
            isPressed = Mouse.current.leftButton.isPressed;
            if (GameManager.Instance != null)
                GameManager.Instance.isPressed = isPressed;
        }
    }
}
