using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 사운드 플레이 테스트 예제
    /// </summary>
    public class SoundTest : MonoBehaviour
    {
        #region Variables
        //재생할 오디오 소스 속성
        public AudioClip clip;

        [SerializeField] private float volumn = 1f;
        [SerializeField] private float pitch = 1f;
        [SerializeField] private bool isLoop = false;
        [SerializeField] private bool playOnAwake = false;

        //참조
        private AudioSource auidoSource;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            auidoSource = GetComponent<AudioSource>();
            if(auidoSource == null)
            {
                auidoSource = gameObject.AddComponent<AudioSource>();
            }

            //auidoSource 셋팅
            auidoSource.clip = clip;
            auidoSource.volume = volumn;
            auidoSource.pitch = pitch;
            auidoSource.loop = isLoop;
            auidoSource.playOnAwake = playOnAwake;
        }

        private void Start()
        {
            //플레이 시작
            auidoSource.Play();
        }
        #endregion
    }
}