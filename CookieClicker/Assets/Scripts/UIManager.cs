using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public Text cookieCountText;
    public Text playTimeText;
    public Text cursorXText;
    public Text cursorYText;
    public Text isPressedText;

    [Header("Panels")]
    public GameObject storePanel;
    public GameObject settingsPanel;
    public GameObject flagPanel;
    public GameObject miniGamePanel;

    [Header("Flag Dashboard")]
    public Text[] flagTexts;

    [Header("Navigation Buttons")]
    public Button storeButton;
    public Button settingsButton;
    public Button flagButton;
    public Button miniGameButton;

    [Header("Panels Close Button")]
    public Button closeStoreButton;
    public Button closeSettingsButton;
    public Button closeFlagButton;
    public Button closeMiniGameButton;

    [Header("Mini-Game Navigation")]
    public Button bakeGameButton;
    public Button icingGameButton;
    public Button lobbyGameButton;
    public Button backFromMiniGames;

    void Start()
    {
        // Wire panel toggles
        if (storeButton != null) storeButton.onClick.AddListener(() => TogglePanel(storePanel));
        if (settingsButton != null) settingsButton.onClick.AddListener(() => TogglePanel(settingsPanel));
        if (flagButton != null) flagButton.onClick.AddListener(() => TogglePanel(flagPanel));
        if (miniGameButton != null) miniGameButton.onClick.AddListener(() => TogglePanel(miniGamePanel));

        if (closeStoreButton != null) closeStoreButton.onClick.AddListener(() => ClosePanels(storePanel));
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(() => ClosePanels(settingsPanel));
        if (closeFlagButton != null) closeFlagButton.onClick.AddListener(() => ClosePanels(flagPanel));
        if (closeMiniGameButton != null) closeMiniGameButton.onClick.AddListener(() => ClosePanels(miniGamePanel));

        // Wire mini-game scene loads
        if (bakeGameButton != null) bakeGameButton.onClick.AddListener(() => LoadScene("BakeScene"));
        if (icingGameButton != null) icingGameButton.onClick.AddListener(() => LoadScene("IcingScene"));
        if (lobbyGameButton != null) lobbyGameButton.onClick.AddListener(() => LoadScene("LobbyScene"));
        if (backFromMiniGames != null) backFromMiniGames.onClick.AddListener(() => TogglePanel(miniGamePanel));

        // Close all panels at start
        CloseAllPanels();

        // Set all panel location right
        flagPanel.transform.localPosition = Vector3.zero;
        storePanel.transform.localPosition = Vector3.zero;
        settingsPanel.transform.localPosition = Vector3.zero;
        miniGamePanel.transform.localPosition = Vector3.zero;

    }

    void Update()
    {
        UpdateHUD();
        UpdateFlagDashboard();
    }

    private void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        if (cookieCountText != null)
            cookieCountText.text = GameManager.Instance.GetCurrencyDisplay();

        if (playTimeText != null)
            playTimeText.text = GameManager.Instance.GetPlayTimeDisplay();

        if (isPressedText != null)
            isPressedText.text = "IsPressed: " + GameManager.Instance.isPressed;
    }

    private void UpdateFlagDashboard()
    {
        if (FlagManager.Instance == null || flagTexts == null) return;

        for (int i = 0; i < flagTexts.Length && i < 5; i++)
        {
            if (flagTexts[i] == null) continue;

            if (FlagManager.Instance.IsFlagSolved(i))
            {
                flagTexts[i].text = FlagManager.FlagNames[i] + "\n<color=#00FF00>SOLVED: " + FlagManager.Instance.GetFlagString(i) + "</color>";
                flagTexts[i].color = Color.green;
            }
            else
            {
                flagTexts[i].text = FlagManager.FlagNames[i] + "\n" + FlagManager.FlagHints[i];
                flagTexts[i].color = Color.black;
            }
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;
        
        // Remember if it was open before we close everything
        bool wasActive = panel.activeSelf;
        
        CloseAllPanels();
        
        // If it wasn't open, open it now (otherwise it stays closed)
        if (!wasActive)
        {
            panel.SetActive(true);
        }
    }

    private void CloseAllPanels()
    {
        if (storePanel != null) storePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (flagPanel != null) flagPanel.SetActive(false);
        if (miniGamePanel != null) miniGamePanel.SetActive(false);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ClosePanels(GameObject panel)
    {
        panel.SetActive(false);
    }
}

    