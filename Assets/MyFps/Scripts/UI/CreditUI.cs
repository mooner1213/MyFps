using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 크레딧 UI 감추기, 메인메뉴 보이기 기능
    /// </summary>
    public class CreditUI : MonoBehaviour
    {
        #region Variables
        //UI 오브젝트
        public GameObject mainMenu;

        //크렛딧 강제로 끝내기
        public InputActionReference escapeAction;
        #endregion

        #region Unity Event Method
        private void Update()
        {
            //Esc 키를 누르면 강제로 플레이씬 가기
            if (escapeAction.action.WasPressedThisFrame())
            {
                HideCredit();
            }
        }
        #endregion

        #region Custom Method
        private void HideCredit()
        {   
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }
        #endregion
    }
}