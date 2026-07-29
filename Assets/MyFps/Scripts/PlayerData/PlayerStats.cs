using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 퍼즐 아이템 목록 정의
    /// </summary>
    public enum PuzzleItem
    {
        None = 0,
        Door01_Key,
        Door02_Key,
        Left_Eye,
        Right_Eye,

        MaxPuzzleItem
    }

    /// <summary>
    /// 플레이어의 속성값들을 관리하는 싱글톤 클래스
    /// </summary>
    public class PlayerStats : PersistanctSingleton<PlayerStats>
    {
        #region Varibles
        private int ammoCount;

        //퍼즐 아이템 획득 여부
        [SerializeField] private bool[] puzzleItems;
        #endregion

        #region Property
        public int AmmoCount => ammoCount;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //초기화
            ammoCount = 0;
            puzzleItems = new bool[(int)PuzzleItem.MaxPuzzleItem];            
        }
        #endregion

        #region Custom Method
        //탄환 추가
        public void AddAmmo(int amount)
        {
            ammoCount += amount;
        }

        //탄환 사용하기
        public bool UseAmmo(int amount = 1)
        {
            if(ammoCount < amount)
            {
                Debug.Log("You need to reload");
                return false;
            }

            ammoCount -= amount;
            return true;
        }

        //퍼즐 아이템 획득 여부 체크
        public bool HavePuzzleItem(PuzzleItem item)
        {
            //아이템 체크
            if(item == PuzzleItem.None || item == PuzzleItem.MaxPuzzleItem)
            {
                Debug.Log("Out of Range");
                return false;
            }

            return puzzleItems[(int)item];
        }

        //퍼즐 아이템 획득 - 획득성공 true, 실패 false
        public bool GainPuzzleItem(PuzzleItem item)
        {
            //아이템 체크
            if (item == PuzzleItem.None || item == PuzzleItem.MaxPuzzleItem)
            {
                Debug.Log("Out of Range");
                return false;
            }

            puzzleItems[(int)item] = true;
            return true;
        }
        #endregion

    }
}