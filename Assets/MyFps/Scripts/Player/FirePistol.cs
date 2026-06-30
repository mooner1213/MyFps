using System.Collections;
using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 권총 사격 및 이펙트 연출을 관리하는 클래스
    /// - 사격 반동: 코드로 직접 위치/회전 킥백 구현 (M9.controller의 gunfire 클립이 비어있으므로)
    /// - 총구 불꽃: Resources/MuzzleFlash.prefab
    /// - 피격 이펙트: Resources/HitImpact.prefab
    /// - 사운드: PistolShot
    /// </summary>
    public class FirePistol : MonoBehaviour
    {
        #region Variables
        [Header("Input")]
        [SerializeField] private CharacterInput playerInput;

        [Header("Gun Settings")]
        [SerializeField] private float damage = 5f;
        [SerializeField] private float range = 100f;
        [SerializeField] private float fireRate = 0.3f;
        private float fireTimer = 0f;

        [Header("Recoil (Code-based)")]
        [SerializeField] private float recoilKickback = 0.04f;   // 뒤로 밀리는 거리
        [SerializeField] private float recoilRotation = 8f;      // 위로 젖혀지는 각도
        [SerializeField] private float recoilRecovery = 8f;      // 원위치 복귀 속도
        private Vector3 originalLocalPos;
        private Quaternion originalLocalRot;
        private bool isRecoiling = false;

        [Header("Effects")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private GameObject hitImpactPrefab;

        [Header("Audio")]
        [SerializeField] private AudioClip pistolShotSound;
        [SerializeField] private AudioSource audioSource;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void Start()
        {
            // 원래 로컬 위치/회전 기억 (반동 후 원위치 복귀용)
            originalLocalPos = transform.localPosition;
            originalLocalRot = transform.localRotation;

            // 플레이어 인풋 자동 탐색
            if (playerInput == null)
            {
                playerInput = GetComponentInParent<CharacterInput>();
                if (playerInput == null)
                    playerInput = FindObjectOfType<CharacterInput>();
            }

            // ★ 격발 사운드: Resources 폴더에서 직접 로드
            if (pistolShotSound == null)
            {
                pistolShotSound = Resources.Load<AudioClip>("PistolShot");
                if (pistolShotSound != null)
                {
                    Debug.Log("[FirePistol] PistolShot sound loaded from Resources.");
                }
                else
                {
                    Debug.LogWarning("[FirePistol] PistolShot audio clip not found in Resources!");
                }
            }

            // MuzzleFlash 프리팹 - Resources 폴더에서 로드
            if (muzzleFlashPrefab == null)
            {
                muzzleFlashPrefab = Resources.Load<GameObject>("MuzzleFlash");
                if (muzzleFlashPrefab != null)
                    Debug.Log("[FirePistol] MuzzleFlash loaded from Resources.");
                else
                    Debug.LogWarning("[FirePistol] MuzzleFlash not found in Assets/MyFps/Resources/");
            }

            // HitImpact 프리팹 - Resources 폴더에서 로드
            if (hitImpactPrefab == null)
            {
                hitImpactPrefab = Resources.Load<GameObject>("HitImpact");
                if (hitImpactPrefab != null)
                    Debug.Log("[FirePistol] HitImpact loaded from Resources.");
                else
                    Debug.LogWarning("[FirePistol] HitImpact not found in Assets/MyFps/Resources/");
            }

            // 총구 위치 설정
            if (muzzlePoint == null)
            {
                Transform existing = transform.Find("MuzzlePoint");
                if (existing != null)
                {
                    muzzlePoint = existing;
                }
                else
                {
                    GameObject mp = new GameObject("MuzzlePoint");
                    mp.transform.SetParent(transform, false);
                    mp.transform.localPosition = new Vector3(0f, 0.05f, 0.3f);
                    muzzlePoint = mp.transform;
                }
                Debug.Log($"[FirePistol] MuzzlePoint at local {muzzlePoint.localPosition}");
            }
        }

        private void Update()
        {
            if (fireTimer > 0f)
                fireTimer -= Time.deltaTime;

            // 반동 후 원위치 복귀 (Lerp)
            if (isRecoiling)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, Time.deltaTime * recoilRecovery);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, originalLocalRot, Time.deltaTime * recoilRecovery);

                // 거의 원위치에 도달하면 정확히 고정
                if (Vector3.Distance(transform.localPosition, originalLocalPos) < 0.001f)
                {
                    transform.localPosition = originalLocalPos;
                    transform.localRotation = originalLocalRot;
                    isRecoiling = false;
                }
            }

            // 발사 조건
            if (playerInput != null && playerInput.IsAttack && fireTimer <= 0f && Time.timeScale > 0f)
            {
                PlayerHealth health = playerInput.GetComponent<PlayerHealth>();
                if (health != null && health.IsDead) return;

                fireTimer = fireRate;
                Shoot();
            }
        }
        #endregion

        #region Custom Methods
        private void Shoot()
        {
            // 1. 사운드
            if (audioSource != null && pistolShotSound != null)
                audioSource.PlayOneShot(pistolShotSound);
            else
                Debug.LogWarning("[FirePistol] PistolShot sound missing!");

            // 2. 코드 기반 반동 킥백 (뒤로 + 위로)
            ApplyRecoil();

            // 3. 총구 불꽃
            if (muzzleFlashPrefab != null && muzzlePoint != null)
            {
                GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
                Destroy(flash, 0.1f);
                Debug.Log("[FirePistol] MuzzleFlash spawned.");
            }
            else
            {
                Debug.LogWarning($"[FirePistol] Cannot spawn MuzzleFlash: prefab={muzzleFlashPrefab != null}, point={muzzlePoint != null}");
            }

            // 4. 레이캐스트 사격
            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Debug.Log($"[FirePistol] Hit: {hit.transform.name}");

                // 적 로봇 데미지
                Robot robot = hit.transform.GetComponent<Robot>()
                           ?? hit.transform.GetComponentInParent<Robot>();
                if (robot != null)
                    robot.TakeDamage(damage);

                // 피격 이펙트
                if (hitImpactPrefab != null)
                {
                    GameObject impact = Instantiate(hitImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 1.5f);
                }
                else
                {
                    Debug.LogWarning("[FirePistol] HitImpact prefab missing!");
                }
            }
        }

        private void ApplyRecoil()
        {
            // 총을 뒤로 당기고 위로 꺾는 코드 기반 반동
            isRecoiling = true;
            transform.localPosition = originalLocalPos + new Vector3(0f, 0f, -recoilKickback);
            transform.localRotation = originalLocalRot * Quaternion.Euler(-recoilRotation, 0f, 0f);
        }
        #endregion
    }
}
