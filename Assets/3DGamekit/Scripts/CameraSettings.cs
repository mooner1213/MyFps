using UnityEngine;
using Unity.Cinemachine;

namespace My3DGame
{
    /// <summary>
    /// 카메라 설정 관리 클래스
    /// </summary>
    public class CameraSettings : MonoBehaviour
    {
        #region Variables
        public CinemachineCamera freeLookCamera;

        public Transform follow;        //카메라가 따라가는 오브젝트
        public Transform lookAt;        //카메가가 바라보는 오브젝트
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //카메라 초기 설정
            UpdateCameraSettings();
        }
        #endregion

        #region Custom Method
        private void UpdateCameraSettings()
        {
            freeLookCamera.Follow = follow;
            freeLookCamera.LookAt = lookAt;
        }
        #endregion
    }
}
