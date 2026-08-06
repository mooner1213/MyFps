using UnityEngine;
using Unity.FPS.Game;

namespace MyFps
{
    /// <summary>
    /// 적(HoverBot)이 발사하는 총알(projectile_hoverbot) 이동 및 플레이어 충돌 데미지 처리
    /// </summary>
    public class EnemyProjectile : MonoBehaviour
    {
        #region Variables
        private Vector3 moveDirection;
        private float moveSpeed = 22f;
        private float damage = 10f;
        private GameObject ownerObj;
        private Transform playerTarget;

        private float lifeTime = 5f;
        private bool isHit = false;
        private float checkRadius = 1.8f; // 피격 판정 범위 (1.8m)
        #endregion

        #region Unity Event Method
        private void Start()
        {
            Destroy(gameObject, lifeTime);

            // 플레이어 타겟 구하기 (위치 추적용)
            FindPlayerTarget();

            // 호버봇 본체와 총알 간 물리 충돌 완전 무시
            if (ownerObj != null)
            {
                Collider[] ownerCols = ownerObj.GetComponentsInChildren<Collider>();
                Collider myCol = GetComponent<Collider>();
                if (myCol != null)
                {
                    foreach (var col in ownerCols)
                    {
                        Physics.IgnoreCollision(myCol, col, true);
                    }
                }
            }
        }

        private void Update()
        {
            if (isHit) return;

            if (playerTarget == null)
            {
                FindPlayerTarget();
            }

            Vector3 prevPos = transform.position;

            // 총알 조준 방향 전진 이동
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // 1. 플레이어 본체와의 3D 거리가 1.8m 이내면 즉시 명중 판정!
            if (playerTarget != null)
            {
                Vector3 playerCenter = playerTarget.position + Vector3.up * 1.0f;
                float distToPlayer = Vector3.Distance(transform.position, playerCenter);

                if (distToPlayer <= checkRadius)
                {
                    HandleHit(playerTarget.gameObject);
                    return;
                }
            }

            // 2. Physics 궤적 SphereCastAll 충돌 감지
            Vector3 moveVec = transform.position - prevPos;
            float dist = moveVec.magnitude;

            if (dist > 0.001f)
            {
                RaycastHit[] hits = Physics.SphereCastAll(prevPos, checkRadius, moveVec.normalized, dist);
                foreach (var hit in hits)
                {
                    if (IsOwner(hit.collider.gameObject)) continue;
                    if (hit.collider.gameObject == gameObject) continue;

                    HandleHit(hit.collider.gameObject);
                    return;
                }
            }

            // 3. OverlapSphere 로컬 감지
            Collider[] overlapHits = Physics.OverlapSphere(transform.position, checkRadius);
            foreach (var col in overlapHits)
            {
                if (IsOwner(col.gameObject)) continue;
                if (col.gameObject == gameObject) continue;

                if (col.CompareTag("Player") || col.GetComponentInParent<PlayerHealth>() != null || col.GetComponentInParent<Health>() != null || col.name.Contains("Player"))
                {
                    HandleHit(col.gameObject);
                    return;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isHit) return;
            if (IsOwner(other.gameObject)) return;

            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isHit) return;
            if (IsOwner(collision.gameObject)) return;

            HandleHit(collision.gameObject);
        }
        #endregion

        #region Custom Method
        public void Initialize(Vector3 direction, float speed, float damageAmount, GameObject owner)
        {
            moveDirection = direction.normalized;
            moveSpeed = speed;
            damage = damageAmount;
            ownerObj = owner;

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        private void FindPlayerTarget()
        {
            var pObj = GameObject.FindWithTag("Player");
            if (pObj != null)
            {
                playerTarget = pObj.transform;
                return;
            }

            var pComp = FindFirstObjectByType<Player>();
            if (pComp != null)
            {
                playerTarget = pComp.transform;
                return;
            }

            var pcc = FindFirstObjectByType<Unity.FPS.Gameplay.PlayerCharacterController>();
            if (pcc != null)
            {
                playerTarget = pcc.transform;
                return;
            }

            if (Camera.main != null)
            {
                playerTarget = Camera.main.transform;
            }
        }

        private bool IsOwner(GameObject obj)
        {
            if (ownerObj == null || obj == null) return false;
            return (obj == ownerObj || obj.transform.IsChildOf(ownerObj.transform));
        }

        private void HandleHit(GameObject hitObj)
        {
            if (isHit) return;
            isHit = true;

            bool damageDealt = false;

            // 1. MyFps.PlayerHealth 체력 처리
            var playerHealth = hitObj.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null) playerHealth = hitObj.GetComponentInChildren<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                damageDealt = true;
                Debug.Log($"<color=red>[HoverBot HIT!]</color> 플레이어 명중! PlayerHealth 체력 {damage} 차감 완료!");
            }
            // 2. Unity.FPS.Game.Health 체력 처리
            else
            {
                var fpsHealth = hitObj.GetComponentInParent<Health>();
                if (fpsHealth == null) fpsHealth = hitObj.GetComponentInChildren<Health>();

                if (fpsHealth != null)
                {
                    fpsHealth.TakeDamage(damage, ownerObj);
                    damageDealt = true;
                    Debug.Log($"<color=red>[HoverBot HIT!]</color> 플레이어 명중! Unity.FPS.Health 체력 {damage} 차감 완료!");
                }
                // 3. IDamageable 인터페이스 연동
                else
                {
                    var damageable = hitObj.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage);
                        damageDealt = true;
                        Debug.Log($"<color=red>[HoverBot HIT!]</color> 플레이어 명중! IDamageable 체력 {damage} 차감 완료!");
                    }
                }
            }

            // 4. 전역 씬 플레이어 체력 차감 Fallback
            if (!damageDealt)
            {
                var globalHealth = FindFirstObjectByType<PlayerHealth>();
                if (globalHealth != null)
                {
                    globalHealth.TakeDamage(damage);
                    Debug.Log($"<color=red>[HoverBot HIT Fallback]</color> 플레이어 명중! 체력 {damage} 차감 완료!");
                }
                else
                {
                    var globalFpsHealth = FindFirstObjectByType<Health>();
                    if (globalFpsHealth != null)
                    {
                        globalFpsHealth.TakeDamage(damage, ownerObj);
                        Debug.Log($"<color=red>[HoverBot HIT Fallback]</color> 플레이어 명중! FPS Health 체력 {damage} 차감 완료!");
                    }
                }
            }

            Destroy(gameObject);
        }
        #endregion
    }
}
