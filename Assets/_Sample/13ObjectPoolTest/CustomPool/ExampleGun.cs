using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 풀에 저장되어 있는 발사체를 발사하는 총을 관리하는 오브젝트
    /// </summary>
    public class ExampleGun : MonoBehaviour
    {
        #region Variables
        public InputActionReference fireAction;

        //뷸렛 프리팹
        //public GameObject bulletPrefab;

        public float muzzleVelocity = 700f;
        public Transform muzzlePosition;
        public float cooldowWindow = 0.1f;
        private float nextTimeToShoot;

        //오브젝트 풀
        public ObjectPool objectPool;
        #endregion

        #region Unity Event Method
        private void FixedUpdate()
        {
            //마우스 좌클릭하면 발사
            if(fireAction.action.IsPressed() && objectPool != null 
                && Time.time >= nextTimeToShoot)
            {
                //GameObject bulletGo = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                //Destroy(bulletGo, 3f);

                //풀에서 오브젝트 가져오기
                GameObject bulletObject = objectPool.GetPooledObject().gameObject;

                if(bulletObject != null)
                {
                    bulletObject.SetActive(true);

                    //발사체 기능
                    bulletObject.transform.SetPositionAndRotation(
                        muzzlePosition.position, muzzlePosition.rotation);

                    bulletObject.GetComponent<Rigidbody>().AddForce(
                        bulletObject.transform.forward * muzzleVelocity,
                        ForceMode.Acceleration);

                    //킬 예약
                    ExampleProjectile projectile = bulletObject.GetComponent<ExampleProjectile>();
                    projectile?.Deactivate();

                    //다음에 쏠 시간
                    nextTimeToShoot = Time.time + cooldowWindow;
                }
            }
        }
        #endregion
    }
}