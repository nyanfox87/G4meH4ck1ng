# Cookie Cracker: Reverse Engineering Lab — Implementation Plan

A "Vulnerable-by-Design" Unity cookie clicker game for teaching memory manipulation, DLL patching, and automation via a CTF learning path. Built in Unity 2D URP with uGUI.

## User Review Required

> [!IMPORTANT]
> **Cookie Image Customization**: To use your own image as the cookie, you'll import a PNG/JPG into `Assets/Sprites/`, set its Texture Type to **Sprite (2D)**, then drag it onto the Big Cookie's `Image` component in the Inspector. Full steps documented at the end.

> [!WARNING]
> **Intentional Vulnerabilities**: The game uses public fields (`f_CursorX`, `f_CursorY`, `isPressed`, `f_PlayTime`, `f_BakePos`) by design so students can find them with Cheat Engine / memory scanners. `cookieCents` is private but discoverable.

## Proposed Changes

### Folder Structure

#### [NEW] `Assets/Scripts/` — All game scripts
#### [NEW] `Assets/Sprites/` — Cookie and UI sprites
#### [NEW] `Assets/Scenes/` — Game scenes (already exists, will add new scenes)

---

### Core Singleton Scripts

#### [NEW] [GameManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/GameManager.cs)
- Singleton with `DontDestroyOnLoad`
- Holds `cookieCents` (private int), `f_PlayTime` (public float), `f_CookieRate` (private float)
- Auto-saves every 1 second via `PlayerPrefs`
- Hard Reset clears all `PlayerPrefs`

#### [NEW] [FlagManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/FlagManager.cs)
- Singleton with `DontDestroyOnLoad`
- Tracks 5 flags (FLG-01 through FLG-05) with solved booleans
- Each frame checks trigger conditions and unlocks flags
- Persists solved state in `PlayerPrefs`

#### [NEW] [SaveSystem.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/SaveSystem.cs)
- Static helper: `Save()`, `Load()`, `HardReset()`
- Uses `PlayerPrefs` for local storage

---

### Main Game Scripts

#### [NEW] [CursorController.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/CursorController.cs)
- Hides hardware cursor (`Cursor.visible = false`)
- Public `f_CursorX` / `f_CursorY` floats updated from `Input.mousePosition`
- Moves a UI Image to follow the cursor position
- Updates HUD text with X/Y coordinates

#### [NEW] [CookieClicker.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/CookieClicker.cs)
- Attached to the Big Cookie button
- On click: increment `cookieCents`, spawn falling cookie with drop animation
- Drop speed formula: `f_DropSpeed = baseSpeed * (1 + f_CookieRate)`, hard-clamped at 99.0
- Public `isPressed` bool for memory injection lab

#### [NEW] [StoreManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/StoreManager.cs)
- 3 upgrades: Kneader (cost 100¢), Oven (cost 500¢), Flour (cost 1000¢)
- Each upgrade modifies `f_CookieRate`
- Deducts `cookieCents` on purchase

#### [NEW] [SettingsPanel.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/SettingsPanel.cs)
- Speed slider modifies base cookie rate
- Reset button calls `SaveSystem.HardReset()`
- Hint button shows tips for memory scanning

#### [NEW] [UIManager.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/UIManager.cs)
- Manages panel visibility (Store, Settings, Flags, Mini-Games)
- Updates HUD displays (cookie count as `$X.XX`, timer, cursor coords)

---

### Mini-Game Scripts

#### [NEW] [BakeMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/BakeMinigame.cs)
- Scene 2: Slider bounces 0–100
- Color zones: Gray (0-40, 60-100), Yellow (40-48, 52-60), Orange (48-52)
- Mouse click stops slider, Escape resumes
- Public `f_BakePos` float; FLG-03 triggers at exactly 50.00

#### [NEW] [IcingMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/IcingMinigame.cs)
- Scene 4: Player traces a circle on a cookie
- Compares drawn path against ideal circle for accuracy %
- FLG-04 triggers at >99% precision

#### [NEW] [LobbyMinigame.cs](file:///c:/Users/D1248959/Documents/UnityProject/CookieClicker/Assets/Scripts/LobbyMinigame.cs)
- Scene 5: Simulated multiplayer lobby
- Checks `playerCount >= 2` to start
- FLG-05 triggers if game starts with 1 player (requires DLL patching)

---

### Scene Setup (via UnityMCP)

#### Main Game Hub (`SampleScene`)
- Canvas with HUD layer (top-right: cookie count, top-left: timer, bottom: cursor XY)
- Big Cookie button (center)
- Sidebar navigation buttons
- Overlay panels (Store, Settings, Flag Dashboard)

#### [NEW] `BakeScene` — The Perfect Bake mini-game
#### [NEW] `IcingScene` — Icing Artist mini-game
#### [NEW] `LobbyScene` — Lone Wolf Lobby mini-game

---

## How to Change the Cookie to Your Own Image

1. **Import your image**: Drag your PNG/JPG file into `Assets/Sprites/` in the Unity Project window
2. **Configure as Sprite**: Select the image → Inspector → set **Texture Type** to `Sprite (2D and UI)` → click **Apply**
3. **Assign to Cookie**: In the scene, select the `BigCookie` GameObject → find the `Image` component → drag your sprite into the **Source Image** field
4. **Adjust size**: Modify the RectTransform width/height or set **Preserve Aspect** to keep proportions

## Verification Plan

### Manual Verification
1. **Enter Play mode** in Unity and verify:
   - The Big Cookie is clickable and increments the dollar counter
   - Cookie count displays as `$X.XX` format
   - Cursor coordinates update in HUD
   - Store panel opens/closes; upgrades can be purchased
   - Settings slider changes cookie drop speed
   - Flag Dashboard shows all 5 labs as LOCKED initially
2. **Test mini-game scenes** by navigating to each and verifying basic mechanics work
3. **Test flag triggers** by modifying public variables in the Inspector during play mode
