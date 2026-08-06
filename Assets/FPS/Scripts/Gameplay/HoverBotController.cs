using UnityEngine;
using UnityEngine.AI;
using Unity.FPS.Game;
using MyFps;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// HoverBot 적 AI 컨트롤러
    /// - 플레이어가 마우스를 어디로 회전하든 상관없이 플레이어 본체 Position에 시선 완벽 고정
    /// - GunMuzzle에서 Projectile_Hoverbot 총알을 똑바로 조준 발사
    /// </summary>
    public class HoverBotController : MonoBehaviour, IDamageable
    {
        #region Variables
        // 참조
        private NavMeshAgent agent;
        private Transform thePlayer;
        private Health healthComponent;
        private AudioSource audioSource;
        private EnemyController enemyController;

        [Header("Gun & Muzzle")]
        [Tooltip("총알이 발사될 위치 (GunMuzzle)")]
        [SerializeField] private Transform gunMuzzle;

        [Tooltip("발사체 프리팹 (Projectile_Hoverbot)")]
        [SerializeField] private GameObject projectilePrefab;

        [Header("State")]
        [SerializeField] private EnemyState currentState = EnemyState.E_Idle;
        private EnemyState beforeState;

        [Header("Health")]
        [SerializeField] private float maxHealth = 30f;
        private float currentHealth;
        private bool isDeath = false;

        [Header("Patrol & Wander AI")]
        [Tooltip("패트롤 지점 목록")]
        public Transform[] wayPoints;
        private int wayPointIndex = 0;
        private bool isPatrol = false;

        [Tooltip("웨이포인트가 없을 때 무작위 자유 배회(Wander) 사용 여부")]
        [SerializeField] private bool useRandomWander = true;

        [Tooltip("무작위 배회 범위 (스폰 지점 기준 반경)")]
        [SerializeField] private float wanderRadius = 10f;

        [SerializeField] private float idleTimer = 2f;
        private float countdown = 0f;
        private Vector3 startPosition;

        [Header("Detecting - AI")]
        [Tooltip("플레이어 인지 거리 (이 거리 안에 들어오면 추격 시작)")]
        [SerializeField] private float detectDistance = 25f;

        [Header("Attack Settings")]
        [Tooltip("공격 사거리 (이 거리 안에 들어오면 정지하고 사격)")]
        [SerializeField] private float attackRange = 15f;

        [Tooltip("총알 발사 간격 (초)")]
        [SerializeField] private float attackTimer = 1.0f;

        [Tooltip("총알 데미지")]
        [SerializeField] private float attackDamage = 10f;

        [Tooltip("총알 날아가는 속도")]
        [SerializeField] private float projectileSpeed = 25f;

        private float shootCountdown = 0f;

        // 프로퍼티
        public bool IsDeath => (enemyController != null) ? enemyController.IsDeath : isDeath;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            healthComponent = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            enemyController = GetComponent<EnemyController>();

            // GunMuzzle 자동 탐색
            if (gunMuzzle == null)
            {
                foreach (Transform t in GetComponentsInChildren<Transform>())
                {
                    if (t.name.Equals("GunMuzzle", System.StringComparison.OrdinalIgnoreCase))
                    {
                        gunMuzzle = t;
                        break;
                    }
                }
                if (gunMuzzle == null) gunMuzzle = transform;
            }

            FindPlayerTarget();
        }

        private void Start()
        {
            if (enemyController != null)
            {
                enemyController.OnDeath += Die;
            }

            currentHealth = maxHealth;
            startPosition = transform.position;
            ChangeState(EnemyState.E_Idle);

            isPatrol = (wayPoints != null && wayPoints.Length >= 2);
            wayPointIndex = 0;
        }

        private void OnDestroy()
        {
            if (enemyController != null)
            {
                enemyController.OnDeath -= Die;
            }
        }

        private void Update()
        {
            if (IsDeath) return;

            // 플레이어 타겟 탐색
            if (thePlayer == null)
            {
                FindPlayerTarget();
            }

            if (thePlayer == null) return;

            // 플레이어 본체와의 수평 거리 계산
            Vector3 playerPos = thePlayer.position;
            Vector3 myPos = transform.position;
            playerPos.y = myPos.y; // 동일 평면 거리

            float distanceToPlayer = Vector3.Distance(playerPos, myPos);

            // 상태 결정 (사거리 15m 이내면 바로 공격)
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EnemyState.E_Attack);
            }
            else if (distanceToPlayer <= detectDistance)
            {
                ChangeState(EnemyState.E_Chase);
            }
            else
            {
                if (isPatrol || useRandomWander)
                    ChangeState(EnemyState.E_Walk);
                else
                    ChangeState(EnemyState.E_Idle);
            }

            // 상태별 FSM 처리
            switch (currentState)
            {
                case EnemyState.E_Idle:
                    ProcessIdle();
                    break;
                case EnemyState.E_Walk:
                    ProcessPatrol();
                    break;
                case EnemyState.E_Chase:
                    ProcessChase();
                    break;
                case EnemyState.E_Attack:
                    ProcessAttack(distanceToPlayer);
                    break;
                case EnemyState.E_Death:
                    break;
            }
        }
        #endregion

        #region Custom Method
        private void FindPlayerTarget()
        {
            // 1. 태그가 Player인 오브젝트
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                thePlayer = playerObj.transform;
                return;
            }

            // 2. Player 스크립트 컴포넌트
            var playerComp = FindFirstObjectByType<Player>();
            if (playerComp != null)
            {
                thePlayer = playerComp.transform;
                return;
            }

            // 3. PlayerCharacterController 컴포넌트
            var pcc = FindFirstObjectByType<Unity.FPS.Gameplay.PlayerCharacterController>();
            if (pcc != null)
            {
                thePlayer = pcc.transform;
                return;
            }

            // 4. Main Camera
            if (Camera.main != null)
            {
                thePlayer = Camera.main.transform;
            }
        }

        private void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;

            beforeState = currentState;
            currentState = newState;

            switch (currentState)
            {
                case EnemyState.E_Idle:
                    if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
                    countdown = idleTimer;
                    if (enemyController != null) enemyController.SetTargeting(false);
                    break;
                case EnemyState.E_Walk:
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        if (!isPatrol && useRandomWander)
                        {
                            SetNextRandomDestination();
                        }
                    }
                    if (enemyController != null) enemyController.SetTargeting(false);
                    break;
                case EnemyState.E_Chase:
                    if (agent != null && agent.isOnNavMesh) agent.isStopped = false;
                    if (enemyController != null) enemyController.SetTargeting(true);
                    break;
                case EnemyState.E_Attack:
                    if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
                    shootCountdown = 0.1f; // 공격 진입 직후 즉시 발사
                    if (enemyController != null) enemyController.SetTargeting(true);
                    break;
            }
        }

        private void ProcessIdle()
        {
            if (isPatrol || useRandomWander)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0f)
                {
                    ChangeState(EnemyState.E_Walk);
                }
            }
        }

        private void ProcessPatrol()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            if (isPatrol && wayPoints != null && wayPoints.Length > 0)
            {
                Transform targetWayPoint = wayPoints[wayPointIndex];
                agent.SetDestination(targetWayPoint.position);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
                {
                    wayPointIndex = (wayPointIndex + 1) % wayPoints.Length;
                    ChangeState(EnemyState.E_Idle);
                }
            }
            else if (useRandomWander)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
                {
                    ChangeState(EnemyState.E_Idle);
                }
            }
        }

        private void SetNextRandomDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPosition;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
        }

        private void ProcessChase()
        {
            if (thePlayer == null) return;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(thePlayer.position);
            }
            LookAtPlayer();
        }

        private void ProcessAttack(float distanceToPlayer)
        {
            LookAtPlayer();

            shootCountdown -= Time.deltaTime;
            if (shootCountdown <= 0f)
            {
                shootCountdown = attackTimer;
                ShootProjectile();
            }
        }

        private void LookAtPlayer()
        {
            if (thePlayer == null) return;

            Vector3 targetPos = thePlayer.position;
            targetPos.y = transform.position.y;

            Vector3 dir = (targetPos - transform.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 12f);
            }
        }

        private void ShootProjectile()
        {
            if (thePlayer == null) return;

            Vector3 spawnPos = (gunMuzzle != null) ? gunMuzzle.position : (transform.position + transform.forward * 1.2f + Vector3.up * 0.5f);
            Vector3 targetPoint = thePlayer.position + Vector3.up * 1.0f;
            Vector3 shootDir = (targetPoint - spawnPos).normalized;

            if (shootDir == Vector3.zero) shootDir = transform.forward;

            Quaternion spawnRot = Quaternion.LookRotation(shootDir);

            Debug.Log($"<color=yellow>[HoverBot SHOOT!]</color> 플레이어를 향해 총알 발사됨! (조준 방향: {shootDir})");

            GameObject projObj = null;

            if (projectilePrefab != null)
            {
                projObj = Instantiate(projectilePrefab, spawnPos, spawnRot);
            }
            else
            {
                projObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projObj.transform.position = spawnPos;
                projObj.transform.rotation = spawnRot;
                projObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.6f);
                var renderer = projObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }

            var projBase = projObj.GetComponent<ProjectileBase>();
            if (projBase != null)
            {
                projBase.Shoot(gameObject);
            }

            var projStandard = projObj.GetComponent<Unity.FPS.Gameplay.ProjectileStandard>();
            if (projStandard != null)
            {
                projStandard.speed = projectileSpeed;
                projStandard.damage = attackDamage;
            }
        }

        public void TakeDamage(float damage)
        {
            if (IsDeath) return;

            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
                return;
            }

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damage, thePlayer != null ? thePlayer.gameObject : null);
            }

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDeath) return;
            isDeath = true;

            ChangeState(EnemyState.E_Death);

            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            Destroy(gameObject, 1.5f);
        }
        #endregion
    }
}
