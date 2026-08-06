using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 체력을 관리하는 클래스
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;
        private bool isDeath = false;

        //체력 위험 경계 비율
        [SerializeField] private float criticalHealthRatio = 0.3f;

        //이벤트 함수
        public UnityAction<float, GameObject> onDamaged;    //데미지 입었을때 등록된 함수 호출
        public UnityAction onDeath;                         //죽었을때 등록된 함수 호출
        public UnityAction<float> onHeal;                   //힐 할때 등록된 함수 호출
        #endregion

        #region Property
        public float CurrentHealth { get; private set; }
        //무적 모드
        public bool Invincible { get; private set; }        
        //체력 게이지 비율 - UI
        public float HealthRatio => CurrentHealth / maxHealth;
        //위험 체크
        public bool IsCritical => HealthRatio <= criticalHealthRatio;
        //힐 아이템을 먹을 수 있는지 체크
        public bool CanPickup => CurrentHealth < maxHealth;     
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //초기화
            CurrentHealth = maxHealth;
            Invincible = false;
        }
        #endregion

        #region Custom Method
        //체력 회복
        public bool Heal(float amount)
        {
            float beforeHealth = CurrentHealth; //힐 계산전의 체력 값

            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
            Debug.Log($"CurrentHealth: {CurrentHealth}");

            //리얼 힐 값 계산
            float realHeal = CurrentHealth - beforeHealth;

            if(realHeal > 0)
            {
                //힐 구현
                onHeal?.Invoke(realHeal);

                return true;    //힐 성공
            }

            return false; //힐 실패
        }

        //데미지 처리. damage:데미지량, damageSource: 데미지를 주는 주체
        public void TakeDamage(float damage, GameObject damageSource)
        {
            //무적 모드 체크
            if (Invincible) return;

            float beforeHealth = CurrentHealth; //데미지 계산전의 체력 값

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
            Debug.Log($"CurrentHealth: {CurrentHealth}");

            //real Damage
            float realDamage = beforeHealth - CurrentHealth;

            if (realDamage > 0f)
            {
                //데미지 구현 (VFX, SFX, UI ...)
                onDamaged?.Invoke(realDamage, damageSource);
            }

            //죽음 처리
            HandleDeath();
        }

        //죽음 처리
        void HandleDeath()
        {
            //죽음 체크
            if (isDeath) return;

            //죽었으면
            if(CurrentHealth <= 0f)
            {
                isDeath = true;

                //죽음 구현
                onDeath?.Invoke();
            }
        }
        #endregion
    }
}