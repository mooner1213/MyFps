using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// Interactive를 상속받는다 
    /// 액션 : 권총 아이템 획득
    /// </summary>
    public class PikcupPistol : Interactive
    {
        #region Variables
        [Header("Action")]        
        public GameObject realPistol;
        public GameObject arrow;

        public GameObject ammoUI;
        #endregion

        #region Custom Method
        protected override void DoAction()
        {
            //- 오른손 쪽의 총은 화면 출력 -활성화
            //- Ammo UI 보여주기
            //- 책상위의 가이드 화살표는 없어진다            
            //-테이블 위의 총은 없어지고 - 비활성화
            //- 다시 캐스팅해도 트리거가 작동이 안되어야 한다

            realPistol.SetActive(true);
            ammoUI.SetActive(true);

            //arrow.SetActive(false);
            //this.gameObject.SetActive(false); //fakePistol
            //castCollider.enabled = false;
            
            Destroy(arrow);

            //콜라이더 기능 제거
            Destroy(this.gameObject);
        }
        #endregion
    }
}