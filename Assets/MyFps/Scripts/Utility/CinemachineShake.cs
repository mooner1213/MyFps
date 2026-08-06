using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 화면 흔들림 효과 구현 싱글톤 클래스
    /// 흔들림 계수: 흔들림 속도, 흔들림 크기, 흔들림 시간
    /// </summary>
    public class CinemachineShake : Singleton<CinemachineShake>
    {
        #region Variables
        //참조
        private CinemachineBasicMultiChannelPerlin multiChannelPerlin;

        //흔들림 크기, 흔들림 속도, 흔들림 시간
        [SerializeField] private float amplitude = 0f;
        [SerializeField] private float frequency = 0f;
        [SerializeField] private float shakeTimer = 1f;

        private bool isShake = false;   //흔들림 체크
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //참조
            multiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
        #endregion

        #region Custom Method
        //카메라 흔들기 - 흔들림 크기, 흔들림 속도, 흔들림 시간
        public void ShakeCarmera(float shakeTimer = 1f, float amplitude = 1f, float frequency = 1f)
        {
            //흔들림 체크
            if (isShake)
                return;

            StartCoroutine(ShakeStart(shakeTimer, amplitude, frequency));
        }

        IEnumerator ShakeStart(float shakeTimer, float amplitude, float frequency)
        {
            isShake = true;

            //흔들림 설정
            multiChannelPerlin.AmplitudeGain = amplitude;
            multiChannelPerlin.FrequencyGain = frequency;

            yield return new WaitForSeconds(shakeTimer);

            //흔들림 정지
            multiChannelPerlin.AmplitudeGain = 0f;
            multiChannelPerlin.FrequencyGain = 0f;
            isShake = false;
        }
        #endregion
    }
}