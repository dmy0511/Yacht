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

    [SerializeField] private TextMeshProUGUI bestText;
    [SerializeField] private TextMeshProUGUI currentText;

    private int rollCount = 100;
    private int diceRollAttempts = 0;

    private int currentCoin;
    private int currentDiamond;
    private int currentClover;
    private int bestScore;
    private int currentScore;

    public event System.Action<int> OnCoinUpdated;
    public event System.Action<int> OnDiamondUpdated;
    public event System.Action<int> OnCloverUpdated;

    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Game Over Panel")]
    [SerializeField] private TextMeshProUGUI gameOverBestText;
    [SerializeField] private TextMeshProUGUI gameOverCurrentText;
    [SerializeField] private GameObject gameOverPanel;

    public event System.Action OnRollComplete;

    void Start()
    {
        if (!PlayerPrefs.HasKey("FirstLoad"))
        {
            PlayerPrefs.SetInt("CurrentCoin", 0);
            PlayerPrefs.SetInt("CurrentDiamond", 0);
            PlayerPrefs.SetInt("CurrentClover", 0);
            PlayerPrefs.SetInt("RollCount", 100);
            PlayerPrefs.SetInt("BestScore", 0);
            PlayerPrefs.SetInt("CurrentScore", 0);
            PlayerPrefs.SetInt("FirstLoad", 1);
            PlayerPrefs.Save();
        }

        rollCount = PlayerPrefs.GetInt("RollCount", 100);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        currentCoin = PlayerPrefs.GetInt("CurrentCoin", 0);
        currentDiamond = PlayerPrefs.GetInt("CurrentDiamond", 0);
        currentClover = PlayerPrefs.GetInt("CurrentClover", 0);

        if (coinText != null) UpdateCoinText(0);
        if (cloverText != null) UpdateCloverText(0);
        if (diamondText != null) UpdateDiamondText(0);

        if (rollText != null) UpdateRollText();
        if (bestText != null && currentText != null) UpdateScoreTexts();

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(OnRollDice);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            UpdateGameOverTexts();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPlayerPrefs();
        }
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.SetInt("CurrentCoin", 0);
        PlayerPrefs.SetInt("CurrentDiamond", 0);
        PlayerPrefs.SetInt("CurrentClover", 0);
        PlayerPrefs.SetInt("RollCount", 100);
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.SetInt("CurrentScore", 0);
        PlayerPrefs.SetInt("FirstLoad", 1);
        PlayerPrefs.Save();

        rollCount = 100;
        bestScore = 0;
        currentScore = 0;
        currentCoin = 0;
        currentDiamond = 0;
        currentClover = 0;

        if (coinText != null) UpdateCoinText(0);
        if (cloverText != null) UpdateCloverText(0);
        if (diamondText != null) UpdateDiamondText(0);
        if (rollText != null) UpdateRollText();
        if (bestText != null && currentText != null) UpdateScoreTexts();

        ResetGame();

        Debug.Log("PlayerPrefs가 초기화되었습니다!");
    }

    public void UpdateCoinText(int score)
    {
        currentCoin += score;

        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.Save();

        coinText.text = ChangeNumber(currentCoin.ToString());

        OnCoinUpdated?.Invoke(currentCoin);
    }

    public void UpdateDiamondText(int reward)
    {
        currentDiamond += reward;

        PlayerPrefs.SetInt("CurrentDiamond", currentDiamond);
        PlayerPrefs.Save();

        diamondText.text = ChangeNumber(currentDiamond.ToString());

        OnDiamondUpdated?.Invoke(currentDiamond);
    }

    public void UpdateCloverText(int coins)
    {
        currentClover += coins;

        PlayerPrefs.SetInt("CurrentClover", currentClover);
        PlayerPrefs.Save();

        cloverText.text = ChangeNumber(currentClover.ToString());

        OnCloverUpdated?.Invoke(currentClover);
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

    public void AddRollCount(int amount)
    {
        rollCount += amount;
        PlayerPrefs.SetInt("RollCount", rollCount);
        PlayerPrefs.Save();
        UpdateRollText();
    }

    public void UpdateScoreTexts()
    {
        bestText.text = "BEST " + bestScore.ToString() + "F";
        currentText.text = "NOW " + currentScore.ToString() + "F";
        UpdateGameOverTexts();
    }

    private void UpdateGameOverTexts()
    {
        if (gameOverBestText != null)
        {
            gameOverBestText.text = "BEST " + bestScore.ToString() + "F";
        }
        if (gameOverCurrentText != null)
        {
            gameOverCurrentText.text = "NOW " + currentScore.ToString() + "F";
        }
    }

    public void IncrementCurrentScore()
    {
        currentScore++;
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.Save();
        UpdateScoreTexts();
    }

    private void UpdateBestScore()
    {
        if (currentScore >= bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
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
                float defenseRate = 0f;
                if (upgradeManager != null)
                {
                    defenseRate = upgradeManager.GetRollDefenseRate();
                }

                bool isDefenseSuccessful = Random.Range(0f, 100f) < defenseRate;

                if (!isDefenseSuccessful)
                {
                    rollCount--;
                    PlayerPrefs.SetInt("RollCount", rollCount);
                    PlayerPrefs.Save();
                }

                UpdateRollText();
                OnRollComplete?.Invoke();
                diceRollAttempts = 0;
            }
        }

        if (rollCount <= 0)
        {
            rollText.text = "게임 오버";
            UpdateBestScore();
            currentScore = 0;
            PlayerPrefs.SetInt("CurrentScore", currentScore);
            PlayerPrefs.Save();
            UpdateScoreTexts();
            ShowGameOverPanel();
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            UpdateGameOverTexts();
            gameOverPanel.SetActive(true);
            rollButton.interactable = false;
        }
    }

    private void ResetGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            rollButton.interactable = true;
        }
    }
}
