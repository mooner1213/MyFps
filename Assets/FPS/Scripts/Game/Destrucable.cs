using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 죽었을때 오브젝트(Health를 가지고 있는)를 킬하는 클래스
    /// </summary>
    public class Destrucable : MonoBehaviour
    {
        #region Variables
        //참조
        private Health health;

        //kill delay
        [SerializeField] private float delayTime = 0f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            //helth 이벤트 함수 등록
            health.onDamaged += OnDamaged;
            health.onDeath += OnDie;
        }

        private void OnDisable()
        {
            //helth 이벤트 함수 해제
            health.onDamaged -= OnDamaged;
            health.onDeath -= OnDie;
        }
        #endregion

        #region Custom Method
        //데미지 입었을 때 호출되어 실행되는 함수
        private void OnDamaged(float damage, GameObject damageSource)
        {
            //TODO : 데미지 구현 내용

        }

        //죽었을때 호출되어 실행되는 함수
        private void OnDie()
        {
            //킬
            Destroy(this.gameObject, delayTime);
        }
        #endregion
    }
}
