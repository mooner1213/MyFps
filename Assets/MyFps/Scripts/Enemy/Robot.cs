using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 로봇의 상태 정의
    /// </summary>
    public enum RobotState
    {
        R_Idle = 0,
        R_Walk,
        R_Attack,
        R_Death
    }

    /// <summary>
    /// 로봇 적을 관리하는 클래스
    /// IDamageable 상속 받는다
    /// </summary>
    public class Robot : MonoBehaviour, IDamageable
    {
        #region Variables
        //참조
        private Animator animator;
        private Transform thePlayer;
        private Player player;

        //로봇의 상태 (enum)
        [SerializeField] private RobotState currentState;    //현재 상태
        private RobotState beforeState;     //현재 상태의 바로 이전 상태

        //이동
        [SerializeField] private float moveSpeed = 2f;

        //공격
        [SerializeField] private float attakRange = 1.5f;   //공격 범위
        [SerializeField] private float attackDamage = 5f;   //공격력

        //[SerializeField] private float attackTimer = 2f;
        //private float countdown = 0f;

        //체력
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth = 0f;
        private bool isDeath = false;       //죽음 체크

        //애니메이션 파라미터
        private const string enemyState = "EnemyState";
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
            player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                thePlayer = player.transform;
            }
        }

        private void Start()
        {
            //초기화
            ChangeState(RobotState.R_Idle);
            currentHealth = maxHealth;
        }

        private void Update()
        {
            //적의 죽음 체크
            if(isDeath)
            {
                return;
            }

            //타겟 체크
            if(thePlayer == null)
            {
                player = FindFirstObjectByType<Player>();
                if (player != null)
                {
                    thePlayer = player.transform;
                }
                return;
            }

            //플레이어 죽음 체크
            if(player.IsDeath)
            {
                return;
            }

            //타겟팅
            Vector3 dir = thePlayer.position - transform.position;
            float distance = Vector3.Distance(thePlayer.position, transform.position);

            //상태에 따른 구현
            switch(currentState)
            {
                case RobotState.R_Idle:
                    //플레이어가 공격 범위 안에 들어오면 공격 상태로 바꾼다
                    if (distance <= attakRange)
                    {
                        ChangeState(RobotState.R_Attack);
                    }
                    break;

                case RobotState.R_Walk: //타겟(플레이어)를 향해 이동
                    //방향 * Time.delatTime * moveSpeed
                    transform.Translate(dir.normalized * Time.deltaTime * moveSpeed, Space.World);
                    //타겟을 바라본다
                    transform.LookAt(thePlayer);

                    //플레이어가 공격 범위 안에 들어오면 공격 상태로 바꾼다
                    if(distance <= attakRange)
                    {
                        ChangeState(RobotState.R_Attack);
                    }
                    break;

                case RobotState.R_Attack:   //일정거리안에 들어오면 공격한다
                    /*//공격 타이머
                    countdown += Time.deltaTime;
                    if(countdown >= attackTimer)
                    {
                        //공격
                        Attack();

                        //타이머 초기화
                        countdown = 0f;
                    }*/

                    //타겟을 바라본다
                    transform.LookAt(thePlayer);

                    //공격중에 플레이어가 도망가면 다시 추격한다
                    if (distance > attakRange)
                    {
                        ChangeState(RobotState.R_Walk);
                    }
                    break;

                case RobotState.R_Death:
                    break;
            }
        }
        #endregion

        #region Custom Method
        //상태 변경 - 매개변수로 들어온 상태로 변경한다
        public void ChangeState(RobotState newState)
        {
            //상태 변경전에 현재상태를 이전상태에 저장
            beforeState = currentState;

            //새로운 상태로 변경
            currentState = newState;

            //새로운 상태변경에 따른 처리사항 구현
            animator.SetInteger(enemyState, (int)currentState);

            //...
        }

        //공격
        void Attack()
        {
            if (thePlayer == null)
                return;

            /*PlayerHealth playerHealth = thePlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }*/
            IDamageable damageable = thePlayer.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }

        //데미지 입기
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} currentHealth: {currentHealth}");

            //데미지 효과 처리(VFX, SFX)

            //죽음 체크
            if(currentHealth <= 0f && isDeath == false)
            {
                Die();
            }
        }

        //죽기
        void Die()
        {
            isDeath = true;

            //죽음 처리 (VFX, SFX, 보상처리)

            //상태 변경
            ChangeState(RobotState.R_Death);
        }
        #endregion
    }
}