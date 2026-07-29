using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 필드에 떨어진 아이템 줍기의 부모 추상 클래스
    /// </summary>
    public abstract class PickupItem : MonoBehaviour
    {
        //추상메서드 - 구현하도록 강제하는 기능 정의
        #region abstract
        protected abstract bool OnPickup(); //아이템 획득 성공 true, 실패 false
        #endregion

        #region Variables
        [Header("Float Motion")]
        [SerializeField] private float floatAmplitude = 0.25f; // 움직임 높이
        [SerializeField] private float floatFrequency = 1f; // 움직임 속도(Hz)

        private Vector3 startPosition;
        private float phaseOffset;

        [Header("Rotation")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f); // 각 축별 회전 속도(도/초)
        #endregion

        #region Unity Event Method
        private void Start()
        {
            startPosition = transform.position;
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            // 아이템을 위아래로 사인(sine) 곡선으로 움직이게 하기
            float omega = floatFrequency * Mathf.PI * 2f; // 각주파수(2πf)
            float y = Mathf.Sin((Time.time + phaseOffset) * omega) * floatAmplitude;
            transform.position = startPosition + Vector3.up * y;

            // 회전 적용 (도 단위 속도)
            if (rotationSpeed != Vector3.zero)
            {
                transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //플레이어 체크
            if(other.gameObject.tag == "Player")
            {
                if(OnPickup())
                {
                    //킬
                    Destroy(gameObject);
                }
            }
        }
        #endregion

        #region Custom Method        
        #endregion
    }
}