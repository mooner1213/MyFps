using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 게임 내 모든 캐릭터, 적, 상호작용 오브젝트의 최상위 기본 액터 클래스
    /// </summary>
    public class Actor : MonoBehaviour
    {
        #region Variables
        [Header("Actor Basic Info")]
        [Tooltip("액터 이름")]
        [SerializeField] private string actorName = "Actor";

        [Tooltip("액터 진영/팀 (0: Player, 1: Enemy, 2: Neutral)")]
        [SerializeField] private int affiliation = 1;

        [Header("Actor Health")]
        [Tooltip("최대 체력")]
        [SerializeField] private float maxHealth = 100f;

        [Tooltip("현재 체력")]
        [SerializeField] private float currentHealth = 100f;

        [Tooltip("사망 여부")]
        [SerializeField] private bool isDead = false;

        // 프로퍼티
        public string ActorName => actorName;
        public int Affiliation => affiliation;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDead => isDead;
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        protected virtual void Start()
        {
            // ActorManager에 자기 자신 자동 등록
            if (ActorManager.Instance != null)
            {
                ActorManager.Instance.RegisterActor(this);
            }
        }

        protected virtual void OnDestroy()
        {
            // ActorManager에서 자기 자신 등록 해제
            if (ActorManager.Instance != null)
            {
                ActorManager.Instance.UnregisterActor(this);
            }
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// 액터가 데미지를 입었을 때 호출되는 기본 메소드
        /// </summary>
        /// <param name="damage">입을 데미지 양</param>
        public virtual void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// 액터 체력이 0 이하가 되어 사망할 때 호출되는 기본 메소드
        /// </summary>
        public virtual void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log($"<color=red>[Actor]</color> {actorName} 사망 처리됨");
        }
        #endregion
    }
}
