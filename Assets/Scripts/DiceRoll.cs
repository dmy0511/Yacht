using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 주사위 굴리기를 담당하는 클래스
public class DiceRoll : MonoBehaviour
{
    private UpgradeManager upgradeManager;
    private Rigidbody rb;

    // 최대 랜덤 힘과 초기 굴리기 힘
    [SerializeField] private float maxRandomForceValue = 10f, startRollingForce = 10f;

    private float forceX, forceY, forceZ;           // 주사위에 가해질 3축 힘

    public int diceFaceNum;                         // 현재 주사위 눈
    public bool isRolling { get; private set; }     // 굴리는 중인지 여부

    // 주사위 굴리기 시작/종료 이벤트
    public event System.Action OnRollStart;
    public event System.Action OnRollEnd;

    // 초기화
    private void Awake()
    {
        Initialize();
    }

    public void RollDice()
    {
        if (isRolling) return;      // 이미 굴리는 중이면 리턴

        isRolling = true;
        OnRollStart?.Invoke();
        rb.isKinematic = false;

        AdjustForceBasedOnProbability();        // 확률에 따른 힘 조절

        // 주사위에 힘과 회전력 가하기
        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

    // 주사위 정지 체크
    private void Update()
    {
        if (rb.velocity == Vector3.zero)
        {
            if (isRolling)
            {
                isRolling = false;
                OnRollEnd?.Invoke();
            }
        }
    }

    // 확률에 따른 힘 조절 메서드
    private void AdjustForceBasedOnProbability()
    {
        if (upgradeManager == null)
        {
            forceX = Random.Range(0, maxRandomForceValue);
            forceY = Random.Range(0, maxRandomForceValue);
            forceZ = Random.Range(0, maxRandomForceValue);
            return;
        }

        float[] probabilities = new float[6];
        float totalProbability = 0f;

        for (int i = 1; i <= 6; i++)
        {
            probabilities[i - 1] = upgradeManager.GetDiceNumberProbability(i);
            totalProbability += probabilities[i - 1];
        }

        float randomValue = Random.Range(0f, 1f);
        float currentSum = 0f;

        for (int i = 0; i < 6; i++)
        {
            float normalizedProb = totalProbability > 0 ? probabilities[i] / totalProbability : 1f / 6f;
            currentSum += normalizedProb;

            if (randomValue <= currentSum)
            {
                AdjustForceForNumber(i + 1);
                return;
            }
        }

        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);
    }

    // 목표 숫자에 따른 힘 조절
    private void AdjustForceForNumber(int targetNumber)
    {
        switch (targetNumber)
        {
            case 1:
                forceX = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceY = Random.Range(0, maxRandomForceValue * 0.3f);
                forceZ = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                break;
            case 2:
                forceX = Random.Range(0, maxRandomForceValue * 0.3f);
                forceY = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue * 0.3f);
                break;
            case 3:
                forceX = Random.Range(0, maxRandomForceValue * 0.3f);
                forceY = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue * 0.3f);
                break;
            case 4:
                forceX = Random.Range(0, maxRandomForceValue * 0.3f);
                forceY = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue * 0.3f);
                break;
            case 5:
                forceX = Random.Range(0, maxRandomForceValue * 0.3f);
                forceY = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue * 0.3f);
                break;
            case 6:
                forceX = Random.Range(0, maxRandomForceValue * 0.3f);
                forceY = Random.Range(maxRandomForceValue * 0.7f, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue * 0.3f);
                break;
            default:
                forceX = Random.Range(0, maxRandomForceValue);
                forceY = Random.Range(0, maxRandomForceValue);
                forceZ = Random.Range(0, maxRandomForceValue);
                break;
        }
    }

    // 주사위 초기화
    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        // 랜덤한 회전값으로 초기화
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
        diceFaceNum = 0;
        isRolling = false;
    }
}
