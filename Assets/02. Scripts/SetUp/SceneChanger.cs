
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void Awake()
    {
        // 씬을 전환해도 내버려둬라.
        DontDestroyOnLoad(gameObject);
    }

    // 타이틀 화면에서 게임 시작 버튼을 누르면 게임씬으로 넘어가라.
    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 게임 종료 버튼을 누르면 게임을 종료해라.
    public void GameExit()
    {
        // 유니티 에디터에서도 반응이 보이도록 하기
        #if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;  // 게임을 중단하라.
        #else
        // 빌드 파일일 경우
        Application.Quit();                               // 게임 나가기
        #endif
    }

    // 게임 씬에서 타이틀 씬으로 가는 버튼을 누르면 타이틀 씬으로 보내라.
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
