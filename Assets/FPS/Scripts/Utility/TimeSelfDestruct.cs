using UnityEngine;

namespace Unity.FPS.Utility
{
    /// <summary>
    /// 일정 시간(LifeTime) 후에 오브젝트를 제거하는 컴포넌트
    /// </summary>

    public class TimeSelfDestruct : MonoBehaviour
    {
        public float lifeTime = 3f; // 제거될 때까지의 시간
        private float spawnTime; // 오브젝트가 생성된 시간

        private void Awake()
        {
            spawnTime = Time.time; // 오브젝트가 생성된 시간 기록
        }

        private void Update()
        {
            // 현재 시간과 생성된 시간을 비교하여 lifeTime이 지났는지 확인
            if (Time.time > spawnTime + lifeTime)
            {
                Destroy(gameObject); // 오브젝트 제거
            }
        }
    }
}