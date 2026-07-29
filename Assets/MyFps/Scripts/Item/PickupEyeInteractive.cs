using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어가 조준하고 상호작용 키(E)를 누르면 퍼즐 눈 조각(Left/Right Eye)을 획득하는 클래스
    /// </summary>
    public class PickupEyeInteractive : Interactive
    {
        #region Variables
        [SerializeField] private PuzzleItem puzzleItem = PuzzleItem.Left_Eye;
        #endregion

        #region abstract
        protected override void DoAction()
        {
            // 퍼즐 아이템 획득 처리
            PlayerStats.Instance.GainPuzzleItem(puzzleItem);
            Debug.Log($"Gained Eye Puzzle Item: {puzzleItem}");

            // 아이템 제거
            Destroy(this.gameObject);
        }
        #endregion

        #region Custom Setup (Editor only)
        public void Setup(PuzzleItem item)
        {
            puzzleItem = item;
        }
        #endregion
    }
}
