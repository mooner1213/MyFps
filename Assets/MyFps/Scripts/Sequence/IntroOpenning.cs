using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 인트로 씬을 관리하는 클래스
    /// 인트로 연출, ...
    /// </summary>
    public class IntroOpenning : MonoBehaviour
    {
        #region Variables
        //참조
        public SceneFader fader;
        [SerializeField] private string loadToScene = "PlayScene01";
        [SerializeField] private int loadToSceneNumber = 2;

        //인트로 강제로 끝내기
        public InputActionReference escapeAction;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //페이드 인
            fader.FadeStart();

        }

        private void Update()
        {
            //Esc 키를 누르면 강제로 플레이씬 가기
            if(escapeAction.action.WasPressedThisFrame())
            {
                Exit();
            }
        }
        #endregion


        #region Custom Method
        //오픈닝 시퀀스

        //
        public void Exit()
        {
            StartCoroutine(ExitSequence());
        }

        IEnumerator ExitSequence()
        {
            //게임 데이터 저장 - 다음 씬번호 저장
            PlayerStats.Instance.SceneNumber = loadToSceneNumber;
            SaveLoad.SaveData();

            yield return new WaitForSeconds(1f);
            fader.FadeTo(loadToSceneNumber);
        }
        #endregion
    }
}