using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    private string[] conditions =
    {
        "더블", "오버", "트리플", "쿼드라플", "제곱", "풀하우스", "야추", "이븐", "오드", "16", "스몰", "라지"
    };

    private string currentCondition;
    private bool conditionMetPreviously = false;
    private int currentSpawnIndex = 0;

    private bool isRewardGiven = false;

    private void Start()
    {
        SetRandomCondition();
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

    void SpawnItems()
    {
        totalConditionMetCount++;

        string rewardType;
        if (totalConditionMetCount % 4 == 0)
            rewardType = "Roll";
        else if (totalConditionMetCount % 4 == 1)
            rewardType = "Coin";
        else if (totalConditionMetCount % 4 == 2)
            rewardType = "Diamond";
        else
            rewardType = "Clover";

        if (rewardType == "Roll")
        {
            goodsManager.RewardPlayer(rewardType);
            return;
        }

        if (currentSpawnIndex < contentParents.Length && miningPrefabs[currentSpawnIndex] != null)
        {
            GameObject spawnedItem = Instantiate(miningPrefabs[currentSpawnIndex], contentParents[currentSpawnIndex].transform);
            lastSpawnedItem = spawnedItem;

            Slider slider = spawnedItem.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                float rewardAmount = rewardType switch
                {
                    "Coin" => 500f,
                    "Clover" => 200f,
                    "Diamond" => 20f,
                    _ => 0f
                };

                float fillDuration = rewardType switch
                {
                    "Coin" => (rewardAmount / 5f) * 3f,
                    "Clover" => rewardAmount / 1f,
                    "Diamond" => rewardAmount * 10f,
                    _ => 5f
                };

                StartCoroutine(MonitorSliderWithAnimation(slider, rewardType, fillDuration, rewardAmount));
            }

            TextMeshProUGUI textComponent = spawnedItem.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"{totalConditionMetCount}F";
            }
        }

        currentSpawnIndex = (currentSpawnIndex + 1) % contentParents.Length;
    }

    IEnumerator MonitorSliderWithAnimation(Slider slider, string rewardType, float fillDuration, float rewardAmount)
    {
        float elapsedTime = 0f;

        while (true)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(0f, 1f, elapsedTime / fillDuration);

            if (slider.value >= 1f)
            {
                switch (rewardType)
                {
                    case "Coin":
                        textManager.UpdateCoinText((int)rewardAmount);
                        break;
                    case "Clover":
                        textManager.UpdateCloverText((int)rewardAmount);
                        break;
                    case "Diamond":
                        textManager.UpdateDiamondText((int)rewardAmount);
                        break;
                }

                slider.value = 0f;
                elapsedTime = 0f;

                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }

    private void SetRandomCondition()
    {
        currentCondition = conditions[Random.Range(0, conditions.Length)];
        pedigreeText.text = currentCondition;
        pedigreeText2.text = currentCondition;
    }

    public bool CheckCondition()
    {
        HashSet<string> uniqueScores = new HashSet<string>();
        Dictionary<string, int> scoreCount = new Dictionary<string, int>();

        foreach (var scoreText in scoreTexts)
        {
            if (scoreText != null && !string.IsNullOrEmpty(scoreText.text))
            {
                string score = scoreText.text;
                uniqueScores.Add(score);

                if (scoreCount.ContainsKey(score))
                    scoreCount[score]++;
                else
                    scoreCount[score] = 1;
            }
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
