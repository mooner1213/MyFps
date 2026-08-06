using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
        public GameObject loadGame;

        //저장된 씬 번호
        private int sceneNumber;

        //옵션 - 볼륨 조절
        public AudioMixer audioMixer;

        public Slider bgmSlider;        //배경음 볼륨조절 슬라이더
        public Slider sfxSlider;        //효과음 볼륨조절 슬라이더

        //오디오믹서 파라미터, PlayerPrefs의 키값
        private const string BgmVolume = "BgmVolume";
        private const string SfxVolume = "SfxVolume";
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //게임 데이터 초기화
            GameDataInit();

            //로드게임 메뉴 셋팅
            if (sceneNumber < 0)
            {
                loadGame.SetActive(false);
            }
            else
            {
                loadGame.SetActive(true);
            }

            //참조
            audioManager = AudioManager.Instance;

            //배경음 플레이
            audioManager.PlayBgm("MenuBgm");

            //마우스 커서 초기화
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endregion

        #region Custom Method  
        //게임 데이터 초기화
        private void GameDataInit()
        {
            //게임 처음 실행하면 저장된 옵션값 로드하기
            LoadOptions();

            //파일에서 저장된 데이터 가져오기
            PlayData playData = SaveLoad.LoadData();
            PlayerStats.Instance.PlayerStatsInit(playData);

            //저장된 씬 번호 가져오기
            //sceneNumber = PlayerPrefs.GetInt("SceneNumber", -1);
            sceneNumber = PlayerStats.Instance.SceneNumber;
            //Debug.Log($"MainMenu Load sceneNumber : {sceneNumber}");
        }

        public void NewGame()
        {
            //사운드 처리
            audioManager.Stop("MenuBgm");
            audioManager.Play("MenuButton");

            //게임 데이터 초기화
            PlayerStats.Instance.PlayerStatsInit(null);
            
            fader.FadeTo(loadToScene);
        }

        public void LoadGame()
        {
            //사운드 처리            
            audioManager.Stop("MenuBgm");
            audioManager.Play("MenuButton");
            
            fader.FadeTo(sceneNumber);
        }

        public void Options()
        {
            //사운드 처리            
            audioManager.Play("MenuButton");

            ShowOptions();
        }

        public void Credits()
        {
            //사운드 처리            
            audioManager.Play("MenuButton");
            ShowCredit();
        }

        public void QuitGame()
        {
            //사운드 처리            
            audioManager.Play("MenuButton");

            //저장된 게임 리셋
            PlayerPrefs.DeleteAll();

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
            audioMixer.SetFloat(BgmVolume, value);
        }

        //효과음 슬라이더로 볼륨 조절
        public void SetSfxVolume(float value)
        {
            //효과음 저장하기
            PlayerPrefs.SetFloat(SfxVolume, value);

            //Debug.Log($"SfxVolume: {value}");
            audioMixer.SetFloat(SfxVolume, value);
        }

        //저장된 옵션 값 로드하기
        private void LoadOptions()
        {
            //배경음, 효과음 가져오기
            float bgmVolume = PlayerPrefs.GetFloat(BgmVolume, 0f);
            //Debug.Log($"Load bgmVolume : {bgmVolume}");
            audioMixer.SetFloat(BgmVolume, bgmVolume);
            bgmSlider.value = bgmVolume;

            float sfxVolume = PlayerPrefs.GetFloat(SfxVolume, 0f);
            //Debug.Log($"Load sfxVolume : {sfxVolume}");
            audioMixer.SetFloat(SfxVolume, sfxVolume);
            sfxSlider.value = sfxVolume;

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