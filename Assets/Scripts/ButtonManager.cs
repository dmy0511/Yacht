using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// UI 버튼과 화면을 관리하는 클래스
public class ButtonManager : MonoBehaviour
{
    // UI 요소들의 타입을 정의하는 열거형
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

    // UI 요소 그룹을 정의하는 클래스 
    [System.Serializable]
    public class UIElementGroup
    {
        public UIElementType type;          // UI 타입
        public GameObject uiObject;         // UI 오브젝트
        public Image buttonImage;           // 버튼 이미지
        public Sprite defaultSprite;        // 기본 스프라이트
        public Sprite activeSprite;         // 활성화 스프라이트
        public bool isActive;               // 활성화 상태
    }

    // UI 요소들 리스트
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

    // 타워 관련 변수
    public GameObject stoneTower;
    private bool isStoneTowerActive = false;

    // 씬 타입 정의
    private enum SceneType
    {
        Shop,
        Build,
        Upgrade,
        Skin
    }
    private SceneType currentScene;

    // 시작시 모든 UI 비활성화
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

    // 씬 전환 메서드들
    public void Shop() { ChangeScene(SceneType.Shop); }
    public void Build() { ChangeScene(SceneType.Build); }
    public void Upgrade() { ChangeScene(SceneType.Upgrade); }
    public void Skin() { ChangeScene(SceneType.Skin); }

    // 씬 전환 처리
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

    // UI 요소 토글 (켜기/끄기)
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

    // UI 요소 닫기
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

    // 각 UI 요소별 열기/닫기 메서드들
    public void MenuBtnOpen() { ToggleUIElement(UIElementType.Menu); }
    public void MenuBtnClose() { CloseUIElement(UIElementType.Menu); }

    public void ChallengeBtnOpen() { ToggleUIElement(UIElementType.Challenge); }
    public void ChallengeBtnClose() { CloseUIElement(UIElementType.Challenge); }

    public void AnnouncementBtnOpen() { ToggleUIElement(UIElementType.Announcement); }
    public void AnnouncementBtnClose() { CloseUIElement(UIElementType.Announcement); }

    public void MailBoxBtnOpen() { ToggleUIElement(UIElementType.MailBox); }
    public void MailBoxBtnClose() { CloseUIElement(UIElementType.MailBox); }

    public void SettingBtnOpen() { ToggleUIElement(UIElementType.Setting); }
    public void SettingBtnClose() { CloseUIElement(UIElementType.Setting); }

    public void PedigreeBtnOpen() { ToggleUIElement(UIElementType.Pedigree); }
    public void PedigreeBtnClose() { CloseUIElement(UIElementType.Pedigree); }

    public void MiningBtnOpen() { ToggleUIElement(UIElementType.MiningStatus); }
    public void MiningBtnClose() { CloseUIElement(UIElementType.MiningStatus); }

    public void QuestionBtnOpen() { ToggleUIElement(UIElementType.Question); }
    public void QuestionBtnClose() { CloseUIElement(UIElementType.Question); }

    public void StairWindowOpen() { ToggleUIElement(UIElementType.Stair); }
    public void StairWindowClose() { CloseUIElement(UIElementType.Stair); }

    // 층계 보상창 열기
    public void Stone_Tower()
    {
        if (stoneTower != null)
        {
            isStoneTowerActive = !isStoneTowerActive;
            stoneTower.SetActive(isStoneTowerActive);
        }
    }

    // 층계 보상창 닫기
    public void Stone_Tower_Close()
    {
        if (stoneTower != null) stoneTower.SetActive(false);
        isStoneTowerActive = false;
    }

    // 현재 씬 타입 가져오기
    private SceneType GetCurrentSceneType()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return (SceneType)System.Enum.Parse(typeof(SceneType), currentSceneName.Replace("Scene", ""));
    }
}
