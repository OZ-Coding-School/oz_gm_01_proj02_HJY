
using UnityEngine;

public class FollowCam : MonoBehaviour
{

    [Header("설정")]
    public Transform target; // 플레이어를 따라가라

    [SerializeField] private Vector2 offset = new Vector2(0.0f, 1.0f); // 카메라 위치 오프셋 (x, y)
    [SerializeField] private float followSpeed = 5.0f;                 // 카메라가 따라가는 속도
                                          


    // 카메라 추적
    private void LateUpdate()
    {
        if (!target) return; // target이 없으면 실행하지 않음

        // 목표 위치 계산
        Vector3 targetPos = new Vector3(
            transform.position.x,                // X축은 고정 (수직 게임이므로)
            target.position.y + offset.y,        // Y축만 플레이어 따라감
            transform.position.z                 // Z축은 카메라 기본값 유지
        );


        // 카메라가 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}