using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 게임의 텍스트 UI와 게임 진행 상태를 관리하는 클래스
public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }
    // UI 요소들
    [SerializeField] private TextMeshProUGUI coinText;      // 코인
    [SerializeField] private TextMeshProUGUI diamondText;   // 다이아
    [SerializeField] private TextMeshProUGUI cloverText;    // 클로버
    [SerializeField] private TextMeshProUGUI rollText;      // 롤 횟수
    [SerializeField] public Button rollButton;             // 롤 버튼
    [SerializeField] private Score scoreManager;

    // 점수 UI
    [SerializeField] private TextMeshProUGUI bestText;      // 최고 점수
    [SerializeField] private TextMeshProUGUI currentText;   // 현재 점수

    public GameObject[] Roll;
    private PedigreeManager pedigreeManager;

    // 게임 상태 변수들
    public int rollCount = 100;         // 남은 주사위 굴리기 횟수
    public int currentCoin;             // 현재 코인
    public int currentDiamond;          // 현재 다이아몬드
    public int currentClover;           // 현재 클로버
    private int bestScore;              // 최고 점수
    private int currentScore;           // 현재 점수
    private int diceRollAttempts = 0;   // 현재 시도 횟수

    // 재화 업데이트 이벤트
    public event System.Action<int> OnCoinUpdated;
    public event System.Action<int> OnDiamondUpdated;
    public event System.Action<int> OnCloverUpdated;
    public event System.Action OnRollComplete;

    [SerializeField] private UpgradeManager upgradeManager;    // 업그레이드 매니저 참조

    // 게임오버 패널 UI
    [Header("Game Over Panel")]
    [SerializeField] private TextMeshProUGUI gameOverBestText;    // 게임오버시 최고점수
    [SerializeField] private TextMeshProUGUI gameOverCurrentText; // 게임오버시 현재점수
    [SerializeField] private GameObject gameOverPanel;            // 게임오버 패널

    // 클릭 제어 변수
    private float lastClickTime = 0f;
    private const float CLICK_DELAY = 0.01f;

    // 주사위 관련 변수
    private DiceRoll[] diceRolls;                              // 모든 주사위
    private List<DiceRoll> rollingDice = new List<DiceRoll>(); // 굴리는 중인 주사위

    private bool[] diceLockStates = new bool[5]; // 주사위 잠금 상태를 저장하는 배열


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

        // Score 컴포넌트에서 잠금 상태 변경 이벤트 구독
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

        Debug.Log("PlayerPrefs가 초기화되었습니다!");

        if (ChallengeManager._Instance != null)
        {
            ChallengeManager._Instance.ResetChallenges();
        }

    }

    // 코인 업데이트
    public void UpdateCoinText(int score)
    {
        currentCoin += score;

        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.Save();

        coinText.text = ChangeNumber(currentCoin.ToString());

        OnCoinUpdated?.Invoke(currentCoin);
    }

    // 다이아몬드 업데이트 
    public void UpdateDiamondText(int reward)
    {
        currentDiamond += reward;

        PlayerPrefs.SetInt("CurrentDiamond", currentDiamond);
        PlayerPrefs.Save();

        diamondText.text = ChangeNumber(currentDiamond.ToString());

        OnDiamondUpdated?.Invoke(currentDiamond);
    }

    // 클로버 업데이트
    public void UpdateCloverText(int coins)
    {
        currentClover += coins;

        PlayerPrefs.SetInt("CurrentClover", currentClover);
        PlayerPrefs.Save();

        cloverText.text = ChangeNumber(currentClover.ToString());

        OnCloverUpdated?.Invoke(currentClover);
    }

    // 롤 횟수 업데이트
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

    // 재화 단위 변경 (K, M, B 단위 사용)
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

    // 잠금 상태가 변경될 때 호출되는 메서드
    private void UpdateDiceLockState(int diceIndex, bool isLocked)
    {
        diceLockStates[diceIndex] = isLocked;
        UpdateRollButtonState();
    }

    // 실행 버튼 상태 업데이트
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

        // 모든 주사위가 잠겼을 때는 버튼 비활성화, 아니면 활성화
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
            // 주사위가 모두 멈췄을 때, 잠금 상태를 확인
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

    // 주사위 굴리기 처리
    public void OnRollDice()
    {
        if (scoreManager != null)
        {
            // 잠금되지 않은 주사위만 Roll 활성화
            int unlockedCount = 0;
            for (int i = 0; i < Roll.Length; i++)
            {
                if (!scoreManager.GetLockState(i))
                {
                    Roll[i].SetActive(true);
                    unlockedCount++;
                }
            }

            // 모든 주사위가 잠겼는지 확인
            if (unlockedCount == 0)
            {
                rollButton.interactable = false;  // 모든 주사위가 잠겼을 때만 비활성화
            }
            else
            {
                // 다른 조건들(주사위가 굴러가는 중이 아닐 때 등)이 충족되면 활성화
                if (Time.time - lastClickTime >= CLICK_DELAY && rollCount > 0)
                {
                    rollButton.interactable = true;
                }
            }

            // 버튼이 비활성화되어있거나 딜레이 중이면 리턴
            if (!rollButton.interactable || Time.time - lastClickTime < CLICK_DELAY)
            {
                return;
            }

            if (rollCount > 0 && diceRollAttempts < 5)
            {
                rollButton.interactable = false;
                lastClickTime = Time.time;
                rollingDice.Clear();

                // 여기서 먼저 diceRollAttempts를 5로 설정
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

                // 이미 5로 설정했으므로 항상 실행됨
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

                // 주사위를 굴릴 때 도전과제 업데이트 추가
                if (ChallengeManager._Instance != null)
                    ChallengeManager._Instance.UpdateChallengeProgress("주사위 굴리기 10회 달성", 1);


                OnRollComplete?.Invoke();
                diceRollAttempts = 0;
                StartCoroutine(CheckDiceStopCoroutine());
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

                // 주사위가 모두 멈췄을 때, 잠금 상태를 다시 확인
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

    // 게임 리셋
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