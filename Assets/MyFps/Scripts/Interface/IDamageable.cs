using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 데미지 입는 기능 정의
    /// </summary>
    public interface IDamageable
    {
        public void TakeDamage(float damage);
    }
}