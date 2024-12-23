using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PedigreeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private TextMeshProUGUI pedigreeText;
    [SerializeField] private TextMeshProUGUI pedigreeText2;

    [SerializeField] private GameObject[] contentParents;
    [SerializeField] private GameObject[] miningPrefabs;

    [SerializeField] private TextManager textManager;
    [SerializeField] private GoodsManager goodsManager;

    private GameObject lastSpawnedItem;
    private int totalConditionMetCount = 0;

    [SerializeField] private Button globalSpeedUpButton;
    private bool isGlobalSpeedUp = false;

    private string[] conditions =
    {
        "더블", "오버", "트리플", "쿼드라플", "제곱", "풀하우스", "야추", "이븐", "오드", "16", "스몰", "라지"
    };

    private string currentCondition;
    private bool conditionMetPreviously = false;
    private int currentSpawnIndex = 0;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private bool isCheckingCondition = false;
    private DiceRoll[] diceRolls;


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

        if (textManager != null)
        {
            textManager.OnRollComplete += OnDiceRollComplete;
        }
    }

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

    public GameObject GetLastSpawnedItem()
    {
        return lastSpawnedItem;
    }

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

    void SpawnItems()
    {
        totalConditionMetCount++;
        PedigreeDataManager.Instance.savedTotalConditionMetCount = totalConditionMetCount;
        textManager.IncrementCurrentScore();

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

    public bool CheckCondition()
    {
        if (diceRolls.Any(d => d.isRolling))
        {
            return false;
        }

        HashSet<string> uniqueScores = new HashSet<string>();
        Dictionary<string, int> scoreCount = new Dictionary<string, int>();

        foreach (var scoreText in scoreTexts)
        {
            if (scoreText != null && !string.IsNullOrEmpty(scoreText.text) && scoreText.text != "?")
            {
                string score = scoreText.text;
                uniqueScores.Add(score);

                if (scoreCount.ContainsKey(score))
                    scoreCount[score]++;
                else
                    scoreCount[score] = 1;
            }
        }

        if (uniqueScores.Count == 0 || scoreCount.Values.Sum() != scoreTexts.Length)
        {
            return false;
        }

        switch (currentCondition)
        {
            case "더블":
                bool result = scoreCount.ContainsValue(2);
                return result;

            case "오버":
                return uniqueScores.Count == scoreTexts.Length;

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
                return scoreCount.ContainsValue(scoreTexts.Length);

            case "이븐":
                foreach (var score in uniqueScores)
                {
                    if (int.TryParse(score, out int num) && num % 2 != 0)
                        return false;
                }
                return true;

            case "오드":
                foreach (var score in uniqueScores)
                {
                    if (int.TryParse(score, out int num) && num % 2 == 0)
                        return false;
                }
                return true;

            case "16":
                foreach (var score in uniqueScores)
                {
                    if (score != "1" && score != "6")
                        return false;
                }
                return true;

            case "스몰":
                return uniqueScores.SetEquals(new HashSet<string> { "1", "2", "3", "4", "5" });

            case "라지":
                return uniqueScores.SetEquals(new HashSet<string> { "2", "3", "4", "5", "6" });

            default:
                return false;
        }
    }
}
