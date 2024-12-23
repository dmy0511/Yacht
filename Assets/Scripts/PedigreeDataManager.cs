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
        public int parentIndex;        // contentParents �ε���
        public string floorNumber;     // ���� �ؽ�Ʈ
        public string rewardType;      // ���� Ÿ��
        public float progress;         // ���൵
        public float baseSpeed;        // �⺻ �ӵ�
        public float speedMultiplier;  // ���� �ӵ� ����
        public float rewardAmount;     // ���� ��
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
