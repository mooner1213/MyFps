using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 플레이어 인풋을 관리하는 클래스 : 뉴인풋
    /// </summary>
    public class CharacterInput : MonoBehaviour
    {
        #region Variables
        //inputSystem class 인스턴스 선언
        private InputSystem_Actions inputActions;

        //이동 입력 값 - wasd
        private Vector2 move;
        [SerializeField] private bool isSprint;

        //마우스 입력값 - 마우스 위치 
        private Vector2 look;

        //점프
        [SerializeField] private bool isJump;
        #endregion

        #region Property
        public Vector2 Move
        {
            get { return move; }
            private set { move = value; }
        }

        public bool IsSprint
        {
            get { return isSprint; }
            private set { isSprint = value; }
        }

        public Vector2 Look
        {
            get { return look; }
            private set { look = value; }
        }

        public bool IsJump
        {
            get { return isJump; }
            set { isJump = value; }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            //inputSystem class 인스턴스 생성
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            //inputSystem class 인스턴스 활성화
            inputActions.Enable();
        }

        private void OnDisable()
        {
            //inputSystem class 인스턴스 비활성화
            inputActions.Disable();

        }

        private void Update()
        {
            //value 입력값 처리 : 인스턴스이름.액션맵이름.액션이름.ReadValue
            Move = inputActions.Player.Move.ReadValue<Vector2>();
            Look = inputActions.Player.Look.ReadValue<Vector2>();

            //버튼 입력값 처리
            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                IsJump = true;
            }
            if(inputActions.Player.Sprint.WasPressedThisFrame())
            {
                IsSprint = true;
            }
            else if (inputActions.Player.Sprint.WasReleasedThisFrame())
            {
                IsSprint = false;
            }
        }
        #endregion

    }
}