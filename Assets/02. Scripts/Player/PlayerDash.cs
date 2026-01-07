

using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("대시")]
    public float dashTime = 5.0f;
    public float maxDashTime = 7.0f;
    public bool isDash = false;
    public Vector2 tempoWay = Vector2.zero;
    public float moveSpeed = 8.0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // E 키를 눌렀을 때 바라보는 방향으로 대시 시작
        if (Input.GetKeyDown(KeyCode.E) && !isDash)
        {
            Vector2 dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            StartDash(dashDirection);
        }

        // 대시 중일 때 처리
        if (isDash)
        {
            dashTime += Time.deltaTime;

            if (tempoWay == Vector2.zero)
                tempoWay = Vector2.right;

            rb.velocity = tempoWay.normalized * moveSpeed * 5;

            if (dashTime >= maxDashTime)
            {
                dashTime = 0;
                isDash = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void StartDash(Vector2 direction)
    {
        tempoWay = direction;
        isDash = true;
        dashTime = 0f;
    }
}




