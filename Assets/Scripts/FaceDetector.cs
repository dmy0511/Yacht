using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ֻ����� ������ �����ϴ� Ŭ���� 
public class FaceDetector : MonoBehaviour
{
    // �ٸ� �ݶ��̴��� ���������� ������ �� ȣ��
    private void OnTriggerStay(Collider other)
    {
        // �浹�� ������Ʈ�� �θ𿡼� DiceRoll ������Ʈ ��������
        DiceRoll dice = other.GetComponentInParent<DiceRoll>();
        if (dice != null)
        {
            // �ֻ����� Rigidbody ������Ʈ ��������
            Rigidbody rb = dice.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity == Vector3.zero)  // �ֻ����� �����ִٸ�
            {
                // �ݶ��̴� �̸��� ���ڷ� ��ȯ �õ�
                if (int.TryParse(other.name, out int faceValue))
                {
                    // 1~6 ������ ��ȿ�� �ֻ��� ���̶�� �� ����
                    if (faceValue >= 1 && faceValue <= 6)
                    {
                        dice.diceFaceNum = faceValue;
                    }
                }
            }
        }
    }
}
