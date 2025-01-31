using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ���� �ý��� ������ �����ϴ� Ŭ����
public class PedigreeManager : MonoBehaviour
{
    public static PedigreeDataManager Instance { get; private set; }
    // UI �ؽ�Ʈ ��ҵ�
    [SerializeField] private TextMeshProUGUI[] scoreTexts;      // ���� �ؽ�Ʈ��
    [SerializeField] private TextMeshProUGUI pedigreeText;      // ���� ǥ�� �ؽ�Ʈ 1
    [SerializeField] private TextMeshProUGUI pedigreeText2;     // ���� ǥ�� �ؽ�Ʈ 2

    // �����հ� �θ� ������Ʈ
    [SerializeField] private GameObject[] contentParents;       // ������ �θ� ������Ʈ��
    [SerializeField] private GameObject[] miningPrefabs;        // ä�� �����յ�

    // �Ŵ��� ����
    [SerializeField] private TextManager textManager;           // �ؽ�Ʈ �Ŵ���
    [SerializeField] private GoodsManager goodsManager;         // ��ȭ �Ŵ���
    [SerializeField] private Score scoreManager;                // ���� �Ŵ���

    private GameObject lastSpawnedItem;                         // ������ ������ ������
    private int totalConditionMetCount = 0;                     // �޼��� �� ���� ��

    // ���� �ӵ� ���� ����
    [SerializeField] private Button globalSpeedUpButton;         // ��ü �ӵ� ���� ��ư
    private bool isGlobalSpeedUp = false;                        // ��ü �ӵ� ���� ����

    // ���� ����
    private string[] conditions = {                              // ������ ���ǵ�
       "����", "����", "Ʈ����", "�������", "����", "Ǯ�Ͽ콺",
       "����", "�̺�", "����", "16", "����", "����"
   };

    private string currentCondition;                             // ���� ����
    private bool conditionMetPreviously = false;                 // ���� ���� �޼� ����
    private int currentSpawnIndex = 0;                           // ���� ���� �ε���

    private List<GameObject> spawnedItems = new List<GameObject>();     // ������ �����۵�

    private bool isCheckingCondition = false;   // ���� üũ ������ ����
    private DiceRoll[] diceRolls;               // �ֻ��� �迭

    // �ʱ�ȭ
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

    // �� ������ ������Ʈ
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

    // ������ ����
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

    // ������ ����
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

    // ���� üũ
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

    // ���� Ÿ�� ����
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

    // ���� ���
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

    // �⺻ ä��� �ð� ���
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

    // �ӵ� ���� ���
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
            if (score != 0)  // 0�� ���� ������ ���� �ֻ���
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

        bool conditionMet = false;

        switch (currentCondition)
        {
            case "����":
                conditionMet = scoreCount.Values.Count(v => v == 2) == 1;
                break;

            case "����":
                conditionMet = uniqueScores.Count == diceRolls.Length;
                break;

            case "Ʈ����":
                conditionMet = scoreCount.ContainsValue(3);
                break;

            case "�������":
                conditionMet = scoreCount.ContainsValue(4);
                break;

            case "����":
                conditionMet = scoreCount.Values.Count(v => v == 2) == 2;
                break;

            case "Ǯ�Ͽ콺":
                conditionMet = scoreCount.ContainsValue(2) && scoreCount.ContainsValue(3);
                break;

            case "����":
                conditionMet = scoreCount.ContainsValue(diceRolls.Length);
                break;

            case "�̺�":
                conditionMet = uniqueScores.All(num => num % 2 == 0);
                break;

            case "����":
                conditionMet = uniqueScores.All(num => num % 2 != 0);
                break;

            case "16":
                conditionMet = uniqueScores.All(num => num == 1 || num == 6);
                break;

            case "����":
                conditionMet = uniqueScores.SetEquals(new HashSet<int> { 1, 2, 3, 4, 5 });
                break;

            case "����":
                conditionMet = uniqueScores.SetEquals(new HashSet<int> { 2, 3, 4, 5, 6 });
                break;

            default:
                conditionMet = false;
                break;
        }

        if (conditionMet)
        {
            scoreManager.ResetRollCount();
        }
        return conditionMet;
    }
}
