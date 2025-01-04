using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 채굴 아이템의 속도 증가를 제어하는 클래스
public class SpeedUpCtrl : MonoBehaviour
{
    // UI 컴포넌트
    private Slider slider;            // 진행도 슬라이더
    private Button speedUpButton;     // 속도 증가 버튼

    // 아이템 속성
    private string rewardType;                          // 보상 타입
    private float baseSpeed;                            // 기본 속도
    private float individualSpeedMultiplier = 1f;       // 개별 속도 배율
    private float rewardAmount;                         // 보상량
    private bool isInitialized = false;                 // 초기화 여부
    private static float globalSpeedMultiplier = 1f;    // 전역 속도 배율
    private int itemIndex;                              // 아이템 인덱스

    // 매니저 참조
    private TextManager textManager;
    private ButtonManager buttonManager;
    private UpgradeManager upgradeManager;

    // 초기화 (채굴 시작)
    public void Initialize(string type, float baseFillDuration, float amount, TextManager txtManager, int itemIndex)
    {
        slider = GetComponentInChildren<Slider>();
        speedUpButton = GetComponentInChildren<Button>();
        buttonManager = FindObjectOfType<ButtonManager>();
        upgradeManager = FindObjectOfType<UpgradeManager>();

        textManager = txtManager;
        rewardType = type;
        rewardAmount = amount;
        baseSpeed = 1f / baseFillDuration;

        if (upgradeManager != null)
        {
            float timeReduction = upgradeManager.GetMiningTimeReduction(type);
            baseFillDuration = Mathf.Max(1f, baseFillDuration - timeReduction);
        }

        baseSpeed = 1f / baseFillDuration;

        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(ToggleIndividualSpeedUp);
        }
        isInitialized = true;
        this.itemIndex = itemIndex;

        MiningManager.Instance.StartMiningProcess(this);
    }

    // 초기화 (채굴 시작하지 않음)
    public void InitializeWithoutStarting(string type, float baseFillDuration, float amount, TextManager txtManager, int itemIndex)
    {
        slider = GetComponentInChildren<Slider>();
        speedUpButton = GetComponentInChildren<Button>();
        textManager = txtManager;
        rewardType = type;
        rewardAmount = amount;
        baseSpeed = 1f / baseFillDuration;
        buttonManager = FindObjectOfType<ButtonManager>();
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(ToggleIndividualSpeedUp);
        }
        this.itemIndex = itemIndex;
        isInitialized = true;
    }

    // 총 속도 배율 반환
    public float GetTotalSpeedMultiplier()
    {
        return individualSpeedMultiplier * globalSpeedMultiplier;
    }

    // 기본 속도 반환
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }

    // 진행도 업데이트
    public void UpdateProgress(float progress)
    {
        if (slider != null)
        {
            slider.value = Mathf.Min(progress, 1f);

            PedigreeDataManager.Instance.SaveItemProgress(itemIndex, progress, individualSpeedMultiplier);
        }
    }

    // 보상 지급
    public void GiveReward()
    {
        switch (rewardType)
        {
            case "Coin":
                textManager.UpdateCoinText((int)rewardAmount);
                break;
            case "Clover":
                textManager.UpdateCloverText((int)rewardAmount);
                break;
            case "Diamond":
                textManager.UpdateDiamondText((int)rewardAmount);
                break;
        }

        if (slider != null)
        {
            slider.value = 0f;
        }
    }

    // 개별 속도 증가 토글
    public void ToggleIndividualSpeedUp()
    {
        // 속도 배율 토글
        if (individualSpeedMultiplier > 1f)
        {
            individualSpeedMultiplier = 1f;
        }
        else
        {
            // 보상 타입별 속도 배율 설정
            individualSpeedMultiplier = rewardType switch
            {
                "Coin" => 2f,
                "Diamond" => 1.2f,
                "Clover" => 1.5f,
                _ => 1f
            };
        }

        // 버튼 색상 변경
        if (speedUpButton != null)
        {
            speedUpButton.GetComponent<Image>().color =
                individualSpeedMultiplier > 1f ? Color.yellow : Color.white;
        }

        // 상태 저장
        PedigreeDataManager.Instance.SaveItemProgress(
           itemIndex,
           slider?.value ?? 0f,
           individualSpeedMultiplier
       );
    }

    // 전역 속도 배율 설정
    public static void SetGlobalSpeedMultiplier(float multiplier)
    {
        globalSpeedMultiplier = multiplier;
    }
}
