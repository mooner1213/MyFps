using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 눈 퍼즐 전체를 관리하는 싱글톤 매니저.
    /// 왼쪽/오른쪽 눈이 모두 슬롯에 끼워지면 마지막 문을 엽니다.
    /// </summary>
    public class EyePuzzleManager : MonoBehaviour
    {
        #region Singleton
        public static EyePuzzleManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        #endregion

        #region Variables
        [Header("Puzzle State")]
        private bool leftInserted = false;
        private bool rightInserted = false;

        [Header("Final Door")]
        [Tooltip("양쪽 눈이 모두 꽂히면 열릴 마지막 문")]
        [SerializeField] public Door finalDoor;
        #endregion

        #region Custom Methods
        /// <summary>
        /// 슬롯에 눈이 끼워졌을 때 호출됩니다.
        /// </summary>
        public void OnSlotInserted(bool isLeft)
        {
            if (isLeft)
                leftInserted = true;
            else
                rightInserted = true;

            Debug.Log($"[EyePuzzleManager] Left={leftInserted}, Right={rightInserted}");

            // 양쪽 다 꽂혔으면 문 열기
            if (leftInserted && rightInserted)
            {
                OpenFinalDoor();
            }
        }

        private void OpenFinalDoor()
        {
            if (finalDoor != null)
            {
                finalDoor.Activate();
                Debug.Log("[EyePuzzleManager] 양쪽 눈 퍼즐 완성! 마지막 문을 엽니다.");
            }
            else
            {
                Debug.LogWarning("[EyePuzzleManager] finalDoor 레퍼런스가 없습니다!");
            }
        }
        #endregion
    }
}
