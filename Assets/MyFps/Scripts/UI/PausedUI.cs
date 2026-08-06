using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 게임중 메뉴를 관리하는 클래스
    /// </summary>
    public class PausedUI : MonoBehaviour
    {
        #region Variables
        //UI 오브젝트
        public GameObject pausedUI;

        //입력 처리
        public InputActionReference pausedAction;

        //메인메뉴 가기
        public SceneFader fader;
        [SerializeField] private string loadToScene = "MainMenu";
        #endregion

        #region Unity Event Method
        private void Update()
        {
            //인풋
            if(pausedAction.action.WasPressedThisFrame())
            {
                Toggle();
            }
        }
        #endregion

        #region Custsom Method
        void Toggle()
        {
            pausedUI.SetActive(!pausedUI.activeSelf);

            //창이 open
            if(pausedUI.activeSelf)
            {
                Time.timeScale = 0f;

                //마우스 커서
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else //창이 close
            {
                Time.timeScale = 1f;

                //마우스 커서
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void Continue()
        {
            Toggle();
        }

        public void MainMenu()
        {
            Time.timeScale = 1f;
            fader.FadeTo(loadToScene);
        }
        #endregion

    }
}