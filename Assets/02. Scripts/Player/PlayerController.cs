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
/// 아이템을 상자 이미지로 넣을지 아니면 타일을 부수는 걸로 할지 타일을 부수면 랜덤으로 아이템으로 인식할지 정하기
/// 아이템을 얻은 걸 상단에다 상시 표시할지 아니면 게임 오버할 때만 보이게 할지 고민하기
/// 어택으로 적과 타일을 부수는데 플레이어의 위나 아래는 점프로 없애고 어택으로는 좌우의 타일을 없애게 하기
/// 플레이어가 점프를 터치 두번으로 하고 드래그로 좌우로 움직이게 하기
/// 플레이어가 점프하면 닿은 위쪽 타일 한 칸이 부서지게 만들기
/// 플레이어를 당길 때 적이 감지되면 공격 모션이 시작되고 적이 공격을 맞고 사라지게 만들기
/// 플레이어의 공격 방법을 정하든가 적 감지시 공격 모션을 취하게 하기
/// 아이템은 플레이어랑 접촉하면 얻어지지만 게임에서는 사라져야 함
/// 아이템이 랜덤으로 나오게 할 것인지 아니면 맵에다 스프라이트로 찍을 것인지 고민하기
/// 드림 씬 대사를 스크립터블 오브젝트로 할지 JSON으로 할지 Ink로 할지 정하기
/// 플레이어를 아래로 당기면 공격 모션이 취해지게 해서 아래 타일이 부서지게 만들기
/// 부수려는 타일을 두 번 터치하면 공격 모션을 취하고 타일이 부서지게 할지 방법 정하기
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
