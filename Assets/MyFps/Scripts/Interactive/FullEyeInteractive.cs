using System.Collections;
using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// Full Eye 오브젝트 상호작용 처리 클래스.
    /// - Left Eye / Right Eye 퍼즐 조각을 모두 획득한 상태에서 상호작용하면
    ///   자식 오브젝트 Left Eye → Right Eye를 순차적으로 활성화합니다.
    /// - 퍼즐 조각이 없는 상태에서 마우스를 갖다 대면 안내 텍스트를 표시합니다.
    /// - 자식 오브젝트가 모두 활성화되면 연결된 오른쪽 문을 열어줍니다.
    /// </summary>
    public class FullEyeInteractive : Interactive
    {
        #region Variables
        [Header("Eye Slot Children")]
        [Tooltip("Full Eye의 자식 오브젝트 중 왼쪽 눈 (순차 활성화 첫 번째)")]
        [SerializeField] private GameObject leftEyeChild;

        [Tooltip("Full Eye의 자식 오브젝트 중 오른쪽 눈 (순차 활성화 두 번째)")]
        [SerializeField] private GameObject rightEyeChild;

        [Header("Door")]
        [Tooltip("두 눈이 모두 활성화되면 열릴 오른쪽 문")]
        [SerializeField] private Door rightDoor;

        [Header("Activation Settings")]
        [Tooltip("Left Eye → Right Eye 활성화 사이의 딜레이(초)")]
        [SerializeField] private float activationDelay = 1.0f;

        [Header("UI Messages")]
        [Tooltip("퍼즐 조각이 없을 때 표시할 안내 텍스트")]
        [SerializeField] private string missingPieceMessage = "퍼즐 조각이 필요합니다";

        [Tooltip("퍼즐 조각이 있을 때 표시할 상호작용 안내 텍스트")]
        [SerializeField] private string insertMessage = "눈을 끼우기";

        private bool activated = false;
        #endregion

        #region Interactive Override
        protected override void ShowActionUI()
        {
            if (activated) return;

            bool hasLeft = PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Left_Eye);
            bool hasRight = PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Right_Eye);
            bool hasBoth = hasLeft && hasRight;

            // 보유 상태에 따라 표시 텍스트 분기
            action = hasBoth ? insertMessage : missingPieceMessage;
            base.ShowActionUI();
        }

        protected override void DoAction()
        {
            if (activated) return;

            bool hasLeft = PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Left_Eye);
            bool hasRight = PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Right_Eye);

            if (!hasLeft || !hasRight)
            {
                // 퍼즐 조각이 부족하면 동작 안 함
                Debug.Log("[FullEyeInteractive] Left Eye 또는 Right Eye 조각이 없습니다!");
                return;
            }

            // 두 눈 모두 있으면 순차 활성화 시작
            activated = true;
            HideActionUI();
            StartCoroutine(ActivateSequence());
        }
        #endregion

        #region Update Override
        protected override void Update()
        {
            if (activated) return;
            base.Update();
        }
        #endregion

        #region Coroutine
        private IEnumerator ActivateSequence()
        {
            // 1단계: Left Eye 자식 활성화
            if (leftEyeChild != null)
            {
                leftEyeChild.SetActive(true);
                Debug.Log("[FullEyeInteractive] Left Eye 자식 활성화");
            }

            // 딜레이
            yield return new WaitForSeconds(activationDelay);

            // 2단계: Right Eye 자식 활성화
            if (rightEyeChild != null)
            {
                rightEyeChild.SetActive(true);
                Debug.Log("[FullEyeInteractive] Right Eye 자식 활성화");
            }

            // 두 자식 모두 활성화 완료 → 문 열기
            yield return new WaitForSeconds(activationDelay);
            OpenRightDoor();
        }

        private void OpenRightDoor()
        {
            if (rightDoor != null)
            {
                rightDoor.Activate();
                Debug.Log("[FullEyeInteractive] Full Eye 퍼즐 완성! 오른쪽 문을 열었습니다.");
            }
            else
            {
                Debug.LogWarning("[FullEyeInteractive] rightDoor 레퍼런스가 없습니다!");
            }
        }
        #endregion
    }
}
