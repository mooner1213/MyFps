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

        public AudioSource line01;
        public AudioSource line02;
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
            //1.페이드인 연출(3초 대기후 페인드인 효과)
            //2.화면 하단에 시나리오 텍스트 화면 출력 및 음성 재생(2초)
            //(...Where am I?)
            //3.화면 하단에 시나리오 텍스트 화면 출력 및 음성 재생(2초)
            //(I need get out of here)
            //4. 2초후에 시나리오 텍스트 없어진다
            //5.플레이 캐릭터 활성화
            Debug.Log("SequencePlay");
            AudioManager.Instance.PlayBgm("Bgm01");

            thePlayer.SetActive(false);
            fader.FadeStart(3f);

            sequenceText.gameObject.SetActive(true);
            sequenceText.text = "...Where am I?";
            line01.Play();
            yield return new WaitForSeconds(2f);

            sequenceText.text = "I need get out of here";
            line02.Play();
            yield return new WaitForSeconds(2f); 
            
            sequenceText.gameObject.SetActive(false);
            sequenceText.text = "";

            thePlayer.SetActive(true);
        }
        #endregion
    }
}