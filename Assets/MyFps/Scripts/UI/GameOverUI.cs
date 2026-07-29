using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    public class GameOverUI : MonoBehaviour
    {
        #region Variables
        //참조
        public SceneFader fader;
        private string loadToScene = "MainMenu";
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            //마우스 커서
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endregion

        #region Custom Method
        public void Retry()
        {
            fader.FadeTo(SceneManager.GetActiveScene().name);
        }

        public void MainMenu()
        {
            //Debug.Log("Goto MainMenu");
            fader.FadeTo(loadToScene);
        }
        #endregion
    }
}