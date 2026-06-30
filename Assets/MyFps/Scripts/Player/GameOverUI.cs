using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 寃뚯엫?ㅻ쾭 ?ъ쓽 UI 援ъ꽦 諛??ㅼ떆?섍린, 硫붾돱媛湲?醫낅즺 ?깆쓽 ?숈옉???쒖뼱?섎뒗 ?대옒??    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        #region Variables
        [Header("Scene Config")]
        [SerializeField] private string playSceneName = "PlayScene";

        [Header("Scene UI Buttons (Auto-Searched if Null)")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;

        [Header("Dynamic UI Fallback (If no UI in scene)")]
        [SerializeField] private Canvas fallbackCanvas;
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            // 寃뚯엫?ㅻ쾭 ???쒖옉 ??利됱떆 留덉슦??而ㅼ꽌 ?댁젣
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 移대찓???멸낸 ?щ갚???ㅻⅨ ???대?吏???섎뒛??Skybox) ?깆쑝濡??몄텧?섎뒗 寃껋쓣 諛⑹??섍린 ?꾪빐 移대찓??諛곌꼍???⑥깋 寃??됱쑝濡?媛뺤젣 ??뼱?곷땲??
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = Color.black;
            }
        }

        private void Start()
        {
            // 1. ?ъ슜?먭? ?ъ뿉 誘몃━ 留뚮뱾????UI ?붿냼瑜??먮룞 ?먯깋 諛?諛붿씤??(?덉씠?꾩썐 媛뺤젣 ?섏젙? 濡ㅻ갚)
            AutoFindSceneButtons();

            // 2. 留뚯빟 ?ъ뿉 ?ъ슜?먭? 諛곗튂??踰꾪듉???섎굹???녿떎硫? ?덉쟾?μ튂濡??숈쟻 UI Canvas ?앹꽦
            if (retryButton == null && quitButton == null)
            {
                Debug.LogWarning("[GameOverUI] No UI buttons found in the scene. Generating fallback dynamic UI...");
                CreateFallbackUIHierarchy();
            }

            // 3. 李얠븯嫄곕굹 ?앹꽦??媛?踰꾪듉???몃쾭 ???띿뒪?멸? 而ㅼ????좊땲硫붿씠???④낵 異붽?
            ApplyHoverEffects();
        }
        #endregion

        #region UI Button Event Methods
        /// <summary>
        /// ?ㅼ떆?섍린 踰꾪듉 ?대┃ ?대깽?? ?뚮젅?????щ줈??        /// </summary>

        public void LoadPlayScene()
        {
            Debug.Log("[GameOverUI] Restarting game, loading PlayScene...");
            SceneManager.LoadScene(playSceneName);
        }

        public void QuitGame()
        {
            Debug.Log("[GameOverUI] Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        /// </summary>
        private void AutoFindSceneButtons()
        {
            Button[] allButtons = FindObjectsOfType<Button>(true);
            foreach (Button btn in allButtons)
            {
                string btnName = btn.gameObject.name.ToLower();

                // 'retry', 'restart' ?대쫫 ?먯깋
                if (retryButton == null && (btnName.Contains("retry") || btnName.Contains("restart")))
                {
                    retryButton = btn;
                    retryButton.onClick.RemoveAllListeners();
                    retryButton.onClick.AddListener(LoadPlayScene);
                    Debug.Log($"[GameOverUI] Automatically bound Retry action to button: {btn.gameObject.name}");
                }
                // 'quit', 'exit', 'menu' ?대쫫 ?먯깋
                else if (quitButton == null && (btnName.Contains("quit") || btnName.Contains("exit") || btnName.Contains("menu")))
                {
                    quitButton = btn;
                    quitButton.onClick.RemoveAllListeners();
                    quitButton.onClick.AddListener(QuitGame);
                    Debug.Log($"[GameOverUI] Automatically bound Quit action to button: {btn.gameObject.name}");
                }
            }
        }

        /// <summary>
        /// ?ъ뿉 媛먯????앹꽦??踰꾪듉?ㅼ뿉 ?숈쟻?쇰줈 ?띿뒪???뺣? Hover ?④낵 而댄룷?뚰듃 ?μ갑
        /// </summary>
        private void ApplyHoverEffects()
        {
            if (retryButton != null && retryButton.gameObject.GetComponent<ButtonHoverEffect>() == null)
            {
                retryButton.gameObject.AddComponent<ButtonHoverEffect>();
            }
            if (quitButton != null && quitButton.gameObject.GetComponent<ButtonHoverEffect>() == null)
            {
                quitButton.gameObject.AddComponent<ButtonHoverEffect>();
            }
        }
        #endregion

        #region Fallback UI Builder (?덉쟾?μ튂)
        private void CreateFallbackUIHierarchy()
        {
            GameObject canvasObj = new GameObject("GameOverFallbackCanvas");
            fallbackCanvas = canvasObj.AddComponent<Canvas>();
            fallbackCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject bgObj = new GameObject("BackgroundPanel");
            bgObj.transform.SetParent(fallbackCanvas.transform, false);
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.1f, 0.02f, 0.02f, 0.95f);
            
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            GameObject textObj = new GameObject("GameOverText");
            textObj.transform.SetParent(fallbackCanvas.transform, false);
            TextMeshProUGUI goText = textObj.AddComponent<TextMeshProUGUI>();
            goText.text = "GAME OVER";
            goText.fontSize = 72;
            goText.fontStyle = FontStyles.Bold;
            goText.color = new Color(0.85f, 0.1f, 0.1f, 1f);
            goText.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.7f);
            textRect.anchorMax = new Vector2(0.5f, 0.7f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(600, 100);

            retryButton = CreateFallbackButton("RetryButton", "Retry (?ㅼ떆?섍린)", new Vector2(0, -20), LoadPlayScene);
            quitButton = CreateFallbackButton("QuitButton", "Quit (寃뚯엫醫낅즺)", new Vector2(0, -90), QuitGame);
        }

        private Button CreateFallbackButton(string objName, string buttonText, Vector2 pos, UnityEngine.Events.UnityAction onClickAction)
        {
            GameObject btnObj = new GameObject(objName);
            btnObj.transform.SetParent(fallbackCanvas.transform, false);
            
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImg;
            btn.onClick.AddListener(onClickAction);

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.45f);
            rect.anchorMax = new Vector2(0.5f, 0.45f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(240, 50);

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = buttonText;
            txt.fontSize = 20;
            txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.Center;

            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.25f, 0.25f, 0.25f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.1f, 0.1f, 1f);
            colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            btn.colors = colors;

            return btn;
        }
        #endregion
    }

    /// <summary>
    /// 踰꾪듉??留덉슦?ㅻ? ?щ졇????Hover) ?띿뒪?몄쓽 ?ш린瑜??쒖꽌???ㅼ썱??以꾩씠???좊땲硫붿씠???④낵 ?대옒??    /// </summary>
    public class ButtonHoverEffect : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
    {
        #region Variables
        private TextMeshProUGUI tmpText;
        private Text legacyText;
        private float originalSize;
        private float targetSize;
        private bool isHovering = false;
        private float scaleSpeed = 10f;     // ?ш린 蹂寃??좊땲硫붿씠???띾룄
        #endregion

        #region Unity Event Methods
        private void Start()
        {
            // ?먯떇 ?ㅻ툕?앺듃?먯꽌 Text 而댄룷?뚰듃 ?먮룞 ?먯깋 (TMP ?곗꽑)
            tmpText = GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                originalSize = tmpText.fontSize;
                targetSize = originalSize * 1.15f; // 15% ???ш쾶
            }
            else
            {
                legacyText = GetComponentInChildren<Text>();
                if (legacyText != null)
                {
                    originalSize = legacyText.fontSize;
                    targetSize = originalSize * 1.15f; // 15% ???ш쾶
                }
            }
        }

        private void Update()
        {
            // Hover ?곹깭???곕씪 Lerp瑜??ъ슜?섏뿬 遺?쒕윭???뺣?/異뺤냼 ?좊땲硫붿씠???섑뻾
            if (isHovering)
            {
                if (tmpText != null)
                {
                    tmpText.fontSize = Mathf.Lerp(tmpText.fontSize, targetSize, Time.deltaTime * scaleSpeed);
                }
                else if (legacyText != null)
                {
                    legacyText.fontSize = (int)Mathf.Lerp(legacyText.fontSize, targetSize, Time.deltaTime * scaleSpeed);
                }
            }
            else
            {
                if (tmpText != null)
                {
                    tmpText.fontSize = Mathf.Lerp(tmpText.fontSize, originalSize, Time.deltaTime * scaleSpeed);
                }
                else if (legacyText != null)
                {
                    legacyText.fontSize = (int)Mathf.Lerp(legacyText.fontSize, originalSize, Time.deltaTime * scaleSpeed);
                }
            }
        }

        private void OnDisable()
        {
            // 踰꾪듉??鍮꾪솢?깊솕?섎뒗 ?깆쓽 ?곹솴?먯꽌 ?띿뒪???ш린瑜?利됱떆 ?먮옒 ?ш린濡?由ъ뀑
            isHovering = false;
            if (tmpText != null)
            {
                tmpText.fontSize = originalSize;
            }
            else if (legacyText != null)
            {
                legacyText.fontSize = (int)originalSize;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// 留덉슦?ㅺ? UI ?곸뿭 ?덉쑝濡??ㅼ뼱?????몄텧
        /// </summary>
        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            isHovering = true;
        }

        /// <summary>
        /// 留덉슦?ㅺ? UI ?곸뿭 諛뽰쑝濡??섍컝 ???몄텧
        /// </summary>
        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            isHovering = false;
        }
        #endregion
    }
}
