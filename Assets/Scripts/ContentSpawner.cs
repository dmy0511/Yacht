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
            Debug.LogWarning("Content ������Ʈ�� ������ �������� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }
}
