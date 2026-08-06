using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// Interactive를 상속받는다 
    /// 액션 : 퍼즐 조각이 모두 맞으면 퍼즐 아이템(door key) 지급
    /// </summary>
    public class PickupFullEye : Interactive
    {
        #region Variables
        public GameObject leftEye;
        public GameObject rightEye;

        //열쇠 필요 여부 : PuzzleItem.None 키 필요 없을때
        [SerializeField] private PuzzleItem puzzleKey01 = PuzzleItem.None;
        [SerializeField] private PuzzleItem puzzleKey02 = PuzzleItem.None;

        [Header("Action")]
        [SerializeField] private PuzzleItem puzzleItem; //지급 아이템
        #endregion

        #region abstract
        protected override void DoAction()
        {
            //퍼즐 조각 보여주기
            if(PlayerStats.Instance.HavePuzzleItem(puzzleKey01))
            {
                leftEye.SetActive(true);
            }
            if(PlayerStats.Instance.HavePuzzleItem(puzzleKey02))
            {
                rightEye.SetActive(true);
            }

            if (NeedKey())
            {
                //액션
                //Debug.Log($"퍼즐 아이템 {puzzleItem}개 지급하기");
                PlayerStats.Instance.GainPuzzleItem(puzzleItem);

                //콜라이더 기능 제거
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        #endregion

        #region Custom Method
        protected override void ShowActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(true);
                extraCross.SetActive(true);

                if (NeedKey())
                {
                    actionText.text = action;
                }
                else
                {
                    actionText.text = "Collect more puzzle pieces";
                }
            }
        }

        bool NeedKey()
        {
            bool hasKey = false;
            //도어 키 체크
            if (puzzleKey01 == PuzzleItem.None && puzzleKey02 == PuzzleItem.None)
            {
                hasKey = true;
            }
            else
            {
                hasKey = PlayerStats.Instance.HavePuzzleItem(puzzleKey01) &&
                    PlayerStats.Instance.HavePuzzleItem(puzzleKey02);
            }

            return hasKey;
        }
        #endregion
    }
}