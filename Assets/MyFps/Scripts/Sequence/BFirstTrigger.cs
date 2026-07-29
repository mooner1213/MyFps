using UnityEngine;
using System.Collections;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 첫번째 복도끝 시퀀스 트리거 구현
    /// 시퀀스 내용 : 무기발견 연출 구현
    /// </summary>
    public class BFirstTrigger : SequenceTrigger
    {
        #region Variables
        //연출        
        public TextMeshProUGUI sequenceText;
        public GameObject arrow;

        public AudioSource line03;
        #endregion

        #region Custom Method
        protected override IEnumerator SequencePlay(GameObject player)
        {
            //-플레이 캐릭터 비활성화(플레이 멈춤)
            //-대사 출력 및 음성 재생: "Looks like a weapon on that table."
            //- 1초 딜레이
            //-화살표 활성화
            //- 1초 딜레이
            //-플레이 캐릭터 활성화(다시 플레이)            

            player.SetActive(false);

            sequenceText.gameObject.SetActive(true);
            sequenceText.text = "Looks like a weapon on that table";
            line03.Play();
            yield return new WaitForSeconds(1f);

            arrow.SetActive(true);
            yield return new WaitForSeconds(1f);
            sequenceText.gameObject.SetActive(false);
            sequenceText.text = "";

            player.SetActive(true);
        }
        #endregion

    }
}