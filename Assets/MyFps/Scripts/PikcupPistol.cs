using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 가까이 가면 권총 아이템을 자동으로 습득하도록 처리하는 클래스
    /// </summary>
    public class PikcupPistol : MonoBehaviour
    {
        #region Variables
        [Header("Interactive UI (Optional)")]
        public GameObject actionUI;
        public GameObject extraCross;
        public TextMeshProUGUI actionText;
        
        public InputActionReference interactAction;
        public string action = "action Text";

        private Collider castCollider;
        private bool currentCasting = false;
        private bool wasCasting = false;

        [Header("Action Targets")]
        public GameObject realPistol;
        public GameObject arrow;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            castCollider = GetComponent<Collider>();
            
            // 콜라이더를 자동으로 트리거로 설정하고 감지 범위를 적절하게 확장
            if (castCollider is BoxCollider boxCol)
            {
                boxCol.isTrigger = true;
                boxCol.size = new Vector3(2.5f, 2.5f, 2.5f);
            }
        }

        private void Start()
        {
            // 1. realPistol 자동 감지: Player 자식에서 "RealPistol" 탐색
            if (realPistol == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Transform[] children = player.GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in children)
                    {
                        if (child.name == "RealPistol")
                        {
                            realPistol = child.gameObject;
                            Debug.Log("[PikcupPistol] RealPistol found under Player.");
                            break;
                        }
                    }
                }
            }

            // 2. fallback: "RealPistol"이 없으면 이 오브젝트 자신(FakePistol == 테이블 위 M9)을 사용
            //    플레이어 손에 든 총이 별도로 없을 경우, 집어든 이 총에 직접 FirePistol을 붙여 처리
            if (realPistol == null)
            {
                realPistol = this.gameObject;
                Debug.LogWarning("[PikcupPistol] RealPistol not found. Using FakePistol (this object) as fallback.");
            }

            // 3. arrow 자동 감지
            if (arrow == null)
            {
                GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name == "Arrow" && obj.scene.isLoaded)
                    {
                        arrow = obj;
                        break;
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (interactAction != null && interactAction.action != null)
            {
                interactAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (interactAction != null && interactAction.action != null)
            {
                interactAction.action.Disable();
            }
        }

        private void Update()
        {
            // 기존 조준 방식 획득 예외 처리
            if (PlayerCasting.DistanceFromTarget > 2f)
            {
                HideActionUI();
                wasCasting = false;
                return;
            }

            currentCasting = PlayerCasting.CastGameObject != null && PlayerCasting.CastGameObject == this.gameObject;

            if (currentCasting != wasCasting && currentCasting == true)
            {
                ShowActionUI();
            }
            else if (currentCasting != wasCasting && currentCasting == false)
            {
                HideActionUI();
            }

            if (currentCasting && interactAction != null && interactAction.action != null && interactAction.action.WasPressedThisFrame())
            {
                DoAction();
            }

            wasCasting = currentCasting;
        }

        private void OnTriggerEnter(Collider other)
        {
            // 플레이어 또는 플레이어 무브 컴포넌트 감지 시 자동으로 즉시 습득
            bool isPlayer = other.CompareTag("Player") || 
                            other.GetComponent<PlayerMove>() != null || 
                            other.GetComponentInParent<PlayerMove>() != null;

            if (isPlayer)
            {
                Debug.Log("[PikcupPistol] Player approached FakePistol. Automatically picking up weapon.");
                DoAction();
            }
        }
        #endregion

        #region Custom Methods
        void DoAction()
        {
            // ─── M9 오브젝트(총 모델)를 FakePistol에서 분리해 카메라 자식으로 이동 ───
            // FakePistol 자체를 Destroy하면 자식인 M9도 같이 사라지므로
            // M9를 먼저 카메라 자식으로 재부착하고 FakePistol만 제거함

            // 1. 씬에서 M9 오브젝트 탐색 (this.gameObject의 자식에서 찾기)
            Transform m9Transform = null;
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child != transform && child.GetComponent<MeshRenderer>() != null)
                {
                    // 최상위 자식 메쉬 (=M9 루트)를 찾음
                    if (child.parent == transform)
                    {
                        m9Transform = child;
                        break;
                    }
                }
            }

            // 2. M9를 메인 카메라 자식으로 재부착
            Camera mainCam = Camera.main;
            if (m9Transform != null && mainCam != null)
            {
                // 메인 카메라의 Near Clip Plane(근접 클리핑 평면)을 아주 얇게 세팅하여,
                // 총기 모델링 뒷부분이 카메라 렌즈에 잘려 내부 속이 뚫려 보이는 현상 방지
                mainCam.nearClipPlane = 0.02f;

                // 카메라 자식으로 재부착
                m9Transform.SetParent(mainCam.transform, false);

                // 화면 오른쪽 아래에 자연스럽게 들고 있는 포즈로 설정 (클리핑 방지를 위해 Z를 0.5f로 미세 이동)
                m9Transform.localPosition = new Vector3(0.2f, -0.22f, 0.5f);
                m9Transform.localRotation = Quaternion.Euler(-85f, 175f, 180f);
                m9Transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

                // 3. FirePistol 컴포넌트를 M9에 부착 (사격 기능)
                if (m9Transform.GetComponent<FirePistol>() == null)
                {
                    m9Transform.gameObject.AddComponent<FirePistol>();
                    Debug.Log("[PikcupPistol] FirePistol attached to M9.");
                }

                Debug.Log("[PikcupPistol] M9 moved to Camera as held weapon.");
            }
            else
            {
                // fallback: M9를 찾지 못한 경우 this에 FirePistol만 부착
                if (GetComponent<FirePistol>() == null)
                    gameObject.AddComponent<FirePistol>();
                Debug.LogWarning("[PikcupPistol] M9 child not found. FirePistol attached to FakePistol self.");
            }

            // 4. 가이드 화살표 제거
            if (arrow != null)
                Destroy(arrow);

            // 5. UI 숨김
            HideActionUI();

            // 6. FakePistol 껍데기만 제거 (M9는 이미 카메라 자식으로 이동됨)
            Destroy(this.gameObject);
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