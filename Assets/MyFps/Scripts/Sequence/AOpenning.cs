using UnityEngine;
using TMPro;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 첫번째 플레이씬 오프닝 연출
    /// </summary>
    public class AOpenning : MonoBehaviour
    {
        #region Variables
        //참조
        public GameObject thePlayer;
        public SceneFader fader;
        public TextMeshProUGUI sequenceText;

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
            //1.페이드인 연출(1초 대기후 페인드인 효과)
            //2.화면 하단에 시나리오 텍스트 화면 출력(3초)
            //(I need get out of here)
            //3. 3초후에 시나리오 텍스트 없어진다
            //4.플레이 캐릭터 활성화

            thePlayer.SetActive(false);
            fader.FadeStart(1f);

            sequenceText.gameObject.SetActive(true);
            sequenceText.text = "I need get out of here";

            yield return new WaitForSeconds(3f);                        
            sequenceText.gameObject.SetActive(false);
            sequenceText.text = "";

            thePlayer.SetActive(true);
        }
        #endregion
    }
}