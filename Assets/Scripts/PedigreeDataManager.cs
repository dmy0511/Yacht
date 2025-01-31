using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

// 족보 데이터를 관리하는 싱글톤 클래스
public class PedigreeDataManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PedigreeDataManager Instance { get; private set; }

    // 조건을 충족한 총 개수 저장
    public int savedTotalConditionMetCount = 0;

    // 아이템 데이터 구조체
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

    // 저장된 아이템 목록
    public List<ItemData> savedItems = new List<ItemData>();

    // 싱글톤 초기화
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

    // 아이템의 진행도와 속도 배율 저장
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
