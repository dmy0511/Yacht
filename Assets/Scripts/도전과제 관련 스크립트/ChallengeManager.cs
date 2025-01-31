using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ChallengeManager : MonoBehaviour
{
    // �̱������� ����
    public static ChallengeManager _Instance { get; private set; }

    [Header("�������� UI ����")]
    public GameObject ChallengePrefab;          // �������� ������Ʈ ������
    public Transform ChallengeContainer;        // ��ũ�Ѻ��� Content

    private List<Challenge> ChallengeList = new List<Challenge>();
    private List<ChallengeUI> ChallengeUIList = new List<ChallengeUI>();

    public Sprite coinRewardSprite;         // ��� ���� �̹���
    public Sprite gemRewardSprite;          // ���̾� ���� �̹���
    public Sprite diceRewardSprite;         // �� Ƚ�� ���� ���� �̹���
    public Sprite emeraldRewardSprite;       // ���޶��� ���� �̹���

    // �������� ���� �̺�Ʈ
    public event Action<string, int> OnChallengeProgressUpdated;

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(gameObject); ;
        }
    }

    private void Start()
    {
        LoadChallenges();
        if (ChallengeList.Count == 0) // ����� �����Ͱ� ���� ��츸 �ʱ�ȭ
        {
            InitializeChallenges();
        }
        CreateUI();

        // ���� �� �ý��۰� ����
        if (TextManager.Instance != null)
        {
            TextManager.Instance.OnRollComplete += () => UpdateChallengeProgress("�ֻ��� ������ 10ȸ �޼�", 1);
        }
        if (Score.Instance != null)
        {
            Score.Instance.OnScoreUpdated += (score) => UpdateChallengeProgress("���� 5ȸ �޼�", 1);
        }
    }
    // �������� ��� �ʱ�ȭ
    private void InitializeChallenges()
    {

        ChallengeList.Add(new Challenge("�ֻ��� ������ 10ȸ �޼�", 10, 1000, coinRewardSprite, false,
            goal => goal + 10, reward => reward + 100));

        ChallengeList.Add(new Challenge("���� 5ȸ �޼�", 5, 50, gemRewardSprite, false,
            goal => goal + 5, reward => reward + 10));

        ChallengeList.Add(new Challenge("��� 10000��� �޼�", 10000, 3, diceRewardSprite, false,
            goal => goal + 10000, reward => reward));

        ChallengeList.Add(new Challenge("���޶��� 10000�� �޼�", 10000, 200, emeraldRewardSprite, false,
            goal => goal + 10000, reward => reward + 50));

        ChallengeList.Add(new Challenge("���̾� 100�� �޼�", 100, 1000, coinRewardSprite, false,
            goal => goal + 500, reward => reward + 500));

        ChallengeList.Add(new Challenge("Ÿ�� 100�� �޼�", 100, 10, gemRewardSprite, false,
            goal => goal + 100, reward => reward + 10));

        ChallengeList.Add(new Challenge("��Ų 10�� ����", 10, 100, gemRewardSprite, false,
            goal => goal + 5, reward => reward + 50));

        ChallengeList.Add(new Challenge("��� ä���ӵ� max(50��) �޼�", 50, 50000, coinRewardSprite, true,
            null, null));

        ChallengeList.Add(new Challenge("�ֻ��� �� Ƚ�� ���� ��� max(15��) �޼�", 15, 30, gemRewardSprite, true,
            null, null));
    }

    // UI ����
    private void CreateUI()
    {
        foreach (var challenge in ChallengeList)
        {
            GameObject challengeObj = Instantiate(ChallengePrefab, ChallengeContainer);
            ChallengeUI challengeUI = challengeObj.GetComponent<ChallengeUI>();
            challengeUI.Initialize(challenge);
            ChallengeUIList.Add(challengeUI);
        }
    }

    // UI ��ü ������Ʈ
    private void UpdateUI()
    {
        foreach (var ui in ChallengeUIList)
        {
            ui.UpdateUI();
        }
    }

    // �������� ���൵ ������Ʈ
    public void UpdateChallengeProgress(string challengeName, int amount)
    {
        foreach (var challenge in ChallengeList)
        {
            if (challenge.challengeName == challengeName)
            {
                challenge.UpdateProgress(amount);
                SaveChallenges();
                UpdateUI();
                OnChallengeProgressUpdated?.Invoke(challengeName, challenge.currentProgress);
                return;
            }
        }
    }

    // �������� ���� ���� ��� �߰�
    public void ClaimReward(string challengeName)
    {
        foreach (var challenge in ChallengeList)
        {
            if (challenge.challengeName == challengeName && challenge.isCompleted)
            {
                // ���� ���� ����
                ApplyReward(challenge.rewardAmount, challenge.rewardIcon);

                if (!challenge.isOneTimeChallenge)
                {
                    // ���� ��ǥ�� �����ϰ� ��ǥ ����
                    int previousGoal = challenge.goal;
                    challenge.IncreaseGoalAndReward();


                    // ��ǥ ���൵ ����
                    challenge.currentProgress = Mathf.Max(challenge.currentProgress, previousGoal);

                    // ���� ������Ʈ (��Ȯ�� ���ڸ� ����)
                    challenge.challengeName = UpdateChallengeTitle(challenge.challengeName, previousGoal, challenge.goal);
                }
                else
                {
                    // 1ȸ�� ���������� �Ϸ� ���� ����
                    challenge.isCompleted = true;
                }

                SaveChallenges();
                UpdateUI();
                return;
            }
        }
    }

    // ���� ������ �����ϴ� �Լ�
    private void ApplyReward(int rewardAmount, Sprite rewardIcon)
    {
        if (TextManager.Instance == null) return;

        if (rewardIcon == coinRewardSprite)
        {
            TextManager.Instance.currentCoin += rewardAmount;
            TextManager.Instance.UpdateCoinText(0);
        }
        else if (rewardIcon == gemRewardSprite)
        {
            TextManager.Instance.currentDiamond += rewardAmount;
            TextManager.Instance.UpdateDiamondText(0);
        }
        else if (rewardIcon == diceRewardSprite)
        {
            TextManager.Instance.rollCount += rewardAmount;
            TextManager.Instance.UpdateRollText();
        }
        else if (rewardIcon == emeraldRewardSprite)
        {
            TextManager.Instance.currentClover += rewardAmount;
            TextManager.Instance.UpdateCloverText(0);
        }

        // ����� �� ����
        TextManager.Instance.SaveGameData();
    }

    // �������� ������ ����
    private void SaveChallenges()
    {
        for (int i = 0; i < ChallengeList.Count; i++)
        {
            PlayerPrefs.SetInt($"Challenge_{i}_Progress", ChallengeList[i].currentProgress);
            PlayerPrefs.SetInt($"Challenge_{i}_Completed", ChallengeList[i].isCompleted ? 1 : 0);
            PlayerPrefs.SetInt($"Challenge_{i}_Goal", ChallengeList[i].goal);
            PlayerPrefs.SetString($"Challenge_{i}_Title", ChallengeList[i].challengeName);
        }
        PlayerPrefs.Save();
    }

    // �������� ������ �ҷ�����
    private void LoadChallenges()
    {
        if (ChallengeList.Count == 0) InitializeChallenges();

        for (int i = 0; i < ChallengeList.Count; i++)
        {
            if (PlayerPrefs.HasKey($"Challenge_{i}_Progress"))
            {
                ChallengeList[i].currentProgress = PlayerPrefs.GetInt($"Challenge_{i}_Progress", 0);
                ChallengeList[i].isCompleted = PlayerPrefs.GetInt($"Challenge_{i}_Completed", 0) == 1;
            }
            if (PlayerPrefs.HasKey($"Challenge_{i}_Goal"))
            {
                ChallengeList[i].goal = PlayerPrefs.GetInt($"Challenge_{i}_Goal");
            }
            if (PlayerPrefs.HasKey($"Challenge_{i}_Title"))
            {
                ChallengeList[i].challengeName = PlayerPrefs.GetString($"Challenge_{i}_Title");
            }
        }
        UpdateUI();
    }

    // �Ϸ�� �������� ó��
    public void ResetCompletedChallenges()
    {
        for (int i = ChallengeList.Count - 1; i >= 0; i--)
        {
            if (ChallengeList[i].isCompleted)
            {
                if (ChallengeList[i].isOneTimeChallenge)
                {
                    // �������� �ʰ� �Ϸ� ���¸� ����
                    ChallengeList[i].isCompleted = true;
                }
                else
                {
                    ChallengeList[i].IncreaseGoalAndReward(); // ��ǥ ����
                }
            }
        }
        SaveChallenges();
        UpdateUI();
    }

    // ���� ��ǥ ���ڸ� ���ο� ��ǥ ���ڷ� �����ϴ� �Լ�
    private string UpdateChallengeTitle(string challengeName, int oldGoal, int newGoal)
    {
        return Regex.Replace(challengeName, @"\d+", newGoal.ToString());
    }
    public void ResetChallenges()
    {
        PlayerPrefs.DeleteKey("ChallengeData"); // ���� �������� ������ ����
        PlayerPrefs.Save();

        ChallengeList.Clear(); // ����Ʈ ����
        InitializeChallenges(); // �������� �缳��
        SaveChallenges(); // �ٽ� ����
        UpdateUI(); // UI ������Ʈ

        Debug.Log("�������� �����Ͱ� �ʱ�ȭ�Ǿ����ϴ�!");
    }

}
