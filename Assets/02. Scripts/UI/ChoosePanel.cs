
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChoosePanel : MonoBehaviour
{

    [Header("패널")]
    public GameObject choosePanel;     // 선택 패널


    public void Awake()
    {
        // 씬을 전환해도 내버려둬라.
        DontDestroyOnLoad(gameObject);

        // 선택 패널을 꺼라.
        choosePanel.SetActive(false);
    }


    // 닫기 버튼을 누르면 선택 패널을 닫아라.
    public void OffChoose()
    {
        choosePanel.SetActive(false);
    }

    // 게임으로 돌아가기 버튼을 누르면 게임 씬으로 넘어가라.
    public void PushGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 타이틀로 돌아가기 버튼을 누르면 타이틀 씬으로 넘어가라.
    public void PushTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
