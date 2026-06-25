using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 상호작용 가능한 오브젝트에 붙이는 컴포넌트 (표준 MonoBehaviour)
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private string hoverMessage = "[ E ]";

        public string GetHoverMessage()
        {
            return hoverMessage;
        }

        public void Interact()
        {
            // 이 오브젝트에 붙어있는 다른 모든 컴포넌트 중 "OnInteract"라는 이름의 함수를 호출합니다.
            // DontRequireReceiver 옵션을 주면, 그 함수가 없는 스크립트가 있더라도 에러가 발생하지 않습니다.
            gameObject.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }
}
