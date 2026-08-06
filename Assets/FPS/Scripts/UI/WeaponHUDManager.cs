using System.Collections.Generic;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 무기(Ammo) UI들을 관리하는 클래스
    /// - 무기 추가시 Weapon UI(ammoCounterPrefab) 생성 후 추가
    /// - 무기 제거시 Weapon UI 삭제
    /// - Active 무기 변경시 Weapon UI 갱신 (크기, 알파값)
    /// </summary>

    public class WeaponHUDManager : MonoBehaviour
    {
        #region Variables
        // 참조
        private PlayerWeaponManager playerWeaponsManager;

        // UI 추가
        public RectTransform ammoPannel;        // 탄약 UI 패널
        public GameObject ammoCountPrefab;      // 탄약 UI 프리팹
        private List<AmmoCounter> ammoCounters = new List<AmmoCounter>();

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            playerWeaponsManager = GameObject.FindFirstObjectByType<PlayerWeaponManager>();
        }

        private void OnEnable()
        {
            // 이벤트 함수 등록
            playerWeaponsManager.OnAddedWeapon += AddWeapon;
            playerWeaponsManager.OnRemovedWeapon += RemoveWeapon;
            playerWeaponsManager.OnSwitchToWeapon += SwitchWeapon;
        }

        private void OnDisable()
        {
            // 이벤트 함수 해제
            playerWeaponsManager.OnAddedWeapon -= AddWeapon;
            playerWeaponsManager.OnRemovedWeapon -= RemoveWeapon;
            playerWeaponsManager.OnSwitchToWeapon -= SwitchWeapon;
        }
        #endregion

        #region Custom Method
        // 무기 추가시 호출되는 함수
        private void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            // Inspector 연결이 안 된 경우 조용히 건너뜀 (예외가 PlayerWeaponManager.Start()를 중단시키지 않도록)
            if (ammoCountPrefab == null || ammoPannel == null)
                return;

            // ammoCountPrefab 생성 후 ammoPannel의 자식으로 추가
            AmmoCounter ammoCounter = Instantiate(ammoCountPrefab, ammoPannel)
                .GetComponent<AmmoCounter>();

            if (ammoCounter == null)
                return;

            // 초기화: 어떤 무기와 연결되는지, 인덱스 설정
            ammoCounter.Initialize(newWeapon, weaponIndex);

            // 처음 추가된 무기는 비활성 상태 UI로 표시
            ammoCounter.SetWeaponActive(false);

            ammoCounters.Add(ammoCounter);
        }

        // 무기 제거시 호출되는 함수
        private void RemoveWeapon(WeaponController oldWeapon, int weaponIndex)
        {
            // 인덱스에 해당하는 AmmoCounter 찾기
            int counterToRemoveIndex = -1;
            for (int i = 0; i < ammoCounters.Count; i++)
            {
                if (ammoCounters[i].WeaponCounterIndex == weaponIndex)
                {
                    counterToRemoveIndex = i;
                    break;
                }
            }

            // 찾았으면 UI 삭제
            if (counterToRemoveIndex >= 0)
            {
                Destroy(ammoCounters[counterToRemoveIndex].gameObject);
                ammoCounters.RemoveAt(counterToRemoveIndex);
            }
        }

        // 무기 교체시 호출되는 함수
        private void SwitchWeapon(WeaponController newActiveWeapon)
        {
            // 모든 AmmoCounter를 순회해서 현재 액티브 무기와 비교
            for (int i = 0; i < ammoCounters.Count; i++)
            {
                // 액티브 무기의 인덱스와 같으면 active, 아니면 inactive
                bool isActiveWeapon =
                    ammoCounters[i].WeaponCounterIndex == playerWeaponsManager.ActiveWeaponIndex;
                ammoCounters[i].SetWeaponActive(isActiveWeapon);
            }
        }
        #endregion
    }
}