using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 캐릭터의 머리위에 있는 HealthBar를 표시하는 UI 관리
    /// 1. Current Health 값에 따른 게이지 관리 (healthBarImage.fillAmount = health.HealthRatio)
    /// 2. 월드 캔버스 UI - 게이지바가 항상 플레이어(카메라)를 바라본다.
    /// 3. 현재 Health값이 maxHealth 이면 게이지바 UI를 보이지 않게 한다.
    /// 4. 데미지를 입어서 HP가 줄어들면 그때부터 게이지바를 보여준다.
    /// </summary>
    public class WorldSpaceHealthBar : MonoBehaviour
    {
        #region Variables
        [Tooltip("체력을 관리하는 Health 컴포넌트 참조")]
        [SerializeField] private Health health;

        [Tooltip("체력 게이지 UI 이미지 (Fill Type)")]
        [SerializeField] private Image healthBarImage;

        [Tooltip("체력바 전체 부모 오브젝트 (HP Full 시 비활성화용)")]
        [SerializeField] private GameObject healthBarParent;

        [Tooltip("UI가 플레이어(카메라) 정면을 바로 바라보도록 180도 반전 회전")]
        [SerializeField] private bool rotate180 = true;

        // 플레이어 카메라 Transform 참조
        private Transform mainCameraTransform;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조 자동 설정
            if (health == null)
                health = GetComponentInParent<Health>();

            // healthBarParent가 미지정 시 자식 Canvas를 자동 지정 (본체 transform 회전 방지)
            if (healthBarParent == null)
            {
                Canvas canvas = GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    healthBarParent = canvas.gameObject;
                }
            }

            // 메인 카메라 구하기
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            if (mainCameraTransform == null && Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (health == null) return;

            // 1. Current Health 값에 따른 게이지 관리
            if (healthBarImage != null)
            {
                healthBarImage.fillAmount = health.HealthRatio;
            }

            // 3 & 4. 현재 Health값이 maxHealth(HealthRatio >= 1) 이면 보이지 않게 하고,
            // 데미지를 입어 HP가 줄어들면(HealthRatio < 1) 게이지바를 보여준다.
            bool shouldShow = health.HealthRatio < 1f && health.CurrentHealth > 0f;

            if (healthBarParent != null)
            {
                healthBarParent.SetActive(shouldShow);
            }
            else if (healthBarImage != null)
            {
                healthBarImage.gameObject.SetActive(shouldShow);
            }

            // 2. 월드 캔버스 UI만 항상 플레이어(카메라)를 바라보도록 회전 (로봇 본체 transform은 절대로 건드리지 않음!)
            Transform targetUITransform = (healthBarParent != null) ? healthBarParent.transform : null;

            if (targetUITransform != null && mainCameraTransform != null)
            {
                if (rotate180)
                {
                    targetUITransform.rotation = mainCameraTransform.rotation * Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    targetUITransform.rotation = mainCameraTransform.rotation;
                }
            }
        }
        #endregion
    }
}