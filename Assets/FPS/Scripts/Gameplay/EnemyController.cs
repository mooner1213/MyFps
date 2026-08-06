using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using MyFps;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// 적 캐릭터의 공통 기능(체력 관리, 피격 시 Material 흰색 Flash, 피격/사망 사운드 재생, 하버봇 타겟팅 눈알 Material 변경)을 담당하는 클래스
    /// IDamageable 인터페이스를 상속받습니다.
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("체력 설정 (Health Settings)")]
        [Tooltip("적의 최대 체력")]
        [SerializeField] private float maxHealth = 50f;
        [Tooltip("적의 현재 체력")]
        [SerializeField] private float currentHealth = 0f;
        [Tooltip("사망 여부 체크")]
        [SerializeField] private bool isDeath = false;

        // 체력 및 사망 관련 이벤트
        public event System.Action OnDeath;
        public event System.Action<float> OnDamaged;

        // Health 컴포넌트 자동 연동 참조
        private Health healthComponent;

        [Header("데미지 플래시 설정 (Damage Flash Settings)")]
        [Tooltip("피격 시 적용할 흰색 Material (비어있으면 동적 생성)")]
        [SerializeField] private Material whiteFlashMaterial;
        [Tooltip("흰색 Material 유지 시간 (초 단위)")]
        [SerializeField] private float flashDuration = 0.15f;
        [Tooltip("플래시 효과를 적용할 Renderer 배열 (비어있으면 GetComponentsInChildren으로 자동 수집)")]
        [SerializeField] private Renderer[] renderersToFlash;

        // 원본 Material 데이터 구조 및 목록
        private class RendererMaterialData
        {
            public Renderer renderer;
            public Material[] originalMaterials;
        }
        private List<RendererMaterialData> originalRendererDataList = new List<RendererMaterialData>();
        private Coroutine flashCoroutine;
        private MaterialPropertyBlock flashPropertyBlock;

        [Header("사운드 설정 (Sound Settings)")]
        [Tooltip("데미지를 입었을 때 재생할 피격 사운드 AudioClip")]
        [SerializeField] private AudioClip hitSound;
        [Tooltip("적의 피가 전부 닳아서 없어질 때 재생할 폭발/사망 사운드 AudioClip")]
        [SerializeField] private AudioClip deathSound;
        [Tooltip("사운드 재생용 AudioSource 컴포넌트")]
        [SerializeField] private AudioSource audioSource;

        [Header("하버봇 타겟팅 눈알 설정 (HoverBot Eye Settings)")]
        [Tooltip("플레이어 타겟팅 시 Material을 변경할 눈알(레이저 발사 위치) 오브젝트의 Renderer")]
        [SerializeField] private Renderer eyeRenderer;
        [Tooltip("눈알 Material 인덱스 (기본 -1: 이름에 Eye가 포함된 Material 자동 탐색)")]
        [SerializeField] private int eyeMaterialIndex = -1;
        [Tooltip("타겟팅 시 적용할 빨간색 Material (비어있으면 동적 생성)")]
        [SerializeField] private Material redEyeMaterial;
        [Tooltip("눈알의 원래 Material (Start 시 자동 저장됨)")]
        [SerializeField] private Material originalEyeMaterial;
        [Tooltip("현재 플레이어를 타겟팅 중인지 여부")]
        [SerializeField] private bool isTargeting = false;

        // 프로퍼티
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDeath => isDeath;
        public bool IsTargeting => isTargeting;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // AudioSource 컴포넌트 자동 참조 및 추가
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            // MaterialPropertyBlock 생성
            flashPropertyBlock = new MaterialPropertyBlock();

            // Health 컴포넌트 자동 감지 및 피격 이벤트 구독
            healthComponent = GetComponent<Health>();
            if (healthComponent == null)
            {
                healthComponent = GetComponentInParent<Health>();
            }
            if (healthComponent != null)
            {
                healthComponent.onDamaged += OnHealthComponentDamaged;
            }
        }

        private void Start()
        {
            // 체력 초기화
            currentHealth = maxHealth;
            isDeath = false;

            // 1. Material 플래시 대상 Renderer 수집 및 원본 Material 백업
            InitRendererData();

            // 2. 피격 시 사용할 흰색 Material 준비
            InitWhiteFlashMaterial();

            // 3. 하버봇 눈알(Eye) Renderer 및 Material 백업 및 빨간색 Material 준비
            InitEyeMaterialData();

            // 4. 자식 콜라이더 피격 전달 중계 컴포넌트 자동 등록
            InitChildCollidersRelay();
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.onDamaged -= OnHealthComponentDamaged;
            }
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// 모든 Renderer를 찾아 원본 Material 목록을 백업 저장합니다.
        /// </summary>
        private void InitRendererData()
        {
            if (renderersToFlash == null || renderersToFlash.Length == 0)
            {
                renderersToFlash = GetComponentsInChildren<Renderer>(true);
            }

            originalRendererDataList.Clear();
            foreach (Renderer rend in renderersToFlash)
            {
                if (rend == null) continue;
                if (rend is ParticleSystemRenderer || rend is TrailRenderer || rend is CanvasRenderer) continue;

                RendererMaterialData data = new RendererMaterialData
                {
                    renderer = rend,
                    originalMaterials = rend.sharedMaterials
                };
                originalRendererDataList.Add(data);
            }
        }

        /// <summary>
        /// 피격 시 전체 Material을 흰색으로 만들 커스텀 Material을 생성합니다.
        /// </summary>
        private void InitWhiteFlashMaterial()
        {
            if (whiteFlashMaterial == null)
            {
                Shader flashShader = Shader.Find("Universal Render Pipeline/Unlit");
                if (flashShader == null) flashShader = Shader.Find("Unlit/Color");
                if (flashShader == null) flashShader = Shader.Find("Standard");

                whiteFlashMaterial = new Material(flashShader);
                whiteFlashMaterial.color = Color.white;

                if (whiteFlashMaterial.HasProperty("_BaseColor"))
                    whiteFlashMaterial.SetColor("_BaseColor", Color.white);
                if (whiteFlashMaterial.HasProperty("_Color"))
                    whiteFlashMaterial.SetColor("_Color", Color.white);

                whiteFlashMaterial.name = "Dynamic_WhiteFlashMaterial";
            }
        }

        /// <summary>
        /// 눈알(Eye) Renderer 및 Material 인덱스를 탐색하여 백업합니다.
        /// </summary>
        private void InitEyeMaterialData()
        {
            if (eyeRenderer == null)
            {
                Renderer[] rends = GetComponentsInChildren<Renderer>(true);
                foreach (Renderer r in rends)
                {
                    Material[] mats = r.sharedMaterials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        if (mats[i] != null && mats[i].name.ToLower().Contains("eye"))
                        {
                            eyeRenderer = r;
                            eyeMaterialIndex = i;
                            break;
                        }
                    }
                    if (eyeRenderer != null) break;
                }
            }

            if (eyeRenderer != null)
            {
                Material[] mats = eyeRenderer.sharedMaterials;

                if (eyeMaterialIndex < 0 || eyeMaterialIndex >= mats.Length)
                {
                    eyeMaterialIndex = 0;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        if (mats[i] != null && mats[i].name.ToLower().Contains("eye"))
                        {
                            eyeMaterialIndex = i;
                            break;
                        }
                    }
                }

                if (originalEyeMaterial == null && eyeMaterialIndex >= 0 && eyeMaterialIndex < mats.Length)
                {
                    originalEyeMaterial = mats[eyeMaterialIndex];
                }

                if (redEyeMaterial == null)
                {
                    Shader redShader = Shader.Find("Universal Render Pipeline/Unlit");
                    if (redShader == null) redShader = Shader.Find("Unlit/Color");
                    if (redShader == null) redShader = Shader.Find("Standard");

                    redEyeMaterial = new Material(redShader);
                    redEyeMaterial.color = Color.red;

                    if (redEyeMaterial.HasProperty("_BaseColor"))
                        redEyeMaterial.SetColor("_BaseColor", Color.red);
                    if (redEyeMaterial.HasProperty("_Color"))
                        redEyeMaterial.SetColor("_Color", Color.red);
                    if (redEyeMaterial.HasProperty("_EmissionColor"))
                    {
                        redEyeMaterial.EnableKeyword("_EMISSION");
                        redEyeMaterial.SetColor("_EmissionColor", Color.red * 2f);
                    }

                    redEyeMaterial.name = "Dynamic_RedEyeMaterial";
                }
            }
        }

        /// <summary>
        /// 자식 오브젝트의 콜라이더에 IDamageable 피격을 부모 EnemyController로 중계해주는 컴포넌트를 등록합니다.
        /// </summary>
        private void InitChildCollidersRelay()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider col in colliders)
            {
                if (col.gameObject != this.gameObject)
                {
                    if (col.GetComponent<IDamageable>() == null)
                    {
                        var relay = col.gameObject.AddComponent<EnemyDamageRelay>();
                        relay.Setup(this);
                    }
                }
            }
        }

        /// <summary>
        /// Unity FPS Health 컴포넌트의 onDamaged 이벤트 발생 시 자동으로 피격 효과 처리
        /// </summary>
        private void OnHealthComponentDamaged(float damage, GameObject damageSource)
        {
            TriggerHitEffects(damage);
        }

        /// <summary>
        /// 데미지를 입었을 때 호출되는 메소드 (IDamageable 구현)
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (isDeath) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            TriggerHitEffects(damage);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// 피격 시 Material 흰색 Flash 및 피격 사운드를 실행합니다.
        /// </summary>
        private void TriggerHitEffects(float damage)
        {
            if (isDeath) return;

            PlayHitSound();
            FlashWhite();
            OnDamaged?.Invoke(damage);
        }

        /// <summary>
        /// 피격 사운드를 재생합니다.
        /// </summary>
        private void PlayHitSound()
        {
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        /// <summary>
        /// 사망 시 사망/폭발 사운드를 재생하고 사망 로직을 처리합니다.
        /// </summary>
        private void Die()
        {
            if (isDeath) return;
            isDeath = true;

            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            SetTargeting(false);
            OnDeath?.Invoke();

            Debug.Log($"<color=red>[EnemyController]</color> {gameObject.name} 적 파괴됨 (사망 폭발 사운드 재생)");
        }

        /// <summary>
        /// 적의 모든 Material을 흰색으로 변경 후 원래대로 복원하는 코루틴을 시작합니다.
        /// </summary>
        private void FlashWhite()
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashWhiteRoutine());
        }

        /// <summary>
        /// 지정된 flashDuration 동안 흰색 Material 및 PropertyBlock을 적용하고 본래 Material로 돌아가는 코루틴
        /// </summary>
        private IEnumerator FlashWhiteRoutine()
        {
            flashPropertyBlock.SetColor("_BaseColor", Color.white);
            flashPropertyBlock.SetColor("_Color", Color.white);
            flashPropertyBlock.SetColor("_EmissionColor", Color.white);

            foreach (var data in originalRendererDataList)
            {
                if (data.renderer == null) continue;

                int matCount = data.originalMaterials.Length;
                Material[] flashMats = new Material[matCount];
                for (int i = 0; i < matCount; i++)
                {
                    flashMats[i] = whiteFlashMaterial != null ? whiteFlashMaterial : data.originalMaterials[i];
                }
                data.renderer.materials = flashMats;
                data.renderer.SetPropertyBlock(flashPropertyBlock);
            }

            yield return new WaitForSeconds(flashDuration);

            RestoreOriginalMaterials();
            flashCoroutine = null;
        }

        /// <summary>
        /// 적의 모든 Renderer의 Material을 원본 Material로 복원하고 PropertyBlock을 초기화합니다.
        /// </summary>
        private void RestoreOriginalMaterials()
        {
            foreach (var data in originalRendererDataList)
            {
                if (data.renderer != null)
                {
                    data.renderer.SetPropertyBlock(null);
                    if (data.originalMaterials != null)
                    {
                        data.renderer.materials = data.originalMaterials;
                    }
                }
            }

            if (isTargeting)
            {
                ApplyEyeTargetMaterial(true);
            }
        }

        /// <summary>
        /// 플레이어 타겟팅 여부에 따라 하버봇 '눈알' 인덱스의 Material만 정교하게 변경합니다.
        /// </summary>
        public void SetTargeting(bool targeting)
        {
            isTargeting = targeting;
            ApplyEyeTargetMaterial(targeting);
        }

        /// <summary>
        /// eyeRenderer의 eyeMaterialIndex 슬롯만 선택적으로 빨간색 Material 또는 원본 Material로 교체합니다.
        /// </summary>
        private void ApplyEyeTargetMaterial(bool targeting)
        {
            if (eyeRenderer == null) return;

            Material[] currentMats = eyeRenderer.materials;
            if (eyeMaterialIndex < 0 || eyeMaterialIndex >= currentMats.Length) return;

            if (targeting)
            {
                if (redEyeMaterial != null)
                {
                    currentMats[eyeMaterialIndex] = redEyeMaterial;
                    eyeRenderer.materials = currentMats;
                }
            }
            else
            {
                if (originalEyeMaterial != null)
                {
                    currentMats[eyeMaterialIndex] = originalEyeMaterial;
                    eyeRenderer.materials = currentMats;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 자식 콜라이더 오브젝트에 부착되어 피격 데미지를 부모 EnemyController로 전달하는 중계 클래스
    /// </summary>
    public class EnemyDamageRelay : MonoBehaviour, IDamageable
    {
        #region Variables
        private EnemyController parentController;
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        public void Setup(EnemyController controller)
        {
            parentController = controller;
        }

        public void TakeDamage(float damage)
        {
            if (parentController != null)
            {
                parentController.TakeDamage(damage);
            }
        }
        #endregion
    }
}
