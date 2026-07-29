using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 애니메이터의 블랜드 트리 테스트 예제 클래스
    /// </summary>
    public class BlendTreeTest : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        //이동
        [SerializeField] private float moveSpeed = 5f;

        //인풋 액션
        public InputActionReference moveAction;

        private Vector2 inputMove;

        //애니메이션
        private string moveState = "MoveState";
        private string moveX = "MoveX";
        private string moveY = "MoveY";
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
        }

        private void OnDisable()
        {
            //인풋 액션 비활성화
            moveAction.action.Disable();
        }

        private void Update()
        {
            //인풋 처리
            inputMove = moveAction.action.ReadValue<Vector2>();

            //애니메이터 처리
            //AnimationStateTest(inputMove);
            AnimationBlendTreeTest(inputMove);


            //캐릭터 이동 : 방향 * Time.delatime * moveSpeed;
            Vector3 dir = new Vector3(inputMove.x, 0f, inputMove.y);
            transform.Translate(dir * Time.deltaTime * moveSpeed, Space.World);
        }
        #endregion

        #region Custom Method
        void AnimationBlendTreeTest(Vector2 moveDir)
        {
            animator.SetFloat(moveX, moveDir.x);
            animator.SetFloat(moveY, moveDir.y);
        }

        void AnimationStateTest(Vector2 moveDir)
        {
            if(moveDir == Vector2.zero)
            {
                animator.SetInteger(moveState, 0);  //대기 상태
            }
            else
            {
                if(moveDir.y > 0f)
                {
                    animator.SetInteger(moveState, 1);  //앞으로 이동
                }

                if (moveDir.y < 0f)
                {
                    animator.SetInteger(moveState, 2);  //뒤로 이동
                }

                if (moveDir.x < 0f)
                {
                    animator.SetInteger(moveState, 3);  //좌로 이동
                }

                if (moveDir.x > 0f)
                {
                    animator.SetInteger(moveState, 4);  //우로 이동
                }
            }
        }
    }
    #endregion
}
