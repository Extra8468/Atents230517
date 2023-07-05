using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 이동 속도
    /// </summary>
    public float speed = 3.0f;

    /// <summary>
    /// 현재 입력받은 이동 방향
    /// </summary>
    Vector2 inputDir = Vector2.zero;

    /// <summary>
    /// 공격을 시작했을 때 백업 해놓은 이동 방향
    /// </summary>
    Vector2 oldInputDir = Vector2.zero;

    /// <summary>
    /// 현재 이동중인지 표시하는 변수(공격이 끝났을 때 여전히 이동키를 누르고 있었을 때만 복구를 위해)
    /// </summary>
    bool isMove = false;

    /// <summary>
    /// 인풋 액션
    /// </summary>
    PlayerInputActions inputActions;

    // 컴포넌트들    
    Animator animator;
    Rigidbody2D rigid;

    // 애니메이터 파라메터 해쉬들
    readonly int InputX_Hash = Animator.StringToHash("InputX");
    readonly int InputY_Hash = Animator.StringToHash("InputY");
    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int Attack_Hash = Animator.StringToHash("Attack");

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        // 이동 처리
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * speed * inputDir);
    }

    /// <summary>
    /// Move 액션을 발동시켰을 때 실행되는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();   // 입력 받고

        inputDir = input;                               // 방향 기록하고
        animator.SetFloat(InputX_Hash, inputDir.x);     // 애니메이터 파라메터 변경(블랜드 트리 조정)
        animator.SetFloat(InputY_Hash, inputDir.y);

        isMove = true;                                  // 이동 중이라고 표시
        animator.SetBool(IsMove_Hash, true);            // 이동 애니메이션 시작
    }

    /// <summary>
    /// Move 액션 발동이 끝났을 때 실행되는 함수
    /// </summary>
    /// <param name="_"></param>
    private void OnStop(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        inputDir = Vector2.zero;                // 방향 초기화

        isMove = false;                         // 이동 끝났다고 표시
        animator.SetBool(IsMove_Hash, false);   // Idle 애니메이션으로 돌리기
    }

    /// <summary>
    /// Attack 액션을 발동 시켰을 때 실행되는 함수
    /// </summary>
    /// <param name="_"></param>
    private void OnAttack(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        animator.SetTrigger(Attack_Hash);   // 공격 애니메이션 시작
        oldInputDir = inputDir;             // 이동 방향을 백업
        inputDir = Vector2.zero;            // 이동 방향 zero로 설정
    }

    /// <summary>
    /// 이동 방향을 복구 시키는 함수(PlayerAttackBlendTree에서 상태가 끝날 때 실행)
    /// </summary>
    public void RestorInputDir()
    {
        if( isMove )                    // 여전히 이동 키를 누르고 있을 때만
        {
            inputDir = oldInputDir;     // 이동 방향 복구
        }
    }
}