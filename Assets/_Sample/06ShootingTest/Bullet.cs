using UnityEngine;

namespace MyFps
{
    public class Bullet : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody rb;

        //이동
        [SerializeField] private float moveForce = 50f;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            //참조
            rb = GetComponent<Rigidbody>();
        }

        /*private void OnTriggerEnter(Collider other)
        {
            //데미지 주기
        }

        private void OnCollisionEnter(Collision collision)
        {
            //데미지 주기
        }*/
        #endregion

        #region Custom Method
        //탄환 발사시 앞방향으로 힘을 주어 이동시킨다
        public void MoveForce()
        {
            if(rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            rb.AddForce(transform.forward * moveForce, ForceMode.Impulse);
        }
        #endregion
    }
}