
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public static bool bgmOn = true;   // 배경 음악 상태
    public static bool sfxOn = true;   // 효과음 상태

    private AudioSource currentBGM;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // 씬을 전환해도 내버려둬라.
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬마다 다른 BGM 사용
    public void RegisterBGM(AudioSource bgmSource)
    {
        currentBGM = bgmSource;

        // 현재 상태 반영
        currentBGM.mute = !bgmOn; 
    }

    // 배경 음악
    public void ToggleBGM()
    {
        bgmOn = !bgmOn;
        if (currentBGM != null)
            currentBGM.mute = !bgmOn;
    }

    // 플레이어 및 적 효과음
    public void ToggleSFX(AudioSource[] playerSources, AudioSource[] enemySources)
    {
        sfxOn = !sfxOn;

        foreach (AudioSource src in playerSources)
            src.mute = !sfxOn;

        foreach (AudioSource src in enemySources)
            src.mute = !sfxOn;
    }
}