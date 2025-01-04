using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ä�� �������� �ӵ� ������ �����ϴ� Ŭ����
public class SpeedUpCtrl : MonoBehaviour
{
    // UI ������Ʈ
    private Slider slider;            // ���൵ �����̴�
    private Button speedUpButton;     // �ӵ� ���� ��ư

    // ������ �Ӽ�
    private string rewardType;                          // ���� Ÿ��
    private float baseSpeed;                            // �⺻ �ӵ�
    private float individualSpeedMultiplier = 1f;       // ���� �ӵ� ����
    private float rewardAmount;                         // ����
    private bool isInitialized = false;                 // �ʱ�ȭ ����
    private static float globalSpeedMultiplier = 1f;    // ���� �ӵ� ����
    private int itemIndex;                              // ������ �ε���

    // �Ŵ��� ����
    private TextManager textManager;
    private ButtonManager buttonManager;
    private UpgradeManager upgradeManager;

    // �ʱ�ȭ (ä�� ����)
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

    // �ʱ�ȭ (ä�� �������� ����)
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

    // �� �ӵ� ���� ��ȯ
    public float GetTotalSpeedMultiplier()
    {
        return individualSpeedMultiplier * globalSpeedMultiplier;
    }

    // �⺻ �ӵ� ��ȯ
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }

    // ���൵ ������Ʈ
    public void UpdateProgress(float progress)
    {
        if (slider != null)
        {
            slider.value = Mathf.Min(progress, 1f);

            PedigreeDataManager.Instance.SaveItemProgress(itemIndex, progress, individualSpeedMultiplier);
        }
    }

    // ���� ����
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

    // ���� �ӵ� ���� ���
    public void ToggleIndividualSpeedUp()
    {
        // �ӵ� ���� ���
        if (individualSpeedMultiplier > 1f)
        {
            individualSpeedMultiplier = 1f;
        }
        else
        {
            // ���� Ÿ�Ժ� �ӵ� ���� ����
            individualSpeedMultiplier = rewardType switch
            {
                "Coin" => 2f,
                "Diamond" => 1.2f,
                "Clover" => 1.5f,
                _ => 1f
            };
        }

        // ��ư ���� ����
        if (speedUpButton != null)
        {
            speedUpButton.GetComponent<Image>().color =
                individualSpeedMultiplier > 1f ? Color.yellow : Color.white;
        }

        // ���� ����
        PedigreeDataManager.Instance.SaveItemProgress(
           itemIndex,
           slider?.value ?? 0f,
           individualSpeedMultiplier
       );
    }

    // ���� �ӵ� ���� ����
    public static void SetGlobalSpeedMultiplier(float multiplier)
    {
        globalSpeedMultiplier = multiplier;
    }
}
