using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 플레이어의 시야(레이캐스트)를 통해 상호작용 가능한 오브젝트를 탐색하고 실행하는 클래스
    /// </summary>
    public class PlayerCasting : MonoBehaviour
    {
        #region Variables
        [Header("UI Settings")]
        [SerializeField] private GameObject actionUI;           // 화면에 활성화/비활성화할 UI 오브젝트 (ActionUI)
        [SerializeField] private TextMeshProUGUI actionText;     // 텍스트를 변경할 TMPro 컴포넌트 (ActionText)

        [Header("Casting Settings")]
        [SerializeField] private float maxDistance = 2f;         // 상호작용 가능한 최대 거리
        [SerializeField] private Transform cameraTransform;      // 레이저를 발사할 카메라 트랜스폼

        private Interactable currentInteractable;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            // UI 요소 자동 탐색 및 할당
            if (actionUI == null)
            {
                GameObject canvasObj = GameObject.Find("Canvas");
                if (canvasObj != null)
                {
                    Transform t = canvasObj.transform.Find("ActionUI");
                    if (t != null)
                    {
                        actionUI = t.gameObject;
                    }
                }
            }

            if (actionText == null && actionUI != null)
            {
                actionText = actionUI.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            // 카메라 트랜스폼 자동 할당
            if (cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    cameraTransform = Camera.main.transform;
                }
                else
                {
                    cameraTransform = transform; // 차선책으로 현재 오브젝트 기준
                }
            }

            // 시작할 때 UI 비활성화
            if (actionUI != null)
            {
                actionUI.SetActive(false);
            }
        }

        private void Update()
        {
            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            // Physics.Raycast를 이용하여 카메라 정면으로 레이저를 발사
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // 레이저에 맞은 오브젝트에서 Interactable 컴포넌트 추출
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                
                if (interactable != null)
                {
                    currentInteractable = interactable;

                    // UI 활성화 및 텍스트 설정
                    if (actionUI != null)
                    {
                        if (actionText != null)
                        {
                            actionText.text = interactable.GetHoverMessage();
                        }
                        actionUI.SetActive(true);
                    }

                    // New Input System 기준 E키를 눌렀을 때 상호작용 실행
                    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        interactable.Interact();
                        // 상호작용 완료 후 즉시 UI 비활성화
                        if (actionUI != null)
                        {
                            actionUI.SetActive(false);
                        }
                        currentInteractable = null;
                    }
                    return;
                }
            }

            // 레이저가 빗나가거나 거리가 멀어지면 UI 비활성화
            if (actionUI != null && actionUI.activeSelf)
            {
                actionUI.SetActive(false);
            }
            currentInteractable = null;
        }

        // 선택 시 기즈모 그리기 (레이캐스트 거리 시각화)
        private void OnDrawGizmosSelected()
        {
            Transform origin = cameraTransform != null ? cameraTransform : transform;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin.position, origin.forward * maxDistance);
        }
        #endregion
    }
}
