using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 문 개방 상호작용 컴포넌트.
    /// </summary>
    public class DoorCellOpen : MonoBehaviour
    {
        #region Variables
        [Header("Interaction Settings")]
        [SerializeField] private Animator doorAnimator;         // 문 애니메이터
        [SerializeField] private string animParameter = "isOpen"; // 애니메이터 파라미터명
        [SerializeField] private AudioSource audioSource;       // 문 개방 효과음 재생용 오디오 소스

        private Collider myCollider;
        private bool isOpened = false;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            myCollider = GetComponent<Collider>();
            
            // 애니메이터 자동 탐색
            if (doorAnimator == null)
            {
                doorAnimator = GetComponentInParent<Animator>();
            }

            // 오디오 소스 자동 탐색
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        #endregion

        #region Custom Methods
        public void OnInteract()
        {
            if (!isOpened)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            isOpened = true;

            // 문 여는 애니메이션 실행 (메카님)
            if (doorAnimator != null)
            {
                doorAnimator.SetBool(animParameter, true);
            }

            // Door Trigger 제거 (콜라이더 비활성화) -> 플레이어가 통과 가능
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }

            // 문 사운드 출력
            if (audioSource != null)
            {
                audioSource.Play();
            }

            Debug.Log("Door opened and trigger collider disabled.");
        }
        #endregion
    }
}