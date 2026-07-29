using MyFps;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// Nav Agent를 관리하는 클래스 예제
    /// 마우스로 맵을 클릭하면 클릭한 지점으로 Agent가 이동한다
    /// </summary>
    public class AgentController : MonoBehaviour
    {
        #region Variables
        //참조
        private NavMeshAgent m_Agent;

        //입력 처리
        public InputActionReference clickAction;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            m_Agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            clickAction.action.Enable();
        }

        private void OnDisable()
        {
            clickAction.action.Disable();
        }

        private void Update()
        {
            //마우스로 좌클릭하면 좌클릭한 월드포지션 가져오기
            //가져온 월드포지션을 m_Agent의 이동 목표 지점으로 설정한다
            //입력처리 및 탄환 체크
            if (clickAction.action.WasPressedThisFrame())
            {
                Vector3 worldPos = ScreenToRay();
                m_Agent.SetDestination(worldPos);
            }
        }
        #endregion

        #region Custom Method        
        private Vector3 ScreenToRay()
        {
            Vector3 worldPosition = Vector3.zero;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mousePosition = new Vector3(mousePos.x, mousePos.y, 0f);
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                worldPosition = hit.point;
            }

            return worldPosition;
        }
        #endregion
    }
}