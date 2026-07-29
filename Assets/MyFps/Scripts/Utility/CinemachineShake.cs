using UnityEngine;
using Unity.Cinemachine;

namespace MyFps
{
    public class CinemachineShake : MonoBehaviour
    {
        public static CinemachineShake Instance { get; private set; }

        private CinemachineCamera cinemachineCamera;
        private CinemachineBasicMultiChannelPerlin channelPerlin;

        [SerializeField] private float shakeIntensity = 2f;
        [SerializeField] private float shakeTime = 0.5f;

        private float shakeTimer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            cinemachineCamera = GetComponent<CinemachineCamera>();
            if (cinemachineCamera != null)
            {
                channelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        public void ShakeCarmera()
        {
            if (channelPerlin != null)
            {
                channelPerlin.AmplitudeGain = shakeIntensity;
                shakeTimer = shakeTime;
            }
        }

        private void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0f)
                {
                    if (channelPerlin != null)
                    {
                        channelPerlin.AmplitudeGain = 0f;
                    }
                }
            }
        }
    }
}
