using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 퍼즐 슬롯(눈 끼우는 곳)에 E키로 상호작용하면
    /// 해당 퍼즐 조각이 있어야 끼울 수 있고,
    /// 양쪽 다 꽂으면 마지막 문을 열어줍니다.
    /// </summary>
    public class EyeSlotInteractive : Interactive
    {
        #region Variables
        [Header("Eye Slot Settings")]
        [SerializeField] private PuzzleItem requiredItem = PuzzleItem.Left_Eye;

        [Tooltip("슬롯에 눈이 끼워진 상태를 보여줄 오브젝트 (없으면 무시)")]
        [SerializeField] private GameObject insertedVisual;

        [Tooltip("양쪽 눈이 모두 끼워지면 열릴 마지막 문 (EyePuzzleManager에서 자동 감지)")]
        [SerializeField] private Door finalDoor;

        [Tooltip("왼쪽 슬롯이면 true, 오른쪽 슬롯이면 false")]
        [SerializeField] private bool isLeftSlot = true;

        private bool inserted = false;
        #endregion

        #region abstract
        protected override void DoAction()
        {
            // 이미 꽂혀 있으면 무시
            if (inserted) return;

            // 해당 퍼즐 조각을 갖고 있는지 확인
            if (!PlayerStats.Instance.HavePuzzleItem(requiredItem))
            {
                Debug.Log($"[EyeSlotInteractive] {requiredItem} 조각이 없습니다!");
                return;
            }

            // 삽입 처리
            inserted = true;
            Debug.Log($"[EyeSlotInteractive] {requiredItem} 조각을 슬롯에 끼웠습니다.");

            // 끼워진 비주얼 활성화
            if (insertedVisual != null)
                insertedVisual.SetActive(true);

            // EyePuzzleManager에 알려서 완료 확인
            EyePuzzleManager.Instance?.OnSlotInserted(isLeftSlot);

            // 더 이상 상호작용 불필요하므로 컴포넌트 비활성화
            this.enabled = false;
            HideActionUI();
        }
        #endregion

        #region Unity Event Method
        protected override void Update()
        {
            // 이미 삽입됐으면 업데이트 불필요
            if (inserted) return;
            base.Update();
        }
        #endregion
    }
}
