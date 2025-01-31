using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 주사위 값과 잠금 상태를 관리하는 클래스
public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; } // 싱글톤 인스턴스
    public event System.Action<int, bool> OnLockStateChanged;

    // UI 요소들
    [SerializeField] private DiceRoll[] dice;                   // 주사위 배열
    [SerializeField] public GameObject[] DiceScore;              // 점수 다이스
    [SerializeField] private Button[] LockButton;               // 잠금 버튼들
    [SerializeField] private Sprite unlockedSprite;             // 잠금해제 이미지
    [SerializeField] private Sprite lockedSprite;               // 잠금 이미지

    // 버튼 크기 설정
    [SerializeField] private Vector2 unlockedSize = new Vector2(74f, 70f);      // 잠금해제 크기
    [SerializeField] private Vector2 lockedSize = new Vector2(61.5f, 70.5f);    // 잠금 크기

    private bool[] isLocked;                    // 잠금 상태 배열
    private PedigreeManager pedigreeManager;                    // 계보 매니저
    private TextManager textManager;                            // 텍스트 매니저
    private int totalRollCount = 0;                             // 총 굴린 횟수
    private int[] newDiceScore;
    private bool isDiceRolling = false;  // 주사위가 굴러가고 있는지 확인하는 플래그

    public event Action<int> OnScoreUpdated;
    private int currentScore;

    // 초기화
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 컴포넌트들 찾기
        dice = FindObjectsOfType<DiceRoll>();
        Array.Sort(dice, (a, b) => a.diceIndex.CompareTo(b.diceIndex));
        isLocked = new bool[dice.Length];
        newDiceScore = new int[dice.Length];

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

        // 버튼의 비활성화 상태 색상 설정
        foreach (var button in LockButton)
        {
            ColorBlock colors = button.colors;
            colors.disabledColor = Color.white; // 또는 원하는 색상
            button.colors = colors;
        }
    }

    // 주사위 굴림 체크 (3회 이상이면 리셋)
    private void CheckDiceRoll()
    {
        totalRollCount++;

        if (totalRollCount >= 3) ResetAllDice();
    }

    // 잠금 상태 토글
    private void ToggleLock(int index)
    {
        if (index >= 0 && index < isLocked.Length && DiceScore[index].activeSelf && !dice[index].isRolling)
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
            OnLockStateChanged?.Invoke(index, isLocked[index]);
        }
    }

    // 모든 주사위 리셋
    public void ResetAllDice()
    {
        for (int i = 0; i < DiceScore.Length; i++)
        {
            if (DiceScore[i] != null) DiceScore[i].SetActive(false);
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            bool wasLocked = isLocked[i];
            isLocked[i] = false;
            newDiceScore[i] = 0;

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
            // 잠금 상태가 변경되었을 때만 이벤트 발생
            if (wasLocked)
            {
                OnLockStateChanged?.Invoke(i, false);
            }
        }
        foreach (var die in dice)
        {
            if (die != null) die.diceFaceNum = 0;
        }
        totalRollCount = 0;
    }

    // 주사위 값과 조건 업데이트
    private void Update()
    {
        // 기본적으로 비활성화 상태로 가정
        isDiceRolling = true;

        // 모든 DiceScore가 활성화되었는지 체크
        bool allDiceScoreActive = true;

        bool allLocked = true;
        int lockedCount = 0;

        TextManager textManager = FindObjectOfType<TextManager>();
        if (textManager != null && textManager.rollButton != null)
        {
            if (!isDiceRolling) // 주사위가 굴러가고 있지 않을 때만 버튼 상태 변경
            {
                textManager.rollButton.interactable = !allLocked;
            }
        }
        for (int i = 0; i < isLocked.Length; i++)
        {
            if (isLocked[i])
                lockedCount++;
            else
                allLocked = false;
        }

        // 잠금 상태가 변경될 때만 디버그 로그 출력
        if (lockedCount != previousLockedCount)
        {
            Debug.Log($"잠긴 주사위: {lockedCount}개, 남은 주사위: {5 - lockedCount}개");
            previousLockedCount = lockedCount;
        }


        for (int i = 0; i < DiceScore.Length; i++)
        {
            if (!DiceScore[i].activeSelf && !isLocked[i])
            {
                allDiceScoreActive = false;
                break;
            }
        }

        // 모든 DiceScore가 활성화되었다면 isDiceRolling을 false로 설정
        if (allDiceScoreActive) isDiceRolling = false;

        for (int i = 0; i < dice.Length; i++)
        {
            LockButton[i].interactable = DiceScore[i].activeSelf && !isDiceRolling;

            if (isLocked[i])
            {
                DiceScore[i].SetActive(true);
                continue;
            }

            if (dice[i].isRolling)
            {
                DiceScore[i].SetActive(false);
                DiceScore[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                continue;
            }
            int currentFaceNum = dice[i].diceFaceNum;

            // 주사위 숫자에 따라 회전값 설정 및 활성화
            switch (dice[i].diceFaceNum)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    if (newDiceScore[i] != currentFaceNum) newDiceScore[i] = currentFaceNum;
                    DiceScore[i].transform.rotation = GetRotationForNumber(currentFaceNum);
                    break;
                default:
                    DiceScore[i].SetActive(false);
                    break;
            }
            // 조건 체크
            if (pedigreeManager != null) pedigreeManager.CheckCondition();
        }

    }

    private int previousLockedCount = 0;  // 이전 잠금 상태를 저장할 변수

    // 이벤트 구독 해제
    private void OnDestroy()
    {
        if (textManager != null) textManager.OnRollComplete -= CheckDiceRoll;
    }
    private Quaternion GetRotationForNumber(int number)
    {
        return number switch
        {
            1 => Quaternion.Euler(0, 0, -90),
            2 => Quaternion.Euler(-90, 0, 0),
            3 => Quaternion.Euler(90, 0, 0),
            4 => Quaternion.Euler(0, 0, 0),
            5 => Quaternion.Euler(0, 0, 180),
            6 => Quaternion.Euler(0, 0, 90),
            _ => Quaternion.Euler(0, 0, 0)
        };
    }

    public IEnumerator appearedDiceDelay(int index)
    {
        yield return new WaitForSeconds(0f);
        DiceScore[index].SetActive(true);
    }

    public int[] GetDiceScores()
    {
        return newDiceScore;
    }
    public bool GetLockState(int index)
    {
        if (index >= 0 && index < isLocked.Length) return isLocked[index];
        return false;
    }
    public bool IsAllDiceLocked()
    {
        int lockedCount = 0;
        for (int i = 0; i < isLocked.Length; i++)
            if (isLocked[i]) lockedCount++;

        return lockedCount >= 5;
    }
    public void ResetRollCount()
    {
        totalRollCount = 0;
        Debug.Log("주사위 롤 횟수가 초기화되었습니다.");
    }
    public void UpdateScore(int amount)
    {
        currentScore += amount;
        OnScoreUpdated?.Invoke(currentScore); // 이벤트 발생
    }

}