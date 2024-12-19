using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{
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

        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);

        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
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
