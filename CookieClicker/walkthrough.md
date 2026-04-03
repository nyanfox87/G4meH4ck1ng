# Cookie Cracker: Reverse Engineering Lab — Walkthrough

## What Was Built

A fully functional "Vulnerable-by-Design" Unity cookie clicker game with 5 CTF labs for teaching reverse engineering concepts.

### Scripts Created (12 total)

| Script | Purpose |
|--------|---------|
| [GameManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/GameManager.cs) | Singleton, currency tracking, auto-save, drop speed formula with 99.0 clamp |
| [FlagManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/FlagManager.cs) | DontDestroyOnLoad, tracks 5 CTF flags, checks trigger conditions each frame |
| [SaveSystem.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/SaveSystem.cs) | Static PlayerPrefs helper for save/load/hard reset |
| [CursorController.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/CursorController.cs) | Hides hardware cursor, public `f_CursorX`/`f_CursorY` for memory scanning |
| [CookieClicker.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/CookieClicker.cs) | Big Cookie click handler, spawns falling cookies, public `isPressed` |
| [FallingCookie.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/FallingCookie.cs) | Falling/fading animation for dropped cookies |
| [StoreManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/StoreManager.cs) | 3 upgrades (Kneader, Oven, Flour) with scaling costs |
| [SettingsPanel.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/SettingsPanel.cs) | Speed slider, reset button, hint cycling |
| [UIManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/UIManager.cs) | HUD updates, panel toggles, scene navigation |
| [BakeMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/BakeMinigame.cs) | Slider lab: stop at 50.00, color zones, FLG-03 |
| [IcingMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/IcingMinigame.cs) | Circle tracing, precision calc, FLG-04 at >99% |
| [LobbyMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/LobbyMinigame.cs) | Multiplayer lobby check, FLG-05 via DLL patching |

### Scenes (4 total, all in build settings)

| Scene | Contents |
|-------|----------|
| **SampleScene** (Main Hub) | BigCookie button, HUD (currency, timer, cursor XY), sidebar nav, 4 overlay panels |
| **BakeScene** | Bouncing slider (0-100), color zones (gray/yellow/orange), click-to-stop/ESC-to-resume |
| **IcingScene** | Draw area with circle guide, precision tracking, clear/back buttons |
| **LobbyScene** | Player count display, "START GAME" button with `playerCount >= 2` check |

### CTF Flag Registry

| ID | Trigger | Flag |
|----|---------|------|
| FLG-01 | `cookieCents > 1,000,000` | `FLAG{MONEY_OVERFLOW}` |
| FLG-02 | `f_DropSpeed > 100.0` (bypass 99.0 clamp) | `FLAG{DYNAMIC_SPEED_HACK}` |
| FLG-03 | `f_BakePos == 50.00` | `FLAG{FROZEN_BAKER}` |
| FLG-04 | `DrawPrecision > 99%` | `FLAG{BOT_ARTIST}` |
| FLG-05 | Game start with 1 player | `FLAG{LOBBY_BYPASS_SUCCESS}` |

---

## How to Change the Cookie to Your Own Image

1. **Import your image** — Drag your PNG/JPG into `Assets/Sprites/` in the Unity Project window
2. **Set as Sprite** — Select the image → Inspector → set **Texture Type** to `Sprite (2D and UI)` → click **Apply**
3. **Assign to BigCookie** — In SampleScene, select `GameCanvas > BigCookie` → find the **Image** component → drag your sprite into **Source Image**
4. **Adjust size** — Turn on **Preserve Aspect** if needed, or adjust the RectTransform dimensions

---

## Testing & Validation

- ✅ All 12 scripts compiled with **zero errors**
- ✅ Main scene fully wired with 19+ serialized field references
- ✅ All 4 scenes registered in Build Settings
- ✅ Mini-game scenes populated via `MiniGameSceneBuilder.cs` editor tool (`Tools > Build Mini-Game Scenes`)
