using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Challenge
{
    public string challengeName;        // 도전과제 이름
    public int goal;                    // 목표 진행도
    public int currentProgress;         // 현재 진행도
    public bool isCompleted;            // 완료 여부
    public int rewardAmount;            // 보상 개수
    public bool isOneTimeChallenge;     // 1회성 도전과제 여부
    public Sprite rewardIcon;           // 보상 이미지

    // 도전과제 및 보상 증가 방식 (델리게이트)
    private Func<int, int> goalIncreaseMethod;
    private Func<int, int> rewardIncreaseMethod;

    public Challenge(string name, int goal, int reward, Sprite rewardIcon, bool isOneTime,
                     Func<int, int> goalIncrease, Func<int, int> rewardIncrease)
    {
        this.challengeName = name;
        this.goal = goal;
        this.rewardAmount = reward;
        this.rewardIcon = rewardIcon;
        this.currentProgress = 0;
        this.isCompleted = false;
        this.isOneTimeChallenge = isOneTime;

        // 증가 방식 지정 (없으면 기본 증가 방식 사용)
        this.goalIncreaseMethod = goalIncrease ?? DefaultGoalIncrease;
        this.rewardIncreaseMethod = rewardIncrease ?? DefaultRewardIncrease;
    }

    public void UpdateProgress(int amount)
    {
        if (isCompleted) return;

        currentProgress += amount;
        if (currentProgress >= goal)
        {
            currentProgress = goal;
            isCompleted = true;
        }
    }

    public void IncreaseGoalAndReward()
    {
        if (isOneTimeChallenge) return; // 1회성 도전과제는 갱신 X

        goal = goalIncreaseMethod(goal);
        rewardAmount = rewardIncreaseMethod(rewardAmount);

        isCompleted = false; // 완료 상태 초기화
        currentProgress = 0; // 진행도 초기화
    }

    // 기본 목표 증가 방식: +10
    private int DefaultGoalIncrease(int currentGoal) => currentGoal + 10;
    private int DefaultRewardIncrease(int currentReward) => currentReward + 100;

    public float GetProgressRatio()
    {
        return (float)currentProgress / goal;
    }
}
