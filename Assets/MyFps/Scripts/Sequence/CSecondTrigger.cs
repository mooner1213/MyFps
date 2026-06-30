using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 두번째 문앞 시퀀스 트리거
    /// 문이 열리고 문 뒤에 있는 적이 활성화 된다
    /// </summary>
    public class CSecondTrigger : MonoBehaviour
    {
        #region Variables
        //연출
        public Animator twoDoorAnimator;
        public GameObject robot;

        private string isOpen = "IsOpen";
        #endregion

        #region Unity Event Method
        private void OnTriggerEnter(Collider other)
        {
            //플레이어 체크
            if (other.gameObject.tag != "Player")
                return;

            Debug.Log($"OnTriggerEnter: {other.gameObject.name}");
            StartCoroutine(SequencePlay(other.gameObject));

            //트리거 제거
            transform.GetComponent<BoxCollider>().enabled = false;
        }
        #endregion

        #region Custom Method
        //연출 내용
        IEnumerator SequencePlay(GameObject player)
        {
            //-플레이 캐릭터 비활성화(플레이 멈춤)
            //문 열기
            //적 활성화
            //이번 프레임 딜레이
            //-플레이 캐릭터 활성화(다시 플레이)

            player.SetActive(false);

            twoDoorAnimator.SetBool(isOpen, true);
            robot.SetActive(true);

            yield return null; //이번 프레임에만 지연, 다음 프레임에서 진행
            player.SetActive(true);
        }
        #endregion
    }
}