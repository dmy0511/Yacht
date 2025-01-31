using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ChallengeManager : MonoBehaviour
{
    // 싱글톤으로 생성
    public static ChallengeManager _Instance { get; private set; }

    [Header("도전과제 UI 관련")]
    public GameObject ChallengePrefab;          // 도전과제 오브젝트 프리팹
    public Transform ChallengeContainer;        // 스크롤뷰의 Content

    private List<Challenge> ChallengeList = new List<Challenge>();
    private List<ChallengeUI> ChallengeUIList = new List<ChallengeUI>();

    public Sprite coinRewardSprite;         // 골드 보상 이미지
    public Sprite gemRewardSprite;          // 다이아 보상 이미지
    public Sprite diceRewardSprite;         // 롤 횟수 증가 보상 이미지
    public Sprite emeraldRewardSprite;       // 에메랄드 보상 이미지

    // 도전과제 진행 이벤트
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
        if (ChallengeList.Count == 0) // 저장된 데이터가 없을 경우만 초기화
        {
            InitializeChallenges();
        }
        CreateUI();

        // 게임 내 시스템과 연결
        if (TextManager.Instance != null)
        {
            TextManager.Instance.OnRollComplete += () => UpdateChallengeProgress("주사위 굴리기 10회 달성", 1);
        }
        if (Score.Instance != null)
        {
            Score.Instance.OnScoreUpdated += (score) => UpdateChallengeProgress("야추 5회 달성", 1);
        }
    }
    // 도전과제 목록 초기화
    private void InitializeChallenges()
    {

        ChallengeList.Add(new Challenge("주사위 굴리기 10회 달성", 10, 1000, coinRewardSprite, false,
            goal => goal + 10, reward => reward + 100));

        ChallengeList.Add(new Challenge("야추 5회 달성", 5, 50, gemRewardSprite, false,
            goal => goal + 5, reward => reward + 10));

        ChallengeList.Add(new Challenge("골드 10000골드 달성", 10000, 3, diceRewardSprite, false,
            goal => goal + 10000, reward => reward));

        ChallengeList.Add(new Challenge("에메랄드 10000개 달성", 10000, 200, emeraldRewardSprite, false,
            goal => goal + 10000, reward => reward + 50));

        ChallengeList.Add(new Challenge("다이아 100개 달성", 100, 1000, coinRewardSprite, false,
            goal => goal + 500, reward => reward + 500));

        ChallengeList.Add(new Challenge("타워 100층 달성", 100, 10, gemRewardSprite, false,
            goal => goal + 100, reward => reward + 10));

        ChallengeList.Add(new Challenge("스킨 10개 보유", 10, 100, gemRewardSprite, false,
            goal => goal + 5, reward => reward + 50));

        ChallengeList.Add(new Challenge("모든 채굴속도 max(50렙) 달성", 50, 50000, coinRewardSprite, true,
            null, null));

        ChallengeList.Add(new Challenge("주사위 롤 횟수 차감 방어 max(15렙) 달성", 15, 30, gemRewardSprite, true,
            null, null));
    }

    // UI 생성
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

    // UI 전체 업데이트
    private void UpdateUI()
    {
        foreach (var ui in ChallengeUIList)
        {
            ui.UpdateUI();
        }
    }

    // 도전과제 진행도 업데이트
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

    // 도전과제 보상 지급 기능 추가
    public void ClaimReward(string challengeName)
    {
        foreach (var challenge in ChallengeList)
        {
            if (challenge.challengeName == challengeName && challenge.isCompleted)
            {
                // 보상 지급 로직
                ApplyReward(challenge.rewardAmount, challenge.rewardIcon);

                if (!challenge.isOneTimeChallenge)
                {
                    // 기존 목표를 저장하고 목표 증가
                    int previousGoal = challenge.goal;
                    challenge.IncreaseGoalAndReward();


                    // 목표 진행도 유지
                    challenge.currentProgress = Mathf.Max(challenge.currentProgress, previousGoal);

                    // 제목 업데이트 (정확한 숫자만 변경)
                    challenge.challengeName = UpdateChallengeTitle(challenge.challengeName, previousGoal, challenge.goal);
                }
                else
                {
                    // 1회성 도전과제는 완료 상태 유지
                    challenge.isCompleted = true;
                }

                SaveChallenges();
                UpdateUI();
                return;
            }
        }
    }

    // 실제 보상을 지급하는 함수
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

        // 변경된 값 저장
        TextManager.Instance.SaveGameData();
    }

    // 도전과제 데이터 저장
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

    // 도전과제 데이터 불러오기
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

    // 완료된 도전과제 처리
    public void ResetCompletedChallenges()
    {
        for (int i = ChallengeList.Count - 1; i >= 0; i--)
        {
            if (ChallengeList[i].isCompleted)
            {
                if (ChallengeList[i].isOneTimeChallenge)
                {
                    // 삭제하지 않고 완료 상태만 유지
                    ChallengeList[i].isCompleted = true;
                }
                else
                {
                    ChallengeList[i].IncreaseGoalAndReward(); // 목표 증가
                }
            }
        }
        SaveChallenges();
        UpdateUI();
    }

    // 기존 목표 숫자를 새로운 목표 숫자로 변경하는 함수
    private string UpdateChallengeTitle(string challengeName, int oldGoal, int newGoal)
    {
        return Regex.Replace(challengeName, @"\d+", newGoal.ToString());
    }
    public void ResetChallenges()
    {
        PlayerPrefs.DeleteKey("ChallengeData"); // 기존 도전과제 데이터 삭제
        PlayerPrefs.Save();

        ChallengeList.Clear(); // 리스트 비우기
        InitializeChallenges(); // 도전과제 재설정
        SaveChallenges(); // 다시 저장
        UpdateUI(); // UI 업데이트

        Debug.Log("도전과제 데이터가 초기화되었습니다!");
    }

}
