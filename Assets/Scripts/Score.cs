using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �ֻ��� ���� ��� ���¸� �����ϴ� Ŭ����
public class Score : MonoBehaviour
{
    // UI ��ҵ�
    [SerializeField] private DiceRoll[] dice;                   // �ֻ��� �迭
    [SerializeField] private TextMeshProUGUI[] scoreTexts;      // ���� �ؽ�Ʈ��
    [SerializeField] private Button[] LockButton;               // ��� ��ư��
    [SerializeField] private Sprite unlockedSprite;             // ������� �̹���
    [SerializeField] private Sprite lockedSprite;               // ��� �̹���

    // ��ư ũ�� ����
    [SerializeField] private Vector2 unlockedSize = new Vector2(74f, 70f);      // ������� ũ��
    [SerializeField] private Vector2 lockedSize = new Vector2(61.5f, 70.5f);    // ��� ũ��

    private bool[] isLocked;                   // ��� ���� �迭
    private PedigreeManager pedigreeManager;   // �躸 �Ŵ���
    private TextManager textManager;           // �ؽ�Ʈ �Ŵ���
    private int totalRollCount = 0;            // �� ���� Ƚ��

    // �ʱ�ȭ
    private void Awake()
    {
        // ������Ʈ�� ã��
        dice = FindObjectsOfType<DiceRoll>();
        isLocked = new bool[dice.Length];

        // ��� ��ư �̺�Ʈ ����
        for (int i = 0; i < LockButton.Length; i++)
        {
            int index = i;
            LockButton[i].onClick.AddListener(() => ToggleLock(index));
        }

        // �Ŵ��� ���� ����
        pedigreeManager = FindObjectOfType<PedigreeManager>();
        textManager = FindObjectOfType<TextManager>();
        if (textManager != null)
        {
            textManager.OnRollComplete += CheckDiceRoll;
        }
    }

    // �ֻ��� ���� üũ (3ȸ �̻��̸� ����)
    private void CheckDiceRoll()
    {
        totalRollCount++;

        if (totalRollCount >= 3)
        {
            ResetAllDice();
        }
    }

    // ��� ���� ���
    private void ToggleLock(int index)
    {
        if (index >= 0 && index < isLocked.Length)
        {
            isLocked[index] = !isLocked[index];
            Image buttonImage = LockButton[index].GetComponent<Image>();
            RectTransform rectTransform = LockButton[index].GetComponent<RectTransform>();
            if (buttonImage != null && rectTransform != null)
            {
                buttonImage.sprite = isLocked[index] ? lockedSprite : unlockedSprite;
                Vector2 targetSize = isLocked[index] ? lockedSize : unlockedSize;
                rectTransform.sizeDelta = targetSize;
            }
        }
    }

    // ��� �ֻ��� ����
    public void ResetAllDice()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = "?";
            }
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            isLocked[i] = false;
            if (LockButton[i] != null)
            {
                Image buttonImage = LockButton[i].GetComponent<Image>();
                RectTransform rectTransform = LockButton[i].GetComponent<RectTransform>();
                if (buttonImage != null && rectTransform != null)
                {
                    buttonImage.sprite = unlockedSprite;
                    rectTransform.sizeDelta = unlockedSize;
                }
            }
        }
        foreach (var die in dice)
        {
            if (die != null)
            {
                die.diceFaceNum = 0;
            }
        }
        totalRollCount = 0;
    }

    // �ֻ��� ���� ���� ������Ʈ
    private void Update()
    {
        // ��ݵ��� ���� �ֻ������� ���� ������Ʈ
        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] != null && scoreTexts[i] != null)
            {
                if (!isLocked[i])
                {
                    scoreTexts[i].text = dice[i].diceFaceNum != 0 ? dice[i].diceFaceNum.ToString() : "?";
                }
            }
        }

        // ���� üũ
        if (pedigreeManager != null)
        {
            pedigreeManager.CheckCondition();
        }
    }

    // �̺�Ʈ ���� ����
    private void OnDestroy()
    {
        if (textManager != null)
        {
            textManager.OnRollComplete -= CheckDiceRoll;
        }
    }
}
