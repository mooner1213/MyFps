using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 크로스 헤어를 관리하는 클래스 
    /// </summary>
    public class CrossHairManager : MonoBehaviour
    {
        #region Variables
        public Image crosshairImage;            //UI 이미지
        public Sprite nullCrosshairSprite;      //데이터 없을때 보여지는 스프라이트

        //참조
        private PlayerWeaponManager weaponManager;

        //무기 교체 연출
        [SerializeField] private float crosshairUpdateshrpness = 5f;  //Lerp 계수

        private RectTransform crosshairRectTransform;                 //UI RectTransform

        private CrossHairData crosshairDefalut;                       //평소에 보여지는 크로스헤어
        private CrossHairData crosshairTarget;                        //타겟팅 되었을때 보여지는 크로스 헤어

        private CrossHairData crosshairCurrent;                       //현재 화면에 보여지는 크로스헤어

        private bool wasPointingAtEnemy;                              //적 포착 순간 또는 적 잃어버리는 순간을 계산하기 위한 변수
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            weaponManager = GameObject.FindFirstObjectByType<PlayerWeaponManager>();
            crosshairRectTransform = crosshairImage.GetComponent<RectTransform>();
        }

        private void Start()
        {
            //현재 액티브 무기로 크로스 헤어 교체
            OnWeaponChanged(weaponManager.GetActiveWeapon());

            //무기 교체 이벤트 함수에 등록
            weaponManager.OnSwitchToWeapon += OnWeaponChanged;
        }

        private void Update()
        {
            //크로스 헤어 보여주기
            UpdateCrosshairPointAtEnemy(false);

            //was변수 저장 - 동기화
            wasPointingAtEnemy = weaponManager.IsPointingAtEnemy;
        }
        #endregion

        #region Custom Method
        //크로스 헤어 보여주기 : force = true 강제로 보여주기
        private void UpdateCrosshairPointAtEnemy(bool force)
        {
            //크로스헤어 데이터 체크
            if (crosshairDefalut.CrossHairSprite == null)
                return;

            //적을 포착하는 순간
            if((force == true || wasPointingAtEnemy == false) && weaponManager.IsPointingAtEnemy == true)
            {                
                crosshairCurrent = crosshairTarget;
                crosshairImage.sprite = crosshairCurrent.CrossHairSprite;
            }
            //적을 놓치는 순간
            else if ((force == true || wasPointingAtEnemy == true) && weaponManager.IsPointingAtEnemy == false)
            {                
                crosshairCurrent = crosshairDefalut;
                crosshairImage.sprite = crosshairCurrent.CrossHairSprite;
            }

            //Lerp 변경
            crosshairImage.color = Color.Lerp(crosshairImage.color,
                crosshairCurrent.CrossHairColor, crosshairUpdateshrpness * Time.deltaTime);
            crosshairRectTransform.sizeDelta = Mathf.Lerp(crosshairRectTransform.sizeDelta.x,
                crosshairCurrent.CrossHairSize, crosshairUpdateshrpness * Time.deltaTime) * Vector2.one;

        }

        //무기 교체시 호출 되는 함수
        private void OnWeaponChanged(WeaponController newWeapon)
        {
            //newWeapon 체크
            if(newWeapon == null)
            {
                if(nullCrosshairSprite)
                {
                    crosshairImage.sprite = nullCrosshairSprite;
                }
                else
                {
                    crosshairImage.enabled = false;
                }
            }
            else //새로운 무기가 들어오면
            {
                crosshairImage.enabled = true;
                crosshairDefalut = newWeapon.crossHairDefault;
                crosshairTarget = newWeapon.crossHairTargetInSight;
            }

            //강제로 크로스 헤어 보여주기
            UpdateCrosshairPointAtEnemy(true);
        }
        #endregion
    }
}