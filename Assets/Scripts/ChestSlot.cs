using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class ChestSlot : MonoBehaviour
{
    public ChestType chestType;
    public Button openModalBtn;
    public ChestOpenModel prefab;
    public ChestOpenModel activeModel;
    public TMP_Text timerText;
    public GameObject popUp;
    int i = 0;
    private string status;
    public DateTime unlockEndTime;
    private bool isUnlocking = false;

    public void Initialize(ChestType type, string initialStatus, DateTime endTime, int i)
    {
        chestType = type;
        status = initialStatus;
        unlockEndTime = endTime;
        this.i = i;

        if (status == "Unlocking" && DateTime.Now < unlockEndTime)
        {
            StartCoroutine(TrackTime());
        }
        else if (status == "Unlocking" && DateTime.Now >= unlockEndTime)
        {
            UnlockComplete();
        }

        UpdateUI();
        activeModel = Instantiate(prefab, transform.parent.parent);
        activeModel.startTimerbtn.onClick.AddListener(OnStartButtonClick);
        activeModel.openWithGemsBtn.onClick.AddListener(OnUnlockWithGemsClick);
        activeModel.CloseBtn.onClick.AddListener(ClosePanel);

        if(status == "Locked")
        {
            openModalBtn.onClick.AddListener(OpenModel);
        }
    }
    public void ClosePanel()
    {

        if (activeModel)
        {
            activeModel.gameObject.SetActive(false);
        }
    }
    public void OnStartButtonClick()
    {
        if (status == "Locked")
        {
            unlockEndTime = DateTime.Now.AddMinutes(chestType.unlockTimeInMinutes);
            status = "Unlocking";
            ChestManager.instance.SaveChests(this);
            StartCoroutine(TrackTime());
        }
        activeModel.gameObject.SetActive(false);
    }

    public void OnUnlockWithGemsClick()
    {
        int gemCost = CalculateGemCost();
        if (CurrencyManager.Instance.SpendGems(gemCost))
        {
            CollectReward();
            activeModel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Not enough gems!");
        }
    }

    public  IEnumerator TrackTime()
    {
        isUnlocking = true;

        while (DateTime.Now < unlockEndTime)
        {
            TimeSpan remainingTime = unlockEndTime - DateTime.Now;
            UpdateTimerUI(remainingTime);
            yield return new WaitForSeconds(1);
        }

        UnlockComplete();
    }

    private int CalculateGemCost()
    {
        TimeSpan remainingTime = unlockEndTime - DateTime.Now;
        int minutesRemaining = Mathf.CeilToInt((float)remainingTime.TotalMinutes);
        return Mathf.CeilToInt(minutesRemaining / 10f) * chestType.gemsPer10Minutes;
    }

    public void CollectReward()
    {
        CurrencyManager.Instance.AddCoins(chestType.coinReward);
        CurrencyManager.Instance.AddGems(chestType.gemReward);
        Debug.Log(PlayerPrefs.HasKey($"Chest_{i}_Type"));
        PlayerPrefs.DeleteKey($"Chest_{i}_Type");
        PlayerPrefs.DeleteKey($"Chest_{i}_Status");
        PlayerPrefs.DeleteKey($"Chest_{i}_EndTime");
        int chestCount = PlayerPrefs.GetInt("ChestCount", 0);
        chestCount--;
        PlayerPrefs.SetInt("ChestCount", chestCount);
        PlayerPrefs.Save();
        Destroy(gameObject);
    }

    public void UnlockComplete()
    {
        status = "Unlocked";
        openModalBtn.onClick.RemoveAllListeners();
        ChestManager.instance.SaveChests(this);
        openModalBtn.onClick.AddListener(CollectReward);
        timerText.text = "Unlocked!";
    }

    private void UpdateUI()
    {
        if (status == "Locked")
        {
            timerText.text = "Locked";
        }
        else if (status == "Unlocking")
        {
            TimeSpan remainingTime = unlockEndTime - DateTime.Now;
            UpdateTimerUI(remainingTime);
        }
        else if (status == "Unlocked")
        {
            timerText.text = "Unlocked!";
        }
    }

    private void UpdateTimerUI(TimeSpan remainingTime)
    {
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
            remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }

    public string GetStatus()
    {
        return status;
    }

    public void OpenModel()
    {
        if (activeModel)
        {
            activeModel.gameObject.SetActive(true);
        }
    }
}
