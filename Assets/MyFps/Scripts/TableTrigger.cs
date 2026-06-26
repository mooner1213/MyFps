using System.Collections;
using UnityEngine;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 테이블 근처 트리거 진입 시 가이드 연출을 수행하는 클래스
    /// </summary>
    public class TableTrigger : MonoBehaviour
    {
        #region Variables
        [Header("UI Settings")]
        [SerializeField] private GameObject dialogueUI;          // 대사를 활성화할 UI 오브젝트 (Canvas_Sequence 등)
        [SerializeField] private TextMeshProUGUI dialogueText;   // 대사 텍스트 컴포넌트 (SequenceText 등)
        [SerializeField] private string dialogueLine = "Looks like a weapon on that table."; // 출력할 대사 내용

        [Header("Guide Arrow")]
        [SerializeField] private GameObject guideArrow;          // 활성화할 가이드 화살표 오브젝트

        private bool isTriggered = false;                        // 중복 작동 방지 플래그
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            // 시작 시 가이드 화살표는 비활성화 상태여야 하므로 자동 확인 및 처리
            if (guideArrow != null)
            {
                guideArrow.SetActive(false);
            }

            // UI 요소가 연결되지 않은 경우 자동 검색 시도 (개발 편의)
            if (dialogueUI == null)
            {
                dialogueUI = GameObject.Find("Canvas_Sequence");
            }

            if (dialogueText == null && dialogueUI != null)
            {
                dialogueText = dialogueUI.GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[TableTrigger] OnTriggerEnter detected: {other.gameObject.name} (Tag: {other.gameObject.tag})");

            if (isTriggered) return;

            PlayerMove playerMove = other.GetComponentInParent<PlayerMove>();
            if (playerMove != null)
            {
                Debug.Log($"[TableTrigger] PlayerMove found on {playerMove.gameObject.name}. Starting sequence.");
                isTriggered = true;
                StartCoroutine(TriggerSequence(playerMove, other.GetComponentInParent<MouseLook>()));
            }
            else
            {
                Debug.Log($"[TableTrigger] PlayerMove not found on parent of {other.gameObject.name}.");
            }
        }
        #endregion

        #region Custom Methods
        private IEnumerator TriggerSequence(PlayerMove playerMove, MouseLook mouseLook)
        {
            // 1. 플레이 캐릭터 비활성화 (이동 및 시선 잠금)
            if (playerMove != null)
            {
                playerMove.canMove = false;
            }
            if (mouseLook != null)
            {
                mouseLook.enabled = false;
            }

            // 2. 카메라가 Arrow를 부드럽게 바라보도록 회전 (LERP)
            if (playerMove != null && mouseLook != null && guideArrow != null)
            {
                Vector3 targetPos = guideArrow.transform.position;
                Vector3 cameraPos = mouseLook.cameraRoot.position;
                Vector3 toTarget = targetPos - cameraPos;

                Vector3 yawDir = new Vector3(toTarget.x, 0f, toTarget.z).normalized;
                Quaternion startPlayerRot = playerMove.transform.rotation;
                Quaternion targetPlayerRot = Quaternion.LookRotation(yawDir);

                float distanceXZ = new Vector3(toTarget.x, 0f, toTarget.z).magnitude;
                float targetPitch = -Mathf.Atan2(toTarget.y, distanceXZ) * Mathf.Rad2Deg;

                // MouseLook의 일반적인 clamp 범위 내로 제한
                targetPitch = Mathf.Clamp(targetPitch, -80f, 45f);

                // MouseLook의 private 변수인 cameraTargetPitch 필드 탐색 (리플렉션)
                System.Reflection.FieldInfo pitchField = typeof(MouseLook).GetField("cameraTargetPitch", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                float startPitch = 0f;
                if (pitchField != null)
                {
                    startPitch = (float)pitchField.GetValue(mouseLook);
                }

                float elapsed = 0f;
                float rotateDuration = 0.5f; // 0.5초 동안 회전

                while (elapsed < rotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / rotateDuration;

                    // 플레이어 좌우 회전 (Slerp)
                    playerMove.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);

                    // 카메라 상하 회전 (Lerp)
                    float currentPitch = Mathf.Lerp(startPitch, targetPitch, t);
                    mouseLook.cameraRoot.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);

                    // 리플렉션으로 피치 변수 값 업데이트 (조작 복구 시 화면 튐 방지)
                    if (pitchField != null)
                    {
                        pitchField.SetValue(mouseLook, currentPitch);
                    }

                    yield return null;
                }

                // 최후 고정값 설정
                playerMove.transform.rotation = targetPlayerRot;
                mouseLook.cameraRoot.localRotation = Quaternion.Euler(targetPitch, 0f, 0f);
                if (pitchField != null)
                {
                    pitchField.SetValue(mouseLook, targetPitch);
                }
            }

            // 3. 대사 출력
            if (dialogueUI != null)
            {
                if (dialogueText != null)
                {
                    dialogueText.text = dialogueLine;
                }
                dialogueUI.SetActive(true);
            }

            // 4. 1초 딜레이
            yield return new WaitForSeconds(1f);

            // 5. 화살표 활성화
            if (guideArrow != null)
            {
                guideArrow.SetActive(true);
            }

            // 6. 1초 딜레이
            yield return new WaitForSeconds(1f);

            // 7. 대사 UI 비활성화
            if (dialogueUI != null)
            {
                dialogueUI.SetActive(false);
            }

            // 8. 플레이 캐릭터 활성화 (이동 및 시선 복구)
            if (playerMove != null)
            {
                playerMove.canMove = true;
            }
            if (mouseLook != null)
            {
                mouseLook.enabled = true;
            }

            // 8. 트리거 비활성화 (오브젝트 파괴 대신 콜라이더와 스크립트 비활성화하여 자식 오브젝트 유지)
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            this.enabled = false;
        }
        #endregion
    }
}
