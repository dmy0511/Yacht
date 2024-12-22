using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("Level Texts")]
    [SerializeField] private TextMeshProUGUI coinMiningLevelText;
    [SerializeField] private TextMeshProUGUI cloverMiningLevelText;
    [SerializeField] private TextMeshProUGUI diamondMiningLevelText;
    [SerializeField] private TextMeshProUGUI rollCountDefenseLevelText;
    [SerializeField] private TextMeshProUGUI showDicePointText;

    [Header("Button Components")]
    [SerializeField] private Button coinMiningButton;
    [SerializeField] private Button cloverMiningButton;
    [SerializeField] private Button diamondMiningButton;
    [SerializeField] private Button rollCountDefenseButton;
    [SerializeField] private Button showDiceButton;

    [Header("Show Dice Button Sprites")]
    [SerializeField] private Sprite showDiceNormalSprite;
    [SerializeField] private Sprite showDiceActivatedSprite;

    [Header("References")]
    [SerializeField] private CashData cashData;

    [Serializable]
    public class ProbabilityRow
    {
        public GameObject[] slots = new GameObject[6];
        public TextMeshProUGUI rowText;
        public Button plusButton;
        public Button minusButton;
        public int activeSlots;
    }

    [Header("Dice Probability UI")]
    [SerializeField] private ProbabilityRow[] probabilityRows = new ProbabilityRow[6];

    private GameObject GetSlot(int row, int slot)
    {
        return probabilityRows[row].slots[slot];
    }

    private const int MINING_COST = 500;
    private const int SHOW_DICE_COST = 200;
    private const int DEFENSE_COST = 100;
    private const int MAX_SHOW_DICE_POINT = 21;

    private const int MAX_MINING_LEVEL = 50;
    private const float COIN_TIME_REDUCTION = 5f;
    private const float CLOVER_TIME_REDUCTION = 3f;
    private const float DIAMOND_TIME_REDUCTION = 2f;

    private const int MAX_DEFENSE_LEVEL = 10;
    private const float DEFENSE_RATE_PER_LEVEL = 5f;

    private const string ACTIVE_ROW_KEY = "ActiveProbabilityRow";
    private const string ACTIVE_SLOT_KEY = "ActiveProbabilitySlot";
    private int activeRow = 0;
    private int activeSlot = 0;

    private int currentShowDicePoint;

    private void Start()
    {
        LoadUpgradeLevels();
        UpdateShowDiceButtonState();

        currentShowDicePoint = PlayerPrefs.GetInt("ShowDicePoint", 0);

        LoadSavedState();
        SetupButtons();
        UpdateUI();
    }

    public void CoinMiningUp()
    {
        int currentLevel = PlayerPrefs.GetInt("CoinMiningLevel", 1);
        if (currentLevel >= MAX_MINING_LEVEL) return;

        if (cashData.TryUpgrade(MINING_COST, 0, 0))
        {
            currentLevel++;
            PlayerPrefs.SetInt("CoinMiningLevel", currentLevel);
            PlayerPrefs.Save();

            UpdateLevelText(coinMiningLevelText, currentLevel);

            if (currentLevel >= MAX_MINING_LEVEL)
            {
                coinMiningButton.interactable = false;
            }
        }
    }

    public void CloverMiningUp()
    {
        int currentLevel = PlayerPrefs.GetInt("CloverMiningLevel", 1);
        if (currentLevel >= MAX_MINING_LEVEL) return;

        if (cashData.TryUpgrade(MINING_COST, 0, 0))
        {
            currentLevel++;
            PlayerPrefs.SetInt("CloverMiningLevel", currentLevel);
            PlayerPrefs.Save();

            UpdateLevelText(cloverMiningLevelText, currentLevel);

            if (currentLevel >= MAX_MINING_LEVEL)
            {
                cloverMiningButton.interactable = false;
            }
        }
    }

    public void DiamondMiningUp()
    {
        int currentLevel = PlayerPrefs.GetInt("DiamondMiningLevel", 1);
        if (currentLevel >= MAX_MINING_LEVEL) return;

        if (cashData.TryUpgrade(MINING_COST, 0, 0))
        {
            currentLevel++;
            PlayerPrefs.SetInt("DiamondMiningLevel", currentLevel);
            PlayerPrefs.Save();

            UpdateLevelText(diamondMiningLevelText, currentLevel);

            if (currentLevel >= MAX_MINING_LEVEL)
            {
                diamondMiningButton.interactable = false;
            }
        }
    }

    public void ShowDice()
    {
        int currentPoint = PlayerPrefs.GetInt("ShowDicePoint", 0);
        if (currentPoint >= MAX_SHOW_DICE_POINT)
        {
            return;
        }

        if (cashData.TryUpgrade(0, 0, SHOW_DICE_COST))
        {
            int currentLevel = PlayerPrefs.GetInt("ShowDiceLevel", 1);
            currentLevel++;
            PlayerPrefs.SetInt("ShowDiceLevel", currentLevel);

            currentPoint++;
            PlayerPrefs.SetInt("ShowDicePoint", currentPoint);

            PlayerPrefs.Save();

            showDicePointText.text = $"보유 포인트 : {currentPoint}";

            if (currentPoint >= MAX_SHOW_DICE_POINT)
            {
                showDiceButton.image.sprite = showDiceActivatedSprite;
                showDiceButton.interactable = false;
            }

            UpgradeManager upgradeManager = FindObjectOfType<UpgradeManager>();
            if (upgradeManager != null)
            {
                upgradeManager.OnShowDicePointChanged();
            }
        }
    }

    public void RollCountDefense()
    {
        int currentLevel = PlayerPrefs.GetInt("RollDefenseLevel", 1);
        if (currentLevel >= MAX_DEFENSE_LEVEL) return;

        if (cashData.TryUpgrade(0, DEFENSE_COST, 0))
        {
            currentLevel++;
            PlayerPrefs.SetInt("RollDefenseLevel", currentLevel);
            PlayerPrefs.Save();

            UpdateLevelText(rollCountDefenseLevelText, currentLevel);

            if (currentLevel >= MAX_DEFENSE_LEVEL)
            {
                rollCountDefenseButton.interactable = false;
            }
        }
    }

    private void UpdateShowDiceButtonState()
    {
        int showDicePoint = PlayerPrefs.GetInt("ShowDicePoint", 0);
        if (showDicePoint >= MAX_SHOW_DICE_POINT)
        {
            showDiceButton.image.sprite = showDiceActivatedSprite;
            showDiceButton.interactable = false;
        }
        else
        {
            showDiceButton.image.sprite = showDiceNormalSprite;
            showDiceButton.interactable = true;
        }
    }

    private void UpdateLevelText(TextMeshProUGUI textComponent, int level)
    {
        if (textComponent != null)
        {
            textComponent.text = $"Lv.{level}";
        }
    }

    private void LoadUpgradeLevels()
    {
        UpdateLevelText(coinMiningLevelText, PlayerPrefs.GetInt("CoinMiningLevel", 1));
        UpdateLevelText(cloverMiningLevelText, PlayerPrefs.GetInt("CloverMiningLevel", 1));
        UpdateLevelText(diamondMiningLevelText, PlayerPrefs.GetInt("DiamondMiningLevel", 1));
        UpdateLevelText(rollCountDefenseLevelText, PlayerPrefs.GetInt("RollDefenseLevel", 1));

        int showDicePoint = PlayerPrefs.GetInt("ShowDicePoint", 0);
        if (showDicePointText != null)
        {
            showDicePointText.text = $"보유 포인트 : {showDicePoint}";
        }

        UpdateMiningButtonStates();
    }

    private void UpdateMiningButtonStates()
    {
        int coinLevel = PlayerPrefs.GetInt("CoinMiningLevel", 1);
        int cloverLevel = PlayerPrefs.GetInt("CloverMiningLevel", 1);
        int diamondLevel = PlayerPrefs.GetInt("DiamondMiningLevel", 1);

        coinMiningButton.interactable = coinLevel < MAX_MINING_LEVEL;
        cloverMiningButton.interactable = cloverLevel < MAX_MINING_LEVEL;
        diamondMiningButton.interactable = diamondLevel < MAX_MINING_LEVEL;

        int defenseLevel = PlayerPrefs.GetInt("RollDefenseLevel", 1);
        rollCountDefenseButton.interactable = defenseLevel < MAX_DEFENSE_LEVEL;
    }

    public float GetMiningTimeReduction(string type)
    {
        switch (type)
        {
            case "Coin":
                int coinLevel = PlayerPrefs.GetInt("CoinMiningLevel", 1) - 1;
                return coinLevel * COIN_TIME_REDUCTION;
            case "Clover":
                int cloverLevel = PlayerPrefs.GetInt("CloverMiningLevel", 1) - 1;
                return cloverLevel * CLOVER_TIME_REDUCTION;
            case "Diamond":
                int diamondLevel = PlayerPrefs.GetInt("DiamondMiningLevel", 1) - 1;
                return diamondLevel * DIAMOND_TIME_REDUCTION;
            default:
                return 0f;
        }
    }

    public float GetRollDefenseRate()
    {
        int level = PlayerPrefs.GetInt("RollDefenseLevel", 1) - 1;
        return level * DEFENSE_RATE_PER_LEVEL;
    }

    private void LoadSavedState()
    {
        currentShowDicePoint = PlayerPrefs.GetInt("ShowDicePoint", 0);

        for (int i = 0; i < probabilityRows.Length; i++)
        {
            probabilityRows[i].activeSlots = PlayerPrefs.GetInt($"ProbabilityRow_{i}", 0);
        }
    }

    private void SetupButtons()
    {
        for (int i = 0; i < probabilityRows.Length; i++)
        {
            int rowIndex = i;
            if (probabilityRows[i].plusButton != null)
            {
                probabilityRows[i].plusButton.onClick.AddListener(() => IncreaseProbability(rowIndex));
            }
            if (probabilityRows[i].minusButton != null)
            {
                probabilityRows[i].minusButton.onClick.AddListener(() => DecreaseProbability(rowIndex));
            }
        }
    }

    private void UpdateUI()
    {
        for (int row = 0; row < probabilityRows.Length; row++)
        {
            var currentRow = probabilityRows[row];

            for (int slot = 0; slot < 6; slot++)
            {
                if (currentRow.slots[slot] != null)
                {
                    currentRow.slots[slot].SetActive(slot < currentRow.activeSlots);
                }
            }

            if (currentRow.rowText != null)
            {
                currentRow.rowText.text = $"{currentRow.activeSlots} / 6";
            }

            UpdateRowButtonStates(row);
        }
    }

    private void UpdateRowButtonStates(int row)
    {
        var currentRow = probabilityRows[row];
        bool hasPoints = currentShowDicePoint > 0;

        if (currentRow.plusButton != null)
        {
            currentRow.plusButton.interactable = hasPoints && currentRow.activeSlots < 6;
        }

        if (currentRow.minusButton != null)
        {
            currentRow.minusButton.interactable = hasPoints && currentRow.activeSlots > 0;
        }
    }

    public void IncreaseProbability(int row)
    {
        if (currentShowDicePoint <= 0 || row < 0 || row >= probabilityRows.Length) return;
        if (probabilityRows[row].activeSlots >= 6) return;

        probabilityRows[row].activeSlots++;
        SaveRowState(row);
        UpdateUI();
    }

    public void DecreaseProbability(int row)
    {
        if (currentShowDicePoint <= 0 || row < 0 || row >= probabilityRows.Length) return;
        if (probabilityRows[row].activeSlots <= 0) return;

        probabilityRows[row].activeSlots--;
        SaveRowState(row);
        UpdateUI();
    }

    private void SaveRowState(int row)
    {
        PlayerPrefs.SetInt($"ProbabilityRow_{row}", probabilityRows[row].activeSlots);
        PlayerPrefs.Save();
    }

    public void OnShowDicePointChanged()
    {
        currentShowDicePoint = PlayerPrefs.GetInt("ShowDicePoint", 0);
        UpdateUI();
    }

    private void LoadActiveProbabilityStatus()
    {
        activeRow = PlayerPrefs.GetInt(ACTIVE_ROW_KEY, 0);
        activeSlot = PlayerPrefs.GetInt(ACTIVE_SLOT_KEY, 0);
    }

    private void UpdateProbabilitySlots()
    {
        for (int row = 0; row < 6; row++)
        {
            for (int slot = 0; slot < 6; slot++)
            {
                GameObject slotObject = GetSlot(row, slot);
                if (slotObject != null)
                {
                    bool shouldBeActive = false;

                    if (row < activeRow)
                    {
                        shouldBeActive = true;
                    }
                    else if (row == activeRow)
                    {
                        shouldBeActive = (slot < activeSlot);
                    }

                    slotObject.SetActive(shouldBeActive);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetUpgrades();
        }
    }

    private void ResetUpgrades()
    {
        PlayerPrefs.SetInt("CoinMiningLevel", 1);
        PlayerPrefs.SetInt("CloverMiningLevel", 1);
        PlayerPrefs.SetInt("DiamondMiningLevel", 1);
        PlayerPrefs.SetInt("RollDefenseLevel", 1);
        PlayerPrefs.SetInt("ShowDiceLevel", 1);
        PlayerPrefs.SetInt("ShowDicePoint", 0);

        activeRow = 0;
        activeSlot = 0;
        PlayerPrefs.SetInt(ACTIVE_ROW_KEY, 0);
        PlayerPrefs.SetInt(ACTIVE_SLOT_KEY, 0);
        UpdateProbabilitySlots();

        for (int i = 0; i < probabilityRows.Length; i++)
        {
            probabilityRows[i].activeSlots = 0;
            PlayerPrefs.SetInt($"ProbabilityRow_{i}", 0);
        }
        PlayerPrefs.Save();
        UpdateUI();

        LoadUpgradeLevels();
        UpdateShowDiceButtonState();
    }

    public (int row, int slot) GetActiveProbabilityStatus()
    {
        return (activeRow, activeSlot);
    }
}
