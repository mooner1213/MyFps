using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 인터랙티브한 오브젝트를 관리하는 클래스들의 부모 추상 클래스
    /// 인터랙티브의 공통 기능을 모아서 구현
    /// 가까이 가서 crosshair 캐스팅하면 액션 UI 보여준다
    /// 액션 키를 누르면 액션을 실행한다
    /// </summary>
    public abstract class Interactive : MonoBehaviour
    {
        //추상메서드 - 구현하도록 강제하는 기능 정의
        #region abstract
        protected abstract void DoAction();
        #endregion

        #region Variables
        [Header("Interative UI")]
        //UI 오브젝트
        public GameObject actionUI;
        public GameObject extraCross;
        public TextMeshProUGUI actionText;
        public string action = "action Text";       //인터랙티브 액션 내용

        [Header("Interative Input")]     //헤더 특성
        public InputActionReference interactAction;
        
        //오브젝트 캐스팅
        protected Collider castCollider;
        protected bool currentCasting = false;        //현재 캐스팅 상태
        protected bool wasCasting = false;            //이전 캐스팅 상태
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            //참조
            castCollider = GetComponent<Collider>();
        }

        protected virtual void Update()
        {
            //플레이어의 캐스팅 거리가 체크
            if (PlayerCasting.DistanceFromTarget > 2f)
            {
                HideActionUI();
                wasCasting = false;
                return;
            }

            // 이 오브젝트의 캐스팅한 오브젝트인 체크
            currentCasting = PlayerCasting.CastGameObject != null && PlayerCasting.CastGameObject == this.gameObject;

            // 상태 변화 감지: 경계
            if (currentCasting != wasCasting && currentCasting == true)
            {
                //캐스팅하고 있지 않다가 캐스팅을 시작할때
                ShowActionUI();
            }
            else if (currentCasting != wasCasting && currentCasting == false)
            {
                //캐스팅 하고 있다가 캐스팅을 놓치는것을 시작할때
                HideActionUI();
            }

            if (currentCasting && interactAction.action.WasPressedThisFrame())
            {                
                DoAction();
                HideActionUI();
            }

            //was 상태 저장
            wasCasting = currentCasting;
        }
        #endregion


        #region Custom Method        
        protected virtual void ShowActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(true);
                extraCross.SetActive(true);
                actionText.text = action;
            }
        }

        protected virtual void HideActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(false);
                extraCross.SetActive(false);
                actionText.text = "";
            }
        }
        #endregion
    }
}


/*
인터랙티브 기능
1. 게임오브젝트 제어 : 문 열기
2. 아이템 획득 : 권총 획득
public class PikcupItem : Interactive
public class PikcupPistol : PickupItem
*/