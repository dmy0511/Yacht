using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedigreeDataManager : MonoBehaviour
{
    public static PedigreeDataManager Instance { get; private set; }
    public int savedTotalConditionMetCount = 0;

    [System.Serializable]
    public struct ItemData
    {
        public int parentIndex;        // contentParents 인덱스
        public string floorNumber;     // 층수 텍스트
        public string rewardType;      // 보상 타입
        public float progress;         // 진행도
        public float baseSpeed;        // 기본 속도
        public float speedMultiplier;  // 개별 속도 배율
        public float rewardAmount;     // 보상 양
    }

    public List<ItemData> savedItems = new List<ItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveItemProgress(int index, float progress, float speedMultiplier)
    {
        if (index < savedItems.Count)
        {
            var item = savedItems[index];
            item.progress = progress;
            item.speedMultiplier = speedMultiplier;
            savedItems[index] = item;
        }
    }
}
