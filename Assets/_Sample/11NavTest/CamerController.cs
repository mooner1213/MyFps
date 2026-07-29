using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 플레이어 쫓아가기
    /// </summary>
    public class CamerController : MonoBehaviour
    {
        public Transform thePlayer;
        public Vector3 offset;

        private void LateUpdate()
        {
            transform.position = thePlayer.position + offset;
        }

    }
}