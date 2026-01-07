

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // W 키를 눌렀을 때 공격 애니메이션 실행
        if (Input.GetKeyDown(KeyCode.W) && !isAttacking)
        {
            animator.SetBool("Attack", true);
            isAttacking = true;
        }



        // 공격 애니메이션이 끝났는지 확인
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                animator.SetBool("Attack", false); // 애니메이션 종료 시 false로
                isAttacking = false;
            }
        }


    }



}




