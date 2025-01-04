using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���ȭ���� �����ϴ� Ŭ����
public class BackgroundManager : MonoBehaviour
{
    // ��� �̹��� ����Ʈ�� ǥ���� SpriteRenderer
    [Header("Backgrounds")]
    [SerializeField] private List<Sprite> backgroundSprites;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private int currentBackgroundIndex = 0;

    // ���� ���۽� ù ��� ǥ��
    private void Start()
    {
        if (backgroundSprites != null && backgroundSprites.Count > 0 && backgroundRenderer != null)
        {
            backgroundRenderer.sprite = backgroundSprites[0];
        }
    }

    // ������ ȭ��ǥ�� ��� ��ȯ
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowNextBackground();
        }
    }

    // ���� ������� ��ȯ�ϸ� ��ȯ
    private void ShowNextBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Count == 0 || backgroundRenderer == null)
            return;

        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Count;

        backgroundRenderer.sprite = backgroundSprites[currentBackgroundIndex];
    }
}
