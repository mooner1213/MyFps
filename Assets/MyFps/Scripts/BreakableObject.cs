using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 총을 맞으면(데미지를 입으면) 깨지는 오브젝트
    /// </summary>
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        #region Variables
        public GameObject fakeObejct;
        public GameObject breakObject;

        public AudioSource breakSound;

        //체력
        [SerializeField] private float health = 1;
        private bool isBroken = false;

        //히든 아이템 오브젝트
        public GameObject hiddenItemPrefab;
        [SerializeField] private Vector3 offset;

        //깨지지 않는 설정
        public bool isUnbreakable = false;
        #endregion

        #region Custom Methods
        public void TakeDamage(float damage)
        {
            if (isUnbreakable)
                return;

            health -= 1;
            if (health <= 0 && !isBroken)
            {
                StartCoroutine(BreakSequence());
            }
        }

        IEnumerator BreakSequence()
        {
            isBroken = true;

            // 충돌체 기능 제거
            // 총알이 맞으면 깨지는 오브젝트를 비활성화하고
            // 사운드 효과 플레이
            // 깨진 오브젝트를 활성화한다

            gameObject.GetComponent<BoxCollider>().enabled = false;
            fakeObejct.SetActive(false);
            breakObject.SetActive(true);

            //깨지는 효과
            if (breakSound != null )
            {
                breakSound.Play();
            }

            yield return new WaitForSeconds(0.5f);

            if(hiddenItemPrefab != null )
            {
                Instantiate(hiddenItemPrefab, transform.position + offset, Quaternion.identity);
            }
        }
        #endregion
    }
}