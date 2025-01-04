using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 배경화면을 관리하는 클래스
public class BackgroundManager : MonoBehaviour
{
    // 배경 이미지 리스트와 표시할 SpriteRenderer
    [Header("Backgrounds")]
    [SerializeField] private List<Sprite> backgroundSprites;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private int currentBackgroundIndex = 0;

    // 게임 시작시 첫 배경 표시
    private void Start()
    {
        if (backgroundSprites != null && backgroundSprites.Count > 0 && backgroundRenderer != null)
        {
            backgroundRenderer.sprite = backgroundSprites[0];
        }
    }

    // 오른쪽 화살표로 배경 전환
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowNextBackground();
        }
    }

    // 다음 배경으로 순환하며 전환
    private void ShowNextBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Count == 0 || backgroundRenderer == null)
            return;

        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Count;

        backgroundRenderer.sprite = backgroundSprites[currentBackgroundIndex];
    }
}
