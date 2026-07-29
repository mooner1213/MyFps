using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    /// <summary>
    /// 마우스 포인터 따라가며 바라보기
    /// </summary>
    public class LookAtMouse : MonoBehaviour
    {
        #region Unity Event Method
        private void Update()
        {
            //마우스위치로 부터 월드 위치값 가져오기
            //Vector3 worldPos = ScreenToWorld();
            Vector3 worldPos = ScreenToRay();

            transform.LookAt(worldPos);
        }
        #endregion

        #region Custom Method
        private Vector3 ScreenToWorld()
        {
            float z = 2f;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mousePosition = new Vector3(mousePos.x, mousePos.y, z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            return worldPosition;
        }

        private Vector3 ScreenToRay()
        {
            Vector3 worldPosition = Vector3.zero;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mousePosition = new Vector3(mousePos.x, mousePos.y, 0f);
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                worldPosition = hit.point;
            }

            return worldPosition;
        }
        #endregion
    }
}


