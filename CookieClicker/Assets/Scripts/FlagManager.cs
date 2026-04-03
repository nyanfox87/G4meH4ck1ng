using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }

    // === Flag solved states ===
    public bool flag01_Solved = false; // The Deception: cookieCents > 1000000
    public bool flag02_Solved = false; // Speed Demon: f_DropSpeed > 100.0
    public bool flag03_Solved = false; // Frozen Baker: f_BakePos == 50.00
    public bool flag04_Solved = false; // Bot Artist: DrawPrecision > 99%
    public bool flag05_Solved = false; // Lone Wolf: GameStartWithOnePlayer

    // === Flag strings ===
    public static readonly string FLAG_01 = "FLAG{MONEY_OVERFLOW}";
    public static readonly string FLAG_02 = "FLAG{DYNAMIC_SPEED_HACK}";
    public static readonly string FLAG_03 = "FLAG{FROZEN_BAKER}";
    public static readonly string FLAG_04 = "FLAG{BOT_ARTIST}";
    public static readonly string FLAG_05 = "FLAG{LOBBY_BYPASS_SUCCESS}";

    // === Flag display names ===
    public static readonly string[] FlagNames = {
        "FLG-01: The Deception",
        "FLG-02: Speed Demon",
        "FLG-03: Frozen Baker",
        "FLG-04: Bot Artist",
        "FLG-05: Lone Wolf"
    };

    public static readonly string[] FlagHints = {
        "Hint: What if you had more than a million pennies?",
        "Hint: The speed is clamped at 99... can you push past it?",
        "Hint: Stop the slider at EXACTLY 50.00 - freeze it in memory.",
        "Hint: A perfect circle... even a robot could do it.",
        "Hint: The lobby needs 2 players. What if the DLL disagreed?"
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load flag states
        flag01_Solved = SaveSystem.LoadFlag("FLG01");
        flag02_Solved = SaveSystem.LoadFlag("FLG02");
        flag03_Solved = SaveSystem.LoadFlag("FLG03");
        flag04_Solved = SaveSystem.LoadFlag("FLG04");
        flag05_Solved = SaveSystem.LoadFlag("FLG05");
    }

    void Update()
    {
        CheckFlags();
    }

    private void CheckFlags()
    {
        if (GameManager.Instance == null) return;

        // FLG-01: cookieCents > 1,000,000
        if (!flag01_Solved && GameManager.Instance.GetCookieCents() > 1000000)
        {
            flag01_Solved = true;
            SaveSystem.SaveFlag("FLG01", true);
            Debug.Log("[FLAG] FLG-01 UNLOCKED: " + FLAG_01);
        }

        // FLG-02: actual drop speed exceeds 100 (must bypass the 99.0 clamp)
        if (!flag02_Solved)
        {
            float rawSpeed = GameManager.Instance.baseDropSpeed * (1f + GameManager.Instance.GetCookieRate());
            if (rawSpeed > 100.0f)
            {
                flag02_Solved = true;
                SaveSystem.SaveFlag("FLG02", true);
                Debug.Log("[FLAG] FLG-02 UNLOCKED: " + FLAG_02);
            }
        }
    }

    public void UnlockFlag03()
    {
        if (!flag03_Solved)
        {
            flag03_Solved = true;
            SaveSystem.SaveFlag("FLG03", true);
            Debug.Log("[FLAG] FLG-03 UNLOCKED: " + FLAG_03);
        }
    }

    public void UnlockFlag04()
    {
        if (!flag04_Solved)
        {
            flag04_Solved = true;
            SaveSystem.SaveFlag("FLG04", true);
            Debug.Log("[FLAG] FLG-04 UNLOCKED: " + FLAG_04);
        }
    }

    public void UnlockFlag05()
    {
        if (!flag05_Solved)
        {
            flag05_Solved = true;
            SaveSystem.SaveFlag("FLG05", true);
            Debug.Log("[FLAG] FLG-05 UNLOCKED: " + FLAG_05);
        }
    }

    public bool IsFlagSolved(int index)
    {
        switch (index)
        {
            case 0: return flag01_Solved;
            case 1: return flag02_Solved;
            case 2: return flag03_Solved;
            case 3: return flag04_Solved;
            case 4: return flag05_Solved;
            default: return false;
        }
    }

    public string GetFlagString(int index)
    {
        switch (index)
        {
            case 0: return FLAG_01;
            case 1: return FLAG_02;
            case 2: return FLAG_03;
            case 3: return FLAG_04;
            case 4: return FLAG_05;
            default: return "";
        }
    }
}
