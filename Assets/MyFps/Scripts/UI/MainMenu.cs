using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyFps
{
    /// <summary>
    /// 메인메뉴씬을 관리하는 클래스
    /// 메인메뉴(버튼 5개) 기능
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        #region Variables
        //참조
        private AudioManager audioManager;

        //씬이동
        public SceneFader fader;
        [SerializeField] private string loadToScene = "PlayScene01";

        //UI 오브젝트
        public GameObject mainMenu;
        public GameObject optionUI;
        public GameObject creditUI;

        //불러오기 버튼
        public Button loadGameBtn;

        //옵션 - 볼륨 조절
        public AudioMixer audioMixer;

        public Slider bgmSlider;        //배경음 볼륨조절 슬라이더
        public Slider sfxSlider;        //효과음 볼륨조절 슬라이더

        //

        //오디오믹서 파라미터, PlayerPrefs의 키값
        private const string BgmVolume = "BgmVolume";
        private const string SfxVolume = "SfxVolume";
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //게임 처음 실행하면 저장된 옵션값 로드하기
            LoadOptions();

            //참조
            audioManager = AudioManager.Instance;
            if (audioManager == null)
            {
                Debug.LogWarning("MainMenu: AudioManager.Instance is null. Sound playback will be disabled.");
            }

            //배경음 플레이
            audioManager?.PlayBgm("MenuBgm");

            // 불러오기 버튼 활성화 조건 체크
            if (loadGameBtn != null)
            {
                loadGameBtn.interactable = SaveSystem.HasSaveFile();
            }

            // UI 초기 활성화 상태 설정
            if (mainMenu != null) mainMenu.SetActive(true);
            if (optionUI != null) optionUI.SetActive(false);
            if (creditUI != null) creditUI.SetActive(false);

            //마우스 커서 초기화
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endregion

        #region Custom Method        
        public void NewGame()
        {
            //사운드 처리
            audioManager?.Stop("MenuBgm");
            audioManager?.Play("MenuButton");

            if (fader != null)
            {
                fader.FadeTo(loadToScene);
            }
            else
            {
                Debug.LogError("MainMenu: SceneFader is not assigned!");
            }
        }

        public void LoadGame()
        {
            //사운드 처리            
            audioManager?.Play("MenuButton");

            SaveData data = SaveSystem.Load();
            if (data != null && fader != null)
            {
                Debug.Log($"Load Game! Loading scene index: {data.sceneBuildIndex}");
                fader.FadeTo(data.sceneBuildIndex);
            }
            else
            {
                Debug.LogWarning("MainMenu: LoadGame failed (data or fader is null)");
            }
        }

        public void Options()
        {
            //사운드 처리            
            audioManager?.Play("MenuButton");

            ShowOptions();
        }

        public void Credits()
        {
            //사운드 처리            
            audioManager?.Play("MenuButton");
            ShowCredit();
        }

        public void QuitGame()
        {
            //사운드 처리            
            audioManager?.Play("MenuButton");

            Debug.Log("Quit Game");
            Application.Quit();
        }

        //옵션 UI
        private void ShowOptions()
        {
            //Debug.Log("Show Options");
            mainMenu.SetActive(false);
            optionUI.SetActive(true);
        }

        public void HideOptions()
        {
            optionUI.SetActive(false);
            mainMenu.SetActive(true);
        }

        //배경음 슬라이더로 볼륨 조절
        public void SetBgmVolume(float value)
        {
            //배경음 저장하기
            PlayerPrefs.SetFloat(BgmVolume, value);

            //Debug.Log($"BgmVolume: {value}");
            if (audioMixer != null)
            {
                audioMixer.SetFloat(BgmVolume, value);
            }
        }

        //효과음 슬라이더로 볼륨 조절
        public void SetSfxVolume(float value)
        {
            //효과음 저장하기
            PlayerPrefs.SetFloat(SfxVolume, value);

            //Debug.Log($"SfxVolume: {value}");
            if (audioMixer != null)
            {
                audioMixer.SetFloat(SfxVolume, value);
            }
        }

        //저장된 옵션 값 로드하기
        private void LoadOptions()
        {
            if (audioMixer == null)
            {
                Debug.LogWarning("MainMenu: AudioMixer is not assigned!");
                return;
            }

            //배경음, 효과음 가져오기
            float bgmVolume = PlayerPrefs.GetFloat(BgmVolume, 0f);
            //Debug.Log($"Load bgmVolume : {bgmVolume}");
            audioMixer.SetFloat(BgmVolume, bgmVolume);
            if (bgmSlider != null)
            {
                bgmSlider.value = bgmVolume;
            }

            float sfxVolume = PlayerPrefs.GetFloat(SfxVolume, 0f);
            //Debug.Log($"Load sfxVolume : {sfxVolume}");
            audioMixer.SetFloat(SfxVolume, sfxVolume);
            if (sfxSlider != null)
            {
                sfxSlider.value = sfxVolume;
            }
        }

        //크레딧 UI
        private void ShowCredit()
        {
            mainMenu.SetActive(false);
            creditUI.SetActive(true);
        }
        #endregion

    }
}