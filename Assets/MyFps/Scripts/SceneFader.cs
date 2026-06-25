using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MyFps
{
    /// <summary>
    /// 화면 페이드 효과(어두워짐/밝아짐) 및 씬 이동을 관리하는 스크립트 (싱글톤)
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        #region 싱글톤 설정 (어디서나 쉽게 부르기 위함)
        public static SceneFader Instance { get; private set; }

        private void Awake()
        {
            // 씬에 SceneFader가 유일하게 하나만 존재하도록 보장합니다.
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null); // 부모 오브젝트가 있다면 분리하여 루트 오브젝트로 변경 (DontDestroyOnLoad 경고/버그 방지)
                DontDestroyOnLoad(gameObject); // 씬을 이동해도 파괴되지 않고 유지됩니다.
            }
            else
            {
                Destroy(gameObject); // 중복된 인스턴스가 있다면 파괴합니다.
                return;
            }

            // 에디터에서 개발 편의상 Canvas나 Image 오브젝트를 비활성화해 두었더라도
            // 게임 시작 시 자동으로 활성화하여 정상적으로 페이드인이 시작되도록 만듭니다.
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
            }

            if (img != null)
            {
                img.gameObject.SetActive(true);

                Color color = img.color;
                color.a = 1f; // 시작할 때는 완전 검은색으로 고정
                img.color = color;
            }
        }
        #endregion

        #region 변수 설정
        [Header("UI 이미지")]
        public Image img; // 화면을 덮을 검은색 이미지 컴포넌트

        [Header("페이드 소요 시간")]
        public float fadeDuration = 1f; // 페이드가 진행될 시간 (초 단위)

        private bool isFirstScene = true; // 최초 시작 씬인지 확인하는 플래그
        #endregion

        #region Unity 이벤트 함수
        private void Start()
        {
            // 게임 시작 또는 씬 로드 시 자동으로 밝아지는 효과(FadeIn)를 실행합니다.
            PlayFadeIn();
        }

        // 씬 로드 이벤트를 구독하여 새로운 씬으로 넘어갔을 때도 자동으로 페이드인이 되도록 만듭니다.
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 최초 게임 시작 시(Start)는 OnSceneLoaded에서의 이중 호출을 건너뜁니다.
            if (isFirstScene)
            {
                isFirstScene = false;
                return;
            }

            PlayFadeIn();
        }
        #endregion

        #region 외부에서 호출할 수 있는 쉬운 함수들 (API)

        /// <summary>
        /// 화면을 서서히 밝게 만듭니다. (검은색 -> 투명)
        /// 사용법: SceneFader.Instance.PlayFadeIn();
        /// </summary>
        public void PlayFadeIn()
        {
            StopAllCoroutines(); // 현재 진행 중인 페이드가 있다면 취소하고 새로 시작
            StartCoroutine(FadeInCoroutine());
        }

        /// <summary>
        /// 화면을 서서히 어둡게 만듭니다. (투명 -> 검은색)
        /// 사용법: SceneFader.Instance.PlayFadeOut();
        /// </summary>
        public void PlayFadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine());
        }

        /// <summary>
        /// 화면을 어둡게 만든 후 지정된 씬으로 이동합니다.
        /// 사용법: SceneFader.Instance.FadeToScene("씬이름");
        /// </summary>
        /// <param name="sceneName">이동할 씬의 이름</param>
        public void FadeToScene(string sceneName)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAndLoadCoroutine(sceneName));
        }

        #endregion

        #region 실제 작동 로직 (코루틴)

        // 서서히 밝아지는 코루틴
        private IEnumerator FadeInCoroutine()
        {
            float elapsed = 0f;
            Color color = img.color;
            color.a = 1f; // 시작할 때는 완전 검은색
            img.color = color;

            // 게임 시작 직후 유니티 엔진 초기화 렉(CPU 병목)이 해소될 때까지 3프레임 대기합니다.
            // 렉이 걸려 프레임 타임이 급격히 늘어난 상태로 바로 시작하면 페이드가 스킵되는 문제를 해결합니다.
            yield return null;
            yield return null;
            yield return null;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // 프레임이 밀리거나 정지해도 정상 작동하게 unscaledDeltaTime 사용
                color.a = Mathf.Clamp01(1f - (elapsed / fadeDuration));
                img.color = color;
                yield return null;
            }

            color.a = 0f; // 마지막에는 완전 투명하게 설정
            img.color = color;
        }

        // 서서히 어두워지는 코루틴
        private IEnumerator FadeOutCoroutine()
        {
            float elapsed = 0f;
            Color color = img.color;
            color.a = 0f; // 시작할 때는 투명
            img.color = color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                color.a = Mathf.Clamp01(elapsed / fadeDuration);
                img.color = color;
                yield return null;
            }

            color.a = 1f; // 마지막에는 완전 검은색
            img.color = color;
        }

        // 화면을 어둡게 만든 후 새로운 씬을 로드하는 코루틴
        private IEnumerator FadeAndLoadCoroutine(string sceneName)
        {
            // 1. 먼저 페이드 아웃(어두워짐) 코루틴이 완료될 때까지 대기합니다.
            yield return StartCoroutine(FadeOutCoroutine());

            // 2. 부드러운 전환을 위해 아주 미세한 대기 시간을 둡니다.
            yield return new WaitForSecondsRealtime(0.1f);

            // 3. 씬을 로드합니다. (이후 OnSceneLoaded에 의해 새 씬에서 자동으로 페이드 인이 켜집니다.)
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}