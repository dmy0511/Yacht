using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashData : MonoBehaviour
{
    [Header("재화")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI cloverText;

    [Header("가져올 데이터 저장소")]
    [SerializeField] private TextManager textManager;

    private void OnEnable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated += UpdateCoinText;
            textManager.OnDiamondUpdated += UpdateDiamondText;
            textManager.OnCloverUpdated += UpdateCloverText;
        }
    }

    private void OnDisable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated -= UpdateCoinText;
            textManager.OnDiamondUpdated -= UpdateDiamondText;
            textManager.OnCloverUpdated -= UpdateCloverText;
        }
    }

    private void Start()
    {
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        UpdateCoinText(PlayerPrefs.GetInt("CurrentCoin", 0));
        UpdateDiamondText(PlayerPrefs.GetInt("CurrentDiamond", 0));
        UpdateCloverText(PlayerPrefs.GetInt("CurrentClover", 0));
    }

    public bool TryUpgrade(int coinCost, int diamondCost, int cloverCost)
    {
        int currentCoin = PlayerPrefs.GetInt("CurrentCoin", 0);
        int currentDiamond = PlayerPrefs.GetInt("CurrentDiamond", 0);
        int currentClover = PlayerPrefs.GetInt("CurrentClover", 0);

        if (currentCoin >= coinCost &&
            currentDiamond >= diamondCost &&
            currentClover >= cloverCost)
        {
            PlayerPrefs.SetInt("CurrentCoin", currentCoin - coinCost);
            PlayerPrefs.SetInt("CurrentDiamond", currentDiamond - diamondCost);
            PlayerPrefs.SetInt("CurrentClover", currentClover - cloverCost);
            PlayerPrefs.Save();

            UpdateShopUI();

            if (textManager != null)
            {
                textManager.UpdateCoinText(-coinCost);
                textManager.UpdateDiamondText(-diamondCost);
                textManager.UpdateCloverText(-cloverCost);
            }

            return true;
        }

        Debug.Log("재화가 부족합니다!");
        return false;
    }

    private void UpdateCoinText(int amount)
    {
        if (coinText != null)
            coinText.text = TextManager.ChangeNumber(amount.ToString());
    }

    private void UpdateDiamondText(int amount)
    {
        if (diamondText != null)
            diamondText.text = TextManager.ChangeNumber(amount.ToString());
    }

    private void UpdateCloverText(int amount)
    {
        if (cloverText != null)
            cloverText.text = TextManager.ChangeNumber(amount.ToString());
    }
}
