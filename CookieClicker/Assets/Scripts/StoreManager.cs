using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [Header("Upgrade Costs (in cents)")]
    public int kneaderBaseCost = 100;
    public int ovenBaseCost = 500;
    public int flourBaseCost = 1000;

    [Header("Rate Boost Per Level")]
    public float kneaderBoost = 0.5f;
    public float ovenBoost = 2.0f;
    public float flourBoost = 5.0f;

    [Header("UI References")]
    public Text kneaderInfoText;
    public Text ovenInfoText;
    public Text flourInfoText;
    public Button kneaderButton;
    public Button ovenButton;
    public Button flourButton;

    void Start()
    {
        if (kneaderButton != null) kneaderButton.onClick.AddListener(BuyKneader);
        if (ovenButton != null) ovenButton.onClick.AddListener(BuyOven);
        if (flourButton != null) flourButton.onClick.AddListener(BuyFlour);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        int kCost = kneaderBaseCost * (GameManager.Instance.kneaderLevel + 1);
        int oCost = ovenBaseCost * (GameManager.Instance.ovenLevel + 1);
        int fCost = flourBaseCost * (GameManager.Instance.flourLevel + 1);

        if (kneaderInfoText != null)
            kneaderInfoText.text = "KNEADER Lv." + GameManager.Instance.kneaderLevel +
                "\nCost: $" + (kCost / 100f).ToString("F2") + "\n+" + kneaderBoost.ToString("F1") + " rate";

        if (ovenInfoText != null)
            ovenInfoText.text = "OVEN Lv." + GameManager.Instance.ovenLevel +
                "\nCost: $" + (oCost / 100f).ToString("F2") + "\n+" + ovenBoost.ToString("F1") + " rate";

        if (flourInfoText != null)
            flourInfoText.text = "FLOUR Lv." + GameManager.Instance.flourLevel +
                "\nCost: $" + (fCost / 100f).ToString("F2") + "\n+" + flourBoost.ToString("F1") + " rate";
    }

    public void BuyKneader()
    {
        if (GameManager.Instance == null) return;
        int cost = kneaderBaseCost * (GameManager.Instance.kneaderLevel + 1);
        if (GameManager.Instance.SpendCents(cost))
        {
            GameManager.Instance.kneaderLevel++;
            GameManager.Instance.AddCookieRate(kneaderBoost);
        }
    }

    public void BuyOven()
    {
        if (GameManager.Instance == null) return;
        int cost = ovenBaseCost * (GameManager.Instance.ovenLevel + 1);
        if (GameManager.Instance.SpendCents(cost))
        {
            GameManager.Instance.ovenLevel++;
            GameManager.Instance.AddCookieRate(ovenBoost);
        }
    }

    public void BuyFlour()
    {
        if (GameManager.Instance == null) return;
        int cost = flourBaseCost * (GameManager.Instance.flourLevel + 1);
        if (GameManager.Instance.SpendCents(cost))
        {
            GameManager.Instance.flourLevel++;
            GameManager.Instance.AddCookieRate(flourBoost);
        }
    }
}
