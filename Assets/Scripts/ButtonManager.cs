using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject miningPanel;
    private bool isMiningPanelActive = false;

    private enum SceneType
    {
        Shop,
        Build,
        Upgrade,
        Skin
    }

    private SceneType currentScene;

    private void Start()
    {
        currentScene = GetCurrentSceneType();

        if (miningPanel != null)
        {
            miningPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (miningPanel != null && isMiningPanelActive)
            {
                isMiningPanelActive = false;
                miningPanel.SetActive(false);
            }
        }
    }

    public void Shop()
    {
        ChangeScene(SceneType.Shop);
    }

    public void Build()
    {
        ChangeScene(SceneType.Build);
    }

    public void Upgrade()
    {
        ChangeScene(SceneType.Upgrade);
    }

    public void Skin()
    {
        ChangeScene(SceneType.Skin);
    }

    private void ChangeScene(SceneType targetScene)
    {
        if (currentScene != targetScene)
        {
            string sceneName = targetScene.ToString() + "Scene";
            SceneManager.LoadScene(sceneName);
            currentScene = targetScene;
        }
        else     // 씬 중복 전환 방지
        {
            Debug.Log("이미 " + targetScene.ToString() + " 씬에 있습니다.");
        }
    }

    public void Mining_Status()
    {
        if (miningPanel != null)
        {
            isMiningPanelActive = !isMiningPanelActive;
            miningPanel.SetActive(isMiningPanelActive);
        }
    }

    private SceneType GetCurrentSceneType()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return (SceneType)System.Enum.Parse(typeof(SceneType), currentSceneName.Replace("Scene", ""));
    }
}
