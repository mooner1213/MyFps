using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 애니메이터 레이터 테스트 예제
    /// </summary>
    public class AnimatorLayerTest : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        [SerializeField] private bool isMove;

        private string isMoving = "IsMove";

        //인풋 액션
        public InputActionReference moveAction;     //이동
        public InputActionReference sprintAction;   //조준
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
        #endregion

        #region Unity Event Method
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

            if (sprintAction.action.WasPressedThisFrame())
            {
                animator.SetLayerWeight(1, 1f);
            }
            else if (sprintAction.action.WasReleasedThisFrame())
            {
                animator.SetLayerWeight(1, 0f);
            }
        }
        #endregion
    }
}