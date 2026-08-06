using UnityEngine;
using UnityEngine.Events;

namespace MyFps
{
    /// <summary>
    /// 문 열기/닫기
    /// 문 열릴때 등록된 함수 호출하는 이벤트 구현
    /// 문 닫힐때 등록된 함수 호출하는 이벤트 구현
    /// </summary>
    public class Door : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        //true: 문일 열려 있는 상태, false: 문이 닫혀 있는 상태
        [SerializeField] protected bool isActive = false;

        //활성화, 비활성화시 등록된 함수 호출
        public UnityAction OnActivate;
        public UnityAction OnDeActivate;

        //애니메이터 파라미터
        private string isOpen = "IsOpen";

        //적 등록 하기
        public GunMan[] enemies;
        #endregion

        #region Property
        public bool IsActive
        {
            get {  return isActive; }
            private set
            {
                isActive = value;
                animator.SetBool(isOpen, value);
            }
        }
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            //초기화
            if(isActive)
            {
                animator.SetBool(isOpen, true);
            }
        }
        #endregion

        #region Custom Method
        //문 열기
        public void Activate()
        {
            IsActive = true;

            //문을 열때 등록되어 있는 함수 호출
            OnActivate?.Invoke();

            //
            foreach (var enemy in enemies)
            {
                enemy.IsDetecting = true;
            }
        }

        //문 닫기
        public void DeActivate()
        {
            IsActive = false;

            //문을 닫을때 등록되어 있는 함수 호출
            OnDeActivate?.Invoke();

            foreach (var enemy in enemies)
            {
                enemy.IsDetecting = false;
            }
        }
        #endregion
    }
}