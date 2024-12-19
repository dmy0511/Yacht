using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI cloverText;
    [SerializeField] private TextMeshProUGUI rollText;
    [SerializeField] private Button rollButton;

    private int rollCount = 100;
    private int diceRollAttempts = 0;

    private int currentCoin;
    private int currentDiamond;
    private int currentClover;

    void Start()
    {
        // 첫 로드 시, 데이터 초기화
        if (!PlayerPrefs.HasKey("FirstLoad"))
        {
            PlayerPrefs.SetInt("CurrentCoin", 0);
            PlayerPrefs.SetInt("CurrentDiamond", 0);
            PlayerPrefs.SetInt("CurrentClover", 0);
            PlayerPrefs.SetInt("RollCount", 100);

            PlayerPrefs.SetInt("FirstLoad", 1);
            PlayerPrefs.Save();
        }

        rollCount = PlayerPrefs.GetInt("RollCount", 100);

        currentCoin = PlayerPrefs.GetInt("CurrentCoin", 0);
        currentDiamond = PlayerPrefs.GetInt("CurrentDiamond", 0);
        currentClover = PlayerPrefs.GetInt("CurrentClover", 0);

        UpdateCoinText(0);
        UpdateCloverText(0);
        UpdateDiamondText(0);
        UpdateRollText();

        rollButton.onClick.AddListener(OnRollDice);
    }

    public void UpdateCoinText(int score)
    {
        currentCoin += score;

        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.Save();

        coinText.text = ChangeNumber(currentCoin.ToString());
    }

    public void UpdateCloverText(int coins)
    {
        currentClover += coins;

        PlayerPrefs.SetInt("CurrentClover", currentClover);
        PlayerPrefs.Save();

        cloverText.text = ChangeNumber(currentClover.ToString());
    }

    public void UpdateDiamondText(int reward)
    {
        currentDiamond += reward;

        PlayerPrefs.SetInt("CurrentDiamond", currentDiamond);
        PlayerPrefs.Save();

        diamondText.text = ChangeNumber(currentDiamond.ToString());
    }

    public void UpdateRollText()
    {
        if (rollCount >= 99)
        {
            rollText.text = "99+";
        }
        else
        {
            rollText.text = rollCount.ToString();
        }
    }

    public static string ChangeNumber(string number)
    {
        char[] unitAlphabet = new char[3] { 'K', 'M', 'B' };
        int unit = 0;
        while (number.Length > 6)
        {
            unit++;
            number = number.Substring(0, number.Length - 3);
        }

        if (number.Length > 3)
        {
            int newInt = int.Parse(number);
            if (number.Length > 4)
            {
                return (newInt / 1000).ToString() + unitAlphabet[unit];
            }
            else
            {
                return (newInt / 1000f).ToString("0.0") + unitAlphabet[unit];
            }
        }
        else
        {
            int newInt = int.Parse(number);
            return (newInt).ToString();
        }
    }

    public void OnRollDice()
    {
        if (rollCount > 0 && diceRollAttempts < 5)
        {
            DiceRoll[] diceRolls = FindObjectsOfType<DiceRoll>();
            foreach (var diceRoll in diceRolls)
            {
                if (diceRoll != null && !diceRoll.isRolling)
                {
                    diceRoll.RollDice();
                    diceRollAttempts++;
                }
            }

            if (diceRollAttempts >= 5)
            {
                rollCount--;

                PlayerPrefs.SetInt("RollCount", rollCount);
                PlayerPrefs.Save();

                UpdateRollText();
                diceRollAttempts = 0;
            }
        }
        else if (rollCount <= 0)
        {
            rollText.text = "게임 오버";
        }
    }
}
