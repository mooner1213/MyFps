using UnityEngine;
using UnityEngine.Audio;

namespace MyFps
{
    /// <summary>
    /// 사운드 데이터를 관리하는 싱글톤 클래스
    /// 사운드(배경음, 효과음) 플레이
    /// </summary>
    public class AudioManager : PersistanctSingleton<AudioManager>
    {
        #region Variables
        //관리할 사운드 목록
        public Sound[] sounds;
        //현재 플레이되는 배경음 사운드 이름
        [SerializeField] private string bgmSound = "";

        //사운드 볼륨 조절
        public AudioMixer audioMixer;
        #endregion

        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();

            //AudioMixer에서 필요한 AudioMixerGroup가져오기
            //Master 포함 모든 자식 MixerGroup를 배열로 가져온다 Master:0, BGM:1, SFX:2
            AudioMixerGroup[] mixerGroups = audioMixer.FindMatchingGroups("Master");

            //오디오 소스 셋팅
            foreach (var s in sounds)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();

                s.audioSource.clip = s.clip;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.loop;
                s.audioSource.playOnAwake = s.playOnAwake;

                //배경음 체크
                if(s.audioSource.loop)
                {
                    s.audioSource.outputAudioMixerGroup = mixerGroups[1]; //BGM
                }
                else
                {
                    s.audioSource.outputAudioMixerGroup = mixerGroups[2]; //SFX
                }
            }
        }
        #endregion

        #region Custom Method
        public void Play(string name)
        {
            //이름으로 재생할 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if(s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            //찾지 못했으면
            if(sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            //찾았으면 플레이
            sound.audioSource.Play();
        }

        //이름으로 플레이 중지
        public void Stop(string name)
        {
            //이름으로 중지시킬 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            //찾지 못했으면
            if (sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            //찾았으면
            sound.audioSource.Stop();
        }

        //배경음 플레이
        public void PlayBgm(string name)
        {
            //현재 플레이 되는 배경음 체크 - 플레이 중지
            if(bgmSound == name) //같은 배경음을 플레이하면 변경없이 계속 플레이
            {
                return;
            }

            //현재 플레이되는 배경음 정지
            Stop(bgmSound);

            //이름으로 재생할 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    bgmSound = s.name;  //배경 사운드 이름 저장
                    break;
                }
            }

            //찾지 못했으면
            if (sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            //찾았으면 플레이
            sound.audioSource.Play();
        }

        public void StopBgm()
        {
            Stop(bgmSound);
            bgmSound = "";
        }
        #endregion
    }
}