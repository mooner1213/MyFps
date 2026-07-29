using System.Collections;
using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// FullEye 오브젝트 상호작용 클래스.
    /// 
    /// 동작 규칙:
    ///  - Left Eye / Right Eye 퍼즐 조각(puzzleKey01, puzzleKey02)을 모두 가진 상태에서
    ///    E키를 누르면 자식 leftEye → rightEye를 순차적으로 활성화한다.
    ///  - 퍼즐 조각이 없는 상태에서 마우스를 갖다 대면
    ///    "퍼즐 조각이 필요합니다" 텍스트를 표시한다.
    ///  - 두 자식이 모두 활성화되면 finalDoor(오른쪽 문)를 연다.
    /// </summary>
    public class PickupFullEye : Interactive
    {
        #region Serialized Variables (씬 직렬화 구조 유지)
        [Header("Eye Children (FullEye의 자식 오브젝트)")]
        [Tooltip("FullEye의 자식 LeftEye (초기 비활성화 상태여야 함)")]
        [SerializeField] private GameObject leftEye;

        [Tooltip("FullEye의 자식 RightEye (초기 비활성화 상태여야 함)")]
        [SerializeField] private GameObject rightEye;

        [Header("Required Puzzle Keys")]
        [Tooltip("필요한 첫 번째 퍼즐 조각 (Left_Eye = 3)")]
        [SerializeField] private PuzzleItem puzzleKey01 = PuzzleItem.Left_Eye;

        [Tooltip("필요한 두 번째 퍼즐 조각 (Right_Eye = 4)")]
        [SerializeField] private PuzzleItem puzzleKey02 = PuzzleItem.Right_Eye;

        [Tooltip("(미사용, 직렬화 호환용)")]
        [SerializeField] private PuzzleItem puzzleItem = PuzzleItem.None;

        [Header("Door")]
        [Tooltip("두 눈이 모두 활성화되면 열릴 오른쪽 문")]
        [SerializeField] private Door finalDoor;

        [Header("Settings")]
        [Tooltip("Left Eye → Right Eye 활성화 사이 딜레이 (초)")]
        [SerializeField] private float activationDelay = 1.0f;

        [Tooltip("두 눈 활성화 후 문이 열리기까지의 딜레이 (초)")]
        [SerializeField] private float doorOpenDelay = 0.5f;

        [Tooltip("퍼즐 조각이 있을 때 표시할 텍스트")]
        [SerializeField] private string readyMessage = "Fit the puzzle pieces";

        [Tooltip("퍼즐 조각이 없을 때 표시할 텍스트")]
        [SerializeField] private string missingMessage = "퍼즐 조각이 필요합니다";
        #endregion

        #region Private
        private bool isActivated = false;
        #endregion

        #region Interactive Override

        protected override void ShowActionUI()
        {
            if (isActivated) return;

            bool hasBoth = PlayerStats.Instance.HavePuzzleItem(puzzleKey01)
                        && PlayerStats.Instance.HavePuzzleItem(puzzleKey02);

            // 보유 상태에 따라 표시 텍스트 분기
            action = hasBoth ? readyMessage : missingMessage;
            base.ShowActionUI();
        }

        protected override void DoAction()
        {
            if (isActivated) return;

            if (!PlayerStats.Instance.HavePuzzleItem(puzzleKey01)
             || !PlayerStats.Instance.HavePuzzleItem(puzzleKey02))
            {
                // 조각이 없으면 동작 안 함 (UI 텍스트로 이미 안내됨)
                Debug.Log("[PickupFullEye] 퍼즐 조각이 부족합니다!");
                return;
            }

            // 두 조각 모두 보유 → 순차 활성화 시작
            isActivated = true;
            HideActionUI();
            StartCoroutine(ActivateSequence());
        }

        protected override void Update()
        {
            if (isActivated) return;
            base.Update();
        }
        #endregion

        #region Coroutine
        private IEnumerator ActivateSequence()
        {
            // 1단계: LeftEye 자식 활성화
            if (leftEye != null)
            {
                leftEye.SetActive(true);
                Debug.Log("[PickupFullEye] LeftEye 자식 활성화");
            }

            yield return new WaitForSeconds(activationDelay);

            // 2단계: RightEye 자식 활성화
            if (rightEye != null)
            {
                rightEye.SetActive(true);
                Debug.Log("[PickupFullEye] RightEye 자식 활성화");
            }

            yield return new WaitForSeconds(doorOpenDelay);

            // 3단계: 오른쪽 문 열기
            if (finalDoor != null)
            {
                finalDoor.Activate();
                Debug.Log("[PickupFullEye] 퍼즐 완성! 문을 열었습니다.");
            }
            else
            {
                Debug.LogWarning("[PickupFullEye] finalDoor 레퍼런스가 없습니다! Inspector에서 연결해주세요.");
            }
        }
        #endregion
    }
}
