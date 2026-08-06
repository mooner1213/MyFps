using UnityEngine;
using UnityEngine.AI;

namespace MyFps
{
    //적 캐릭터 상태 정의
    public enum EnemyState
    {
        E_Idle,         //대기
        E_Walk,         //걷기 (패트롤)
        E_Chase,        //추격
        E_Attack,       //공격
        E_Death         //죽기
    }

    /// <summary>
    /// 총을 쏘는 적 캐릭터를 관리하는 클래스
    /// </summary>
    public class GunMan : MonoBehaviour, IDamageable
    {
        #region Variables
        //참조
        private Animator animator;
        private NavMeshAgent agent;
        private Transform thePlayer;

        //상태
        [SerializeField] private EnemyState currentState;       //현재 상태
        private EnemyState beforeState;                         //이전 상태

        //체력
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth = 0f;
        private bool isDeath = false;       //죽음 체크

        //대기 상태
        [SerializeField] private float idleTimer = 2f;          //웨이 포인트 도착시 다음 포인트 출발전 2초 대기
        private float countdown = 0f;

        //패트롤 여부
        [SerializeField] private bool isPatrol = false;       
        public Transform[] wayPoints;                           //웨이 포인트
        private int wayPointIndex = 0;                          //다음 포인트 지점 인덱스

        //처음 스폰 위치
        private Vector3 startPosition = Vector3.zero;

        //추격 상태
        private bool isDetecting = false;
        [SerializeField] private float detectDistance = 10f;   //적이 디텍팅 거리에 들어오면 추격 시작

        //공격 상태
        [SerializeField] private float attackRange = 5f;        //적이 사거리 안에 들어오면 추격을 멈추고 공격 시작
        [SerializeField] private float attackTimer = 2f;        //총 발사 간격        
        [SerializeField] private float attackDamage = 5f;       //공격 데미지

        //애니메이션 파라미터
        private const string MoveSpeed = "MoveSpeed";
        private const string IsDeath = "IsDeath";
        private const string FireTrigger = "FireTrigger";
        #endregion

        #region Property
        public bool IsDetecting
        {
            get {  return isDetecting; }
            set
            { 
                isDetecting = value;
                if(value == false)
                {
                    ChangeState(EnemyState.E_Walk);
                }
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            thePlayer = FindFirstObjectByType<Player>().transform;
        }

        private void Start()
        {
            //초기화
            currentHealth = maxHealth;
            startPosition = this.transform.position;            
            ChangeState(EnemyState.E_Idle);

            isPatrol = wayPoints.Length >= 2 ? true : false;
            wayPointIndex = 1;
        }

        private void Update()
        {
            //죽음, 플레이어 체크
            if (isDeath || thePlayer == null)
                return;

            //플레이어 디텍팅
            float distance = Vector3.Distance(transform.position, thePlayer.position);
            if(distance <= attackRange && IsDetecting)
            {                
                ChangeState(EnemyState.E_Attack);
            }
            else if(distance <= detectDistance && IsDetecting)
            {
                ChangeState(EnemyState.E_Chase);
            }

            //상태 구현
            switch(currentState)
            {
                case EnemyState.E_Idle:
                    if(isPatrol)
                    {
                        countdown += Time.deltaTime;
                        if(countdown > idleTimer)
                        {
                            //다음 포인트 지점으로 이동(패트롤)
                            ChangeState(EnemyState.E_Walk);

                            //초기화
                            countdown = 0f;
                        }
                    }
                    break;
                case EnemyState.E_Walk: //패트롤
                    //도착 판정
                    if(agent.remainingDistance < 0.1f)
                    {
                        //인덱스 증가
                        if(isPatrol)
                        {
                            wayPointIndex++;
                            if(wayPointIndex >= wayPoints.Length)
                            {
                                wayPointIndex = 0;
                            }
                        }
                        ChangeState(EnemyState.E_Idle);
                    }
                    break;

                case EnemyState.E_Chase:
                    //agent 목표지점
                    agent.SetDestination(thePlayer.position);

                    //플레이어 도망가면
                    if(distance > detectDistance)
                    {
                        ChangeState(EnemyState.E_Walk);
                    }
                    break;

                case EnemyState.E_Attack:
                    countdown += Time.deltaTime;
                    if(countdown >= attackTimer)
                    {
                        Shoot();

                        //초기화
                        countdown = 0f;
                    }

                    //플레이어 바라보기
                    transform.LookAt(thePlayer.position);
                    break;

                case EnemyState.E_Death:
                    break;
            }

            //애니메이터 파라미터 처리
            animator.SetFloat(MoveSpeed, agent.velocity.magnitude);
        }

        //디텍팅 거리, 공격 거리 기즈모 그리기
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        #endregion


        #region Custom Method
        public void ChangeState(EnemyState newState)
        {
            //현재 상태 체크
            if(currentState == newState)
                    return;

            //상태 변경전에 현재상태를 이전상태에 저장
            beforeState = currentState;

            //새로운 상태로 변경
            currentState = newState;

            //agent 초기화
            agent.ResetPath();

            countdown = 0f;

            //새로운 상태변경에 따른 처리사항 구현
            switch (newState)
            {   
                case EnemyState.E_Walk:
                    //이동 목표 지점 설정
                    if (isPatrol)
                    {
                        agent.SetDestination(wayPoints[wayPointIndex].position);
                    }
                    else
                    {
                        agent.SetDestination(startPosition);
                    }
                    break;
            }

            //애니메이터 조준 설정
            if(newState == EnemyState.E_Chase || newState == EnemyState.E_Attack)
            {
                animator.SetLayerWeight(1, 1f);
            }
            else
            {
                animator.SetLayerWeight(1, 0f);
            }

            //...
        }

        //데미지 처리
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            //데미지 이펙트 (VFX, SFX)..


            if(currentHealth <= 0f && isDeath == false)
            {
                Death();
            }
        }

        //죽음 처리
        void Death()
        {
            isDeath = true;

            ChangeState(EnemyState.E_Death);

            //죽음과 관련된 내용 구현
            //이펙트 (VFX, SFX), 보상, ...

            //Agent 기능 끄기
            agent.enabled = false;

            //애니메이션
            animator.SetBool(IsDeath, true);

            //킬 여부
            //Destroy(gameObject, 3f);
        }

        //슛
        void Shoot()
        {
            //애니메이션
            animator.SetTrigger(FireTrigger);

            //플레이어에게 데미지 주기
            IDamageable damageable = thePlayer.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
        #endregion

    }
}