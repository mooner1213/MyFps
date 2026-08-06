using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 5번 항목: 특정 컴포넌트를 붙이면 생성하자마자 특정시간(5초)에 자동으로 릴리즈되는 컴포넌트
    /// - 기본 딜레이: 5초
    /// - 오브젝트 전체를 파괴하거나, 이 컴포넌트 자체만 파괴하도록 선택 가능
    /// </summary>
    public class AutoDestroy : MonoBehaviour
    {
        [Tooltip("자동 파괴까지 대기 시간 (초)")]
        [SerializeField] private float destroyDelay = 5f;

        [Tooltip("true: 게임오브젝트 전체 파괴 / false: 이 컴포넌트만 파괴")]
        [SerializeField] private bool destroyGameObject = true;

        private void Start()
        {
            if (destroyGameObject)
            {
                // 게임오브젝트 전체를 destroyDelay 초 후 파괴
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                // 이 컴포넌트만 destroyDelay 초 후 파괴
                Destroy(this, destroyDelay);
            }
        }
    }
}
