using UnityEngine;

public static class SaveSystem
{
    private const string KEY_COOKIE_CENTS = "cookieCents";
    private const string KEY_PLAY_TIME = "f_PlayTime";
    private const string KEY_COOKIE_RATE = "f_CookieRate";
    private const string KEY_KNEADER_LEVEL = "kneaderLevel";
    private const string KEY_OVEN_LEVEL = "ovenLevel";
    private const string KEY_FLOUR_LEVEL = "flourLevel";

    public static void Save(int cookieCents, float playTime, float cookieRate, int kneader, int oven, int flour)
    {
        PlayerPrefs.SetInt(KEY_COOKIE_CENTS, cookieCents);
        PlayerPrefs.SetFloat(KEY_PLAY_TIME, playTime);
        PlayerPrefs.SetFloat(KEY_COOKIE_RATE, cookieRate);
        PlayerPrefs.SetInt(KEY_KNEADER_LEVEL, kneader);
        PlayerPrefs.SetInt(KEY_OVEN_LEVEL, oven);
        PlayerPrefs.SetInt(KEY_FLOUR_LEVEL, flour);
        PlayerPrefs.Save();
    }

    public static void LoadInto(GameManager gm)
    {
        gm.SetCookieCents(PlayerPrefs.GetInt(KEY_COOKIE_CENTS, 0));
        gm.f_PlayTime = PlayerPrefs.GetFloat(KEY_PLAY_TIME, 0f);
        gm.SetCookieRate(PlayerPrefs.GetFloat(KEY_COOKIE_RATE, 0f));
        gm.kneaderLevel = PlayerPrefs.GetInt(KEY_KNEADER_LEVEL, 0);
        gm.ovenLevel = PlayerPrefs.GetInt(KEY_OVEN_LEVEL, 0);
        gm.flourLevel = PlayerPrefs.GetInt(KEY_FLOUR_LEVEL, 0);
    }

    public static void SaveFlag(string flagId, bool solved)
    {
        PlayerPrefs.SetInt("flag_" + flagId, solved ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadFlag(string flagId)
    {
        return PlayerPrefs.GetInt("flag_" + flagId, 0) == 1;
    }

    public static void HardReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Hard Reset complete - all data cleared.");
    }
}
