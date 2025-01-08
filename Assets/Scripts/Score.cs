using System;
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
    //<s[SerializeField] private TextMeshProUGUI[] scoreTexts;      // ���� �ؽ�Ʈ��
    [SerializeField] public GameObject[] DiceScore;              // ���� ���̽�
    [SerializeField] private Button[] LockButton;               // ��� ��ư��
    [SerializeField] private Sprite unlockedSprite;             // ������� �̹���
    [SerializeField] private Sprite lockedSprite;               // ��� �̹���

    // ��ư ũ�� ����
    [SerializeField] private Vector2 unlockedSize = new Vector2(74f, 70f);      // ������� ũ��
    [SerializeField] private Vector2 lockedSize = new Vector2(61.5f, 70.5f);    // ��� ũ��

    private bool[] isLocked;                    // ��� ���� �迭
    private PedigreeManager pedigreeManager;                    // �躸 �Ŵ���
    private TextManager textManager;                            // �ؽ�Ʈ �Ŵ���
    private int totalRollCount = 0;                             // �� ���� Ƚ��
    private int[] newDiceScore;
    private bool isDiceRolling = false;  // �ֻ����� �������� �ִ��� Ȯ���ϴ� �÷���
    // �ʱ�ȭ
    private void Awake()
    {
        // ������Ʈ�� ã��
        dice = FindObjectsOfType<DiceRoll>();
        Array.Sort(dice, (a, b) => a.diceIndex.CompareTo(b.diceIndex));
        isLocked = new bool[dice.Length];
        newDiceScore = new int[dice.Length];

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
        for (int i = 0; i < DiceScore.Length; i++)
        {
            if (DiceScore[i] != null)
            {
                DiceScore[i].SetActive(false);
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
        for (int i = 0; i < dice.Length; i++)
        {

            if (isLocked[i])
            {
                DiceScore[i].SetActive(true);
                continue;
            }

            // �ֻ����� �������� ���̸� ��Ȱ��ȭ�ϰ� ȸ���� �ʱ�ȭ
            if (dice[i].isRolling)
            {
                isDiceRolling = true;
                DiceScore[i].SetActive(false);
                DiceScore[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                continue;
            }

            int currentFaceNum = dice[i].diceFaceNum;

            // �ֻ��� ���ڿ� ���� ȸ���� ���� �� Ȱ��ȭ
            switch (dice[i].diceFaceNum)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    newDiceScore[i] = currentFaceNum;
                    DiceScore[i].transform.rotation = GetRotationForNumber(currentFaceNum);
                    break;
                default:
                    DiceScore[i].SetActive(false);
                    break;
            }

            // ���� üũ
            if (pedigreeManager != null)
            {
                pedigreeManager.CheckCondition();
            }
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
    private Quaternion GetRotationForNumber(int number)
    {
        return number switch
        {
            1 => Quaternion.Euler(0, 0, -90),
            2 => Quaternion.Euler(-90, 0, 0),
            3 => Quaternion.Euler(90, 0, 0),
            4 => Quaternion.Euler(0, 0, 0),
            5 => Quaternion.Euler(0, 0, 180),
            6 => Quaternion.Euler(0, 0, 90),
            _ => Quaternion.Euler(0, 0, 0)
        };
    }

    public IEnumerator appearedDiceDelay(int index)
    {
        yield return new WaitForSeconds(0f);
        DiceScore[index].SetActive(true);
    }

    public int[] GetDiceScores()
    {
        return newDiceScore;
    }
    public bool GetLockState(int index)
    {
        if (index >= 0 && index < isLocked.Length)
        {
            return isLocked[index];
        }
        return false;
    }
}
