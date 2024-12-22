using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private DiceRoll[] dice;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private Button[] LockButton;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;

    private bool[] isLocked;
    private PedigreeManager pedigreeManager;

    [SerializeField] private Vector2 unlockedSize = new Vector2(74f, 70f);
    [SerializeField] private Vector2 lockedSize = new Vector2(61.5f, 70.5f);

    private int lockedRollCount = 0;
    private bool wasLockedOnLastRoll = false;
    private TextManager textManager;

    private void Awake()
    {
        dice = FindObjectsOfType<DiceRoll>();
        isLocked = new bool[dice.Length];
        for (int i = 0; i < LockButton.Length; i++)
        {
            int index = i;
            LockButton[i].onClick.AddListener(() => ToggleLock(index));
        }
        pedigreeManager = FindObjectOfType<PedigreeManager>();
        textManager = FindObjectOfType<TextManager>();

        if (textManager != null)
        {
            textManager.OnRollComplete += CheckDiceRoll;
        }
    }

    private void OnDestroy()
    {
        if (textManager != null)
        {
            textManager.OnRollComplete -= CheckDiceRoll;
        }
    }

    private bool IsAnyDiceLocked()
    {
        for (int i = 0; i < isLocked.Length; i++)
        {
            if (isLocked[i]) return true;
        }
        return false;
    }

    private void CheckDiceRoll()
    {
        bool currentlyLocked = IsAnyDiceLocked();

        if (wasLockedOnLastRoll && currentlyLocked)
        {
            lockedRollCount++;
        }
        else if (!currentlyLocked)
        {
            lockedRollCount = 0;
        }
        else if (!wasLockedOnLastRoll && currentlyLocked)
        {
            lockedRollCount = 1;
        }

        if (currentlyLocked && lockedRollCount >= 4)
        {
            ResetAllDice();
            lockedRollCount = 0;
        }

        wasLockedOnLastRoll = currentlyLocked;
    }

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

            if (!isLocked[index] && !IsAnyDiceLocked())
            {
                lockedRollCount = 0;
                wasLockedOnLastRoll = false;
            }
        }
    }

    private void ResetAllDice()
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
    }

    private void Update()
    {
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
        if (pedigreeManager != null)
        {
            pedigreeManager.CheckCondition();
        }
    }
}
