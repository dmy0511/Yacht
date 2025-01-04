using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI�� �������� �����ϴ� ������ Ŭ����
public class ContentSpawner : MonoBehaviour
{
    public GameObject contentParent;    // ������ �������� �� �θ� ������Ʈ
    public GameObject itemPrefab;       // ������ ������ ������
    private Button spawnButton;          // ���� ��ư

    // ���۽� ��ư ������Ʈ �������� Ŭ�� �̺�Ʈ ����
    void Start()
    {
        spawnButton = GetComponent<Button>();

        spawnButton.onClick.AddListener(SpawnItem);
    }

    // ������ ���� �޼���
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
