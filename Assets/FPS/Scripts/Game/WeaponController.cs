using UnityEngine;
using UnityEngine.Audio;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 조준점 데이터 정의
    /// 이미지, 크기, 컬러 
    /// </summary>
    [System.Serializable]
    public struct CrossHairData
    {
        public Sprite CrossHairSprite;
        public float CrossHairSize;
        public Color CrossHairColor;
    }

    /// <summary>
    /// 무기별 슛 타입 정의
    /// </summary>
    public enum WeaponShootType
    {
        Manual,
        Automatic,
        Charge,
        Sniper,
        //..
    }

    /// <summary>
    /// 총기류 무기를 관리하는 클래스
    /// </summary>
    [RequireComponent (typeof(AudioSource))]
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        //무기 활성화, 비활성
        public GameObject weaponRoot;

        public GameObject Owner { get; set; }               //무기 주인
        public GameObject SourcePrefab { get; set; }        //무기를 생성한 프리팹
        public bool IsWeaponActive { get; private set; }    //무기 활성화 여부

        //슛팅 오디오
        private AudioSource shootAudioSource;
        public AudioClip switchWeaponSfx;           //무기 교체 효과음

        //크로스헤어
        public CrossHairData crossHairDefault;          //기본(평상시)
        public CrossHairData crossHairTargetInSight;    //적 포착시(타겟팅)

        //조준
        [Range(0, 1)] public float aimZoomratio = 1f;   //조준시 줌 비율
        public Vector3 aimOffset = Vector3.zero;        //조준 위치 이동시 무기별 위치 조정값

        //슛팅
        [SerializeField] private WeaponShootType shootType; //슛팅 타입

        [SerializeField] private float maxAmmo = 8f;        //최대 탄환 갯수
        private float currentAmmo;                          //현재 탄환 갯수

        public float CurrentAmmo => currentAmmo;
        public float MaxAmmo => maxAmmo;

        [SerializeField] private float delayBetweenShots = 0.5f;    //연사 방지, 초당 발사 갯수 
        private float lastTimeShot;

        //슛 연출
        public Transform weaponMuzzle;          //총구, 파이어포인트
        public GameObject muzzleFlashPrefab;    //총구 발사 이펙트 프리팹
        public AudioClip shootSfx;              //슛 사운드 클립(소스)

        //슛 반동 Recoil
        public float recoilForce = 0.5f;

        //발사체 Projectile
        public Vector3 MuzzleWorldVelocity { get; private set; }    //총구 이동 속도
        private Vector3 lastMuzzlePosition;
        public float CurrentCharge { get; private set; }            //충전 량

        public ProjectileBase ProjectilePrefab;     //발사체 프리팹
        public int bulletsPerShot = 1;              //한번 발사할때 마다 생성되는 발사체의 갯수
        public float bulletSpreadAngle = 0f;        //발사각

        // --- 차징 (Charge 타입 전용) ---
        [Header("Charge Settings")]
        [Tooltip("완전 차징에 걸리는 시간 (초)")]
        [SerializeField] private float maxChargeTime = 1f;
        private float chargeTimer = 0f;             //현재 차징 누적 시간
        private bool isCharging = false;            //차징 중 여부

        //외부(UI 등)에서 차징 상태 읽기
        public bool IsCharging => isCharging;
        public bool IsFullyCharged => chargeTimer >= maxChargeTime;
        public float ChargeRatio => Mathf.Clamp01(chargeTimer / maxChargeTime); //0~1

        // --- 탄약 자동 회복 & 수동 재장전 ---
        [Header("Reload & Auto Recharge")]
        [Tooltip("마지막 작동 후 자동 회복이 시작될 때까지의 대기시간 (초)")]
        [SerializeField] private float autoReloadDelay = 5f;

        [Tooltip("초당 자동 회복되는 탄약 수")]
        [SerializeField] private float autoReloadRate = 5f;

        [Tooltip("수동 재장전(R키) 소요 시간 (초)")]
        [SerializeField] private float manualReloadDuration = 2f;

        private bool isReloading = false;
        private float reloadTimer = 0f;

        public bool IsReloading => isReloading;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            shootAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            //초기화
            currentAmmo = maxAmmo;
            lastTimeShot = Time.time;
            lastMuzzlePosition = weaponMuzzle.position;
        }

        private void Update()
        {
            // 수동 재장전(R키) 처리 (2초 소요)
            if (isReloading)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= manualReloadDuration)
                {
                    currentAmmo = maxAmmo;
                    isReloading = false;
                    reloadTimer = 0f;
                    lastTimeShot = Time.time; // 재장전 완료 후 5초 타이머 리셋
                }
            }
            else
            {
                // 5초 룰: 마지막 작동 후 5초 동안 쏘지 않으면 탄약 자동 회복 (백그라운드 무기 포함)
                if (Time.time >= lastTimeShot + autoReloadDelay)
                {
                    if (currentAmmo < maxAmmo)
                    {
                        currentAmmo += autoReloadRate * Time.deltaTime;
                        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
                    }
                }
            }

            //이번 프레임의 총구 이동 속도는
            if (Time.deltaTime > 0)
            {   
                MuzzleWorldVelocity = (weaponMuzzle.position - lastMuzzlePosition) / Time.deltaTime;
                //이번 프레임의 위치 저장
                lastMuzzlePosition = weaponMuzzle.position;
            }
        }
        #endregion

        #region Custom Method
        //무기 활성화, 비활성화
        public void ShowWeapon(bool show)
        {
            // 무기를 내릴 때(비활성화/교체 시) 재장전 중이었다면 캔슬
            if (!show && isReloading)
            {
                CancelReload();
            }

            weaponRoot.SetActive(show);
            if(show == true && switchWeaponSfx != null)
            {
                //무기 교체 효과음 플레이
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }
            IsWeaponActive = show;
        }

        // 수동 재장전 시작
        public bool StartReload()
        {
            if (currentAmmo < maxAmmo && !isReloading)
            {
                isReloading = true;
                reloadTimer = 0f;
                chargeTimer = 0f;
                isCharging = false;
                return true;
            }
            return false;
        }

        // 수동 재장전 취소 (스왑 시 호출)
        public void CancelReload()
        {
            isReloading = false;
            reloadTimer = 0f;
            lastTimeShot = Time.time; // 5초 대기 타이머 리셋
        }

        //인풋에 따른 발사 처리
        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            if (isReloading) return false;

            switch(shootType)
            {
                case WeaponShootType.Manual:
                    if(inputDown == true)
                    {
                        return TryShoot();
                    }
                    break;

                case WeaponShootType.Automatic:
                    if (inputHeld == true)
                    {
                        return TryShoot();
                    }
                    break;

                case WeaponShootType.Charge:
                    // 마우스 누르는 중: 차징 누적
                    if (inputHeld)
                    {
                        chargeTimer += Time.deltaTime;
                        chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);
                        isCharging = true;
                    }

                    // 마우스 뗐을 때: 차징 완료 상태면 발사
                    if (inputUp)
                    {
                        if (IsFullyCharged)
                        {
                            bool didShoot = TryShoot();
                            chargeTimer = 0f;
                            isCharging = false;
                            return didShoot;
                        }
                        // 차징 미완료시 취소
                        chargeTimer = 0f;
                        isCharging = false;
                    }

                    // 버튼을 아예 안 누르고 있으면 차징 상태 초기화
                    if (!inputHeld && !inputUp)
                    {
                        chargeTimer = 0f;
                        isCharging = false;
                    }
                    break;

                case WeaponShootType.Sniper:
                    break;
            }

            return false;
        }

        //발사 처리
        private bool TryShoot()
        {
            //ammo 체크, 연사방지 체크
            if(currentAmmo >= 1f && lastTimeShot + delayBetweenShots < Time.time)
            {
                Debug.Log("Shoot!!!!!!");

                currentAmmo -= 1f;
                Debug.Log($"currentAmmo: {currentAmmo}");

                HandleShoot();

                return true;
            }

            return false;
        }

        //슛 연출 처리
        private void HandleShoot()
        {
            //발사체 생성
            for (int i = 0; i < bulletsPerShot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                ProjectileBase projectileInstance = Instantiate(ProjectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                projectileInstance.Shoot(this);
            }

            //효과(vfx, sfx)
            if(muzzleFlashPrefab)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab,
                    weaponMuzzle.position, weaponMuzzle.rotation, weaponMuzzle);
                Destroy(muzzleFlashInstance, 2f);
            }
            if(shootSfx)
            {
                shootAudioSource.PlayOneShot(shootSfx);
            }

            lastTimeShot = Time.time;
        }

        //발사각 설정
        private Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRation = bulletSpreadAngle / 180f;            
            return Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRation);
        }

        /// <summary>
        /// 현재 탄환 비율 반환 (0 ~ 1)
        /// AmmoCounter UI에서 게이지바 fillAmount 계산에 사용
        /// </summary>
        public float GetCurrentAmmoRatio()
        {
            if (maxAmmo <= 0f) return 1f;
            return currentAmmo / maxAmmo;
        }
        #endregion
    }
}