using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public enum UIElementType
    {
        Menu,
        Challenge,
        Announcement,
        MailBox,
        Setting,
        Pedigree,
        MiningStatus,
        Question,
        Stair
    }

    [System.Serializable]
    public class UIElementGroup
    {
        public UIElementType type;
        public GameObject uiObject;
        public Image buttonImage;
        public Sprite defaultSprite;
        public Sprite activeSprite;
        public bool isActive;
    }

    public List<UIElementGroup> uiElements = new List<UIElementGroup>
    {
        new UIElementGroup { type = UIElementType.Menu },
        new UIElementGroup { type = UIElementType.Challenge },
        new UIElementGroup { type = UIElementType.Announcement },
        new UIElementGroup { type = UIElementType.MailBox },
        new UIElementGroup { type = UIElementType.Setting },
        new UIElementGroup { type = UIElementType.Pedigree },
        new UIElementGroup { type = UIElementType.MiningStatus },
        new UIElementGroup { type = UIElementType.Question },
        new UIElementGroup { type = UIElementType.Stair }
    };

    public GameObject stoneTower;

    private bool isStoneTowerActive = false;

    private enum SceneType
    {
        Shop,
        Build,
        Upgrade,
        Skin
    }

    private SceneType currentScene;

    void Start()
    {
        foreach (var element in uiElements)
        {
            if (element.uiObject != null)
            {
                element.uiObject.SetActive(false);
                element.isActive = false;
            }
        }

        currentScene = GetCurrentSceneType();
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

    public void ToggleUIElement(UIElementType type)
    {
        var element = uiElements.Find(e => e.type == type);
        if (element != null)
        {
            element.isActive = !element.isActive;
            element.uiObject.SetActive(element.isActive);

            if (element.buttonImage != null)
            {
                element.buttonImage.sprite = element.isActive
                    ? element.activeSprite
                    : element.defaultSprite;
            }
        }
    }

    public void CloseUIElement(UIElementType type)
    {
        var element = uiElements.Find(e => e.type == type);
        if (element != null)
        {
            element.uiObject.SetActive(false);
            element.isActive = false;

            if (element.buttonImage != null)
            {
                element.buttonImage.sprite = element.defaultSprite;
            }
        }
    }

    public void MenuBtnOpen()
    {
        ToggleUIElement(UIElementType.Menu);
    }

    public void MenuBtnClose()
    {
        CloseUIElement(UIElementType.Menu);
    }

    public void ChallengeBtnOpen()
    {
        ToggleUIElement(UIElementType.Challenge);
    }

    public void ChallengeBtnClose()
    {
        CloseUIElement(UIElementType.Challenge);
    }

    public void AnnouncementBtnOpen()
    {
        ToggleUIElement(UIElementType.Announcement);
    }

    public void AnnouncementBtnClose()
    {
        CloseUIElement(UIElementType.Announcement);
    }

    public void MailBoxBtnOpen()
    {
        ToggleUIElement(UIElementType.MailBox);
    }

    public void MailBoxBtnClose()
    {
        CloseUIElement(UIElementType.MailBox);
    }

    public void SettingBtnOpen()
    {
        ToggleUIElement(UIElementType.Setting);
    }

    public void SettingBtnClose()
    {
        CloseUIElement(UIElementType.Setting);
    }

    public void PedigreeBtnOpen()
    {
        ToggleUIElement(UIElementType.Pedigree);
    }

    public void PedigreeBtnClose()
    {
        CloseUIElement(UIElementType.Pedigree);
    }

    public void MiningBtnOpen()
    {
        ToggleUIElement(UIElementType.MiningStatus);
    }

    public void MiningBtnClose()
    {
        CloseUIElement(UIElementType.MiningStatus);
    }

    public void QuestionBtnOpen()
    {
        ToggleUIElement(UIElementType.Question);
    }

    public void QuestionBtnClose()
    {
        CloseUIElement(UIElementType.Question);
    }

    public void StairWindowOpen()
    {
        ToggleUIElement(UIElementType.Stair);
    }

    public void StairWindowClose()
    {
        CloseUIElement(UIElementType.Stair);
    }

    public void Stone_Tower()
    {
        if (stoneTower != null)
        {
            isStoneTowerActive = !isStoneTowerActive;
            stoneTower.SetActive(isStoneTowerActive);
        }
    }

    public void Stone_Tower_Close()
    {
        if (stoneTower != null) stoneTower.SetActive(false);
        isStoneTowerActive = false;
    }

    private SceneType GetCurrentSceneType()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return (SceneType)System.Enum.Parse(typeof(SceneType), currentSceneName.Replace("Scene", ""));
    }
}
