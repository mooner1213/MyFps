using UnityEngine;
using TMPro;

namespace MyFps
{
    /// <summary>
    /// 플레이어 스탯(AmmouCount)를 UI Text 보여주기
    /// </summary>
    public class DrawAmmoCount : MonoBehaviour
    {
        public TextMeshProUGUI ammoCountText;

        private void Update()
        {
            ammoCountText.text = PlayerStats.Instance.AmmoCount.ToString();
        }
    }
}