using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// Ammo 아이템 줍기 - PickupItem 상속 받기
    /// </summary>
    public class PickupAmmo : PickupItem
    {
        #region Variables        
        [SerializeField] private int giveAmmo = 7;  //탄환 지급 갯수
        #endregion

        #region abstract
        protected override bool OnPickup()
        {
            PlayerStats.Instance.AddAmmo(giveAmmo);
            return true;
        }
        #endregion
    }
}