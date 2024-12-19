using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentSpawner : MonoBehaviour
{
    public GameObject contentParent;

    public GameObject itemPrefab;

    private Button spawnButton;

    void Start()
    {
        spawnButton = GetComponent<Button>();

        spawnButton.onClick.AddListener(SpawnItem);
    }

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
