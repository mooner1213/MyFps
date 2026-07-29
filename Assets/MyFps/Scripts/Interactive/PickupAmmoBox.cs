using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// Interactive를 상속받는다 
    /// 액션 : 탄환 7개를 지급
    /// </summary>
    public class PickupAmmoBox : Interactive
    {
        #region Variables
        [Header("Action")]
        [SerializeField] private int giveAmmo = 7;  //탄환 지급 갯수
        #endregion

        #region abstract
        protected override void DoAction()
        {
            //액션
            //Debug.Log($"탄환 {giveAmmo}개 지급하기");
            PlayerStats.Instance.AddAmmo(giveAmmo);

            //콜라이더 기능 제거
            Destroy(this.gameObject);
        }
        #endregion
    }
}