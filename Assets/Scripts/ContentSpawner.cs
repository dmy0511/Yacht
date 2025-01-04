using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI에 아이템을 생성하는 스포너 클래스
public class ContentSpawner : MonoBehaviour
{
    public GameObject contentParent;    // 생성된 아이템이 들어갈 부모 오브젝트
    public GameObject itemPrefab;       // 생성할 아이템 프리팹
    private Button spawnButton;          // 스폰 버튼

    // 시작시 버튼 컴포넌트 가져오고 클릭 이벤트 연결
    void Start()
    {
        spawnButton = GetComponent<Button>();

        spawnButton.onClick.AddListener(SpawnItem);
    }

    // 아이템 생성 메서드
    public void SpawnItem()
    {
        if (contentParent != null && itemPrefab != null)
        {
            Instantiate(itemPrefab, contentParent.transform);
        }
        else
        {
            Debug.LogWarning("Content 오브젝트나 아이템 프리팹이 할당되지 않았습니다!");
        }
    }
}
