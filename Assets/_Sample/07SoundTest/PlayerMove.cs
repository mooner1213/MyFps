using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 시작하면 앞쪽으로 이동한다
    /// 좌우입력 받아 좌우 이동한다
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody rb;

        //앞으로 이동하는 힘
        [SerializeField] private float fowardForce = 5f;
        [SerializeField] private float sideForce = 5f;

        //인풋 처리
        //입력 처리
        public InputActionReference moveAction;

        private Vector2 move;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            //좌우 인풋 처리
            move = moveAction.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            //앞으로 이동
            rb.AddForce(0f, 0f, fowardForce, ForceMode.Acceleration);

            //좌우 이동 연산
            if(move.x < 0f)
            {
                rb.AddForce(-sideForce, 0f, 0f, ForceMode.Acceleration);
            }
            else if (move.x > 0f)
            {
                rb.AddForce(sideForce, 0f, 0f, ForceMode.Acceleration);
            }

        }
        #endregion
    }
}

/*
Rigidbody

이동 방법:
1. 이동 시킬려고 하는 방향으로 힘을 준다
2. rb.linearVelocity의 값을 직접 조정하여 이동 한다

힘의 종류
ForceMode.Force : 연속적인 힘, 무게(o), 자동차 드라이브
ForceMode.Acceleration : 연속적인 힘, 무게(x), 바람
ForceMode.Impulse : 일회성 힘, 무게(o), 점프
ForceMode.VelocityChange : 일회성 힘, 무게(x), 플레이어 스탭

*/