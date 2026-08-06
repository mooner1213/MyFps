using UnityEngine;
using UnityEngine.InputSystem;

namespace Sample
{
    /// <summary>
    /// 큐브 컬러를 흰색에서 빨간색으로 변환하기
    /// 메테리얼 바꿔치기로 색 변환
    /// 직접 메테리얼의 색을 빨간색으로 바꾸기
    /// </summary>
    public class MaterialTest : MonoBehaviour
    {
        #region Variables
        // 참조
        private Renderer renderer;

        // 인풋
        public InputActionReference jumpAction;
        public Material damageMaterial;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            // 스페이스바를 눌러 큐브 색상 변경
            if(jumpAction.action.WasPressedThisFrame())
            {
                //Debug.Log("큐브색상 변경");
                ChangeMaterialColor();
            }
        }
        #endregion

        #region Custom Method
        // 메테리얼 색상 직접 변경
        private void ChangeMaterialColor()
        {
            renderer.material.SetColor("_BaseColor", Color.red);
        }
        #endregion
    }
}