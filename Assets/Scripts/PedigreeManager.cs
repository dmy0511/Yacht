using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 족보 시스템 전반을 관리하는 클래스
public class PedigreeManager : MonoBehaviour
{
    public static PedigreeDataManager Instance { get; private set; }
    // UI 텍스트 요소들
    [SerializeField] private TextMeshProUGUI[] scoreTexts;      // 점수 텍스트들
    [SerializeField] private TextMeshProUGUI pedigreeText;      // 조건 표시 텍스트 1
    [SerializeField] private TextMeshProUGUI pedigreeText2;     // 조건 표시 텍스트 2

    // 프리팹과 부모 오브젝트
    [SerializeField] private GameObject[] contentParents;       // 아이템 부모 오브젝트들
    [SerializeField] private GameObject[] miningPrefabs;        // 채굴 프리팹들

    // 매니저 참조
    [SerializeField] private TextManager textManager;           // 텍스트 매니저
    [SerializeField] private GoodsManager goodsManager;         // 재화 매니저
    [SerializeField] private Score scoreManager;                // 점수 매니저

    private GameObject lastSpawnedItem;                         // 마지막 생성된 아이템
    private int totalConditionMetCount = 0;                     // 달성한 총 조건 수

    // 전역 속도 증가 관련
    [SerializeField] private Button globalSpeedUpButton;         // 전체 속도 증가 버튼
    private bool isGlobalSpeedUp = false;                        // 전체 속도 증가 상태

    // 조건 관련
    private string[] conditions = {                              // 가능한 조건들
       "더블", "오버", "트리플", "쿼드라플", "제곱", "풀하우스",
       "야추", "이븐", "오드", "16", "스몰", "라지"
   };

    private string currentCondition;                             // 현재 조건
    private bool conditionMetPreviously = false;                 // 이전 조건 달성 여부
    private int currentSpawnIndex = 0;                           // 현재 스폰 인덱스

    private List<GameObject> spawnedItems = new List<GameObject>();     // 생성된 아이템들

    private bool isCheckingCondition = false;   // 조건 체크 중인지 여부
    private DiceRoll[] diceRolls;               // 주사위 배열

    // 초기화
    void Start()
    {
        SetRandomCondition();

        if (globalSpeedUpButton != null)
        {
            globalSpeedUpButton.onClick.AddListener(ToggleGlobalSpeedUp);
        }

        RestoreItems();

        diceRolls = FindObjectsOfType<DiceRoll>();
        textManager = FindObjectOfType<TextManager>();
        scoreManager = FindObjectOfType<Score>();

        if (textManager != null)
        {
            textManager.OnRollComplete += OnDiceRollComplete;
        }
    }

    // 매 프레임 업데이트
    private void Update()
    {
        if (scoreTexts.Length == 0) return;

        bool conditionMet = CheckCondition();

        if (conditionMet && !conditionMetPreviously)
        {
            SpawnItems();
            SetRandomCondition();
        }

        conditionMetPreviously = conditionMet;
    }

    // 아이템 생성
    void SpawnItems()
    {
        totalConditionMetCount++;
        PedigreeDataManager.Instance.savedTotalConditionMetCount = totalConditionMetCount;
        textManager.IncrementCurrentScore();

        foreach (var dice in diceRolls)
        {
            if (dice != null)
            {
                dice.Initialize();
            }
        }

        if (scoreManager != null)
        {
            scoreManager.ResetAllDice();
        }

        string rewardType = DetermineRewardType();

        if (rewardType == "Roll")
        {
            goodsManager.RewardPlayer(rewardType);
            return;
        }

        if (currentSpawnIndex < contentParents.Length && miningPrefabs[currentSpawnIndex] != null)
        {
            float rewardAmount = CalculateRewardAmount(rewardType);
            float baseFillDuration = CalculateBaseFillDuration(rewardType, rewardAmount);

            GameObject spawnedItem = Instantiate(miningPrefabs[currentSpawnIndex], contentParents[currentSpawnIndex].transform);
            lastSpawnedItem = spawnedItem;

            var itemData = new PedigreeDataManager.ItemData
            {
                parentIndex = currentSpawnIndex,
                floorNumber = $"{totalConditionMetCount}F",
                rewardType = rewardType,
                progress = 0f,
                baseSpeed = 1f / baseFillDuration,
                speedMultiplier = 1f,
                rewardAmount = rewardAmount
            };

            int itemIndex = PedigreeDataManager.Instance.savedItems.Count;
            PedigreeDataManager.Instance.savedItems.Add(itemData);

            SpeedUpCtrl speedController = spawnedItem.AddComponent<SpeedUpCtrl>();
            speedController.Initialize(rewardType, baseFillDuration, rewardAmount, textManager, itemIndex);

            TextMeshProUGUI textComponent = spawnedItem.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"{totalConditionMetCount}F";
            }
        }

        currentSpawnIndex = (currentSpawnIndex + 1) % contentParents.Length;
    }

    // 아이템 복원
    private void RestoreItems()
    {
        totalConditionMetCount = PedigreeDataManager.Instance.savedTotalConditionMetCount;

        for (int i = 0; i < PedigreeDataManager.Instance.savedItems.Count; i++)
        {
            var itemData = PedigreeDataManager.Instance.savedItems[i];
            if (itemData.parentIndex < contentParents.Length && miningPrefabs[itemData.parentIndex] != null)
            {
                GameObject spawnedItem = Instantiate(miningPrefabs[itemData.parentIndex], contentParents[itemData.parentIndex].transform);

                SpeedUpCtrl speedController = spawnedItem.AddComponent<SpeedUpCtrl>();
                float baseFillDuration = 1f / itemData.baseSpeed;

                speedController.InitializeWithoutStarting(
                    itemData.rewardType,
                    baseFillDuration,
                    itemData.rewardAmount,
                    textManager,
                    i
                );

                MiningManager.Instance.RestoreMiningProcess(speedController, itemData.progress);

                TextMeshProUGUI textComponent = spawnedItem.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = itemData.floorNumber;
                }
            }
        }

        currentSpawnIndex = PedigreeDataManager.Instance.savedItems.Count % contentParents.Length;
    }

    // 조건 체크
    private IEnumerator CheckConditionAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        while (diceRolls.Any(d => d.isRolling))
        {
            yield return null;
        }

        if (!isCheckingCondition)
        {
            isCheckingCondition = true;
            bool conditionMet = CheckCondition();

            if (conditionMet && !conditionMetPreviously)
            {
                SpawnItems();
                SetRandomCondition();
            }

            conditionMetPreviously = conditionMet;
            isCheckingCondition = false;
        }
    }

    // 보상 타입 결정
    private string DetermineRewardType()
    {
        if (totalConditionMetCount % 4 == 0)
            return "Roll";
        else if (totalConditionMetCount % 4 == 1)
            return "Coin";
        else if (totalConditionMetCount % 4 == 2)
            return "Diamond";
        else
            return "Clover";
    }

    // 보상량 계산
    private float CalculateRewardAmount(string rewardType)
    {
        return rewardType switch
        {
            "Coin" => 500f,
            "Clover" => 200f,
            "Diamond" => 20f,
            _ => 0f
        };
    }

    // 기본 채우기 시간 계산
    private float CalculateBaseFillDuration(string rewardType, float rewardAmount)
    {
        return rewardType switch
        {
            "Coin" => (rewardAmount / 5f) * 3f,
            "Clover" => rewardAmount / 1f,
            "Diamond" => rewardAmount * 10f,
            _ => 5f
        };
    }

    // 속도 증가 토글
    public void ToggleGlobalSpeedUp()
    {
        isGlobalSpeedUp = !isGlobalSpeedUp;
        float multiplier = isGlobalSpeedUp ? 1.25f : 1f;
        SpeedUpCtrl.SetGlobalSpeedMultiplier(multiplier);

        if (globalSpeedUpButton != null)
        {
            globalSpeedUpButton.GetComponent<Image>().color =
                isGlobalSpeedUp ? Color.yellow : Color.white;
        }
    }

    public GameObject GetLastSpawnedItem()
    {
        return lastSpawnedItem;
    }

    private void SetRandomCondition()
    {
        currentCondition = conditions[Random.Range(0, conditions.Length)];
        pedigreeText.text = currentCondition;
        pedigreeText2.text = currentCondition;
    }

    private void OnDestroy()
    {
        if (textManager != null)
        {
            textManager.OnRollComplete -= OnDiceRollComplete;
        }
    }

    private void OnDiceRollComplete()
    {
        StartCoroutine(CheckConditionAfterDelay());
    }


    public bool CheckCondition()
    {
        if (scoreManager == null) return false;

        int[] diceScores = scoreManager.GetDiceScores();
        if (diceScores == null || diceScores.Length == 0) return false;

        HashSet<int> uniqueScores = new HashSet<int>(diceScores.Where(score => score != 0));
        Dictionary<int, int> scoreCount = new Dictionary<int, int>();

        foreach (int score in diceScores)
        {
            if (score != 0)  // 0은 아직 굴리지 않은 주사위
            {
                if (scoreCount.ContainsKey(score))
                    scoreCount[score]++;
                else
                    scoreCount[score] = 1;
            }
        }

        if (uniqueScores.Count == 0 || scoreCount.Values.Sum() != diceScores.Length)
        {
            return false;
        }

        switch (currentCondition)
        {
            case "더블":
                return scoreCount.ContainsValue(2);

            case "오버":
                return uniqueScores.Count == diceRolls.Length;

            case "트리플":
                return scoreCount.ContainsValue(3);

            case "쿼드라플":
                return scoreCount.ContainsValue(4);

            case "제곱":
                int pairs = 0;
                foreach (var count in scoreCount.Values)
                {
                    if (count == 2) pairs++;
                }
                return pairs == 2;

            case "풀하우스":
                return scoreCount.ContainsValue(2) && scoreCount.ContainsValue(3);

            case "야추":
                return scoreCount.ContainsValue(diceRolls.Length);

            case "이븐":
                return uniqueScores.All(num => num % 2 == 0);

            case "오드":
                return uniqueScores.All(num => num % 2 != 0);

            case "16":
                return uniqueScores.All(num => num == 1 || num == 6);

            case "스몰":
                return uniqueScores.SetEquals(new HashSet<int> { 1, 2, 3, 4, 5 });

            case "라지":
                return uniqueScores.SetEquals(new HashSet<int> { 2, 3, 4, 5, 6 });

            default:
                return false;
        }
    }
}
