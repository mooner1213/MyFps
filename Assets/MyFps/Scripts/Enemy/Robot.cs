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

        [SerializeField] private float attackTimer = 2f;
        private float countdown = 0f;
        [SerializeField] private float detectRange = 10f; //감지 범위 추가

        //체력
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth = 0f;
        private bool isDeath = false;       //죽음 체크

        //감지 상태 (플레이어 추적 여부)
        [SerializeField] private bool isDetecting = false;
        public bool IsDetecting
        {
            get { return isDetecting; }
            set
            {
                isDetecting = value;
                // 감지 상태가 되었을 때 대기 상태라면 걷기(추적) 상태로 전환
                if (isDetecting && currentState == RobotState.R_Idle)
                {
                    ChangeState(RobotState.R_Walk);
                }
            }
        }

        // 공격 모션 처리 추가
        private bool isAttacking = false;
        [SerializeField] private float attackDuration = 1.35f; // 공격 전체 애니메이션 시간
        [SerializeField] private float attackDamageDelay = 0.45f; // 실제 데미지 주는 타이밍
        private float attackProgressTimer = 0f;
        private bool hasDealtDamage = false;

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
            }

            //플레이어가 감지 범위 안에 들어오면 감지 상태로 전환
            if (!isDetecting && thePlayer != null)
            {
                float distanceToPlayer = Vector3.Distance(thePlayer.position, transform.position);
                if (distanceToPlayer <= detectRange)
                {
                    IsDetecting = true;
                }
            }

            //플레이어를 감지하지 못한 상태이면 동작하지 않음
            if (!isDetecting)
            {
                return;
            }

            //플레이어 죽음 체크
            if(player != null && player.IsDeath)
            {
                return;
            }

            // 공격 모션이 실행 중일 때는 이동 및 상태 전환을 제한하고, 모션을 마칠 때까지 대기
            if (isAttacking)
            {
                attackProgressTimer += Time.deltaTime;

                // 공격 애니메이션 시작 후 일정 시간 뒤에 단 한 번 실제 데미지를 가함
                if (!hasDealtDamage && attackProgressTimer >= attackDamageDelay)
                {
                    Attack();
                    hasDealtDamage = true;
                }

                // 타겟을 바라본다 (Y축 고정)
                if (thePlayer != null)
                {
                    Vector3 attackLookTarget = thePlayer.position;
                    attackLookTarget.y = transform.position.y;
                    transform.LookAt(attackLookTarget);
                }

                // 공격 동작 마무리 시점
                if (attackProgressTimer >= attackDuration)
                {
                    isAttacking = false;
                    hasDealtDamage = false;
                    
                    // 공격 완료 후 거리 체크하여 걷기(Chase) 또는 재공격 결정
                    if (thePlayer != null)
                    {
                        float finalDist = Vector3.Distance(thePlayer.position, transform.position);
                        if (finalDist <= attakRange)
                        {
                            ChangeState(RobotState.R_Attack);
                        }
                        else
                        {
                            ChangeState(RobotState.R_Walk);
                        }
                    }
                }
                return; // 공격 모션이 끝날 때까지 아래 스위치문 실행 방지
            }

            //타겟팅
            Vector3 dir = thePlayer.position - transform.position;
            float distance = Vector3.Distance(thePlayer.position, transform.position);

            //상태에 따른 구현
            switch(currentState)
            {
                case RobotState.R_Idle:
                    //플레이어가 공격 범위 안에 들어오면 공격 상태로 바꾸고, 멀면 추격 상태로 바꾼다
                    if (distance <= attakRange)
                    {
                        ChangeState(RobotState.R_Attack);
                    }
                    else
                    {
                        ChangeState(RobotState.R_Walk);
                    }
                    break;

                case RobotState.R_Walk: //타겟(플레이어)를 향해 이동
                    // Y축 이동 제한
                    Vector3 moveDir = dir;
                    moveDir.y = 0f;
                    //방향 * Time.deltaTime * moveSpeed
                    transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);
                    
                    //타겟을 바라본다 (Y축 고정)
                    Vector3 walkLookTarget = thePlayer.position;
                    walkLookTarget.y = transform.position.y;
                    transform.LookAt(walkLookTarget);

                    //플레이어가 공격 범위 안에 들어오면 공격 상태로 바꾼다
                    if(distance <= attakRange)
                    {
                        ChangeState(RobotState.R_Attack);
                    }
                    break;

                case RobotState.R_Attack:   //일정거리안에 들어오면 공격한다
                    // 공격 상태 진입 시 모션 타이머 시작
                    isAttacking = true;
                    attackProgressTimer = 0f;
                    hasDealtDamage = false;
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

            //트랜지션 부재 우회를 위해 직접 애니메이션 Play 호출
            if (animator != null)
            {
                switch (currentState)
                {
                    case RobotState.R_Idle:
                        animator.Play("Idle");
                        break;
                    case RobotState.R_Walk:
                        animator.Play("Walk");
                        break;
                    case RobotState.R_Attack:
                        animator.Play("Attack");
                        break;
                    case RobotState.R_Death:
                        animator.Play("Death");
                        break;
                }
            }
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

            // 피격 시 무조건 플레이어를 감지하고 추격 시작
            IsDetecting = true;

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