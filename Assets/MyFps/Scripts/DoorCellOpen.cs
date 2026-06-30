using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 플레이어와 인터랙티브 기능 구현
    /// 가까이 가서 crosshair 캐스팅하면 액션 UI 보여준다
    /// 액션 : 문을 연다
    /// </summary>
    public class DoorCellOpen : MonoBehaviour
    {
        #region Variables
        //UI 오브젝트
        public GameObject actionUI;
        public GameObject extraCross;
        public TextMeshProUGUI actionText;

        private Collider doorCollider;

        private bool currentCasting = false;        //현재 캐스팅 상태
        private bool wasCasting = false;            //이전 캐스팅 상태

        //인터랙브 액션
        public InputActionReference interactAction;
        public string action = "action Text";       //인터랙티브 액션 내용

        public Animator animator;
        private string isOpen = "IsOpen";

        public AudioSource audioSource;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            doorCollider = GetComponent<Collider>();
            if (doorCollider == null)
            {
                Debug.LogError("DoorCellOpen: Collider component not found!");
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
                    Debug.Log("[DoorCellOpen] CreakyDoor audio clip successfully loaded from Resources.");
                }
            }
        }

        private void OnEnable()
        {            
            interactAction.action.Enable();
        }

        private void OnDisable()
        {
            interactAction.action.Disable();
        }

        private void Update()
        {
            //플레이어의 캐스팅 거리가 체크
            if (PlayerCasting.DistanceFromTarget > 2f)
            {
                HideActionUI();
                wasCasting = false;
                return;
            }

            // 이 오브젝트의 캐스팅한 오브젝트인 체크
            currentCasting = PlayerCasting.CastGameObject != null && PlayerCasting.CastGameObject == this.gameObject;

            // 상태 변화 감지: 경계
            if (currentCasting != wasCasting && currentCasting == true)  
            {
                //캐스팅하고 있지 않다가 캐스팅을 시작할때
                ShowActionUI();
            }
            else if (currentCasting != wasCasting && currentCasting == false)
            {
                //캐스팅 하고 있다가 캐스팅을 놓치는것을 시작할때
                HideActionUI();
            }

            if (currentCasting && interactAction.action.WasPressedThisFrame())
            {
                DoAction();
            }

            //was 상태 저장
            wasCasting = currentCasting;
        }
        #endregion

        #region Custom Method
        void DoAction()
        {
            //인터랙티브 액션 - open the door
            animator.SetBool(isOpen, true);

            //사운드 플레이, AudioSource null 체크
            if (audioSource)
            {
                audioSource.Play();
            }

            //초기화
            HideActionUI();
            doorCollider.enabled = false;
        }

        void ShowActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(true);
                extraCross.SetActive(true);
                actionText.text = action;
            }
        }

        void HideActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(false);
                extraCross.SetActive(false);
                actionText.text = "";
            }
        }
        #endregion
    }
}