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
    }

    private void ToggleLock(int index)
    {
        if (index >= 0 && index < isLocked.Length)
        {
            isLocked[index] = !isLocked[index];

            Image buttonImage = LockButton[index].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = isLocked[index] ? lockedSprite : unlockedSprite;
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
