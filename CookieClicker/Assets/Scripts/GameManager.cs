using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // === Hidden currency (private for CTF scanning challenge) ===
    [SerializeField] private int cookieCents = 0;

    // === Public variables (intentionally exposed for memory scanning) ===
    public float f_PlayTime = 0f;
    public bool isPressed = false;

    // === Private rate (discoverable via memory scan) ===
    [SerializeField] private float f_CookieRate = 1f;

    // === Upgrade levels ===
    public int kneaderLevel = 0;
    public int ovenLevel = 0;
    public int flourLevel = 0;

    // === Base speed for drops ===
    public float baseDropSpeed = 2f;

    // === Auto-save timer ===
    private float saveTimer = 0f;
    private const float SAVE_INTERVAL = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved data
        SaveSystem.LoadInto(this);

        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    void Update()
    {
        // Track play time
        f_PlayTime += Time.deltaTime;

        // Auto-save every 1 second
        saveTimer += Time.deltaTime;
        if (saveTimer >= SAVE_INTERVAL)
        {
            saveTimer = 0f;
            SaveSystem.Save(cookieCents, f_PlayTime, f_CookieRate, kneaderLevel, ovenLevel, flourLevel);
        }
    }

    // === Currency accessors ===
    public int GetCookieCents() { return cookieCents; }
    public void SetCookieCents(int value) { cookieCents = value; }

    public void AddCents(int amount)
    {
        cookieCents += amount;
    }

    public bool SpendCents(int amount)
    {
        if (cookieCents >= amount)
        {
            cookieCents -= amount;
            return true;
        }
        return false;
    }

    public float GetCookieRate() { return f_CookieRate; }
    public void SetCookieRate(float value) { f_CookieRate = value; }
    public void AddCookieRate(float amount) { f_CookieRate += amount; }

    // === Drop speed formula with hard clamp at 99.0 ===
    public float CalculateDropSpeed()
    {
        float speed = baseDropSpeed * (1f + f_CookieRate);
        // INTENTIONAL: Hard clamp at 99.0. Students must bypass this for FLG-02.
        if (speed > 99.0f) speed = 99.0f;
        return speed;
    }

    public string GetCurrencyDisplay()
    {
        float dollars = cookieCents / 100f;
        return "$" + dollars.ToString("F2");
    }

    public string GetPlayTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(f_PlayTime / 60f);
        int seconds = Mathf.FloorToInt(f_PlayTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void HardReset()
    {
        cookieCents = 0;
        f_PlayTime = 0f;
        f_CookieRate = 1f;
        kneaderLevel = 0;
        ovenLevel = 0;
        flourLevel = 0;
        isPressed = false;
        SaveSystem.HardReset();
    }
}
