using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �ֻ��� ���� ��� ���¸� �����ϴ� Ŭ����
public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; } // �̱��� �ν��Ͻ�
    public event System.Action<int, bool> OnLockStateChanged;

    // UI ��ҵ�
    [SerializeField] private DiceRoll[] dice;                   // �ֻ��� �迭
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

    public event Action<int> OnScoreUpdated;
    private int currentScore;

    // �ʱ�ȭ
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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

        // ��ư�� ��Ȱ��ȭ ���� ���� ����
        foreach (var button in LockButton)
        {
            ColorBlock colors = button.colors;
            colors.disabledColor = Color.white; // �Ǵ� ���ϴ� ����
            button.colors = colors;
        }
    }

    // �ֻ��� ���� üũ (3ȸ �̻��̸� ����)
    private void CheckDiceRoll()
    {
        totalRollCount++;

        if (totalRollCount >= 3) ResetAllDice();
    }

    // ��� ���� ���
    private void ToggleLock(int index)
    {
        if (index >= 0 && index < isLocked.Length && DiceScore[index].activeSelf && !dice[index].isRolling)
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
            OnLockStateChanged?.Invoke(index, isLocked[index]);
        }
    }

    // ��� �ֻ��� ����
    public void ResetAllDice()
    {
        for (int i = 0; i < DiceScore.Length; i++)
        {
            if (DiceScore[i] != null) DiceScore[i].SetActive(false);
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            bool wasLocked = isLocked[i];
            isLocked[i] = false;
            newDiceScore[i] = 0;

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
            // ��� ���°� ����Ǿ��� ���� �̺�Ʈ �߻�
            if (wasLocked)
            {
                OnLockStateChanged?.Invoke(i, false);
            }
        }
        foreach (var die in dice)
        {
            if (die != null) die.diceFaceNum = 0;
        }
        totalRollCount = 0;
    }

    // �ֻ��� ���� ���� ������Ʈ
    private void Update()
    {
        // �⺻������ ��Ȱ��ȭ ���·� ����
        isDiceRolling = true;

        // ��� DiceScore�� Ȱ��ȭ�Ǿ����� üũ
        bool allDiceScoreActive = true;

        bool allLocked = true;
        int lockedCount = 0;

        TextManager textManager = FindObjectOfType<TextManager>();
        if (textManager != null && textManager.rollButton != null)
        {
            if (!isDiceRolling) // �ֻ����� �������� ���� ���� ���� ��ư ���� ����
            {
                textManager.rollButton.interactable = !allLocked;
            }
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            if (isLocked[i])
                lockedCount++;
            else
                allLocked = false;
        }

        // ��� ���°� ����� ���� ����� �α� ���
        if (lockedCount != previousLockedCount)
        {
            Debug.Log($"��� �ֻ���: {lockedCount}��, ���� �ֻ���: {5 - lockedCount}��");
            previousLockedCount = lockedCount;
        }


        for (int i = 0; i < DiceScore.Length; i++)
        {
            if (!DiceScore[i].activeSelf && !isLocked[i])
            {
                allDiceScoreActive = false;
                break;
            }
        }

        // ��� DiceScore�� Ȱ��ȭ�Ǿ��ٸ� isDiceRolling�� false�� ����
        if (allDiceScoreActive) isDiceRolling = false;

        for (int i = 0; i < dice.Length; i++)
        {
            LockButton[i].interactable = DiceScore[i].activeSelf && !isDiceRolling;

            if (isLocked[i])
            {
                DiceScore[i].SetActive(true);
                continue;
            }

            if (dice[i].isRolling)
            {
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
                    if (newDiceScore[i] != currentFaceNum) newDiceScore[i] = currentFaceNum;
                    DiceScore[i].transform.rotation = GetRotationForNumber(currentFaceNum);
                    break;
                default:
                    DiceScore[i].SetActive(false);
                    break;
            }
            // ���� üũ
            if (pedigreeManager != null) pedigreeManager.CheckCondition();
        }

    }

    private int previousLockedCount = 0;  // ���� ��� ���¸� ������ ����

    // �̺�Ʈ ���� ����
    private void OnDestroy()
    {
        if (textManager != null) textManager.OnRollComplete -= CheckDiceRoll;
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
        if (index >= 0 && index < isLocked.Length) return isLocked[index];
        return false;
    }
    public bool IsAllDiceLocked()
    {
        int lockedCount = 0;
        for (int i = 0; i < isLocked.Length; i++)
            if (isLocked[i]) lockedCount++;

        return lockedCount >= 5;
    }
    public void ResetRollCount()
    {
        totalRollCount = 0;
        Debug.Log("�ֻ��� �� Ƚ���� �ʱ�ȭ�Ǿ����ϴ�.");
    }
    public void UpdateScore(int amount)
    {
        currentScore += amount;
        OnScoreUpdated?.Invoke(currentScore); // �̺�Ʈ �߻�
    }

}