using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    /// <summary>
    /// 플레이어의 체력 및 피격을 관리하는 클래스
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        #region Variables
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private float currentHealth;

        [Header("Visual Effects")]
        [SerializeField] private GameObject redFlashUI;             // 데미지 시 빨간색 화면 플래시 이미지

        [Header("Audio Settings")]
        [SerializeField] private AudioClip[] hurtSounds;            // 피격 시 랜덤 재생할 Hurt 사운드 배열
        [SerializeField] private AudioSource audioSource;

        [Header("Scene Transition")]
        [SerializeField] private string gameOverSceneName = "GameOverScene";

        private bool isDead = false;
        private Coroutine flashCoroutine;
        #endregion

        #region Properties
        public float CurrentHealth => currentHealth;
        public bool IsDead => isDead;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            currentHealth = maxHealth;

            // 오디오 소스 자동 설정
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        private void Start()
        {
            // ★ Hurt 사운드: Resources 폴더에서 직접 동적 로딩 (메모리 누락 방지)
            if (hurtSounds == null || hurtSounds.Length == 0)
            {
                System.Collections.Generic.List<AudioClip> list = new System.Collections.Generic.List<AudioClip>();
                for (int i = 1; i <= 3; i++)
                {
                    AudioClip clip = Resources.Load<AudioClip>($"Hurt0{i}");
                    if (clip != null)
                    {
                        list.Add(clip);
                    }
                }
                if (list.Count > 0)
                {
                    hurtSounds = list.ToArray();
                    Debug.Log($"[PlayerHealth] Successfully loaded {hurtSounds.Length} hurt sound clips from Resources.");
                }
                else
                {
                    Debug.LogWarning("[PlayerHealth] Hurt sounds (Hurt01~03) not found in Assets/MyFps/Resources/!");
                }
            }

            // 빨간색 데미지 플래시 화면 UI가 지정되지 않았다면 런타임에 캔버스를 동적 구성하여 자동 생성합니다.
            if (redFlashUI == null)
            {
                CreateDynamicRedFlashUI();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 플레이어 데미지 가해 함수
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            if (currentHealth < 0f) currentHealth = 0f;

            Debug.Log($"[Player] Took {amount} damage. Current Health: {currentHealth}");

            // 1. 빨간 화면 플래시 연출
            FlashRedScreen();

            // 2. 피격 랜덤 사운드 재생
            PlayRandomHurtSound();

            // 3. 체력 고갈 시 게임오버
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
        #endregion

        #region Private Methods
        private void PlayRandomHurtSound()
        {
            if (audioSource != null && hurtSounds != null && hurtSounds.Length > 0)
            {
                int index = Random.Range(0, hurtSounds.Length);
                AudioClip clip = hurtSounds[index];
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
            }
        }

        private void FlashRedScreen()
        {
            if (redFlashUI == null) return;

            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            UnityEngine.UI.Image img = redFlashUI.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                // 즉시 붉은색 활성화
                float elapsed = 0f;
                float durationIn = 0.05f; // 빠르게 깜빡임 시작
                float startAlpha = img.color.a;

                while (elapsed < durationIn)
                {
                    elapsed += Time.deltaTime;
                    img.color = new Color(1f, 0f, 0f, Mathf.Lerp(startAlpha, 0.4f, elapsed / durationIn));
                    yield return null;
                }

                // 부드럽게 복구
                elapsed = 0f;
                float durationOut = 0.5f;
                while (elapsed < durationOut)
                {
                    elapsed += Time.deltaTime;
                    img.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.4f, 0f, elapsed / durationOut));
                    yield return null;
                }

                img.color = new Color(1f, 0f, 0f, 0f);
            }
        }

        private void Die()
        {
            isDead = true;
            Debug.Log("[Player] Player health reached 0. Triggering Game Over.");

            // 이동 및 시야 조작 컨트롤 잠금
            PlayerMove playerMove = GetComponent<PlayerMove>();
            if (playerMove != null) playerMove.canMove = false;

            MouseLook mouseLook = GetComponent<MouseLook>();
            if (mouseLook != null) mouseLook.enabled = false;

            // SceneFader를 통한 자연스러운 화면 페이드아웃 후 씬 이동 시도
            SceneFader fader = FindObjectOfType<SceneFader>();
            if (fader != null)
            {
                fader.FadeTo(gameOverSceneName);
            }
            else
            {
                // 페이더가 없는 경우 즉시 로딩
                SceneManager.LoadScene(gameOverSceneName);
            }
        }

        private void CreateDynamicRedFlashUI()
        {
            // 씬에서 활성화된 캔버스 탐색
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("PlayerHealthCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // 빨간 플래시 효과용 Image 오브젝트 생성
            GameObject flashObj = new GameObject("RedFlashImage");
            flashObj.transform.SetParent(canvas.transform, false);

            UnityEngine.UI.Image img = flashObj.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(1f, 0f, 0f, 0f); // 투명 상태로 대기
            img.raycastTarget = false;             // UI 클릭 등을 가로막지 않도록 마우스 이벤트 해제

            // RectTransform 설정하여 전체 화면 가득 채움
            RectTransform rect = flashObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            redFlashUI = flashObj;
            Debug.Log("[PlayerHealth] Dynamically created full-screen Red Flash Image overlay.");
        }
        #endregion
    }
}
