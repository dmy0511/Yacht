using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 주사위 값과 잠금 상태를 관리하는 클래스
public class Score : MonoBehaviour
{
    // UI 요소들
    [SerializeField] private DiceRoll[] dice;                   // 주사위 배열
    [SerializeField] private TextMeshProUGUI[] scoreTexts;      // 점수 텍스트들
    [SerializeField] private Button[] LockButton;               // 잠금 버튼들
    [SerializeField] private Sprite unlockedSprite;             // 잠금해제 이미지
    [SerializeField] private Sprite lockedSprite;               // 잠금 이미지

    // 버튼 크기 설정
    [SerializeField] private Vector2 unlockedSize = new Vector2(74f, 70f);      // 잠금해제 크기
    [SerializeField] private Vector2 lockedSize = new Vector2(61.5f, 70.5f);    // 잠금 크기

    private bool[] isLocked;                   // 잠금 상태 배열
    private PedigreeManager pedigreeManager;   // 계보 매니저
    private TextManager textManager;           // 텍스트 매니저
    private int totalRollCount = 0;            // 총 굴린 횟수

    // 초기화
    private void Awake()
    {
        // 컴포넌트들 찾기
        dice = FindObjectsOfType<DiceRoll>();
        isLocked = new bool[dice.Length];

        // 잠금 버튼 이벤트 설정
        for (int i = 0; i < LockButton.Length; i++)
        {
            int index = i;
            LockButton[i].onClick.AddListener(() => ToggleLock(index));
        }

        // 매니저 참조 설정
        pedigreeManager = FindObjectOfType<PedigreeManager>();
        textManager = FindObjectOfType<TextManager>();
        if (textManager != null)
        {
            textManager.OnRollComplete += CheckDiceRoll;
        }
    }

    // 주사위 굴림 체크 (3회 이상이면 리셋)
    private void CheckDiceRoll()
    {
        totalRollCount++;

        if (totalRollCount >= 3)
        {
            ResetAllDice();
        }
    }

    // 잠금 상태 토글
    private void ToggleLock(int index)
    {
        if (index >= 0 && index < isLocked.Length)
        {
            isLocked[index] = !isLocked[index];
            Image buttonImage = LockButton[index].GetComponent<Image>();
            RectTransform rectTransform = LockButton[index].GetComponent<RectTransform>();
            if (buttonImage != null && rectTransform != null)
            {
                buttonImage.sprite = isLocked[index] ? lockedSprite : unlockedSprite;
                Vector2 targetSize = isLocked[index] ? lockedSize : unlockedSize;
                rectTransform.sizeDelta = targetSize;
            }
        }
    }

    // 모든 주사위 리셋
    public void ResetAllDice()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = "?";
            }
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            isLocked[i] = false;
            if (LockButton[i] != null)
            {
                Image buttonImage = LockButton[i].GetComponent<Image>();
                RectTransform rectTransform = LockButton[i].GetComponent<RectTransform>();
                if (buttonImage != null && rectTransform != null)
                {
                    buttonImage.sprite = unlockedSprite;
                    rectTransform.sizeDelta = unlockedSize;
                }
            }
        }
        foreach (var die in dice)
        {
            if (die != null)
            {
                die.diceFaceNum = 0;
            }
        }
        totalRollCount = 0;
    }

    // 주사위 값과 조건 업데이트
    private void Update()
    {
        // 잠금되지 않은 주사위들의 점수 업데이트
        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] != null && scoreTexts[i] != null)
            {
                if (!isLocked[i])
                {
                    scoreTexts[i].text = dice[i].diceFaceNum != 0 ? dice[i].diceFaceNum.ToString() : "?";
                }
            }
        }

        // 조건 체크
        if (pedigreeManager != null)
        {
            pedigreeManager.CheckCondition();
        }
    }

    // 이벤트 구독 해제
    private void OnDestroy()
    {
        if (textManager != null)
        {
            textManager.OnRollComplete -= CheckDiceRoll;
        }
    }
}
