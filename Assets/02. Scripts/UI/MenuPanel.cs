
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [Header("패널")]
    public GameObject menuPanel;     // 메뉴 패널
    public GameObject progressPanel; // 진행도 패널
    public GameObject optionPanel;   // 설정 패널


    [Header("버튼")]
    public Button progressButton; // 진행도 버튼
    public Button optionButton;   // 설정 버튼

    // 게임이 시작되었다는 알림 받기
    private void OnEnable()
    {
        PlayerTouchMove.OnGameStart += HandleGameStart; // 이벤트 구독
    }

    // 패널이 비활성화가 되면 이벤트 해제로 메모리 누수 방지하기
    private void OnDisable()
    {
        PlayerTouchMove.OnGameStart -= HandleGameStart; // 이벤트 해제
    }

    // 게임이 시작되었다면
    private void HandleGameStart()
    {
        // 메뉴 패널을 끄기
        OffMenu(); 
    }


    // 메뉴 패널을 꺼라.
    private void OffMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }  
    }

    public void Awake()
    {
        // 씬을 전환해도 내버려둬라.
        DontDestroyOnLoad(gameObject);

        // 진행도 패널을 꺼라.
        progressPanel.SetActive(false);

        // 설정 패널을 꺼라.
        optionPanel.SetActive(false);
    }

    public void Start()
    {
        // 진행도 버튼을 누르면 진행도 패널을 켜라.
        progressButton.onClick.AddListener(OnProgress);

        // 설정 버튼을 누르면 설정 패널을 켜라.
        optionButton.onClick.AddListener(OnOption);
    }

    // 타이틀을 누르면 타이틀 화면으로 넘어가라.
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 진행도 패널을 켜라.
    public void OnProgress()
    {
        progressPanel.SetActive(true);
    }

    // 꿈꾸기를 누르면 드림 씬으로 넘어가라.
    public void GoDream()
    {
        SceneManager.LoadScene("DreamScene");
    }

    // 설정 패널을 켜라.
    public void OnOption()
    {
        optionPanel.SetActive(true);
    }
}
