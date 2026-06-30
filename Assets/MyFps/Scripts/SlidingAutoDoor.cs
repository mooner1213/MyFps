using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어 접근 시 자동으로 열리고 지나가면 닫히는 미닫이문 제어 스크립트
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class SlidingAutoDoor : MonoBehaviour
    {
        [Header("Door Settings")]
        [SerializeField] private string triggerTag = "Player";
        [SerializeField] private string animatorParameter = "IsOpen";

        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            // BoxCollider 트리거 자동 추가
            BoxCollider col = GetComponent<BoxCollider>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.center = new Vector3(0f, 1.5f, 0f);
                col.size = new Vector3(6f, 3f, 4f);
            }
            else
            {
                col.isTrigger = true;
            }

            // kinematic Rigidbody 자동 추가
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        private void Start()
        {
            // 오디오 소스 자동 설정
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            // 오디오 클립이 설정되지 않았을 경우 Resources에서 CreakyDoor 로드
            if (audioSource.clip == null)
            {
                audioSource.clip = Resources.Load<AudioClip>("CreakyDoor");
                if (audioSource.clip != null)
                {
                    Debug.Log("[SlidingAutoDoor] CreakyDoor audio clip successfully loaded from Resources.");
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 플레이어 판단 조건 (태그 혹은 PlayerMove 컴포넌트 유무)
            bool isPlayer = other.CompareTag(triggerTag) || 
                            other.GetComponent<PlayerMove>() != null || 
                            other.GetComponentInParent<PlayerMove>() != null;

            Debug.Log($"[SlidingAutoDoor] OnTriggerEnter: {other.gameObject.name} (isPlayer: {isPlayer})");

            if (isPlayer)
            {
                if (animator != null)
                {
                    animator.SetBool(animatorParameter, true);
                }

                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 플레이어 판단 조건 (태그 혹은 PlayerMove 컴포넌트 유무)
            bool isPlayer = other.CompareTag(triggerTag) || 
                            other.GetComponent<PlayerMove>() != null || 
                            other.GetComponentInParent<PlayerMove>() != null;

            Debug.Log($"[SlidingAutoDoor] OnTriggerExit: {other.gameObject.name} (isPlayer: {isPlayer})");

            if (isPlayer)
            {
                if (animator != null)
                {
                    animator.SetBool(animatorParameter, false);
                }

                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }
}
