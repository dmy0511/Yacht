using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        DiceRoll dice = other.GetComponentInParent<DiceRoll>();
        if (dice != null)
        {
            Rigidbody rb = dice.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity == Vector3.zero)
            {
                if (int.TryParse(other.name, out int faceValue))
                {
                    if (faceValue >= 1 && faceValue <= 6)
                    {
                        dice.diceFaceNum = faceValue;
                    }
                }
            }
        }
    }
}
