using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// Interactive를 상속받는다 
    /// 액션 : 퍼즐아이템을 지급한다 
    /// </summary>
    public class PickupPuzzleItem : Interactive
    {
        #region Variables
        [Header("Action")]
        [SerializeField] PuzzleItem puzzleItem;
        #endregion

        #region abstract
        protected override void DoAction()
        {
            //액션
            //Debug.Log($"퍼즐 아이템 {puzzleItem}개 지급하기");
            PlayerStats.Instance.GainPuzzleItem(puzzleItem);

            //콜라이더 기능 제거
            Destroy(this.gameObject);
        }
        #endregion
    }
}