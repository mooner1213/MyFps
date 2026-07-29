using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 두번째 문앞 시퀀스 트리거
    /// 문이 열리고 문 뒤에 있는 적이 활성화 된다
    /// </summary>
    public class CSecondTrigger : SequenceTrigger
    {
        #region Variables
        //연출
        public Animator twoDoorAnimator;
        public GameObject robot;

        public AudioSource doorBang;
        //public AudioSource jumpScare;
        //public AudioSource bgm01;

        private string isOpen = "IsOpen";
        #endregion

        #region Unity Event Method        
        #endregion

        #region Custom Method
        //연출 내용
        protected override IEnumerator SequencePlay(GameObject player)
        {
            //-플레이 캐릭터 비활성화(플레이 멈춤)
            //배경음 정지

            //문 열기(애니메이션)
            //문 여는 사운드
            //적 활성화            

            //1초 딜레이
            //Enemy 등장 사운드
            //Enemy가 문이 완전히 열리면 타겟(플레이어)를 향해 걷는다

            //-플레이 캐릭터 활성화(다시 플레이)

            player.SetActive(false);

            AudioManager.Instance.StopBgm();

            twoDoorAnimator.SetBool(isOpen, true);
            doorBang.Play();

            robot.SetActive(true);

            yield return new WaitForSeconds(1f);
            //yield return null; //이번 프레임에만 지연, 다음 프레임에서 진행

            //jumpScare.Play();            
            AudioManager.Instance.PlayBgm("Bgm02");

            Robot enemyRobot = robot.GetComponent<Robot>();
            if(enemyRobot != null)
            {
                enemyRobot.ChangeState(RobotState.R_Walk);
            }

            player.SetActive(true);
        }
        #endregion
    }
}