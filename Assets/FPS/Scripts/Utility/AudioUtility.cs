using Unity.VisualScripting;
using UnityEngine;

namespace Unity.FPS.Utility
{
    /// <summary>
    /// 오디오 관련 함수 구현
    /// </summary>

    public class AudioUtility
    {
        // 오디오 소스를 원하는 위치에서 플레이
        // 클립의 플레이 타임이 끝나면 자동 폭파
        public static void CreateSFX(AudioClip clip, Vector3 position, float spatialBlend = 1f, float rolloffDistanceMin = 1f)
        {
            // 빈 오프젝트 생성
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;

            // 오디오 소스 컴포넌트 추가 후, 설정하고 플레이
            AudioSource audioSource = impactSfxInstance.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = spatialBlend;
            audioSource.minDistance = rolloffDistanceMin;
            audioSource.Play();

            // 자동 폭파
            TimeSelfDestruct timeSelfDestruct = impactSfxInstance.AddComponent<TimeSelfDestruct>();
            timeSelfDestruct.lifeTime = clip.length;
        }
    }
}