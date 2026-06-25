using System.Collections;
using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 게임 시작 후 특정 오브젝트(예: 시퀀스 캔버스 등)를 
    /// 지정된 타이밍에 활성화하고 비활성화하는 간단한 시퀀스 제어 스크립트
    /// </summary>
    public class SequenceController : MonoBehaviour
    {
        [Header("제어할 타겟 오브젝트")]
        [SerializeField] private GameObject targetObject;

        [Header("딜레이 설정")]
        [SerializeField] private float delayBeforeActive = 1f;  // 활성화 전 대기 시간 (초)
        [SerializeField] private float activeDuration = 3f;     // 활성화 유지 시간 (초)

        private IEnumerator Start()
        {
            // 플레이어 이동 제어를 위해 PlayerMove 컴포넌트를 탐색합니다.
            PlayerMove playerMove = FindFirstObjectByType<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.canMove = false; // 움직임 비활성화
            }

            // 1. 시작할 때는 타겟 오브젝트가 꺼져있어야 하므로 비활성화합니다.
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            // 2. 지정된 시간(기본 1초) 동안 대기합니다.
            yield return new WaitForSeconds(delayBeforeActive);

            // 3. 타겟 오브젝트를 활성화합니다.
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            // 4. 활성화된 상태로 지정된 시간(기본 3초) 동안 유지합니다.
            yield return new WaitForSeconds(activeDuration);

            // 5. 다시 타겟 오브젝트를 비활성화합니다.
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            // 6. 시퀀스가 종료되었으므로 플레이어 움직임을 다시 활성화합니다.
            if (playerMove != null)
            {
                playerMove.canMove = true;
            }
        }
    }
}
