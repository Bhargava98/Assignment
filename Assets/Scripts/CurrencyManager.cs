using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int coins;
    public int gems;

    public TMP_Text coinsTxt, gemsTxt;

    public static CurrencyManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Coins", +gems);
        UpdateUI();
    }

    public void AddGems(int amount)
    {
        gems += amount;
        PlayerPrefs.SetInt("Gems", +gems);
        UpdateUI();
    }

    private void UpdateUI()
    {
        PlayerPrefs.GetInt("Coins");
        coinsTxt.text = PlayerPrefs.GetInt("Coins").ToString();
        gemsTxt.text = PlayerPrefs.GetInt("Gems").ToString();
    }
}
