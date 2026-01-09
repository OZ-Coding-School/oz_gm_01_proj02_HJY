

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


public class PlayerTouchMove : MonoBehaviour
{
    [Header("이동/점프")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 7.0f;

    [Header("바닥 감지")]
    public Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("벽 감지")]
    public Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;      // 바닥 체크
    private bool jumpRequested;   // 점프 여부

    private readonly int moveSpeedHash = Animator.StringToHash("Speed");
    private readonly int jumpHash = Animator.StringToHash("IsJumping");

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    public static System.Action OnGameStart; // 이벤트 선언
    private bool gameStarted = false;        // 게임 시작 안 함


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 targetPos = Vector2.zero;
        bool hasInput = false;

        // 모바일 터치 입력
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            targetPos = Camera.main.ScreenToWorldPoint(touch.position);
            hasInput = true;

            // UI 클릭이면 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            // 플레이어 터치 → 점프 요청
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Collider2D col = GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(touchPos) && isGrounded)
                {
                    jumpRequested = true;
                }
            }
        }


        // PC 마우스 입력 
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            // UI 클릭이면 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hasInput = true;
        }

        // 플레이어 클릭 → 점프 요청
        if (Input.GetMouseButtonDown(0))
        {
            // UI 클릭이면 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(clickPos) && isGrounded)
            {
                jumpRequested = true;
            }
        }
#endif

        // 좌우 이동 처리
        if (hasInput)
        {
            Vector2 direction = targetPos - (Vector2)transform.position;

            // Raycast로 벽 감지
            RaycastHit2D hitRight = Physics2D.Raycast(wallCheck.position, Vector2.right, 1f, wallLayer);
            RaycastHit2D hitLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, 1f, wallLayer);

            if ((direction.x > 0 && hitRight.collider != null) || (direction.x < 0 && hitLeft.collider != null))
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // 벽 이동 차단
            }
            else
            {
                rb.velocity = new Vector2(direction.normalized.x * moveSpeed, rb.velocity.y);
            }

            // 방향 전환
            if (direction.x > 0.01f) spriteRenderer.flipX = false;
            else if (direction.x < -0.01f) spriteRenderer.flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // 애니메이션 파라미터 업데이트
        anim.SetFloat(moveSpeedHash, rb.velocity.magnitude);
        anim.SetBool(jumpHash, !isGrounded);


        // 게임 시작 알림
        if (hasInput && !gameStarted)
        {
            gameStarted = true;
            OnGameStart?.Invoke(); // 이벤트 발행
        }

    }

    private void FixedUpdate()
    {
        // 바닥 감지
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 점프 처리
        if (jumpRequested)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpRequested = false;
        }
    }

    // 점프하면 타일을 파괴하라.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground 레이어만 파괴
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!isGrounded) // 점프 중일 때만 파괴
            {
                // 충돌 지점 → 타일맵 좌표 변환
                Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();

                if (tilemap != null)
                {
                    Vector3 hitPoint = collision.contacts[0].point; // 충돌 지점 가져오기
                    Vector3Int cellPos = tilemap.WorldToCell(hitPoint); // 월드 좌표를 셀 좌표로 변환

                    // 해당 칸만 제거
                    tilemap.SetTile(cellPos, null);
                }
            }
        }
    }

    // 센서를 그려라.
    private void OnDrawGizmosSelected()
    {
        // 바닥 감지 센서 (노란색 원)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // 벽 감지 센서 (파란색 선)
        Gizmos.color = Color.blue;
        float wallCheckDistance = 1f;

        Vector3 origin = wallCheck != null ? wallCheck.position : transform.position; // 안전 체크 : 벽 감지가 없으면 스프라이트 중앙을 기준으로 삼아라


        // 오른쪽 벽 감지
        Vector3 rightEnd = origin + Vector3.right * wallCheckDistance;
        Gizmos.DrawLine(origin, rightEnd);
        Gizmos.DrawWireSphere(rightEnd, 0.05f);

        // 왼쪽 벽 감지
        Vector3 leftEnd = origin + Vector3.left * wallCheckDistance;
        Gizmos.DrawLine(origin, leftEnd);
        Gizmos.DrawWireSphere(leftEnd, 0.05f);

    }
}