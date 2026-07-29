using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    public enum ShootMode
    {
        Ray = 0,
        Bullet
    }

    /// <summary>
    /// 무기 발사 구현 클래스
    /// </summary>
    public class PistolShoot : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;
        public Transform firePoint;

        [SerializeField] private ShootMode shootType = ShootMode.Ray;

        //Bullet
        public GameObject bulletPrefab;

        //무기 사거리
        [SerializeField] private float attackRange = 100f;

        //무기 공격력
        [SerializeField] private float attackDamage = 5f;

        //입력 처리
        public InputActionReference fireAction;

        //효과(VFX, SFX)
        public GameObject hitImpactPrefab;
        public AudioSource pistolShot;
        public ParticleSystem muzzleFlash;

        //애니메이션 파라미터
        private const string fireTrigger = "FireTrigger";
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            fireAction.action.Enable();
        }

        private void OnDisable()
        {
            fireAction.action.Disable();
        }

        private void Update()
        {
            //입력처리
            if (fireAction.action.WasPressedThisFrame())
            {
                Shoot();
            }
        }

        //레이 기즈모 
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            RaycastHit hit;
            bool isHit = Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackRange);

            if (isHit)
            {
                //hit한 충돌체까지 Ray그리기
                Gizmos.DrawRay(firePoint.position, firePoint.forward * hit.distance);
            }
            else
            {
                //사거리 만큼 Ray 그리기
                Gizmos.DrawRay(firePoint.position, firePoint.forward * attackRange);
            }
        }
        #endregion

        #region Custom Method
        void Shoot()
        {
            if (shootType == ShootMode.Ray)
            {
                RaycastHit hit;
                bool isHit = Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackRange);

                if (isHit)
                {
                    Debug.Log($"hit object: {hit.transform.name}");

                    //hit Impact
                    if (hitImpactPrefab != null)
                    {
                        GameObject effectGo = Instantiate(hitImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(effectGo, 2f);
                    }

                    //데미지어블한 오브젝트면 데미지 준다
                    IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(attackDamage);
                    }
                }
            }
            else if (shootType == ShootMode.Bullet)
            {
                if (bulletPrefab != null)
                {
                    GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                    Bullet bullet = bulletGo.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        bullet.MoveForce();
                    }

                    Destroy(bulletGo, 3f);
                }
            }

            //총쏘는 연출(VFX, SFX)
            animator.SetTrigger(fireTrigger);
            pistolShot.Play();

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

        }
        #endregion
    }
}