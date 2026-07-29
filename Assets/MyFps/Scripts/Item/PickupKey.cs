using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 열쇠 아이템을 줍는 스크립트
    /// </summary>
    public class PickupKey : PickupItem
    {
        #region Variables        
        //열쇠 퍼즐 아이템
        [SerializeField] private PuzzleItem puzzleItem;
        #endregion

        #region abstract
        protected override bool OnPickup()
        {
            //퍼즐 아이템 줍기
            PlayerStats.Instance.GainPuzzleItem(puzzleItem);

            return true;
        }
        #endregion
    }
}