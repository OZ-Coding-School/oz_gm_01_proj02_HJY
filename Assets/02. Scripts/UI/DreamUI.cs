
using UnityEngine;
using UnityEngine.UI;

public class DreamUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject choosePanel;     // 선택 패널

    [Header("버튼")]
    public Button menuButton;          // 메뉴 버튼

    public void Awake()
    {
        // 씬을 전환해도 내버려둬라.
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        // 메뉴 버튼을 누르면 선택 패널을 켜라.
        menuButton.onClick.AddListener(OnChoose);
    }

    // 선택 패널을 켜라.
    public void OnChoose()
    {
        choosePanel.SetActive(true);
    }
}
