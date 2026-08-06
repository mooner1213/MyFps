using UnityEngine;
using UnityEngine.Events;

namespace MyFps
{
    /// <summary>
    /// 플레이어의 체력을 관리하는 클래스
    /// IDamageable 상속 받는다
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Variables
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth;

        private bool isDeath = false;

        //데미지 입을때 등록된 함수 호출하는 이벤트 함수
        public UnityAction<float> onDamaged;
        //죽었을때 등록된 함수 호출 이벤트 함수
        public UnityAction onDie;
        #endregion

        #region Properties
        public float CurrentHealth
        {
            get {  return currentHealth; }
            private set
            {
                currentHealth = value;
                PlayerStats.Instance.Health = currentHealth;
            }
        }
        public bool IsDeath => isDeath;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //초기화
            CurrentHealth = PlayerStats.Instance.Health;
        }
        #endregion

        #region Custom Method
        //데미지 입기
        public void TakeDamage(float damage)
        {
            if (isDeath)
                return;

            CurrentHealth -= damage;
            //Debug.Log($"{gameObject.name} currentHealth: {currentHealth}");            

            //데미지 입을때 등록된 함수 호출
            onDamaged?.Invoke(damage);

            //데미지 효과 처리(VFX, SFX)

            //죽음 체크
            if (CurrentHealth <= 0f && isDeath == false)
            {
                Die();
            }
        }

        //죽기
        void Die()
        {
            isDeath = true;

            //죽었을때 등록된 함수 호출
            onDie?.Invoke();

            //죽음 처리 (VFX, SFX, 보상처리)

            //게임오버
            //Debug.Log("GameOver!!!");
        }
        #endregion
    }
}