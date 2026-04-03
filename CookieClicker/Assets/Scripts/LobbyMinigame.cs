using UnityEngine;
using UnityEngine.UI;

public class LobbyMinigame : MonoBehaviour
{
    [Header("UI References")]
    public Text playerCountText;
    public Text statusText;
    public Text resultText;
    public Button startGameButton;
    public Button backButton;

    [Header("Lobby Settings")]
    public int playerCount = 1;
    public int requiredPlayers = 2;

    void Start()
    {
        if (startGameButton != null) startGameButton.onClick.AddListener(TryStartGame);
        if (backButton != null) backButton.onClick.AddListener(GoBack);
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerCountText != null)
            playerCountText.text = "Players: " + playerCount + " / " + requiredPlayers;

        if (statusText != null)
        {
            if (playerCount >= requiredPlayers)
                statusText.text = "READY TO START";
            else
                statusText.text = "Waiting for players...";
        }
    }

    public void TryStartGame()
    {
        // INTENTIONAL VULNERABILITY: This check can be bypassed via DLL patching
        if (playerCount >= requiredPlayers)
        {
            if (resultText != null) resultText.text = "Game Started! (But how did you get 2 players?)";
            OnGameStartSuccess();
        }
        else
        {
            if (resultText != null) resultText.text = "Cannot start! Need " + requiredPlayers + " players.\nHint: What if the DLL thought differently?";
        }
    }

    private void OnGameStartSuccess()
    {
        // Check if game was started with only 1 actual player (DLL patching)
        if (playerCount < 2)
        {
            if (FlagManager.Instance != null) FlagManager.Instance.UnlockFlag05();
            if (resultText != null) resultText.text = "LONE WOLF! " + FlagManager.FLAG_05;
        }
    }

    private void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
