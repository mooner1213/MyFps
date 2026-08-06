using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 무기 하나에 대한 Ammo UI를 관리하는 클래스
    /// - 무기 인덱스 보유
    /// - Ammo 카운트에 따라 게이지바 그리기
    /// - 액티브 무기와 비활성화 무기 UI 구분 (크기, 알파값)
    /// - Ammo 0일때 빨간색, 충전시 색 변경 효과 연출
    /// </summary>
    public class AmmoCounter : MonoBehaviour
    {
        #region Variables

        // 무기 인덱스 (WeaponHUDManager에서 초기화)
        public int WeaponCounterIndex { get; private set; }

        // 참조 - WeaponController
        private WeaponController weaponController;

        // --- Ammo 게이지바 UI ---
        [Tooltip("Ammo 게이지 이미지 (foreground fill image)")]
        public Image ammoFillImage;

        [Tooltip("게이지 배경 이미지")]
        public Image ammoBackgroundImage;

        [Header("Text UI (Optional)")]
        [Tooltip("탄약 수치 텍스트")]
        public TMPro.TextMeshProUGUI ammoText;

        [Tooltip("무기 번호 텍스트")]
        public TMPro.TextMeshProUGUI weaponIndexText;

        // --- 액티브 / 비활성 UI 구분 ---
        [Tooltip("액티브 무기 스케일")]
        public float activeScale = 1f;
        [Tooltip("비활성 무기 스케일")]
        public float inactiveScale = 0.7f;

        [Tooltip("액티브 무기 알파값")]
        [Range(0f, 1f)] public float activeAlpha = 1f;
        [Tooltip("비활성 무기 알파값")]
        [Range(0f, 1f)] public float inactiveAlpha = 0.5f;

        [Tooltip("UI 전환 보간 속도")]
        public float switchTransitionSharpness = 5f;

        // 현재 목표 스케일/알파
        private float targetScale;
        private float targetAlpha;
        private CanvasGroup canvasGroup;

        // --- Ammo 컬러 ---
        // 3번 항목: Ammo 0일때 빨간색 배경 이미지, 충전시 색 변경 효과
        [Header("Ammo Color")]
        [Tooltip("일반 상태 게이지 색상")]
        public Color fullAmmoColor = Color.white;
        [Tooltip("Ammo 0일때 게이지 배경 색상")]
        public Color emptyAmmoColor = Color.red;

        // 4번 항목: ForBackColorChange
        [Header("Fill & Background Color")]
        [Tooltip("FillImage 컬러: white")]
        public Color fillColorFull = Color.white;
        [Tooltip("FillImage 컬러: black (ammo 0)")]
        public Color fillColorEmpty = Color.black;

        [Tooltip("BackgroundImage 컬러: black(a:128)")]
        public Color backColorDefault = new Color(0f, 0f, 0f, 0.5f);
        [Tooltip("BackgroundImage 컬러: red(a:128) ammo 0일때")]
        public Color backColorEmpty = new Color(1f, 0f, 0f, 0.5f);

        [Tooltip("Ammo 충전 완료(100%)시 색 전환 효과 지속 시간")]
        public float fullAmmoPulseSpeed = 4f;
        private bool isFullAmmoPulse = false;
        private float pulseTimer = 0f;
        private float pulseDuration = 1.0f;

        // 이전 Ammo 비율 (충전 완료 감지)
        private float prevAmmoRatio = 0f;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // 초기 비활성 상태
            targetScale = inactiveScale;
            targetAlpha = inactiveAlpha;
        }

        private void Update()
        {
            if (weaponController == null) return;

            float ammoRatio = weaponController.GetCurrentAmmoRatio();

            // 게이지 fillAmount 설정
            if (ammoFillImage != null)
                ammoFillImage.fillAmount = ammoRatio;

            // 텍스트 업데이트
            if (ammoText != null)
                ammoText.text = Mathf.CeilToInt(weaponController.CurrentAmmo).ToString();
            if (weaponIndexText != null)
                weaponIndexText.text = (WeaponCounterIndex + 1).ToString();

            // --- 3번 + 4번 항목: ForBackColorChange ---
            UpdateAmmoColors(ammoRatio);

            // 충전 완료 펄스 효과
            UpdateFullAmmoPulse(ammoRatio);

            // 스케일 및 알파 부드럽게 전환
            transform.localScale = Vector3.Lerp(transform.localScale,
                Vector3.one * targetScale, switchTransitionSharpness * Time.deltaTime);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,
                targetAlpha, switchTransitionSharpness * Time.deltaTime);

            prevAmmoRatio = ammoRatio;
        }

        #endregion

        #region Custom Method

        /// <summary>
        /// WeaponHUDManager에서 호출하여 이 AmmoCounter를 초기화
        /// </summary>
        /// <param name="weapon">연결할 WeaponController</param>
        /// <param name="weaponIndex">무기 슬롯 인덱스</param>
        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            weaponController = weapon;
            WeaponCounterIndex = weaponIndex;
        }

        /// <summary>
        /// 이 AmmoCounter가 액티브 무기인지 설정 (UI 크기, 알파)
        /// </summary>
        /// <param name="isActive">액티브 무기 여부</param>
        public void SetWeaponActive(bool isActive)
        {
            targetScale = isActive ? activeScale : inactiveScale;
            targetAlpha = isActive ? activeAlpha : inactiveAlpha;
        }

        // Ammo 양에 따라 FillImage, BackgroundImage 색상 변경
        private void UpdateAmmoColors(float ammoRatio)
        {
            if (ammoRatio <= 0f)
            {
                // Ammo 0: FillImage black, BackgroundImage red(a:128)
                if (ammoFillImage != null)
                    ammoFillImage.color = fillColorEmpty;
                if (ammoBackgroundImage != null)
                    ammoBackgroundImage.color = backColorEmpty;
            }
            else
            {
                // Ammo 있음: FillImage white, BackgroundImage black(a:128)
                if (ammoFillImage != null)
                    ammoFillImage.color = fillColorFull;
                if (ammoBackgroundImage != null)
                    ammoBackgroundImage.color = backColorDefault;
            }
        }

        // Ammo 충전 완료(0 -> 100%)시 색 변경 효과 연출
        private void UpdateFullAmmoPulse(float ammoRatio)
        {
            // 충전 완료 감지: 이전에 0이었다가 1이 된 경우
            if (prevAmmoRatio < 1f && ammoRatio >= 1f)
            {
                isFullAmmoPulse = true;
                pulseTimer = 0f;
            }

            if (isFullAmmoPulse)
            {
                pulseTimer += Time.deltaTime * fullAmmoPulseSpeed;
                float t = Mathf.PingPong(pulseTimer, 1f);

                // 충전 완료 연출: 흰색 <-> 노란색 깜박임
                Color pulseColor = Color.Lerp(Color.white, Color.yellow, t);
                if (ammoFillImage != null)
                    ammoFillImage.color = pulseColor;

                if (pulseTimer >= pulseDuration)
                    isFullAmmoPulse = false;
            }
        }

        #endregion
    }
}
