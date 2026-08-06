using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 플레이어의 HealthBar를 표시하는 UI 관리
    /// 1. Current Health 값에 따른 게이지 관리
    /// 2. 플레이어의 health는 FindObject로 찾아서 참조한다.
    /// 3. 게임 시작 시 100% Full 체력으로 시작하도록 초기화 보장
    /// </summary>
    public class PlayerHealthBar : MonoBehaviour
    {
        #region Variables
        [Tooltip("체력 게이지 UI 이미지 (Fill Type)")]
        [SerializeField] private Image healthBarImage;

        [Tooltip("플레이어 Health 컴포넌트 참조 (미지정 시 자동 탐색)")]
        [SerializeField] private Health playerHealth;

        [Tooltip("게이지 바 부드러운 감소 속도")]
        [SerializeField] private float fillSpeed = 5f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 체력바 이미지 자동 찾기
            if (healthBarImage == null)
            {
                healthBarImage = GetComponent<Image>();
            }

            // 플레이어 Health 컴포넌트 자동 탐색
            FindPlayerHealth();
        }

        private void Start()
        {
            if (playerHealth == null)
            {
                FindPlayerHealth();
            }

            // 게임 시작 시 체력바를 100% Full(1.0)로 즉시 초기화
            if (healthBarImage != null)
            {
                if (playerHealth != null)
                {
                    healthBarImage.fillAmount = playerHealth.HealthRatio;
                }
                else
                {
                    healthBarImage.fillAmount = 1f;
                }
            }
        }

        private void Update()
        {
            if (playerHealth == null)
            {
                FindPlayerHealth();
                if (playerHealth == null) return;
            }

            // 실시간 체력 비율(0 ~ 1) 적용
            float targetRatio = playerHealth.HealthRatio;

            if (healthBarImage != null)
            {
                // 부드럽게 체력바 게이지 업데이트
                healthBarImage.fillAmount = Mathf.Lerp(
                    healthBarImage.fillAmount,
                    targetRatio,
                    Time.deltaTime * fillSpeed
                );
            }
        }
        #endregion

        #region Custom Method
        private void FindPlayerHealth()
        {
            // Player 태그 또는 씬 내 플레이어의 Health 컴포넌트 탐색
            var healths = FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (var h in healths)
            {
                if (h.CompareTag("Player") || h.GetComponentInParent<Unity.FPS.Gameplay.PlayerCharacterController>() != null || h.name.Contains("Player"))
                {
                    playerHealth = h;
                    break;
                }
            }

            if (playerHealth == null && healths.Length > 0)
            {
                playerHealth = healths[0];
            }
        }
        #endregion
    }
}