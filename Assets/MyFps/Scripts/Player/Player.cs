using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 플레이어의 기본 클래스
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region Variables
        //참조
        private PlayerHealth playerHealth;

        //데미지 효과
        public GameObject damagedFlash;

        //죽음 처리
        public GameObject gameOverUI;
        #endregion

        #region Property
        public bool IsDeath => playerHealth.IsDeath;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            //데미지,죽음 이벤트에 함수 등록
            playerHealth.onDamaged += OnDamaged;
            playerHealth.onDie += OnDie;
        }

        void OnDisable()
        {
            //데미지,죽음 이벤트에 함수 해제
            playerHealth.onDamaged -= OnDamaged;
            playerHealth.onDie -= OnDie;
        }
        #endregion

        #region Custom Method
        //데미지 입을때 처리할 함수
        private void OnDamaged(float damage)
        {            
            //데미지 효과 처리(VFX, SFX)
            StartCoroutine(DamageEffect());
        }

        //죽었을때 처리할 함수
        private void OnDie()
        {
            //UI
            gameOverUI.SetActive(true);
        }

        //데미지 효과 처리(VFX, SFX)
        IEnumerator DamageEffect()
        {
            //화면 흔들림 효과
            CinemachineShake.Instance.ShakeCarmera();

            //데미지 플래시 효과
            damagedFlash.SetActive(true);

            //데미지 사운드
            int hurtNumber = Random.Range(1, 4);
            if(hurtNumber == 1)
            {
                AudioManager.Instance.Play("Hurt01");

            }
            else if (hurtNumber == 2)
            {
                AudioManager.Instance.Play("Hurt02");
            }
            else
            {
                AudioManager.Instance.Play("Hurt03");
            }

            yield return new WaitForSeconds(1f);

            damagedFlash.SetActive(false);
        }
        #endregion
    }
}
