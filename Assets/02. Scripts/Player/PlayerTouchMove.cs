
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerTouchMove : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField] private float moveSpeed = 5.0f;  // 이동 속도 (좌우 드래그)
    [SerializeField] private float jumpForce = 7.0f;  // 점프를 하는 힘 (위로 드래그)
    [SerializeField] private float fallForce = 10.0f; // 낙하를 하는 힘 (아래로 드래그)

    [Header("바닥 감지")]
    public Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool jumpRequested;
    private bool fallRequested;
    private bool attackRequested;

    private readonly int moveSpeedHash = Animator.StringToHash("Speed");
    private readonly int jumpHash = Animator.StringToHash("IsJumping");
    private readonly int fallHash = Animator.StringToHash("IsFalling");
    private readonly int attackHash = Animator.StringToHash("IsAttacking");

    private Vector2 dragStartPos;
    private Vector3Int lastDestroyedTile;     // 최근 파괴된 타일 좌표
    private bool tileDestroyedThisAction;     // 액션 중 한 번만 파괴

    public static System.Action OnGameStart;  // 게임 시작 알림 이벤트 선언



    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
#if UNITY_EDITOR
        // 마우스 클릭 시작 → 드래그 시작점 기록
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(clickPos))
            {
                dragStartPos = clickPos;
            }
        }

        // 드래그 중 액션 처리
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = currentPos - dragStartPos;

            // 위로 드래그 → 점프
            if (dragVector.y > 0.1f && isGrounded)
            {
                jumpRequested = true;
                tileDestroyedThisAction = false;
            }
            // 아래로 드래그 → 낙하
            else if (dragVector.y < -0.1f && !isGrounded)
            {
                fallRequested = true;
                tileDestroyedThisAction = false;
            }
            else
            {
                // 좌우 드래그 → 이동
                rb.velocity = new Vector2(dragVector.x * moveSpeed, rb.velocity.y);

                if (dragVector.x > 0.01f)
                {
                    spriteRenderer.flipX = false;
                    if (isGrounded) attackRequested = true; // 공격 모션
                }
                else if (dragVector.x < -0.01f)
                {
                    spriteRenderer.flipX = true;
                    if (isGrounded) attackRequested = true; // 공격 모션
                }
            }


            // 게임 시작되었다는 알림 주기
            if (OnGameStart != null)
            {
                OnGameStart.Invoke(); // 호출하기
                OnGameStart = null;   // 한 번만 발행되도록 초기화
            }


        }
#endif

        // 애니메이션 업데이트
        anim.SetFloat(moveSpeedHash, rb.velocity.magnitude);
        anim.SetBool(jumpHash, !isGrounded && rb.velocity.y > 0);
        anim.SetBool(fallHash, !isGrounded && rb.velocity.y < 0);
        anim.SetBool(attackHash, attackRequested);

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

        // 낙하
        if (fallRequested)
        {
            rb.AddForce(Vector2.down * fallForce, ForceMode2D.Impulse);
            fallRequested = false;
        }

        // 공격
        if (attackRequested && isGrounded)
        {
            // 공격 모션 중 타일 파괴는 충돌 시 처리
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground 레이어만 파괴
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                Vector3Int cellPos = tilemap.WorldToCell(hitPoint);

                // 한 칸의 타일만 부셔라.
                if (cellPos != lastDestroyedTile && !tileDestroyedThisAction)
                {
                    tilemap.SetTile(cellPos, null); // 타일 파괴하기
                    lastDestroyedTile = cellPos;    // 최근에 부순 타일 기록
                    tileDestroyedThisAction = true; // 이번 액션에서는 한 칸만 부수기
                }
            }

            // 착지 시 기본 상태로 복귀
            if (isGrounded)
            {
                attackRequested = false;
                anim.SetBool(attackHash, false);
            }

        }
    }

    // 센서를 그려라.
    private void OnDrawGizmosSelected()
    {
        // 바닥 감지 센서 (노란색 원)
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
