using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어의 이동을 관리하는 클래스
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        //참조
        private CharacterController controller;
        private CharacterInput input;

        //이동
        [Header("Move")]     //헤더 특성 : 직열화된 속성중에 Player와 관련된 내용이다 표시
        [SerializeField] private float walkSpeed = 4f;      //걷는 속도
        [SerializeField] private float sprintSpeed = 7f;    //뛰는 속도
        private float moveSpeed;                            //이동 속도

        //그라운드 체크
        [Header("Ground Check")]
        [SerializeField] private bool isGrounded = false;

        [SerializeField] private float groundedOffset = -0.14f;     //체크 지점 조정값
        [SerializeField] private float groundedRadius = 0.5f;       //체크 범위 영역
        public LayerMask groundLayers;                              //그라운드 레이어 체크 

        //점프
        [Header("Jump")]
        [SerializeField] private float gravity = -9.81f;            //중력
        [SerializeField] private float verticalVelocity = 0;        //y축의 속도 값

        [SerializeField] private float jumpHeight = 1.2f;           //점프 높이
        [SerializeField] private float jumpTimeout = 0.1f;          //점프 키입력 처리 타이머

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            controller = GetComponent<CharacterController>();
            input = GetComponent<CharacterInput>();
        }

        private void Update()
        {
            //그라운드 체크
            CheckGrounded();

            //중력 및 점프처리
            GravityAndJump();

            //이동
            Move();
        }
        #endregion

        #region Custom Method
        void CheckGrounded()
        {
            //캐릭터컨트롤러에서 체크값 가져오기
            //isGrounded = controller.isGrounded;

            //체크 위치 설정
            Vector3 checkPosition = new Vector3(transform.position.x,
                transform.position.y - groundedOffset, transform.position.z);
            //체크 지점에서 그라운드 레이어가 있는지 체크
            isGrounded = Physics.CheckSphere(checkPosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        }

        void GravityAndJump()
        {
            if(isGrounded)
            {
                //지면에 있을때 verticalVelocity 값을 고정 시킨다
                if(verticalVelocity < 0f)
                {
                    verticalVelocity = -2f;
                }

                //점프 입력 체크 
                if (input.IsJump && jumpTimeout <= 0f)
                {
                    //점프 높이 만큼 속도를 지정한다
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
                }

                if(jumpTimeout >= 0f)
                {
                    jumpTimeout -= Time.deltaTime;
                }
            }
            else
            {
                input.IsJump = false;
                jumpTimeout = 0.1f;
            }

            //중력 처리
            verticalVelocity += gravity * Time.deltaTime;
        }

        void Move()
        {
            moveSpeed = input.IsSprint ? sprintSpeed : walkSpeed;

            //이동 인풋 체크
            if (input.Move == Vector2.zero)
                moveSpeed = 0f;

            //인풋에서 방향값 얻어오기
            Vector3 inputDirection = Vector3.zero;

            //플레이어의 로컬 방향 구하기
            if (input.Move != Vector2.zero)
            {
                inputDirection = transform.right * input.Move.x + transform.forward * input.Move.y;
            }

            //이동 : 방향(앞뒤좌우) * Time.deltatime * speed + (위아래) * Time.deltatime * verticalVelocity
            controller.Move(inputDirection * Time.deltaTime * moveSpeed
                + Vector3.up * Time.deltaTime * verticalVelocity);
        }
        #endregion

    }
}