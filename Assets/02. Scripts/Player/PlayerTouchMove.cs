
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

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool jumpRequested;

    private readonly int moveSpeedHash = Animator.StringToHash("Speed");
    private readonly int jumpHash = Animator.StringToHash("IsJumping");
    private readonly int fallHash = Animator.StringToHash("IsFalling");

    private Vector2 dragStartPos;
    private Vector3Int lastDestroyedTile; // 최근 파괴된 타일 좌표
    private bool tileDestroyedThisJump;   // 점프 중 한 번만 파괴

    public static System.Action OnGameStart; // 게임 시작 알림 이벤트 선언



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

        // 드래그 중 → 이동/점프 처리
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
                tileDestroyedThisJump = false; // 점프 시작 시 초기화
            }
            else
            {
                // 좌우 드래그 → 이동
                rb.velocity = new Vector2(dragVector.x * moveSpeed, rb.velocity.y);

                if (dragVector.x > 0.01f) spriteRenderer.flipX = false;
                else if (dragVector.x < -0.01f) spriteRenderer.flipX = true;
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
        anim.SetBool(jumpHash, !isGrounded);
        anim.SetBool(fallHash, !isGrounded && rb.velocity.y < 0);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground 레이어만 파괴
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!isGrounded && !tileDestroyedThisJump)
            {
                Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    Vector3 hitPoint = collision.contacts[0].point;
                    Vector3Int cellPos = tilemap.WorldToCell(hitPoint);

                    // 같은 점프에서 두 번 연속 파괴 방지
                    if (cellPos != lastDestroyedTile)
                    {
                        tilemap.SetTile(cellPos, null);
                        lastDestroyedTile = cellPos;
                        tileDestroyedThisJump = true;
                    }
                }
            }
        }
    }

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