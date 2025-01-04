using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ���� �� ��ȭ(����, ���̾Ƹ��, Ŭ�ι�) �����͸� �����ϴ� Ŭ����
public class CashData : MonoBehaviour
{
    // UI �ؽ�Ʈ ������Ʈ
    [Header("��ȭ")]
    [SerializeField] private TextMeshProUGUI coinText;      // ����
    [SerializeField] private TextMeshProUGUI diamondText;   // ���̾�
    [SerializeField] private TextMeshProUGUI cloverText;    // Ŭ�ι�

    [Header("������ ������ �����")]
    [SerializeField] private TextManager textManager;       // �ؽ�Ʈ �Ŵ��� ����

    // ��ȭ ����� �߻��ϴ� �̺�Ʈ
    public event System.Action OnResourceChanged;

    // ���� ��ȭ ���� PlayerPrefs���� ������ (�⺻�� 0)
    private int currentCoin => PlayerPrefs.GetInt("CurrentCoin", 0);
    private int currentDiamond => PlayerPrefs.GetInt("CurrentDiamond", 0);
    private int currentClover => PlayerPrefs.GetInt("CurrentClover", 0);

    // ��ȭ�� ������� Ȯ���ϴ� �޼���
    public bool HasEnoughResources(int coinCost, int cloverCost, int diamondCost)
    {
        return currentCoin >= coinCost &&
               currentClover >= cloverCost &&
               currentDiamond >= diamondCost;
    }

    // ������Ʈ Ȱ��ȭ�� �̺�Ʈ ����
    private void OnEnable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated += UpdateCoinText;
            textManager.OnDiamondUpdated += UpdateDiamondText;
            textManager.OnCloverUpdated += UpdateCloverText;
        }
    }

    // ������Ʈ ��Ȱ��ȭ�� �̺�Ʈ ���� ����
    private void OnDisable()
    {
        if (textManager != null)
        {
            textManager.OnCoinUpdated -= UpdateCoinText;
            textManager.OnDiamondUpdated -= UpdateDiamondText;
            textManager.OnCloverUpdated -= UpdateCloverText;
        }
    }

    // ���۽� UI ������Ʈ
    private void Start()
    {
        UpdateShopUI();
    }

    // ���� UI ������Ʈ 
    private void UpdateShopUI()
    {
        UpdateCoinText(PlayerPrefs.GetInt("CurrentCoin", 0));
        UpdateDiamondText(PlayerPrefs.GetInt("CurrentDiamond", 0));
        UpdateCloverText(PlayerPrefs.GetInt("CurrentClover", 0));
    }

    // ���׷��̵� �õ� (��� ����)
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

        Debug.Log("��ȭ�� �����մϴ�!");
        return false;
    }

    // �� ��ȭ �ؽ�Ʈ ������Ʈ �޼���
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
