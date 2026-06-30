using UnityEngine;
using System.Collections;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 첫번째 복도끝 시퀀스 트리거 구현
    /// 시퀀스 내용 : 무기발견 연출 구현
    /// </summary>
    public class BFirstTrigger : MonoBehaviour
    {
        #region Variables
        //참조        
        public TextMeshProUGUI sequenceText;
        public GameObject arrow;
        #endregion

        #region Unity Event Method
        private void OnTriggerEnter(Collider other)
        {
            //플레이어 체크
            if (other.gameObject.tag != "Player")
                return;
            
            StartCoroutine(SequencePlay(other.gameObject));

            //트리거 제거
            transform.GetComponent<BoxCollider>().enabled = false;
        }
        #endregion

        #region Custom Method
        IEnumerator SequencePlay(GameObject player)
        {
            // player.SetActive(false); 대신 이동 및 카메라 입력 조작을 비활성화
            PlayerMove playerMove = player.GetComponentInParent<PlayerMove>();
            if (playerMove == null)
            {
                playerMove = player.GetComponent<PlayerMove>();
            }

            MouseLook mouseLook = player.GetComponentInParent<MouseLook>();
            if (mouseLook == null)
            {
                mouseLook = player.GetComponent<MouseLook>();
            }

            // 플레이어 이동 및 시선 조작 잠금
            if (playerMove != null)
            {
                playerMove.canMove = false;
            }
            if (mouseLook != null)
            {
                mouseLook.enabled = false;
            }

            // 1. 카메라가 Desk/Arrow를 향해 부드럽게 회전 (Lerp)
            if (playerMove != null && mouseLook != null && arrow != null)
            {
                Vector3 targetPos = arrow.transform.position;
                Vector3 cameraPos = mouseLook.cameraRoot.position;
                Vector3 toTarget = targetPos - cameraPos;

                Vector3 yawDir = new Vector3(toTarget.x, 0f, toTarget.z).normalized;
                Quaternion startPlayerRot = playerMove.transform.rotation;
                Quaternion targetPlayerRot = Quaternion.LookRotation(yawDir);

                float distanceXZ = new Vector3(toTarget.x, 0f, toTarget.z).magnitude;
                float targetPitch = -Mathf.Atan2(toTarget.y, distanceXZ) * Mathf.Rad2Deg;

                // MouseLook의 제한각 범위 고려
                targetPitch = Mathf.Clamp(targetPitch, -80f, 45f);

                // MouseLook의 private cameraTargetPitch 필드 리플렉션 탐색
                System.Reflection.FieldInfo pitchField = typeof(MouseLook).GetField("cameraTargetPitch", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                float startPitch = 0f;
                if (pitchField != null)
                {
                    startPitch = (float)pitchField.GetValue(mouseLook);
                }

                float elapsed = 0f;
                float rotateDuration = 1.0f; // 1초간 부드럽게 시선 이동

                while (elapsed < rotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / rotateDuration;
                    t = Mathf.SmoothStep(0f, 1f, t); // 감속 곡선 적용

                    // 플레이어 좌우 회전 (Slerp)
                    playerMove.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);

                    // 카메라 상하 회전 (Lerp)
                    float currentPitch = Mathf.Lerp(startPitch, targetPitch, t);
                    mouseLook.cameraRoot.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);

                    // 화면 튐 방지를 위한 피치값 저장
                    if (pitchField != null)
                    {
                        pitchField.SetValue(mouseLook, currentPitch);
                    }

                    yield return null;
                }

                // 값 최종 고정
                playerMove.transform.rotation = targetPlayerRot;
                mouseLook.cameraRoot.localRotation = Quaternion.Euler(targetPitch, 0f, 0f);
                if (pitchField != null)
                {
                    pitchField.SetValue(mouseLook, targetPitch);
                }
            }

            // 2. 대사 출력
            if (sequenceText != null)
            {
                sequenceText.gameObject.SetActive(true);
                sequenceText.text = "Looks like a weapon on that table.";
            }

            // 3. 1초 대기
            yield return new WaitForSeconds(1.0f);

            // 4. 화살표 활성화
            if (arrow != null)
            {
                arrow.SetActive(true);
            }

            // 5. 1.5초 대기 (대사 확인을 위한 여유 시간)
            yield return new WaitForSeconds(1.5f);

            // 6. 대사 제거 및 UI 비활성화
            if (sequenceText != null)
            {
                sequenceText.gameObject.SetActive(false);
                sequenceText.text = "";
            }

            // 7. 플레이어 이동 및 시선 조작 잠금 해제
            if (playerMove != null)
            {
                playerMove.canMove = true;
            }
            if (mouseLook != null)
            {
                mouseLook.enabled = true;
            }
        }
        #endregion

    }
}