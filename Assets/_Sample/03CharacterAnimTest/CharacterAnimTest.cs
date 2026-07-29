using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 캐릭터 애니메이션을 제어하는 예제 클래스
    /// 뉴 인풋시스템
    /// 기본이 대기 상태
    /// W키가 들어오면 걷기 상태
    /// + Shift 키를 누르면 뛰기 상태
    /// </summary>
    public class CharacterAnimTest : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        [SerializeField] private bool isMove;
        [SerializeField] private bool isRun;

        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float moveSpeed = 0f;

        [SerializeField] private float accelerationSpeed = 0.1f;  //가속도

        //애니 파라미터 스트링
        private string isMoving = "IsMove";
        private string isRunning = "IsRun";
        private string velocity = "Velocity";

        //인풋 액션
        public InputActionReference moveAction;
        public InputActionReference sprintAction;
        #endregion

        #region Property
        public bool IsMove
        {
            get { return isMove; }
            private set
            {
                isMove = value;
                animator.SetBool(isMoving, value);
            }
        }

        public bool IsRun
        {
            get { return isRun; }
            private set
            {
                isRun = value;
                animator.SetBool(isRunning, value);
            }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
            private set
            {
                moveSpeed = value;
                animator.SetFloat(velocity, value);
            }
        }
        #endregion

        #region Unity Event Method 
        //참조
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            //인풋 액션 활성화
            moveAction.action.Enable();
            sprintAction.action.Enable();
        }

        private void OnDisable()
        {
            //인풋 액션 비활성화
            moveAction.action.Disable(); 
            sprintAction.action.Disable();
        }

        private void Update()
        {
            //인풋 처리
            Vector2 inputMove = moveAction.action.ReadValue<Vector2>();
            IsMove = inputMove != Vector2.zero;

            if(sprintAction.action.WasPressedThisFrame())
            {
                IsRun = true;
            }
            else if (sprintAction.action.WasReleasedThisFrame())
            {
                IsRun = false;
            }

            //이동 속도 처리
            if (IsMove && !IsRun)
            {
                if (MoveSpeed > walkSpeed)
                {
                    MoveSpeed -= accelerationSpeed;
                    if (MoveSpeed <= walkSpeed)
                    {
                        MoveSpeed = walkSpeed;
                    }
                }
                else
                {
                    MoveSpeed += accelerationSpeed;
                    if (MoveSpeed >= walkSpeed)
                    {
                        MoveSpeed = walkSpeed;
                    }
                }
            }
            else if (IsMove && IsRun) //뛰기
            {
                MoveSpeed += accelerationSpeed;
                if (MoveSpeed >= runSpeed)
                {
                    MoveSpeed = runSpeed;
                }
            }
            else
            {
                if (MoveSpeed > 0f)
                {
                    MoveSpeed -= accelerationSpeed;
                    if (MoveSpeed <= 0f)
                    {
                        MoveSpeed = 0f;
                    }
                }
            }
        }
        #endregion
    }
}