using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 게임 내 재화와 보상을 관리하는 클래스
public class GoodsManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;           // 텍스트 매니저 참조
    [SerializeField] private PedigreeManager pedigreeManager;   // 계보 매니저 참조

    // 보상 타입에 따라 플레이어에게 보상 지급
    public void RewardPlayer(string rewardType)
    {
        switch (rewardType)
        {
            case "Coin":                    // 코인 500 지급
                textManager.UpdateCoinText(500);
                break;
            case "Clover":                  // 클로버 200 지급
                textManager.UpdateCloverText(200);
                break;
            case "Diamond":                 // 다이아몬드 20 지급
                textManager.UpdateDiamondText(20);
                break;
            case "Roll":                    // 주사위 롤 횟수 50 증가
                textManager.AddRollCount(50);
                break;
        }
    }
}
