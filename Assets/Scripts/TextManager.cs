using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ������ �ؽ�Ʈ UI�� ���� ���� ���¸� �����ϴ� Ŭ����
public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }
    // UI ��ҵ�
    [SerializeField] private TextMeshProUGUI coinText;      // ����
    [SerializeField] private TextMeshProUGUI diamondText;   // ���̾�
    [SerializeField] private TextMeshProUGUI cloverText;    // Ŭ�ι�
    [SerializeField] private TextMeshProUGUI rollText;      // �� Ƚ��
    [SerializeField] public Button rollButton;             // �� ��ư
    [SerializeField] private Score scoreManager;

    // ���� UI
    [SerializeField] private TextMeshProUGUI bestText;      // �ְ� ����
    [SerializeField] private TextMeshProUGUI currentText;   // ���� ����

    public GameObject[] Roll;
    private PedigreeManager pedigreeManager;

    // ���� ���� ������
    public int rollCount = 100;         // ���� �ֻ��� ������ Ƚ��
    public int currentCoin;             // ���� ����
    public int currentDiamond;          // ���� ���̾Ƹ��
    public int currentClover;           // ���� Ŭ�ι�
    private int bestScore;              // �ְ� ����
    private int currentScore;           // ���� ����
    private int diceRollAttempts = 0;   // ���� �õ� Ƚ��

    // ��ȭ ������Ʈ �̺�Ʈ
    public event System.Action<int> OnCoinUpdated;
    public event System.Action<int> OnDiamondUpdated;
    public event System.Action<int> OnCloverUpdated;
    public event System.Action OnRollComplete;

    [SerializeField] private UpgradeManager upgradeManager;    // ���׷��̵� �Ŵ��� ����

    // ���ӿ��� �г� UI
    [Header("Game Over Panel")]
    [SerializeField] private TextMeshProUGUI gameOverBestText;    // ���ӿ����� �ְ�����
    [SerializeField] private TextMeshProUGUI gameOverCurrentText; // ���ӿ����� ��������
    [SerializeField] private GameObject gameOverPanel;            // ���ӿ��� �г�

    // Ŭ�� ���� ����
    private float lastClickTime = 0f;
    private const float CLICK_DELAY = 0.01f;

    // �ֻ��� ���� ����
    private DiceRoll[] diceRolls;                              // ��� �ֻ���
    private List<DiceRoll> rollingDice = new List<DiceRoll>(); // ������ ���� �ֻ���

    private bool[] diceLockStates = new bool[5]; // �ֻ��� ��� ���¸� �����ϴ� �迭


    void Start()
    {
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<Score>();
        }

        for (int i = 0; i < diceLockStates.Length; i++)
        {
            diceLockStates[i] = false;
        }

        // Score ������Ʈ���� ��� ���� ���� �̺�Ʈ ����
        if (scoreManager != null)
        {
            scoreManager.OnLockStateChanged += UpdateDiceLockState;
        }

        for (int i = 0; i < Roll.Length; i++) Roll[i].SetActive(false);

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

        if (rollButton != null && Roll != null)
        {
            rollButton.onClick.RemoveAllListeners();
            rollButton.onClick.AddListener(OnRollDice);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            UpdateGameOverTexts();
        }

        diceRolls = FindObjectsOfType<DiceRoll>();
        foreach (var dice in diceRolls)
        {
            dice.OnRollStart += OnAnyDiceRollStart;
            dice.OnRollEnd += OnAnyDiceRollEnd;
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
        currentCoin = 10000;
        currentDiamond = 10000;
        currentClover = 10000;

        if (coinText != null) UpdateCoinText(0);
        if (cloverText != null) UpdateCloverText(0);
        if (diamondText != null) UpdateDiamondText(0);
        if (rollText != null) UpdateRollText();
        if (bestText != null && currentText != null) UpdateScoreTexts();

        ResetGame();

        Debug.Log("PlayerPrefs�� �ʱ�ȭ�Ǿ����ϴ�!");

        if (ChallengeManager._Instance != null)
        {
            ChallengeManager._Instance.ResetChallenges();
        }

    }

    // ���� ������Ʈ
    public void UpdateCoinText(int score)
    {
        currentCoin += score;

        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.Save();

        coinText.text = ChangeNumber(currentCoin.ToString());

        OnCoinUpdated?.Invoke(currentCoin);
    }

    // ���̾Ƹ�� ������Ʈ 
    public void UpdateDiamondText(int reward)
    {
        currentDiamond += reward;

        PlayerPrefs.SetInt("CurrentDiamond", currentDiamond);
        PlayerPrefs.Save();

        diamondText.text = ChangeNumber(currentDiamond.ToString());

        OnDiamondUpdated?.Invoke(currentDiamond);
    }

    // Ŭ�ι� ������Ʈ
    public void UpdateCloverText(int coins)
    {
        currentClover += coins;

        PlayerPrefs.SetInt("CurrentClover", currentClover);
        PlayerPrefs.Save();

        cloverText.text = ChangeNumber(currentClover.ToString());

        OnCloverUpdated?.Invoke(currentClover);
    }

    // �� Ƚ�� ������Ʈ
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

    // ��ȭ ���� ���� (K, M, B ���� ���)
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

    // ��� ���°� ����� �� ȣ��Ǵ� �޼���
    private void UpdateDiceLockState(int diceIndex, bool isLocked)
    {
        diceLockStates[diceIndex] = isLocked;
        UpdateRollButtonState();
    }

    // ���� ��ư ���� ������Ʈ
    private void UpdateRollButtonState()
    {
        int lockedCount = 0;
        for (int i = 0; i < diceLockStates.Length; i++)
        {
            if (diceLockStates[i])
            {
                lockedCount++;
            }
        }

        // ��� �ֻ����� ����� ���� ��ư ��Ȱ��ȭ, �ƴϸ� Ȱ��ȭ
        rollButton.interactable = (lockedCount < 5) && (rollCount > 0);
    }

    private void OnDestroy()
    {
        if (diceRolls != null)
        {
            foreach (var dice in diceRolls)
            {
                if (dice != null)
                {
                    dice.OnRollStart -= OnAnyDiceRollStart;
                    dice.OnRollEnd -= OnAnyDiceRollEnd;
                }
            }
        }
        if (scoreManager != null)
        {
            scoreManager.OnLockStateChanged -= UpdateDiceLockState;
        }
    }

    private void OnAnyDiceRollStart()
    {
        rollButton.interactable = false;
    }

    private int rollingDiceCount = 0;
    private void OnAnyDiceRollEnd()
    {
        rollingDiceCount++;
        if (rollingDiceCount >= 5)
        {
            // �ֻ����� ��� ������ ��, ��� ���¸� Ȯ��
            if (scoreManager != null)
            {
                bool hasUnlockedDice = false;
                for (int i = 0; i < Roll.Length; i++)
                {
                    if (!scoreManager.GetLockState(i))
                    {
                        hasUnlockedDice = true;
                        break;
                    }
                }
                rollButton.interactable = hasUnlockedDice;
            }
            rollingDiceCount = 0;
        }
    }

    // �ֻ��� ������ ó��
    public void OnRollDice()
    {
        if (scoreManager != null)
        {
            // ��ݵ��� ���� �ֻ����� Roll Ȱ��ȭ
            int unlockedCount = 0;
            for (int i = 0; i < Roll.Length; i++)
            {
                if (!scoreManager.GetLockState(i))
                {
                    Roll[i].SetActive(true);
                    unlockedCount++;
                }
            }

            // ��� �ֻ����� ������ Ȯ��
            if (unlockedCount == 0)
            {
                rollButton.interactable = false;  // ��� �ֻ����� ����� ���� ��Ȱ��ȭ
            }
            else
            {
                // �ٸ� ���ǵ�(�ֻ����� �������� ���� �ƴ� �� ��)�� �����Ǹ� Ȱ��ȭ
                if (Time.time - lastClickTime >= CLICK_DELAY && rollCount > 0)
                {
                    rollButton.interactable = true;
                }
            }

            // ��ư�� ��Ȱ��ȭ�Ǿ��ְų� ������ ���̸� ����
            if (!rollButton.interactable || Time.time - lastClickTime < CLICK_DELAY)
            {
                return;
            }

            if (rollCount > 0 && diceRollAttempts < 5)
            {
                rollButton.interactable = false;
                lastClickTime = Time.time;
                rollingDice.Clear();

                // ���⼭ ���� diceRollAttempts�� 5�� ����
                diceRollAttempts = 5;

                DiceRoll[] diceRolls = FindObjectsOfType<DiceRoll>();
                foreach (var diceRoll in diceRolls)
                {
                    if (diceRoll != null && !diceRoll.isRolling &&
                    scoreManager != null && !scoreManager.GetLockState(diceRoll.diceIndex))
                    {
                        rollingDice.Add(diceRoll);
                        diceRoll.RollDice();
                    }
                }

                // �̹� 5�� ���������Ƿ� �׻� �����
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

                // �ֻ����� ���� �� �������� ������Ʈ �߰�
                if (ChallengeManager._Instance != null)
                    ChallengeManager._Instance.UpdateChallengeProgress("�ֻ��� ������ 10ȸ �޼�", 1);


                OnRollComplete?.Invoke();
                diceRollAttempts = 0;
                StartCoroutine(CheckDiceStopCoroutine());
            }

            if (rollCount <= 0)
            {

                rollText.text = "���� ����";
                UpdateBestScore();
                currentScore = 0;
                PlayerPrefs.SetInt("CurrentScore", currentScore);
                PlayerPrefs.Save();
                UpdateScoreTexts();
                ShowGameOverPanel();
            }
        }
    }

    private IEnumerator CheckDiceStopCoroutine()
    {
        while (true)
        {
            bool allStopped = true;
            foreach (var dice in rollingDice)
            {
                if (dice.isRolling)
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped)
            {
                for (int i = 0; i < Roll.Length; i++)
                {
                    if (scoreManager != null && !scoreManager.GetLockState(i))
                    {
                        Roll[i].SetActive(false);
                        Roll[i].transform.SetPositionAndRotation(
                            transform.position = new Vector3(0f, 1.8f, -13f),
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f));
                        StartCoroutine(scoreManager.appearedDiceDelay(i));
                    }
                }

                // �ֻ����� ��� ������ ��, ��� ���¸� �ٽ� Ȯ��
                if (scoreManager != null)
                {
                    bool hasUnlockedDice = false;
                    for (int i = 0; i < Roll.Length; i++)
                    {
                        if (!scoreManager.GetLockState(i))
                        {
                            hasUnlockedDice = true;
                            break;
                        }
                    }
                    rollButton.interactable = hasUnlockedDice;
                }
                yield break;
            }
            yield return new WaitForSeconds(2.3f);
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

    // ���� ����
    private void ResetGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            rollButton.interactable = true;
        }
    }
    public void SaveGameData()
    {
        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.SetInt("CurrentDiamond", currentDiamond);
        PlayerPrefs.SetInt("CurrentClover", currentClover);
        PlayerPrefs.SetInt("RollCount", rollCount);
        PlayerPrefs.Save();
    }
}