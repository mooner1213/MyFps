using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 캐릭터 애니메이션을 제어하는 예제 클래스
    /// 뉴 인풋 시스템
    /// 기본이 대기 상태
    /// w키가 들어오면  걷기상태
    /// + shift키를 누르면 뛰기상태
    /// </summary>
    public class CharaterAnimTest : MonoBehaviour
    {
        #region Variables
        // 참조
        private Animator animator;

        [SerializeField] private bool isMove;
        [SerializeField] private bool isRun;

        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float moveSpeed = 0f;

        [SerializeField] private float accelerationSpeed = 0.1f; // 가속도

        private string isMoving = "isMove";     // 애니메이터 파라미터명 소문자로 수정
        private string isRunning = "isRun";     // 애니메이터 파라미터명 소문자로 수정
        private string velocity = "Velocity";

        // 인풋 액션
        public InputActionReference moveAction;
        public InputActionReference sprintAction;
        #endregion

        #region Property
        public bool IsMove
        {
            get
            {
                return isMove;
            }
            private set 
            { 
                isMove = value;                  // StackOverflow 방지를 위해 필드(isMove)에 대입
                animator.SetBool(isMoving, value);
            }
        }

        public bool IsRun
        {
            get
            {
                return isRun;                    // StackOverflow 방지를 위해 필드(isRun) 반환
            }
            private set 
            { 
                isRun = value;                   // StackOverflow 방지를 위해 필드(isRun)에 대입
                animator.SetBool(isRunning, value);
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            private set
            {
                moveSpeed = value;
                animator.SetFloat(velocity, value);
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            // 인풋액션 활성화
            if (moveAction != null && moveAction.action != null)
            {
                moveAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            // 인풋액션 비활성화
            if (moveAction != null && moveAction.action != null)
            {
                moveAction.action.Disable();
            }
        }

        private void Update()
        {
            // 인풋처리
            if (moveAction == null || moveAction.action == null) return;

            // 이동 입력 값 읽기 (Vector2)
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

            // 이동 입력이 있는지 여부 판단
            IsMove = moveInput != Vector2.zero;

            // Shift 키를 누르고 있고 이동 중일 때 달리기 상태로 전환
            if (IsMove)
            {
                IsRun = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
            }
            else
            {
                IsRun = false;
            }

            // 이동 속도 처리
            if (IsMove && !IsRun) // 1. 걷기 상태
            {
                if (moveSpeed > walkSpeed)
                {
                    moveSpeed -= accelerationSpeed;
                    if (moveSpeed <= walkSpeed)
                    {
                        moveSpeed = walkSpeed;
                    }
                }
                else if (moveSpeed < walkSpeed)
                {
                    moveSpeed += accelerationSpeed;
                    if (moveSpeed >= walkSpeed)
                    {
                        moveSpeed = walkSpeed;
                    }
                }
            }
            else if (IsMove && IsRun) // 2. 뛰기 상태
            {
                if (moveSpeed < runSpeed)
                {
                    moveSpeed += accelerationSpeed;
                    if (moveSpeed >= runSpeed)
                    {
                        moveSpeed = runSpeed;
                    }
                }
            }
            else // 3. 정지 상태
            {
                if (moveSpeed > 0f)
                {
                    moveSpeed -= accelerationSpeed;
                    if (moveSpeed <= 0f)
                    {
                        moveSpeed = 0f;
                    }
                }
            }

            // 애니메이터에 연산된 속도 값 업데이트
            MoveSpeed = moveSpeed;
        #endregion
    }
}
}