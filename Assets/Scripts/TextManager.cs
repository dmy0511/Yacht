using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI cloverText;
    [SerializeField] private TextMeshProUGUI diamondText;

    void Start()
    {
        UpdateGoldText(1000);
        UpdateCloverText(100000);
        UpdateDiamondText(1000000);
    }

    public void UpdateGoldText(int score)
    {
        goldText.text = ChangeNumber(score.ToString());
    }

    public void UpdateCloverText(int coins)
    {
        cloverText.text = ChangeNumber(coins.ToString());
    }

    public void UpdateDiamondText(int reward)
    {
        diamondText.text = ChangeNumber(reward.ToString());
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
}
