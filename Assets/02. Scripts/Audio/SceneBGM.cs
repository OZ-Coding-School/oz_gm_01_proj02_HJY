
using UnityEngine;


public class SceneBGM : MonoBehaviour
{
    private AudioSource bgm;

    void Start()
    {
        // 현재 씬의 오디오 소스 가져오기
        bgm = GetComponent<AudioSource>();

        if (bgm == null) return;

        // 안전 체크 : AudioManager가 초기화 안 됐어도 반환하라.
        if (AudioManager.Instance == null) return; 

        // AudioManager에 등록 (이전 씬 음악은 정지, 새 음악만 재생)
        AudioManager.Instance.RegisterBGM(bgm);
        

        // 옵션 상태 반영: 켜져 있으면 재생, 꺼져 있으면 뮤트
        bgm.mute = !AudioManager.bgmOn;
        if (AudioManager.bgmOn && !bgm.isPlaying)
        {
            bgm.Play();
        }
    }
}
