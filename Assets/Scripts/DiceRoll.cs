using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �ֻ��� �����⸦ ����ϴ� Ŭ����
public class DiceRoll : MonoBehaviour
{
    private UpgradeManager upgradeManager;
    private Rigidbody rb;

    // �ִ� ���� ���� �ʱ� ������ ��
    [SerializeField] private float maxRandomForceValue = 10f, startRollingForce = 10f;

    private float forceX, forceY, forceZ;           // �ֻ����� ������ 3�� ��

    public int diceFaceNum;                         // ���� �ֻ��� ��
    public bool isRolling { get; private set; }     // ������ ������ ����

    // �ֻ��� ������ ����/���� �̺�Ʈ
    public event System.Action OnRollStart;
    public event System.Action OnRollEnd;

    // �ʱ�ȭ
    private void Awake()
    {
        Initialize();
    }

    public void RollDice()
    {
        if (isRolling) return;      // �̹� ������ ���̸� ����

        isRolling = true;
        OnRollStart?.Invoke();
        rb.isKinematic = false;

        AdjustForceBasedOnProbability();        // Ȯ���� ���� �� ����

        // �ֻ����� ���� ȸ���� ���ϱ�
        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

    // �ֻ��� ���� üũ
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

    // Ȯ���� ���� �� ���� �޼���
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

    // ��ǥ ���ڿ� ���� �� ����
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

    // �ֻ��� �ʱ�ȭ
    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        // ������ ȸ�������� �ʱ�ȭ
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
        diceFaceNum = 0;
        isRolling = false;
    }
}
