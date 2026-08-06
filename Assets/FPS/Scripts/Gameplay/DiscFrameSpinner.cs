using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// Secondary_Weapon_Disc_Frame 오브젝트를 차징 상태에 따라 회전시키는 컴포넌트
    /// - 평소(대기 중): 천천히 회전
    /// - 차징 중: ChargeRatio(0~1)에 따라 점차 빠르게 회전
    /// - 차징 해제 후: 다시 천천히 회전으로 복귀
    /// </summary>
    public class DiscFrameSpinner : MonoBehaviour
    {
        #region Variables

        [Tooltip("WeaponController 참조 (부모 계층에서 자동 탐색)")]
        private WeaponController weaponController;

        [Header("Rotation Speed")]
        [Tooltip("평소 느린 회전 속도 (도/초)")]
        [SerializeField] private float idleRotationSpeed = 60f;

        [Tooltip("차징 완료 시 최대 회전 속도 (도/초)")]
        [SerializeField] private float maxChargeRotationSpeed = 720f;

        [Tooltip("속도 변화 부드러움 (Lerp 계수) - 클수록 빠르게 반응")]
        [SerializeField] private float speedTransitionSharpness = 5f;

        [Header("Rotation Axis")]
        [Tooltip("회전 축 (디스크 특성상 Z축 기준)")]
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        // 현재 실제 회전 속도 (Lerp 적용)
        private float currentRotationSpeed;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            // 부모 계층에서 WeaponController 찾기
            weaponController = GetComponentInParent<WeaponController>();

            if (weaponController == null)
            {
                Debug.LogWarning($"[DiscFrameSpinner] WeaponController를 찾지 못했습니다: {gameObject.name}");
            }

            // 처음엔 느린 회전 속도로 시작
            currentRotationSpeed = idleRotationSpeed;
        }

        private void Update()
        {
            // 목표 회전 속도 계산
            float targetSpeed = CalculateTargetSpeed();

            // 현재 속도를 목표 속도로 부드럽게 전환
            currentRotationSpeed = Mathf.Lerp(
                currentRotationSpeed,
                targetSpeed,
                speedTransitionSharpness * Time.deltaTime
            );

            // 회전 적용
            transform.Rotate(rotationAxis, currentRotationSpeed * Time.deltaTime, Space.Self);
        }

        #endregion

        #region Custom Method

        private float CalculateTargetSpeed()
        {
            if (weaponController == null)
                return idleRotationSpeed;

            // 차징 중일 때: ChargeRatio(0~1)에 따라 idleSpeed ~ maxChargeSpeed 사이 보간
            if (weaponController.IsCharging)
            {
                return Mathf.Lerp(
                    idleRotationSpeed,
                    maxChargeRotationSpeed,
                    weaponController.ChargeRatio
                );
            }

            // 차징 안 할 때: 느린 속도로 복귀
            return idleRotationSpeed;
        }

        #endregion
    }
}
