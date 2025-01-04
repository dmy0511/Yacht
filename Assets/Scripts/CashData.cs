using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 게임 내 재화(코인, 다이아몬드, 클로버) 데이터를 관리하는 클래스
public class CashData : MonoBehaviour
{
    // UI 텍스트 컴포넌트
    [Header("재화")]
    [SerializeField] private TextMeshProUGUI coinText;      // 코인
    [SerializeField] private TextMeshProUGUI diamondText;   // 다이아
    [SerializeField] private TextMeshProUGUI cloverText;    // 클로버

    [Header("가져올 데이터 저장소")]
    [SerializeField] private TextManager textManager;       // 텍스트 매니저 참조

    // 재화 변경시 발생하는 이벤트
    public event System.Action OnResourceChanged;

    // 현재 재화 값을 PlayerPrefs에서 가져옴 (기본값 0)
    private int currentCoin => PlayerPrefs.GetInt("CurrentCoin", 0);
    private int currentDiamond => PlayerPrefs.GetInt("CurrentDiamond", 0);
    private int currentClover => PlayerPrefs.GetInt("CurrentClover", 0);

    // 재화가 충분한지 확인하는 메서드
    public bool HasEnoughResources(int coinCost, int cloverCost, int diamondCost)
    {
        return currentCoin >= coinCost &&
               currentClover >= cloverCost &&
               currentDiamond >= diamondCost;
    }

    // 컴포넌트 활성화시 이벤트 구독
    private void OnEnable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated += UpdateCoinText;
            textManager.OnDiamondUpdated += UpdateDiamondText;
            textManager.OnCloverUpdated += UpdateCloverText;
        }
    }

    // 컴포넌트 비활성화시 이벤트 구독 해제
    private void OnDisable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated -= UpdateCoinText;
            textManager.OnDiamondUpdated -= UpdateDiamondText;
            textManager.OnCloverUpdated -= UpdateCloverText;
        }
    }

    // 시작시 UI 업데이트
    private void Start()
    {
        UpdateShopUI();
    }

    // 상점 UI 업데이트 
    private void UpdateShopUI()
    {
        UpdateCoinText(PlayerPrefs.GetInt("CurrentCoin", 0));
        UpdateDiamondText(PlayerPrefs.GetInt("CurrentDiamond", 0));
        UpdateCloverText(PlayerPrefs.GetInt("CurrentClover", 0));
    }

    // 업그레이드 시도 (비용 지불)
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

            OnResourceChanged?.Invoke();

            return true;
        }

        Debug.Log("재화가 부족합니다!");
        return false;
    }

    // 각 재화 텍스트 업데이트 메서드
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
