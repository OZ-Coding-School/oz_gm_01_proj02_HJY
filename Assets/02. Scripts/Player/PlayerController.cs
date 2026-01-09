using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 플레이어가 움직이면 메뉴 패널은 꺼져야 함
/// 플레이어 이동은 키로 할 것인가 조이콘으로 할 것인가
/// 점프 기능을 추가할 거면 카메라가 플레이어를 따라서 위로도 올라가게 할 것
/// 점프로 타일을 부술 것인가 아니면 어택으로 타일을 부술 것인가
/// 적들은 어택으로 없애고 아이템 상자는 접촉하면 아이템으로 점수가 모아지고 아이템 상자는 사라질 것
/// 맵이 작은데 플레이어에게 달리기 기능이 필요한가
/// 하나의 그리드 안에서 벽이랑 그라운드로 타일맵을 나누고 레이어도 나눔
/// 그라운드는 플레이어가 바닥으로 인식을 해야하는 동시에 부술 수 있어야 함
/// 벽은 어택을 해도 안 부숴져야하는데 동시에 플레이어가 벽 밖으로 못 나가게 막아야 함
/// </summary>


public class PlayerController : MonoBehaviour
{
    [Header("이동/점프")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 7.0f;


    [Header("바닥체크")]
    public Transform groundCheck;   //발밑 위치를 나타내는 트랜스폼
    [SerializeField] private float groundCheckRadius = 0.15f; // 감지용 반지름
    [SerializeField] private LayerMask groundLayer;//레이어 설정


    //내부에서 참조할 컴포넌트들
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;


    //입력용
    private float inputX;               //입력용
    private bool isGrounded;            //바닥임?
    private bool jumpRequested;         //점프함?


    private static readonly int moveSpeedHash = Animator.StringToHash("Speed");
    private static readonly int jumpHash = Animator.StringToHash("IsJumping");
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        //방향에 따라 좌우 반전
        if (inputX != 0)//입력값이 0이 아니라면
        {
            if (inputX < 0)//왼쪽
            {
                spriteRenderer.flipX = true;
            }
            else //오른쪽
            {
                spriteRenderer.flipX = false;
            }
        }
        //점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }
        // anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat(moveSpeedHash, Mathf.Abs(rb.velocity.x));
    }

    private void FixedUpdate()
    {
        // 바닥 감지
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 대시 중이면 이동/점프 무시
        if (GetComponent<PlayerDash>().isDash)
            return;

        // 이동
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        // 점프
        if (jumpRequested && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetBool(jumpHash, true);
        }
        jumpRequested = false;

        // 점프 종료
        if (isGrounded && rb.velocity.y <= 0.05f)
        {
            anim.SetBool(jumpHash, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
