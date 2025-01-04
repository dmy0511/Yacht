using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���� �� ��ȭ�� ������ �����ϴ� Ŭ����
public class GoodsManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;           // �ؽ�Ʈ �Ŵ��� ����
    [SerializeField] private PedigreeManager pedigreeManager;   // �躸 �Ŵ��� ����

    // ���� Ÿ�Կ� ���� �÷��̾�� ���� ����
    public void RewardPlayer(string rewardType)
    {
        switch (rewardType)
        {
            case "Coin":                    // ���� 500 ����
                textManager.UpdateCoinText(500);
                break;
            case "Clover":                  // Ŭ�ι� 200 ����
                textManager.UpdateCloverText(200);
                break;
            case "Diamond":                 // ���̾Ƹ�� 20 ����
                textManager.UpdateDiamondText(20);
                break;
            case "Roll":                    // �ֻ��� �� Ƚ�� 50 ����
                textManager.AddRollCount(50);
                break;
        }
    }
}
