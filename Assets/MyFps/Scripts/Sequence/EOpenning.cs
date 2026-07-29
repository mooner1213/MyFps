using TMPro;
using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 두번째 플레이씬 오프닝 시퀀스
    /// </summary>
    public class EOpenning : MonoBehaviour
    {
        #region Variables
        //참조
        public GameObject thePlayer;
        public SceneFader fader;        
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //게임 시작과 동시에 연출 시작
            StartCoroutine(SequencePlay());
        }
        #endregion

        #region Custom Method
        IEnumerator SequencePlay()
        {
            //0.플레이 캐릭터 비 활성화
            //1.페이드인 연출(페인드인 효과)            
            //4. 1초후에
            //5.플레이 캐릭터 활성화
            AudioManager.Instance.PlayBgm("Bgm01");

            thePlayer.SetActive(false);
            fader.FadeStart();

            yield return new WaitForSeconds(1f);

            thePlayer.SetActive(true);
        }
        #endregion
    }
}
