using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// 플레이어 인풋을 관리하는 클래스
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        //inputSystem class 인스턴스 선언
        private InputSystem_Actions inputActions;

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

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

        void Start()
        {   
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector2 move2 = inputActions.Player.Move.ReadValue<Vector2>();
                Vector3 move = new Vector3(move2.x, 0f, move2.y);

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Jump.WasPressedThisFrame();
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Jump.IsPressed();
            }

            return false;
        }

        float GetMouseLookAxis(string mouseInputName)
        {
            if (CanProcessInput())
            {
                float i = 0;

                if (mouseInputName == GameConstants.k_MouseAxisNameVertical)
                {
                    i = inputActions.Player.Look.ReadValue<Vector2>().y;
                }
                else
                {
                    i = inputActions.Player.Look.ReadValue<Vector2>().x;
                }

                // handle inverting vertical input
                if (InvertYAxis && mouseInputName == GameConstants.k_MouseAxisNameVertical)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;

                // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;

                return i;
            }

            return 0f;
        }

        public bool GetCrouchInputDown()
        {            
            if (CanProcessInput())
            {                
                return inputActions.Player.Crouch.WasPressedThisFrame();
            }

            return false;
        }

        public bool GetCrouchInputReleased()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Crouch.WasReleasedThisFrame();
            }

            return false;
        }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Sprint.IsPressed();
            }

            return false;
        }

        //무기 교체 인풋 처리
        public int GetSwitchWeaponInput()
        {
            if (CanProcessInput())
            {
                if(inputActions.Player.WeaponSwitch.ReadValue<Vector2>().y > 0f)
                {
                    return -1;
                }
                if (inputActions.Player.WeaponSwitch.ReadValue<Vector2>().y < 0f)
                {
                    return 1;
                }
            }

            return 0;
        }

        //조준 입력 처리
        public bool GetAimInputHeld()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Aim.IsPressed();
            }

            return false;
        }
        
        //발사 버튼 입력처리
        public bool GetFireInputDown()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Fire.WasPressedThisFrame();
            }

            return false;
        }

        public bool GetFireInputReleased()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Fire.WasReleasedThisFrame();
            }

            return false;
        }

        public bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                return inputActions.Player.Fire.IsPressed();
            }

            return false;
        }

        // 수동 재장전(R키) 입력 처리
        public bool GetReloadInputDown()
        {
            if (CanProcessInput())
            {
                return UnityEngine.Input.GetKeyDown(KeyCode.R);
            }

            return false;
        }

    }
}