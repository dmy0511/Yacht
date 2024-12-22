using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{
    private UpgradeManager upgradeManager;

    private Rigidbody rb;

    [SerializeField] private float maxRandomForceValue = 10f, startRollingForce = 10f;

    private float forceX, forceY, forceZ;

    public int diceFaceNum;
    public bool isRolling { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    public void RollDice()
    {
        if (isRolling) return;

        isRolling = true;
        rb.isKinematic = false;

        AdjustForceBasedOnProbability();

        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

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

    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    private void Update()
    {
        if (rb.velocity == Vector3.zero)
        {
            if (isRolling)
            {
                isRolling = false;
            }
        }
    }
}
