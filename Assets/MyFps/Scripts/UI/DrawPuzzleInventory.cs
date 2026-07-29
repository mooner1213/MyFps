using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어가 획득한 퍼즐 조각(Left Eye, Right Eye) 현황을 화면 우측 상단 UI에 반영하는 클래스
    /// </summary>
    public class DrawPuzzleInventory : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject leftEyeUI;
        [SerializeField] private GameObject rightEyeUI;
        #endregion

        #region Unity Event Methods
        private void Update()
        {
            if (PlayerStats.Instance == null) return;

            // PlayerStats의 인벤토리 내역에 따라 UI 활성화/비활성화 처리
            if (leftEyeUI != null)
            {
                leftEyeUI.SetActive(PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Left_Eye));
            }

            if (rightEyeUI != null)
            {
                rightEyeUI.SetActive(PlayerStats.Instance.HavePuzzleItem(PuzzleItem.Right_Eye));
            }
        }
        #endregion

        #region Custom Setup (Editor only)
        public void Setup(GameObject leftUI, GameObject rightUI)
        {
            leftEyeUI = leftUI;
            rightEyeUI = rightUI;
        }
        #endregion
    }
}
