using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyFps
{
    /// <summary>
    /// 마지막 문 앞 시퀀스 트리거 구현
    /// 시퀀스 내용 : 첫번째 씬 나가지 구현
    /// </summary>
    public class DExitTrigger : SequenceTrigger
    {
        #region Variables
        //참조
        public SceneFader fader;
        [SerializeField] private string loadToScene = "PlayScene02";

        public Animator twoDoorAnimator;
        public AudioSource jumpScare;

        private string isOpen = "IsOpen";
        #endregion

        #region Custom Method
        protected override IEnumerator SequencePlay(GameObject player)
        {
            //- 문열기
            //- 배경음 Stop
            //- 다음씬(PlayScene02)으로 이동

            twoDoorAnimator.SetBool(isOpen, true);
            AudioManager.Instance.StopBgm();

            //씬 클리어 처리...


            yield return new WaitForSeconds(1f);
            fader.FadeTo(loadToScene);

        }
        #endregion
    }
}