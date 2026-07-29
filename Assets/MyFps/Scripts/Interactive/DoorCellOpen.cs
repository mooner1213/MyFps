using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// Interactive를 상속받는다    
    /// 액션 : 문을 연다
    /// </summary>
    public class DoorCellOpen : Interactive
    {
        #region Variables       
        [Header("Action")]
        public Animator animator;
        public AudioSource audioSource;

        //애니메이터 파라미터
        private string isOpen = "IsOpen";
        #endregion

        #region Custom Method
        protected override void DoAction()
        {
            //인터랙티브 액션 - open the door
            animator.SetBool(isOpen, true);

            //사운드 플레이, AudioSource null 체크
            if (audioSource)
            {
                audioSource.Play();
            }

            //콜라이더 기능 제거
            castCollider.enabled = false;
        }
        #endregion
    }
}