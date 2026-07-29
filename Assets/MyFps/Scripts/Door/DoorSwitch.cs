using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 문 스위치 구현, 인터랙트 구현
    /// 문 스위치는 토글, 문이 열려 있으면 빨간색, 문이 닫혀 있으면 원래색
    /// </summary>
    public class DoorSwitch : Interactive
    {
        #region Variables
        public Door door;   //문 등록

        public Renderer renderer;
        public Material closeMaterial;
        private Material originMaterial;

        //열쇠 필요 여부 : PuzzleItem.None 키 필요 없을때
        [SerializeField] private PuzzleItem doorKey = PuzzleItem.None;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            //문 개폐 이벤트에 함수 등록
            door.OnActivate += OpenDoor;
            door.OnDeActivate += CloseDoor;
        }

        private void OnDisable()
        {
            //문 개폐 이벤트에 함수 제거
            door.OnActivate -= OpenDoor;
            door.OnDeActivate -= CloseDoor;
        }


        private void Start()
        {
            //초기화
            originMaterial = renderer.material;
        }
        #endregion

        //추상메서드 - 구현하도록 강제하는 기능 정의
        #region abstract
        protected override void DoAction()
        {
            if (door == null)
                return;

            if(NeedKey())
            {
                StartCoroutine(Toggle());
            }
        }
        #endregion

        #region Custom Method
        protected override void ShowActionUI()
        {
            if (actionUI != null)
            {
                actionUI.SetActive(true);
                extraCross.SetActive(true);

                if (NeedKey())
                {
                    actionText.text = action;
                }
                else
                {
                    actionText.text = "You need doorKey";
                }
            }
        }

        IEnumerator Toggle()
        {
            //콜라이더 기능 제거
            this.GetComponent<BoxCollider>().enabled = false;

            if(door.IsActive)
            {
                door.DeActivate();
            }
            else
            {
                door.Activate();
            }

            yield return new WaitForSeconds(1f);

            //콜라이더 기능 복원
            this.GetComponent<BoxCollider>().enabled = true;
        }

        void OpenDoor()
        {
            action = "Close The Door";
            renderer.material = closeMaterial;
        }

        void CloseDoor()
        {
            action = "Open The Door";
            renderer.material = originMaterial;
        }

        bool NeedKey()
        {
            bool hasKey = false;
            //도어 키 체크
            if (doorKey == PuzzleItem.None)
            {
                hasKey = true;
            }
            else
            {
                hasKey = PlayerStats.Instance.HavePuzzleItem(doorKey);
            }

            return hasKey;
        }
        #endregion
    }
}
