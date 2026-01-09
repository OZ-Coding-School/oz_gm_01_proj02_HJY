
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{

    [Header("설정 패널")]
    public GameObject optionPanel;

    [Header("아이콘")]
    public Image bgmIcon;
    public Sprite bgmOnSprite;
    public Sprite bgmOffSprite;

    public Image sfxIcon;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    [Header("효과음 오디오 소스")]
    public AudioSource[] playerSources;
    public AudioSource[] enemySources;


    // 아이콘 반영
    public void OnEnable()
    {
        // 씬이 바뀌거나 패널이 다시 켜질 때 상태 반영
        bgmIcon.sprite = AudioManager.bgmOn ? bgmOnSprite : bgmOffSprite;
        sfxIcon.sprite = AudioManager.sfxOn ? sfxOnSprite : sfxOffSprite;
    }

    // 배경 움악
    public void ToggleBGM()
    {
        // 게임 씬에서 시작할 때 필요한 안전 체크
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleBGM();
        }
        else
        {
            return;
        }

        bgmIcon.sprite = AudioManager.bgmOn ? bgmOnSprite : bgmOffSprite;
    }

    // 효과음
    public void ToggleSFX()
    {
        // 게임 씬에서 시작할 때 필요한 안전 체크
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleSFX(playerSources, enemySources);
        }
        else
        {
            return;
        }

        sfxIcon.sprite = AudioManager.sfxOn ? sfxOnSprite : sfxOffSprite;
    }

    // 닫기 버튼을 누르면 설정 패널을 닫아라.
    public void OffOption()
    {
        optionPanel.SetActive(false);
    }
}
