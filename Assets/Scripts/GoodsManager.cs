using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodsManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;
    [SerializeField] private PedigreeManager pedigreeManager;

    public void RewardPlayer(string rewardType)
    {
        switch (rewardType)
        {
            case "Coin":
                textManager.UpdateCoinText(500);
                break;
            case "Clover":
                textManager.UpdateCloverText(200);
                break;
            case "Diamond":
                textManager.UpdateDiamondText(20);
                break;
            case "Roll":
                textManager.AddRollCount(5);
                break;
        }
    }
}
