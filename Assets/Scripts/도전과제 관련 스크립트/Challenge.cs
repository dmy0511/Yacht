using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Challenge
{
    public string challengeName;        // �������� �̸�
    public int goal;                    // ��ǥ ���൵
    public int currentProgress;         // ���� ���൵
    public bool isCompleted;            // �Ϸ� ����
    public int rewardAmount;            // ���� ����
    public bool isOneTimeChallenge;     // 1ȸ�� �������� ����
    public Sprite rewardIcon;           // ���� �̹���

    // �������� �� ���� ���� ��� (��������Ʈ)
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

        // ���� ��� ���� (������ �⺻ ���� ��� ���)
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
        if (isOneTimeChallenge) return; // 1ȸ�� ���������� ���� X

        goal = goalIncreaseMethod(goal);
        rewardAmount = rewardIncreaseMethod(rewardAmount);

        isCompleted = false; // �Ϸ� ���� �ʱ�ȭ
        currentProgress = 0; // ���൵ �ʱ�ȭ
    }

    // �⺻ ��ǥ ���� ���: +10
    private int DefaultGoalIncrease(int currentGoal) => currentGoal + 10;
    private int DefaultRewardIncrease(int currentReward) => currentReward + 100;

    public float GetProgressRatio()
    {
        return (float)currentProgress / goal;
    }
}
