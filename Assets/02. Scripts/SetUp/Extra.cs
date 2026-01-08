
using UnityEngine;
using UnityEngine.SceneManagement;

public class Extra : MonoBehaviour
{
    public void Awake()
    {
        // 씬을 전환해도 내버려둬라.
        DontDestroyOnLoad(gameObject);
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
