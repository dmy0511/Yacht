using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 주사위의 윗면을 감지하는 클래스 
public class FaceDetector : MonoBehaviour
{
    // 다른 콜라이더와 지속적으로 접촉할 때 호출
    private void OnTriggerStay(Collider other)
    {
        // 충돌한 오브젝트의 부모에서 DiceRoll 컴포넌트 가져오기
        DiceRoll dice = other.GetComponentInParent<DiceRoll>();
        if (dice != null)
        {
            // 주사위의 Rigidbody 컴포넌트 가져오기
            Rigidbody rb = dice.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity == Vector3.zero)  // 주사위가 멈춰있다면
            {
                // 콜라이더 이름을 숫자로 변환 시도
                if (int.TryParse(other.name, out int faceValue))
                {
                    // 1~6 사이의 유효한 주사위 눈이라면 값 저장
                    if (faceValue >= 1 && faceValue <= 6)
                    {
                        dice.diceFaceNum = faceValue;
                    }
                }
            }
        }
    }
}
