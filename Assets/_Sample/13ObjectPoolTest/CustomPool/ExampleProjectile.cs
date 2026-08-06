using UnityEngine;
using System.Collections;

namespace MySample
{
    /// <summary>
    /// 발사체를 관리하는 클래스
    /// 오브풀에서 꺼내쓰는 발사체
    /// 킬(해제) : 비활성화
    /// </summary>
    [RequireComponent (typeof(PooledObject))]
    public class ExampleProjectile : MonoBehaviour
    {
        //킬(해제) 딜레이
        [SerializeField] private float timeoutDelay = 3f;

        //참조
        private PooledObject pooledObject;

        private void Awake()
        {
            pooledObject = GetComponent<PooledObject>();
        }

        //킬(해제) - 비활성화
        public void Deactivate()
        {
            StartCoroutine(DeactivateRoutine(timeoutDelay));
        }

        IEnumerator DeactivateRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            //reset Projectile
            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            pooledObject.Release();
            gameObject.SetActive(false);
        }
    }
}