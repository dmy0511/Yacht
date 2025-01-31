using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    private Challenge challenge; // 도전과제 데이터 저장

    [Header("도전과제 상태 UI")]
    public TextMeshProUGUI challengeNameText; // 도전과제 이름
    public TextMeshProUGUI progressText;      // 진행 수치
    public Slider progressSlider;             // 진행도 슬라이더

    [Header("도전과제 완료 관련 UI")]
    public TextMeshProUGUI rewardText;        // 보상 개수
    public Button rewardButton;               // 보상 버튼
    public Image rewardImage;                 // 보상 이미지

    private bool isRewardClaimed = false;  // 보상을 받았는지 여부

    // UI 초기화
    public void Initialize(Challenge challengeData)
    {
        challenge = challengeData;
        rewardButton.onClick.AddListener(ClaimReward); // 버튼 클릭 시 보상 받기 실행
        UpdateUI();
    }

    // UI 업데이트
    public void UpdateUI()
    {
        if (challenge == null) return;                                                  // challenge가 없으면 리턴
        challengeNameText.text = challenge.challengeName;                               // 도전과제 이름
        progressText.text = $"{challenge.currentProgress} / {challenge.goal}";          // 진행 수치
        progressSlider.value = challenge.GetProgressRatio();                            // 슬라이더 값 반영
        rewardText.text = FormatNumber(challenge.rewardAmount);                         // 보상 갯수
        rewardImage.sprite = challenge.rewardIcon;                                      // 보상이미지

        // 도전과제 완료 여부에 따라 버튼 활성화
        rewardButton.interactable = challenge.isCompleted;
    }

    // 보상 받기 버튼 클릭 시 실행
    public void ClaimReward()
    {
        if (challenge.isCompleted)
        {
            ChallengeManager._Instance.ClaimReward(challenge.challengeName);

            // 보상을 받은 후에도 버튼 활성화 유지
            if (!challenge.isOneTimeChallenge)
            {
                rewardButton.interactable = true;
            }
            else
            {
                rewardButton.interactable = false;
            }

            UpdateUI();
        }
    }
    public static string FormatNumber(double number)
    {
        if (number < 1000) return number.ToString();                                                    // 1000 미만이면 그대로 표시
        if (number < 1_000_000) return (number / 1_000).ToString("0.#") + "K";                          // 1K = 1,000
        if (number < 1_000_000_000) return (number / 1_000_000).ToString("0.#") + "M";                  // 1M = 1,000,000
        if (number < 1_000_000_000_000) return (number / 1_000_000_000).ToString("0.#") + "B";          // 1B = 1,000,000,000

        // 1조 이상부터는 A, B, C, D...로 표현
        char[] unitAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        int unitIndex = 0;
        double value = number / 1_000_000_000_000; // 1조(T) 단위부터 시작

        while (value >= 1000 && unitIndex < unitAlphabet.Length - 1)
        {
            value /= 1000;
            unitIndex++;
        }

        return value.ToString("0.#") + unitAlphabet[unitIndex];
    }
}