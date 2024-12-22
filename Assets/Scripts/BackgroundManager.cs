using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private List<Sprite> backgroundSprites;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private int currentBackgroundIndex = 0;

    private void Start()
    {
        if (backgroundSprites != null && backgroundSprites.Count > 0 && backgroundRenderer != null)
        {
            backgroundRenderer.sprite = backgroundSprites[0];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowNextBackground();
        }
    }

    private void ShowNextBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Count == 0 || backgroundRenderer == null)
            return;

        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Count;

        backgroundRenderer.sprite = backgroundSprites[currentBackgroundIndex];
    }
}
